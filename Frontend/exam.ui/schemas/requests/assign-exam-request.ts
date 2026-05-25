import { z } from "zod";

export const assignExamSchema = z.object({
  candidateIds: z
    .array(z.string().min(1, "Select at least one candidate"))
    .min(1, "Select at least one candidate"),
  startDeadline: z
    .string()
    .min(1, "A start deadline is required")
    .refine((value) => {
      const deadline = new Date(value);
      return !Number.isNaN(deadline.getTime()) && deadline > new Date();
    }, "Start deadline must be in the future"),
  endDeadline: z
    .string()
    .min(1, "An end deadline is required")
    .refine((value) => {
      const deadline = new Date(value);
      return !Number.isNaN(deadline.getTime()) && deadline > new Date();
    }, "End deadline must be in the future"),
});

export type AssignExamFormValues = z.infer<typeof assignExamSchema>;
