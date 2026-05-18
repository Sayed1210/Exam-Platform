import { apiFetch } from "@/lib/api";
import { Exam } from "@/types/exam";
const BASE_URL = "/api/exams";
export const getExams = () => apiFetch(BASE_URL);
export const getExamById = (id: number) => apiFetch(`${BASE_URL}/${id}/questions`);
export const createExam = (data: { title: string; durationMins: number; questionIds: number[] }) =>
  apiFetch(BASE_URL, {
    method: "POST",
    body: JSON.stringify(data),
  });
export const updateExam = (id: number, data: { title: string; durationMins: number; questionIds: number[] }) =>
  apiFetch(`${BASE_URL}/${id}`, {
    method: "PATCH", // Your backend uses MapMethods(["PATCH"])
    body: JSON.stringify(data),
  });
export const deleteExam = (id: number) =>
  apiFetch(`${BASE_URL}/${id}`, { 
    method: "DELETE" 
  });    
export const examService={

}