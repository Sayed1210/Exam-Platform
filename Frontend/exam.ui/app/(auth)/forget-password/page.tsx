"use client";

import { useState } from "react";
import Input from "@/components/Input";
import Button from "@/components/Button";
import { useRouter } from "next/navigation";
import Image from "next/image";
import { FormValidation } from "@/schemas/form-validation";
import { forgetPasswordSchema } from "@/schemas/requests/forget-password-request";
import { forgetPassword } from "@/services/auth-service";
import Modal from "@/components/Modal";

export default function ForgetPasswordPage() {
    const router = useRouter();

    const [email, setEmail] = useState("");
    const [errors, setErrors] = useState<{ email?: string }>({});
    const [loading, setLoading] = useState(false);
    const [open, setOpen] = useState(false);


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
            alert(result.message);
            return;
        }
        setOpen(true);
        setLoading(false);
    }

    return (
        <div className="min-h-screen flex items-center justify-center bg-background">
            <div className="w-fit max-w-md bg-white rounded-2xl shadow-xl p-8 text-center flex flex-col items-center">
                <Image src="/images/enozom-logo.png" alt="logo" width={100} height={100}/>

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
                    className="btn-primary" 
                    loading={loading}
                    loadingText="Sending..."
                />
            </div>
            
            {/* Popup Window */}
            <Modal open={open} onClose={() => setOpen(false)}>
                <div className="flex flex-col gap-2 text-center">
                    <h3 className="text-heading">Reset Email Sent</h3>
                    <p className="text-muted">Check your email <br></br> It may take a few seconds</p>
                </div>
                
                <div className="flex gap-3">  
                    <Button text="Back to Login" onClick={() => router.push("/login")} className="btn-primary flex-1" />
                </div>
            </Modal>    
        </div>
    );
}
