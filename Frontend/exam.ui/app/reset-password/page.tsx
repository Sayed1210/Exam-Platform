"use client";

import { useState } from "react";
import Input from "@/components/Input";
import Button from "@/components/Button";
import { useRouter } from "next/navigation";
import Image from "next/image";

export default function ResetPasswordPage() {
  const router = useRouter();

  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [errors, setErrors] = useState<{ password?: string; confirmPassword?: string }>({});

  const validate = () => {
    const newErrors: { password?: string; confirmPassword?: string } = {};

    // Password required
    if (!password) {
      newErrors.password = "Password is required";
    } 
    // Password length
    else if (password.length < 8) {
      newErrors.password = "Password must be at least 8 characters";
    }

    // Confirm Password required & must match
    if (!confirmPassword) {
      newErrors.confirmPassword = "Password confirmation is required";
    } 
    else if (password !== confirmPassword) {
      newErrors.confirmPassword = "Passwords do not match";
    }

    setErrors(newErrors);

    return Object.keys(newErrors).length === 0;
  };

  const handleChangePassword = () => {
    if (!validate()) return;
    // TODO: call API
    // router.push("/candidates");
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background">
      <div className="w-fit max-w-md bg-white rounded-2xl shadow-xl p-8 text-center flex flex-col items-center">
        <Image src="/images/enozom-logo.png" alt="logo" width={100} height={100}/>

        <h2 className="text-title">Enozom</h2>
        <div style={{ height: '10px' }} />
        <div className="text-left">
          <label className="text-label">Password</label>
          <Input placeholder="••••••••" type="password" value={password}onChange={setPassword}/>
          <p className="text-error mt-1 min-h-[10px]">{errors.password}</p>

          <label className="text-label">Confirm Password</label>
          <Input placeholder="••••••••" type="password" value={confirmPassword}onChange={setConfirmPassword}/>
          <p className="text-error mt-1 min-h-[10px]">{errors.confirmPassword}</p>
        </div>
        
        <div style={{ height: '10px' }} />
        <Button text="Change Password" onClick={handleChangePassword} className="btn-primary" />
      </div>
    </div>
  );
}
