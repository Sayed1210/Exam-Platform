import { z } from "zod";

const optionalImageUrlSchema = z.preprocess(
  (value) =>
    typeof value === "string" && value.trim() === ""
      ? undefined
      : value,
  z.string()
    .trim()
    .startsWith("/", {
      message: "Invalid image path.",
    })
    .optional()
);

const optionalTextSchema = z.preprocess(
  (value) => value === null ? undefined : value,
  z.string().trim().optional()
);

const examOptionSchema = z
  .object({
    text: optionalTextSchema,
    imageUrl: optionalImageUrlSchema,
    isCorrect: z.boolean(),
  })
  .superRefine((option, ctx) => {
    const hasText = typeof option.text === 'string' && option.text.trim().length > 0;
    const hasImage = Boolean(option.imageUrl);

    if (hasText === hasImage) {
      ctx.addIssue({
        code: 'custom',
        message: 'A choice must include exactly one of text or imageUrl.',
        path: ['text'],
      });
    }
  });

const examQuestionSchema = z
  .object({
    text: z.string().trim().min(1, "Question text is required."),
    topic: z.string().trim().min(1, "Question topic is required."),
    imageUrl: optionalImageUrlSchema,
    options: z
      .array(examOptionSchema)
      .min(2, "Each question must have at least 2 answer options."),
  })
  .superRefine((question, ctx) => {
    const correctOptions = question.options.filter((option) => option.isCorrect);
    if (correctOptions.length !== 1) {
      ctx.addIssue({
        code: "custom",
        message: "Each question must have exactly one correct option.",
        path: ["options"],
      });
    }
  });

export const createExamStepOneSchema = z.object({
  title: z.string().trim().min(1, "Exam title is required."),
  durationMins: z.number().int().min(1, "Exam duration must be at least 1 minute."),
});

export const createExamSchema = z.object({
  title: z.string().trim().min(1, "Exam title is required."),
  durationMins: z.number().int().min(1, "Exam duration must be at least 1 minute."),
  questions: z
    .array(examQuestionSchema)
    .min(1, "Add at least one question to the exam."),
});

export const createExamBackendSchema = z.object({
  title: z.string().trim().min(1, "Exam title is required."),
  durationMins: z.number().int().min(1, "Duration must be at least 1 minute."),
  questionIds: z.array(z.number()).min(1, "At least one question ID is required."),
});

export type CreateExamFormData = z.infer<typeof createExamSchema>;
export type CreateExamBackendRequest = z.infer<typeof createExamBackendSchema>;
