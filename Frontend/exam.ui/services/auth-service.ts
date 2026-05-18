// src/services/auth.service.ts

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL;

type LoginRequest = {
  email: string;
  password: string;
};

type ForgetPasswordRequest = {
  email: string;
};

type ResetPasswordRequest = {
  newPassword: string;
  token: string;
};

export async function login(data: LoginRequest) {
    const res = await fetch(`${API_BASE_URL}/api/auth/login`, {
        method: "POST",
        headers: {
        "Content-Type": "application/json",
        },
        credentials: "include", // Without this, the browser will ignore the cookie.
        body: JSON.stringify(data),
    });

    // const text = await res.text();
    // const result = text ? JSON.parse(text) : null;
    const result = await res.json();

    if (!res.ok) {
        return {
        success: false,
        message: result?.message || "Wrong email or password",
        };
    }

    // store token and expiry
    // if (result?.token) {
    //     localStorage.setItem("token", result.token);
    //     localStorage.setItem("token_expires", result.expiresAt);
    // }

    return {
        success: true,
        data: result,
    };
}

export async function forgetPassword(data: ForgetPasswordRequest) {
  const res = await fetch(`${API_BASE_URL}/api/auth/forget-password`, {
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

// LOGOUT (IMPORTANT FIX)
export async function logout() {
  await fetch(`${API_BASE_URL}/api/auth/logout`, {
    method: "POST",
    credentials: "include",
  });
}

// export function getToken() {
//   return localStorage.getItem("token");
// }

// export function isLoggedIn() {
//   const token = localStorage.getItem("token");
//   const expiresAt = localStorage.getItem("token_expires");

//   if (!token || !expiresAt) return false;

//   return new Date(expiresAt) > new Date();
// }
