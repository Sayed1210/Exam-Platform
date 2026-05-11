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