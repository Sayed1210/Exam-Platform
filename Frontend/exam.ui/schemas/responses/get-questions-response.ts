import { z } from "zod";

const questionChoiceResponseSchema = z
  .object({
    text: z.string().trim().min(1, "Choice text is required.").nullable(),
    isCorrect: z.boolean(),
    imageUrl: z.string().trim().url("Choice image URL must be valid.").nullable(),
  })
  .superRefine((choice, context) => {
    const hasText = Boolean(choice.text);
    const hasImage = Boolean(choice.imageUrl);

    if (hasText === hasImage) {
      context.addIssue({
        code: "custom",
        message: "A choice must include exactly one of text or imageUrl.",
        path: ["text"],
      });
    }
  });

const questionResponseSchema = z
  .object({
    id: z.string().trim().min(1, "Question id is required."),
    topicId: z.string().trim().min(1, "Topic id is required."),
    topicTitle: z
      .string()
      .trim()
      .min(2, "Topic title must be at least 2 characters.")
      .max(100, "Topic title must be at most 100 characters."),
    text: z.string().trim().min(1, "Question statement is required."),
    imageUrl: z.string().trim().url("Question image URL must be valid.").nullable(),
    choices: z
      .array(questionChoiceResponseSchema)
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

export const getQuestionsResponseSchema = z.object({
  items: z.array(questionResponseSchema),
  page: z.number().int().min(1),
  pageSize: z.number().int().min(1),
  totalCount: z.number().int().min(0),
  totalPages: z.number().int().min(0),
});
