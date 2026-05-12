import { z } from "zod";

export const getTopicsRequestSchema = z.object({
  currentPage: z.coerce
    .number()
    .int("Page must be a whole number.")
    .min(1, "Page must be at least 1."),
  pageSize: z.literal(20).default(20),
});
