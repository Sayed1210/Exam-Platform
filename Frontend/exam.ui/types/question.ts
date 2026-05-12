import type { z } from "zod";
import type { createQuestionRequestSchema } from "@/schemas/requests/create-question-request";
import type { createTopicRequestSchema } from "@/schemas/requests/create-topic-request";
import type { deleteQuestionRequestSchema } from "@/schemas/requests/delete-question-request";
import type { getQuestionsRequestSchema } from "@/schemas/requests/get-questions-request";
import type { getTopicsRequestSchema } from "@/schemas/requests/get-topics-request";
import type { updateQuestionRequestSchema } from "@/schemas/requests/update-question-request";
import type { createQuestionResponseSchema } from "@/schemas/responses/create-question-response";
import type { createTopicResponseSchema } from "@/schemas/responses/create-topic-response";
import type { deleteQuestionResponseSchema } from "@/schemas/responses/delete-question-response";
import type { getQuestionsResponseSchema } from "@/schemas/responses/get-questions-response";
import type { getTopicsResponseSchema } from "@/schemas/responses/get-topics-response";
import type { updateQuestionResponseSchema } from "@/schemas/responses/update-question-response";

type TextChoice = {
  id: string;
  text: string;
  imageUrl?: never;
  isCorrect: boolean;
};

type ImageChoice = {
  id: string;
  text?: never;
  imageUrl: string;
  isCorrect: boolean;
};

export type QuestionChoice =
  | TextChoice
  | ImageChoice;

export type Question = {
  id: string;
  topic: string;
  statement: string;
  imageUrl?: string;
  choices: QuestionChoice[];
};

export type Topic = {
  id: string;
  name: string;
};

export type GetTopicsRequest = z.infer<typeof getTopicsRequestSchema>;
export type GetTopicsResponse = z.infer<typeof getTopicsResponseSchema>;
export type GetQuestionsRequest = z.infer<typeof getQuestionsRequestSchema>;
export type GetQuestionsResponse = z.infer<typeof getQuestionsResponseSchema>;
export type DeleteQuestionRequest = z.infer<typeof deleteQuestionRequestSchema>;
export type DeleteQuestionResponse = z.infer<typeof deleteQuestionResponseSchema>;
export type CreateTopicRequest = z.infer<typeof createTopicRequestSchema>;
export type CreateTopicResponse = z.infer<typeof createTopicResponseSchema>;
export type CreateQuestionRequest = z.infer<typeof createQuestionRequestSchema>;
export type UpdateQuestionRequest = z.infer<typeof updateQuestionRequestSchema>;
export type CreateQuestionResponse = z.infer<typeof createQuestionResponseSchema>;
export type UpdateQuestionResponse = z.infer<typeof updateQuestionResponseSchema>;
