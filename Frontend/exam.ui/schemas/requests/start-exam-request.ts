import { z } from "zod";

export const startExamSchema = z.object({
  candidateId: z.number().min(1, "Candiate ID is required"),
});

export type StartExamFormData = z.infer<typeof startExamSchema>;