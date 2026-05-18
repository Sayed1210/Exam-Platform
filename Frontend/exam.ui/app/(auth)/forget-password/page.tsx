"use client";

import { useState } from "react";
import Input from "@/components/Input";
import Button from "@/components/Button";
import { useRouter } from "next/navigation";
import { FormValidation } from "@/schemas/form-validation";
import { forgetPasswordSchema } from "@/schemas/requests/forget-password-request";
import { forgetPassword } from "@/services/auth-service";
import { toast } from "sonner";

export default function ForgetPasswordPage() {
    const router = useRouter();

    const [email, setEmail] = useState("");
    const [errors, setErrors] = useState<{ email?: string }>({});
    const [loading, setLoading] = useState(false);

    const validate = () => {
        const result = FormValidation(forgetPasswordSchema, { email });

        if (!result.success) {
        setErrors(result.errors);
        return false;
        }

        setErrors({});
        return true;
    };

    const handleResetPassword = async () => {
        if (!validate()) return;
        setLoading(true);
        const result = await forgetPassword({ email });
        
        if (!result.success) {
            setLoading(false);
            toast.error(result.message);
            return;
        }
        toast.success("Reset Email Sent. It may take a few seconds.");
        setLoading(false);
        router.push("/login")
    }

    return (
        <div>
            <h1 className="text-title">Reset Password</h1>
            <p className="text-muted">We will send a reset link to your email</p>
            <div style={{ height: '10px' }} />
            
            <div className="text-left">
                <label className="text-label">Email</label>
                <Input placeholder="admin@enozom.com" value={email} onChange={setEmail} />
                <p className="text-error mt-1">{errors.email}</p>
            </div>
            <p className="text-muted cursor-pointer hover:underline mt-2" onClick={() => router.push("/login")}>
            Back To Login
            </p>
            <Button 
                text="Send Reset Link" 
                onClick={handleResetPassword} 
                className="btn-primary w-full" 
                loading={loading}
                loadingText="Sending..."
            />
        </div>
    );
}
