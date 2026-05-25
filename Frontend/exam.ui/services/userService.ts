import { apiFetch } from "@/lib/api";
import { CreateAdminRequest } from "@/types/user";

const BASE_URL = "/api/users";

export const getUsers = () => apiFetch(BASE_URL);

export const createAdmin = (data: CreateAdminRequest) =>
  apiFetch(BASE_URL, {
    method: "POST",
    body: JSON.stringify(data),
  });
