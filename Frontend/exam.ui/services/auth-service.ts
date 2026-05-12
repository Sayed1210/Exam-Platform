// src/services/auth.service.ts

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL;

type LoginRequest = {
  email: string;
  password: string;
};

type ResetPasswordRequest = {
  password: string;
  confirmPassword: string;
};

export async function login(data: LoginRequest) {
    const res = await fetch(`${API_BASE_URL}/api/auth/login`, {
        method: "POST",
        headers: {
        "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
    });

    const text = await res.text();
    const result = text ? JSON.parse(text) : null;

    if (!res.ok) {
        return {
        success: false,
        message: result?.message || "Wrong email or password",
        };
    }

    if (result?.token) {
        localStorage.setItem("token", result.token);
    }

    return {
        success: true,
        data: result,
    };
}

type ForgetPasswordRequest = {
  email: string;
};

export async function forgetPassword(data: ForgetPasswordRequest) {
  const res = await fetch(`${API_BASE_URL}/auth/forget-password`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
  });

  const text = await res.text();
  const result = text ? JSON.parse(text) : null;

  if (!res.ok) {
    return {
      success: false,
      message: result?.message || "Something went wrong",
    };
  }

  return {
    success: true,
    message:
      result?.message ||
      "If this email exists, a reset link has been sent.",
  };
}

export async function resetPassword(data: ResetPasswordRequest) {
  const res = await fetch(`${API_BASE_URL}/api/auth/reset-password`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
  });

  const text = await res.text();
  const result = text ? JSON.parse(text) : null;

  if (!res.ok) {
    return {
      success: false,
      message: result?.message || "Failed to reset password",
    };
  }

  return {
    success: true,
    message: result?.message || "Password reset successfully",
  };
}

export function logout() {
  localStorage.removeItem("token");
}

export function getToken() {
  return localStorage.getItem("token");
}

export function isAuthenticated() {
  return !!getToken();
}
