"use client";

import { CandidateDetail } from "@/types/candidate";
import ModalPortal from "@/components/ModalPortal";

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
    <ModalPortal>
      <div className="modal-overlay" onClick={onClose}>
        <div
          className="modal-panel max-w-[540px]"
          onClick={(event) => event.stopPropagation()}
        >
          <div className="modal-header items-start">
            <h2 className="modal-title">{candidate.name}</h2>
            <button
              className="modal-close-button mt-0.5"
              onClick={onClose}
              aria-label="Close candidate details modal"
            >
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="#6b7280" strokeWidth="2">
                <path d="M18 6L6 18M6 6l12 12" />
              </svg>
            </button>
          </div>

        <div className="modal-body">
          <div className="px-6 py-5">
            <p className="nav-section-label">
              Candidate Details
            </p>
            <div className="border-t border-gray-100">
              {[
                { label: "Email", value: candidate.email },
                { label: "Phone Number", value: candidate.phone ?? "-" },
              ].map(({ label, value }) => (
                <div key={label} className="flex items-center justify-between gap-4 py-3 border-b border-gray-100">
                  <span className="text-label">{label}</span>
                  <span className="text-right text-sm font-medium text-gray-900">{value}</span>
                </div>
              ))}
            </div>
          </div>

          {candidate.exams.length > 0 && candidate.exams.map((exam, examIndex) => {
            const status = statusMap[exam.status] ?? statusMap[3];

            return (
              <div key={examIndex} className="px-6 pb-5">
                <p className="nav-section-label">
                  EXAM INFO - {exam.examTitle}
                </p>
                <div className="border-t border-gray-100">
                  {[
                    {
                      label: "Invited At",
                      value: exam.invitedAt
                        ? new Date(exam.invitedAt).toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" })
                        : "-",
                    },
                    {
                      label: "Started At",
                      value: exam.startedAt
                        ? new Date(exam.startedAt).toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" })
                        : "-",
                    },
                  ].map(({ label, value }) => (
                    <div key={label} className="flex items-center justify-between gap-4 py-3 border-b border-gray-100">
                      <span className="text-label">{label}</span>
                      <span className="text-right text-sm font-medium text-gray-900">{value}</span>
                    </div>
                  ))}

                  <div className="flex items-center justify-between gap-4 py-3 border-b border-gray-100">
                    <span className="text-label">Status</span>
                    <span
                      className="inline-block px-3 py-1 rounded-full text-[13px] font-medium"
                      style={{ background: status.bg, color: status.color }}
                    >
                      {status.label}
                    </span>
                  </div>

                  <div className="flex items-center justify-between gap-4 py-3 border-b border-gray-100">
                    <span className="text-label">Score</span>
                    <span className="text-right text-sm font-medium text-gray-900">
                      {exam.score !== null ? `${exam.score}%` : "-"}
                    </span>
                  </div>
                </div>

                {exam.answers.length > 0 && (
                  <div className="mt-4">
                    <p className="nav-section-label">
                      ANSWERS
                    </p>
                    <div className="flex flex-col gap-2.5">
                      {exam.answers.map((answer, answerIndex) => (
                        <div key={answerIndex} className="border border-gray-200 rounded-lg px-4 py-3">
                          <p className="text-sm font-medium text-gray-900 mb-1">
                            Q{answerIndex + 1}: {answer.questionText}
                          </p>
                          <p className="text-sm text-blue-600">-&gt; {answer.choiceText}</p>
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
        </div>

        <div className="border-t border-slate-100 px-6 py-5">
          <div className="flex justify-end">
            <button
              className="btn-secondary"
              onClick={onClose}
            >
              Close
            </button>
          </div>
        </div>
        </div>
      </div>
    </ModalPortal>
  );
}
