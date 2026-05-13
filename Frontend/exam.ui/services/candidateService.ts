import { apiFetch } from "@/lib/api";

export const getCandidates = () => apiFetch("/candidates");

export const getCandidateById = (id: number) => apiFetch(`/candidates/${id}`);

export const addCandidate = (data: { firstName: string; lastName: string; email: string }) =>
  apiFetch("/candidates", {
    method: "POST",
    body: JSON.stringify(data),
  });

export const deleteCandidate = (id: number) =>
  apiFetch(`/candidates/${id}`, { method: "DELETE" });
export const getCandidateDetails = (id: number) =>
  apiFetch(`/candidates/${id}/details`);