export interface CandidateAnswer {
 questionText: string;
  choiceText: string;
}

export interface Candidate {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  status: number | string | null;
  score: number | null;
  invitedAt: string | null;
  startedAt: string | null;
}
export interface CandidateExam {
  examTitle: string;
  invitedAt: string;
  startedAt: string | null;
  status: number;
  score: number | null;
  answers: CandidateAnswer[];
}

export interface CandidateDetail {
  id: number;
  name: string;
  email: string;
  phone: string;
  exams: CandidateExam[];
}