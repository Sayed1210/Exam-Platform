export interface Choice {
  id: number;
  text: string;
  isCorrect: boolean;
  imageUrl?: string | null;
}

export interface Question {
  id: number;
  text: string;
  imageUrl?: string | null;
  choices: Choice[];
  topicTitle: string;
  topicId: number;
}

export interface Exam {
  id: number;
  title: string;
  durationMins: number;
  totalQuestions: number;
  createdAt: string;
  questions?: Question[];
  topics?: string;
}
