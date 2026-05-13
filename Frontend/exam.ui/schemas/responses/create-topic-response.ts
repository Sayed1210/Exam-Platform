import { z } from "zod";

export const createTopicResponseSchema = z.object({
  id: z.string().trim().min(1, "Topic id is required."),
  questionsCount: z.number().int().min(0),
  title: z
    .string()
    .trim()
    .min(2, "Topic title must be at least 2 characters.")
    .max(100, "Topic title must be at most 100 characters."),
});
