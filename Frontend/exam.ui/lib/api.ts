const BASE_URL =
  process.env.NEXT_PUBLIC_API_URL ;

type ApiFetchOptions = RequestInit & {
  isFormData?: boolean;
};


export function getImageUrl(
  path?: string | null
) {
  if (!path) return "";

  // already absolute URL
  if (path.startsWith("http")) {
    return path;
  }

  return `${BASE_URL}${path}`;
}

export async function apiFetch(
  endpoint: string,
  options: ApiFetchOptions = {}
) {
  const token = localStorage.getItem("token");

  const headers = new Headers(options.headers);

  // DO NOT set content type manually for FormData
  if (!options.isFormData) {
    headers.set(
      "Content-Type",
      "application/json"
    );
  }

  if (token) {
    headers.set(
      "Authorization",
      `Bearer ${token}`
    );
  }

  const res = await fetch(
    `${BASE_URL}${endpoint}`,
    {
      ...options,
      headers,
    }
  );

  const data = await res
    .json()
    .catch(() => null);

  if (!res.ok) {
    throw {
      message:
        data?.message ||
        "Something went wrong",
      status: res.status,
      data,
    };
  }

  return data;
}