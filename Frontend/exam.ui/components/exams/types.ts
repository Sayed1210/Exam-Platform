'use client';

export interface Option {
  text: string;
  isCorrect: boolean;
  imageUrl?: string | null;
}

export interface QuestionForm {
  id?: number;
  topicId?: number;
  topic: string;
  text: string;
  imageUrl?: string | null;
  options: Option[];
  tempId?: string;
}
