"use client";

import { useState } from "react";
import ModalLayout from "@/components/common/ModalLayout";
import { CreateAdminRequest } from "@/types/user";

interface Props {
  onClose: () => void;
  onSubmit: (data: CreateAdminRequest) => void;
}

export default function AddAdminModal({ onClose, onSubmit }: Props) {
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [errors, setErrors] = useState<Record<string, string>>({});

  const handleSubmit = () => {
    const nextErrors: Record<string, string> = {};

    if (!firstName.trim()) nextErrors.firstName = "First name is required";
    if (!lastName.trim()) nextErrors.lastName = "Last name is required";
    if (!email.trim()) nextErrors.email = "Email is required";
    if (!/^\S+@\S+\.\S+$/.test(email)) nextErrors.email = "Invalid email format";
    if (password.length < 8) nextErrors.password = "Password must be at least 8 characters";

    if (Object.keys(nextErrors).length > 0) {
      setErrors(nextErrors);
      return;
    }

    setErrors({});
    onSubmit({
      firstName,
      lastName,
      email,
      password,
    });
  };

  return (
    <ModalLayout
      title="Add Admin"
      footer={
        <>
          <button className="btn-secondary" onClick={onClose}>Cancel</button>
          <button className="btn-primary" onClick={handleSubmit}>Save</button>
        </>
      }
    >
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <div className="flex flex-col gap-1.5">
          <label className="text-label">First Name</label>
          <input
            placeholder="Ahmed"
            value={firstName}
            onChange={(event) => setFirstName(event.target.value)}
            className="input"
          />
          {errors.firstName && <p className="text-error">{errors.firstName}</p>}
        </div>

        <div className="flex flex-col gap-1.5">
          <label className="text-label">Last Name</label>
          <input
            placeholder="Hassan"
            value={lastName}
            onChange={(event) => setLastName(event.target.value)}
            className="input"
          />
          {errors.lastName && <p className="text-error">{errors.lastName}</p>}
        </div>
      </div>

      <div className="flex flex-col gap-1.5">
        <label className="text-label">Email</label>
        <input
          type="email"
          placeholder="admin@email.com"
          value={email}
          onChange={(event) => setEmail(event.target.value)}
          className="input"
        />
        {errors.email && <p className="text-error">{errors.email}</p>}
      </div>

      <div className="flex flex-col gap-1.5">
        <label className="text-label">Password</label>
        <input
          type="password"
          placeholder="Minimum 8 characters"
          value={password}
          onChange={(event) => setPassword(event.target.value)}
          className="input"
        />
        {errors.password && <p className="text-error">{errors.password}</p>}
      </div>
    </ModalLayout>
  );
}
