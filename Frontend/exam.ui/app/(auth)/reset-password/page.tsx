"use client";

import { useSearchParams } from "next/navigation";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Input from "@/components/Input";
import Button from "@/components/Button";
import Image from "next/image";
import { FormValidation } from "@/schemas/form-validation";
import { resetPasswordSchema } from "@/schemas/requests/reset-password-request";
import { resetPassword } from "@/services/auth-service";
import Modal from "@/components/Modal";
import { toast } from "sonner";

export default function ResetPasswordPage() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const token = searchParams.get("token");

  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState<{ newPassword?: string; confirmPassword?: string }>({});

  const validate = () => {
      const result = FormValidation(resetPasswordSchema, { token, newPassword, confirmPassword });
  
      if (!result.success) {
        setErrors(result.errors);
        return false;
      }
  
      setErrors({});
      return true;
    };

  const handleChangePassword = async () => {
    if (!validate()) return;
    if (!token) {
      toast.error("Invalid reset link");
      return;
    }

    setLoading(true);
    const result = await resetPassword({token, newPassword});

    if (!result.success) {
      setLoading(false);
      toast.error(result.message);
      return;
    }
    toast.success("Password Changed Successfully");
    setLoading(false);
    router.push("/login")
  };

  return (
    <div>
      <h2 className="text-title">Set New Password</h2>
      <div style={{ height: '10px' }} />

      <div className="text-left">
        <label className="text-label">Password</label>
        <Input placeholder="••••••••" type="password" value={newPassword}onChange={setNewPassword}/>
        <p className="text-error mt-1">{errors.newPassword}</p>
        <div style={{ height: '10px' }} />
        <label className="text-label">Confirm Password</label>
        <Input placeholder="••••••••" type="password" value={confirmPassword}onChange={setConfirmPassword}/>
        <p className="text-error mt-1">{errors.confirmPassword}</p>
      </div>
      
      <Button 
        text="Change Password" 
        onClick={handleChangePassword} 
        className="btn-primary w-full" 
        loading={loading}
        loadingText="Sending..."
        />
    </div>
  );
}
