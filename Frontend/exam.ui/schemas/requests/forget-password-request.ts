import { z } from "zod";

export const forgetPasswordSchema = z.object({
  email: z.email({ message: "Please enter a valid email" }),

});

export type ForgetPasswordFormData = z.infer<typeof forgetPasswordSchema>;
