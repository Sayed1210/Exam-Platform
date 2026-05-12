"use client";

import { useState } from "react";
import Input from "@/components/Input";
import Button from "@/components/Button";
import Image from "next/image";
import { FormValidation } from "@/schemas/form-validation";
import { resetPasswordSchema } from "@/schemas/requests/reset-password-request";

export default function ResetPasswordPage() {
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [errors, setErrors] = useState<{ password?: string; confirmPassword?: string }>({});

  const validate = () => {
      const result = FormValidation(resetPasswordSchema, { password, confirmPassword });
  
      if (!result.success) {
        setErrors(result.errors);
        return false;
      }
  
      setErrors({});
      return true;
    };

  const handleChangePassword = () => {
    if (!validate()) return;
    // TODO: call API to change password
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
