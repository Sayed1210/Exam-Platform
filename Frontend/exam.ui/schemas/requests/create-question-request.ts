import { z } from "zod";

const optionalImageUrlSchema = z.preprocess(
  (value) =>
    typeof value === "string" && value.trim() === ""
      ? undefined
      : value,
  z.string().trim().url("Image URL must be valid.").optional()
);

const textChoiceRequestSchema = z.object({
  text: z.string().trim().min(1, "Choice text is required."),
  imageUrl: z.undefined().optional(),
  isCorrect: z.boolean(),
});

const imageChoiceRequestSchema = z.object({
  text: z.undefined().optional(),
  imageUrl: z.string().trim().url("Choice image URL must be valid."),
  isCorrect: z.boolean(),
});

const questionChoiceRequestSchema = z.union([
  textChoiceRequestSchema,
  imageChoiceRequestSchema,
]);

export const createQuestionRequestSchema = z
  .object({
    topicId: z.string().trim().min(1, "Topic id is required."),
    text: z.string().trim().min(1, "Question statement is required."),
    imageUrl: optionalImageUrlSchema,
    choices: z
      .array(questionChoiceRequestSchema)
      .length(4, "A question must have exactly 4 choices."),
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
  });
