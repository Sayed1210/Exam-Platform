import { apiFetch } from "@/lib/api";

const BASE_URL = "/api/invitations";

export const sendExamInvitations = (data: { 
  examId: number; 
  candidateIds: number[]; 
  expiryDate: string; // ISO UTC string passed from the form
}) =>
  apiFetch(`${BASE_URL}/send`, {
    method: "POST",
    body: JSON.stringify(data),
  });

export const invitationService = {
  sendExamInvitations,
};