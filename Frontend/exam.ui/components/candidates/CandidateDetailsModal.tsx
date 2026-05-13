"use client";

import { CandidateDetail } from "@/types/candidate";

const statusMap: Record<number, { label: string; bg: string; color: string }> = {
  0: { label: "Pending",     bg: "#eff6ff", color: "#1d4ed8" },
  1: { label: "Expired",     bg: "#fee2e2", color: "#991b1b" },
  2: { label: "In Progress", bg: "#fef9c3", color: "#854d0e" },
  3: { label: "Done",        bg: "#d1fae5", color: "#065f46" },
};
interface Props {
  candidate: CandidateDetail;
  onClose: () => void;
}

export default function CandidateDetailsModal({ candidate, onClose }: Props) {
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
          <h2 className="text-lg font-bold text-gray-900">{candidate.name}</h2>
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
              { label: "Phone Number", value: candidate.phone ?? "—" },
            ].map(({ label, value }) => (
              <div key={label} className="flex items-center justify-between py-3 border-b border-gray-100">
                <span className="text-sm text-gray-500">{label}</span>
                <span className="text-sm font-medium text-gray-900">{value}</span>
              </div>
            ))}
          </div>
        </div>

        {/* Exams */}
        {candidate.exams.length > 0 && candidate.exams.map((exam, ei) => {
          const status = statusMap[exam.status] ?? statusMap[3];
          return (
            <div key={ei} className="px-6 pb-5">
              <p className="text-[11px] font-bold tracking-widest text-gray-400 mb-3">
                EXAM INFO — {exam.examTitle}
              </p>
              <div className="border-t border-gray-100">
                {[
                  {
                    label: "Invited At",
                    value: exam.invitedAt
                      ? new Date(exam.invitedAt).toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" })
                      : "—",
                  },
                  {
                    label: "Started At",
                    value: exam.startedAt
                      ? new Date(exam.startedAt).toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" })
                      : "—",
                  },
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
                    style={{ background: status.bg, color: status.color }}
                  >
                    {status.label}
                  </span>
                </div>

                <div className="flex items-center justify-between py-3 border-b border-gray-100">
                  <span className="text-sm text-gray-500">Score</span>
                  <span className="text-sm font-medium text-gray-900">
                    {exam.score !== null ? `${exam.score}%` : "—"}
                  </span>
                </div>
              </div>

              {/* Answers */}
              {exam.answers.length > 0 && (
                <div className="mt-4">
                  <p className="text-[11px] font-bold tracking-widest text-gray-400 mb-3">
                    ANSWERS
                  </p>
                  <div className="flex flex-col gap-2.5">
                    {exam.answers.map((a, i) => (
                      <div key={i} className="border border-gray-200 rounded-lg px-4 py-3">
                        <p className="text-sm font-medium text-gray-900 mb-1">
                          Q{i + 1}: {a.questionText}
                        </p>
                        <p className="text-sm text-blue-600">→ {a.choiceText}</p>
                      </div>
                    ))}
                  </div>
                </div>
              )}
            </div>
          );
        })}

        {candidate.exams.length === 0 && (
          <div className="px-6 pb-5">
            <p className="text-sm text-gray-400 italic">No exam assigned yet.</p>
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
