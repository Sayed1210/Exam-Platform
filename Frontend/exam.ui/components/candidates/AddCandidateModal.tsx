"use client";

import { useState } from "react";
import { candidateSchema } from "@/schemas/requests/candidateSchema";
import { FormValidation } from "@/schemas/form-validation";
import ModalLayout from "../common/ModalLayout";

interface Props {
  onClose: () => void;
  onSubmit: (data: {
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
  }) => void;
    backendError?: string;
  clearBackendError?: () => void;
}

export default function AddCandidateModal({ onClose, onSubmit, //backendError,
  clearBackendError, }: Props) {
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [phoneNumber, setPhoneNumber] = useState("");
  const [errors, setErrors] = useState<Record<string, string>>({});
  
  const handleSubmit = () => {
    const result = FormValidation(candidateSchema, {
      firstName,
      lastName,
      email,
      phoneNumber,
    });

    if (!result.success) {
      setErrors(result.errors);
      return;
    }

    setErrors({});
    onSubmit(result.data);
  };

  return (
    <ModalLayout
      title="Add Candidate"
      footer={
        <>
          <button className="btn-secondary" onClick={onClose}>Cancel</button>
          <button className="btn-primary" onClick={handleSubmit}>Save</button>
        </>
      }
    >
        {/* Name */}
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <div className="flex flex-col gap-1.5">
            <label className="text-label">First Name</label>

            <input
              placeholder="Ahmed"
              value={firstName}
              onChange={(e) => setFirstName(e.target.value)}
              className="input"
            />
            {errors.firstName && (
              <p className="text-error">{errors.firstName}</p>
            )}
          </div>

          <div className="flex flex-col gap-1.5">
            <label className="text-label">Last Name</label>

            <input
              placeholder="Hassan"
              value={lastName}
              onChange={(e) => setLastName(e.target.value)}
              className="input"
            />

            {errors.lastName && (
              <p className="text-error">{errors.lastName}</p>
            )}
          </div>
        </div>
        
        {/* Email */}
        <div className="flex flex-col gap-1.5">
          <label className="text-label">Email</label>

          <input
            type="email"
            placeholder="candidate@email.com"
            value={email}
            onChange={(e) => {
              setEmail(e.target.value);
              clearBackendError?.();
            }}
            className="input"
          />

          {errors.email && (
            <p className="text-error">{errors.email}</p>
          )}

          {/* {backendError && (
            <p className="text-error">{backendError}</p>
          )} */}
        </div>

        {/* Phone Number */}
        <div className="flex flex-col gap-1.5">
          <label className="text-label"> Phone Number </label>

          <input
            type="tel"
            placeholder="+20 100 123 4567"
            value={phoneNumber}
            onChange={(e) => setPhoneNumber(e.target.value)}
            className="input"
          />

          {errors.phoneNumber && (
            <p className="text-error">{errors.phoneNumber}</p>
          )}
        </div>
    </ModalLayout>
  );
}
