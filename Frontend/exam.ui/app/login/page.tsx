"use client";

import { useState } from "react";
import Input from "@/components/Input";
import Button from "@/components/Button";
import Modal from "@/components/Modal";
import { useRouter } from "next/navigation";
import Image from "next/image";

export default function LoginPage() {
  const router = useRouter();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [open, setOpen] = useState(false);
  const [resetEmail, setResetEmail] = useState("");
  const [errors, setErrors] = useState<{ email?: string; password?: string }>({});

  const validate = () => {
    const newErrors: { email?: string; password?: string } = {};

    // Email required
    if (!email.trim()) {
      newErrors.email = "Email is required";
    } 
    // Email format
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      newErrors.email = "Please enter a valid email";
    }

    // Password required
    if (!password) {
      newErrors.password = "Password is required";
    } 
    // Password length
    else if (password.length < 8) {
      newErrors.password = "Password must be at least 8 characters";
    }

    setErrors(newErrors);

    return Object.keys(newErrors).length === 0;
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
          <p className="text-error mt-1 min-h-[10px]">{errors.email}</p>
          <div style={{ height: '10px' }} />
          <label className="text-label">Password</label>
          <Input placeholder="••••••••" type="password" value={password}onChange={setPassword}/>
          <p className="text-error mt-1 min-h-[10px]">{errors.password}</p>
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
            <h3 className="text-title mb-3">Reset Password</h3>
            <p className="text-muted">Enter your email and we'll send a reset link</p>
        </div>
        
        <div>   
            <Input placeholder="Enter your email" value={resetEmail} onChange={setResetEmail}/>
            <div className="mt-3">  
              <Button text="Send Reset Link" onClick={() => setOpen(false)} className="btn-primary" />
              <Button text="Cancel" onClick={() => setOpen(false)} className="btn-secondary w-full" />

            </div>
        </div>
      </Modal>
    </div>
  );
}
// can do validation on blur instead of on submit, but for simplicity doing it on submit
