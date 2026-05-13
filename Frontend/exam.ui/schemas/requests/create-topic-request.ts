import { z } from "zod";

export const createTopicRequestSchema = z.object({
  title: z
    .string()
    .trim()
    .min(2, "Topic title must be at least 2 characters.")
    .max(100, "Topic title must be at most 100 characters."),
});
