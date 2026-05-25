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

const questionChoiceRequestSchema = z
  .object({
    text: z.string().trim().optional(),
    imageUrl: optionalImageUrlSchema,
    isCorrect: z.boolean(),
  })
  .superRefine((choice, context) => {
    const hasText = typeof choice.text === "string" && choice.text.trim().length > 0;
    const hasImage = Boolean(choice.imageUrl);

    if (!hasText && !hasImage) {
      context.addIssue({
        code: "custom",
        message: "Each choice must include at least text or imageUrl.",
        path: ["text"],
      });
    }
  });

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

    const texts = question.choices
      .map((choice) => choice.text?.trim().toLowerCase())
      .filter((text): text is string => Boolean(text));
    const hasDuplicates = new Set(texts).size !== texts.length;

    if (hasDuplicates) {
      context.addIssue({
        code: "custom",
        message: "Duplicate choice texts are not allowed.",
        path: ["choices"],
      });
    }
  });
