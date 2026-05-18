import { apiFetch } from "@/lib/api";

export const getQuestions = (
  page: number = 1,
  pageSize: number = 10,
  search?: string,
  topicIds?: number[]
) => {
  const params = new URLSearchParams();
  params.append("page", String(page));
  params.append("pageSize", String(pageSize));
  if (search) params.append("search", search);
  if (topicIds && topicIds.length > 0) {
    topicIds.forEach((id) => params.append("topicId", String(id)));
  }
  return apiFetch(`/api/questions?${params.toString()}`);
};

export const getTopics = () => apiFetch("/api/topics");

export const createQuestion = (data: any) =>
  apiFetch("/api/questions", { method: "POST", body: JSON.stringify(data) });

export const updateQuestion = (id: number, data: any) =>
  apiFetch(`/api/questions/${id}`, { method: "PATCH", body: JSON.stringify(data) });

export const deleteQuestion = (id: number) =>
  apiFetch(`/api/questions/${id}`, { method: "DELETE" });

export const createTopic = (title: string) =>
  apiFetch("/api/topics", { method: "POST", body: JSON.stringify({ title }) });

export const uploadImage = async (file: File) => {
  const formData = new FormData();

  formData.append("file", file);

  return apiFetch("/api/upload", {
    method: "POST",
    body: formData,
    isFormData: true,
  });
};