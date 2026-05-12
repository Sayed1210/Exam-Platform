"use client";

import { useState } from "react";
import Input from "@/components/Input";
import Button from "@/components/Button";
import Modal from "@/components/Modal";
import { useRouter } from "next/navigation";
import Image from "next/image";
import { loginSchema } from "@/schemas/requests/login-request";
import { FormValidation } from "@/schemas/form-validation";

export default function LoginPage() {
  const router = useRouter();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [open, setOpen] = useState(false);
  const [resetEmail, setResetEmail] = useState("");
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

  const handleLogin = () => {
    if (!validate()) return;
    // TODO: call API
    // router.push("/candidates");
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background">
      <div className="w-fit max-w-md bg-white rounded-2xl shadow-xl p-8 text-center flex flex-col items-center">
        <Image src="/images/enozom-logo.png" alt="logo" width={100} height={100}/>

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

        <p className="text-muted cursor-pointer hover:underline text-left" onClick={() => setOpen(true)}>
          Forgot password?
        </p>
        <div style={{ height: '10px' }} />
        <Button text="Sign In" onClick={handleLogin} className="btn-primary" />
      </div>
      
      {/* Popup Window */}
      <Modal open={open} onClose={() => setOpen(false)}>
        <div className="flex flex-col gap-2 text-center">
            <h3 className="text-title">Reset Password</h3>
            <p className="text-muted">Enter your email and we'll send a reset link</p>
        </div>
        
        <div>   
            <Input placeholder="Enter your email" value={resetEmail} onChange={setResetEmail}/>
            <div style={{ height: '8px' }} />
            <Button text="Send Reset Link" onClick={() => setOpen(false)} className="btn-primary" />
            <Button text="Cancel" onClick={() => setOpen(false)} className="btn-secondary" />
        </div>
      </Modal>
    </div>
  );
}
