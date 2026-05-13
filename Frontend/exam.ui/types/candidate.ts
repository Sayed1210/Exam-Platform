export type CandidateStatus = "Completed" | "In Progress" | "Invited" | "Expired"|null;

export interface CandidateAnswer {
  question: string;
  answer: string;
}

export interface Candidate {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  status: CandidateStatus;
  score: number | null;
  invitedAt: string | null;
  startedAt: string | null;
  answers: CandidateAnswer[];
}
