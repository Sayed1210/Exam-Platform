const BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5129";

export async function apiFetch(endpoint: string, options?: RequestInit) {
  const token = localStorage.getItem("token");

  const res = await fetch(`${BASE_URL}${endpoint}`, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...options?.headers,
    },
  });

 const data = await res.json().catch(() => null);

  if (!res.ok) {
    throw {
      message: data?.message || "Something went wrong",
      status: res.status,
      data,
    };
  }

  return data;
}