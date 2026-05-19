"use client";

import { useState } from "react";
import { candidateSchema } from "@/schemas/requests/candidateSchema";
import { FormValidation } from "@/schemas/form-validation";
import ModalPortal from "@/components/ModalPortal";

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

export default function AddCandidateModal({ onClose, onSubmit, backendError,
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
    <ModalPortal>
      <div
        className="modal-overlay"
        onClick={onClose}
      >
        <div
          className="modal-panel max-w-[520px]"
          onClick={(e) => e.stopPropagation()}
        >
          {/* Header */}
          <div className="modal-header">
            <h2 className="modal-title">
              Add Candidate
            </h2>

            <button
              className="modal-close-button"
              onClick={onClose}
              aria-label="Close add candidate modal"
            >
              <svg
                width="18"
                height="18"
                viewBox="0 0 24 24"
                fill="none"
                stroke="#6b7280"
                strokeWidth="2"
              >
                <path d="M18 6L6 18M6 6l12 12" />
              </svg>
            </button>
          </div>

        {/* Body */}
        <div className="modal-body px-6 py-5 flex flex-col gap-4">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <div className="flex flex-col gap-1.5">
              <label className="text-label">
                First Name
              </label>

              <input
                placeholder="Ahmed"
                value={firstName}
                onChange={(e) => setFirstName(e.target.value)}
                className="border border-gray-300 rounded-lg px-3.5 py-2.5 text-sm text-gray-900 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-500/10 transition placeholder:text-gray-400"
              />
              {errors.firstName && (
              <p className="text-red-500 text-xs mt-1">
              {errors.firstName}
              </p>
)}
            </div>

            <div className="flex flex-col gap-1.5">
              <label className="text-label">
                Last Name
              </label>

              <input
                placeholder="Hassan"
                value={lastName}
                onChange={(e) => setLastName(e.target.value)}
                className="border border-gray-300 rounded-lg px-3.5 py-2.5 text-sm text-gray-900 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-500/10 transition placeholder:text-gray-400"
              />

              {errors.lastName && (
              <p className="text-red-500 text-xs mt-1">
             {errors.lastName}
              </p>
                 )}
            </div>
          </div>

          <div className="flex flex-col gap-1.5">
            <label className="text-label">
              Email
            </label>

            <input
              type="email"
              placeholder="candidate@email.com"
              value={email}
              onChange={(e) => {
                setEmail(e.target.value);
                clearBackendError?.();
              }}
              className="border border-gray-300 rounded-lg px-3.5 py-2.5 text-sm text-gray-900 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-500/10 transition placeholder:text-gray-400"
            />

                 {errors.email && (
              <p className="text-red-500 text-xs">{errors.email}</p>
            )}

            {backendError && (
              <p className="text-red-500 text-xs">{backendError}</p>
            )}
          </div>

          {/* Phone Number */}
          <div className="flex flex-col gap-1.5">
            <label className="text-label">
              Phone Number
            </label>

            <input
              type="tel"
              placeholder="+20 100 123 4567"
              value={phoneNumber}
              onChange={(e) => setPhoneNumber(e.target.value)}
              className="border border-gray-300 rounded-lg px-3.5 py-2.5 text-sm text-gray-900 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-500/10 transition placeholder:text-gray-400"
            />
            {errors.phoneNumber && (
  <p className="text-red-500 text-xs mt-1">
    {errors.phoneNumber}
  </p>
)}
          </div>
        </div>

        {/* Footer */}
        <div className="flex items-center justify-end gap-3 border-t border-slate-100 px-6 py-5">
          <button
            className="btn-secondary"
            onClick={onClose}
          >
            Cancel
          </button>

          <button
            className="btn-primary px-6 py-2.5"
            onClick={handleSubmit}
          >
            Add Candidate
          </button>
        </div>
        </div>
      </div>
    </ModalPortal>
  );
}
