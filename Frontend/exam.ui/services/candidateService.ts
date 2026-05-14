import { apiFetch } from "@/lib/api";
export const getCandidates = (
  page: number = 1,
  pageSize: number = 8,
  search?: string,
  status?: number,
  noStatus?: boolean
) => {
  const params = new URLSearchParams();
  params.append("page", String(page));
  params.append("pageSize", String(pageSize));
  if (search) params.append("search", search);
  if (status !== undefined) params.append("status", String(status));
  if (noStatus) params.append("noStatus", "true");

  return apiFetch(`/candidates?${params.toString()}`);
};

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