import { z } from "zod";

export const candidateSchema = z.object({
  firstName: z.string().min(1, "First name is required"),
  lastName: z.string().min(1, "Last name is required"),
  email: z.email("Invalid Email"),
  phoneNumber: z.string().min(11, "Invalid Number"),
});

export type CandidateInput = z.infer<typeof candidateSchema>;