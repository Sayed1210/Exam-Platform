"use client";

import { useState } from "react";
import Input from "@/components/Input";
import Button from "@/components/Button";
import { useRouter } from "next/navigation";
import { loginSchema } from "@/schemas/requests/login-request";
import { FormValidation } from "@/schemas/form-validation";
import { login } from "@/services/auth-service";

export default function LoginPage() {
  const router = useRouter();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [errors, setErrors] = useState<{ email?: string; password?: string }>({});

  const validate = () => {
    const result = FormValidation(loginSchema, { email, password });

    if (!result.success) {
      setErrors(result.errors);
      return false;
    }

    setErrors({});
    return true;
  };

  const handleLogin = async () => {
    if (!validate()) return;

    const result = await login({ email, password });

    if (!result.success) {
      alert(result.message);
      return;
    }

    router.push("/candidates");
  };

  return (
    <div>
      <h1 className="text-title">Enozom</h1>
      <p className="text-muted">Admin Portal</p>
      <div style={{ height: '10px' }} />
      
      <div className="text-left">
        <label className="text-label">Email</label>
        <Input placeholder="admin@enozom.com" value={email} onChange={setEmail} />
        <p className="text-error mt-1">{errors.email}</p>
        <div style={{ height: '10px' }} />
        <label className="text-label">Password</label>
        <Input placeholder="••••••••" type="password" value={password}onChange={setPassword}/>
        <p className="text-error mt-1">{errors.password}</p>
      </div>

      <p className="text-muted cursor-pointer hover:underline mt-2" onClick={() => router.push("/forget-password")}>
        Forgot password?
      </p>
      <Button text="Sign In" onClick={handleLogin} className="btn-primary w-full" />
    </div>
  );
}

// admin password: admin123
