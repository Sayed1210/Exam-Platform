"use client";

import { Candidate } from "@/types/candidate";

const statusStyles: Record<string, { bg: string; color: string }> = {
  Completed: { bg: "#d1fae5", color: "#065f46" },// green
  "In Progress": { bg: "#fef9c3", color: "#854d0e" },//yellow
  Invited: { bg: "#eff6ff", color: "#1d4ed8" },//blue
  Expired: { bg: "#fee2e2", color: "#991b1b" },//red
};

interface Props {
  candidate: Candidate;
  onClose: () => void;
}

export default function CandidateDetailsModal({ candidate, onClose }: Props) {
  const style = candidate.status ? statusStyles[candidate.status] : statusStyles["Expired"];

  return (
    <div
      className="fixed inset-0 bg-black/40 flex items-center justify-center z-50"
      onClick={onClose}
    >
      <div
        className="bg-white rounded-xl w-full max-w-[540px] shadow-2xl overflow-hidden max-h-[90vh] overflow-y-auto"
        onClick={(e) => e.stopPropagation()}
      >
        {/* Header */}
        <div className="flex items-start justify-between px-6 pt-5 pb-4">
          <h2 className="text-lg font-bold text-gray-900">
            {candidate.firstName} {candidate.lastName}
          </h2>
          <button
            className="p-1 rounded hover:bg-gray-100 transition mt-0.5"
            onClick={onClose}
          >
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="#6b7280" strokeWidth="2">
              <path d="M18 6L6 18M6 6l12 12" />
            </svg>
          </button>
        </div>

        {/* Candidate Details */}
        <div className="px-6 pb-5">
          <p className="text-[11px] font-bold tracking-widest text-gray-400 mb-3">
            Candidate Details
          </p>
          <div className="border-t border-gray-100">
            {[
              { label: "Email", value: candidate.email },
              { label: "Invited At", value: candidate.invitedAt },
              { label: "Started At", value: candidate.startedAt ?? "—" },
            ].map(({ label, value }) => (
              <div key={label} className="flex items-center justify-between py-3 border-b border-gray-100">
                <span className="text-sm text-gray-500">{label}</span>
                <span className="text-sm font-medium text-gray-900">{value}</span>
              </div>
            ))}

            <div className="flex items-center justify-between py-3 border-b border-gray-100">
              <span className="text-sm text-gray-500">Status</span>
              <span
                className="inline-block px-3 py-1 rounded-full text-[13px] font-medium"
                style={{ background: style.bg, color: style.color }}
              >
                {candidate.status ?? "—"}
              </span>
            </div>

            <div className="flex items-center justify-between py-3 border-b border-gray-100">
              <span className="text-sm text-gray-500">Score</span>
              <span className="text-sm font-medium text-gray-900">
                {candidate.score !== null ? `${candidate.score}%` : "—"}
              </span>
            </div>
          </div>
        </div>

        {/* Answers */}
        {candidate.answers.length > 0 && (
          <div className="px-6 pb-5">
            <p className="text-[11px] font-bold tracking-widest text-gray-400 mb-3">
              ANSWERS
            </p>
            <div className="flex flex-col gap-2.5">
              {candidate.answers.map((a, i) => (
                <div key={i} className="border border-gray-200 rounded-lg px-4 py-3">
                  <p className="text-sm font-medium text-gray-900 mb-1">
                    Q{i + 1}: {a.question}
                  </p>
                  <p className="text-sm text-blue-600">→ {a.answer}</p>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Footer */}
        <div className="flex justify-end px-6 py-5">
          <button
            className="bg-white border border-gray-300 rounded-lg px-5 py-2.5 text-sm font-medium text-gray-700 hover:bg-gray-50 transition cursor-pointer"
            onClick={onClose}
          >
            Close
          </button>
        </div>
      </div>
    </div>
  );
}