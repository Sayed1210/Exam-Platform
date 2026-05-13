import { z } from "zod";

export const deleteQuestionRequestSchema =
  z.object({
    id: z.coerce
      .number()
      .int("Question id must be an integer.")
      .positive("Question id must be positive."),
  });
