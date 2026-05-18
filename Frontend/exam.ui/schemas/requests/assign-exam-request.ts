import { z } from "zod";

export const assignExamSchema = z.object({
  candidateIds: z
    .array(z.string().min(1, "Select at least one candidate"))
    .min(1, "Select at least one candidate"),
  deadline: z
    .string()
    .min(1, "A deadline is required")
    .refine((value) => {
      const deadline = new Date(value);
      return !Number.isNaN(deadline.getTime()) && deadline > new Date();
    }, "Deadline must be in the future"),
});

export type AssignExamFormValues = z.infer<typeof assignExamSchema>;
