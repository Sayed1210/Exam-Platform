import { z } from "zod";

const optionalImageUrlSchema = z.preprocess(
  (value) =>
    typeof value === "string" &&
    value.trim() === ""
      ? undefined
      : value,
  z.string()
    .trim()
    .startsWith("/", {
      message: "Invalid image path.",
    })
    .optional()
);

const textChoiceRequestSchema = z.object({
  text: z.string().trim().min(1, "Choice text is required."),
  imageUrl: z.undefined().optional(),
  isCorrect: z.boolean(),
});

const imageChoiceRequestSchema = z.object({
  text: z.undefined().optional(),
  imageUrl: z.string()
    .trim()
    .startsWith("/", {
      message: "Invalid image path.",
    })
    .optional(),
  isCorrect: z.boolean(),
});

const questionChoiceRequestSchema = z.union([
  textChoiceRequestSchema,
  imageChoiceRequestSchema,
]);

export const createQuestionRequestSchema = z
  .object({
    topicId: z.number().positive("Topic id is required."),
    text: z.string().trim().min(5, "Question statement must be at least 5 characters long."),
    imageUrl: optionalImageUrlSchema,
    choices: z
      .array(questionChoiceRequestSchema)
      .min(2, "A question must have at least 2 choices."),
  })
  .superRefine((question, context) => {
    const correctChoices = question.choices.filter((choice) => choice.isCorrect);

    if (correctChoices.length !== 1) {
      context.addIssue({
        code: "custom",
        message: "A question must have exactly one correct choice.",
        path: ["choices"],
      });
    }
      // check for duplicate text choices
  const texts = question.choices
    .map((c) => ("text" in c ? c.text?.trim().toLowerCase() : null))
    .filter(Boolean);
  const hasDuplicates = new Set(texts).size !== texts.length;
  if (hasDuplicates) {
    context.addIssue({
      code: "custom",
      message: "Duplicate choice texts are not allowed.",
      path: ["choices"],
    });
  }
  });
