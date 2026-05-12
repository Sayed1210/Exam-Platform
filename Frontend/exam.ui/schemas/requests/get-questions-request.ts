import { z } from "zod";

export const getQuestionsRequestSchema = z.object({
  search: z.string().trim().optional(),
  topicIds: z
  .array(
    z.coerce
      .number()
      .int("Topic id must be an integer.")
      .positive("Topic id must be positive.")
  )
  .optional(),
  currentPage: z.coerce
    .number()
    .int("Current page must be a whole number.")
    .min(1, "Current page must be at least 1."),
  pageSize: z.literal(20).default(20),
});
