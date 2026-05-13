import { apiFetch } from "@/lib/api";

export const getCandidates = () => apiFetch("/candidates");

export const getCandidateById = (id: number) => apiFetch(`/candidates/${id}`);

export const addCandidate = (data: { firstName: string; lastName: 
                      string; email: string; phoneNumber: string }) =>
  apiFetch("/candidates", {
    method: "POST",
       body: JSON.stringify({
      firstName: data.firstName,
      lastName: data.lastName,
      email: data.email,
      phone: data.phoneNumber,
    }),
  });
export const deleteCandidate = (id: number) =>
  apiFetch(`/candidates/${id}`, { method: "DELETE" });
export const getCandidateDetails = (id: number) =>
  apiFetch(`/candidates/${id}/details`);