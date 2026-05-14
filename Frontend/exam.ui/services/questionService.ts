import { apiFetch } from "@/lib/api";

export const getQuestions = (
  page: number = 1,
  pageSize: number = 10,
  search?: string,
  topicId?: number
) => {
  const params = new URLSearchParams();
  params.append("page", String(page));
  params.append("pageSize", String(pageSize));
  if (search) params.append("search", search);
  if (topicId !== undefined) params.append("topicId", String(topicId));
  return apiFetch(`/api/questions?${params.toString()}`);
};

export const getTopics = () => apiFetch("/api/topics");

export const createQuestion = (data: any) =>
  apiFetch("/api/questions", { method: "POST", body: JSON.stringify(data) });

export const updateQuestion = (id: number, data: any) =>
  apiFetch(`/api/questions/${id}`, { method: "PUT", body: JSON.stringify(data) });

export const deleteQuestion = (id: number) =>
  apiFetch(`/api/questions/${id}`, { method: "DELETE" });

export const createTopic = (title: string) =>
  apiFetch("/api/topics", { method: "POST", body: JSON.stringify({ title }) });