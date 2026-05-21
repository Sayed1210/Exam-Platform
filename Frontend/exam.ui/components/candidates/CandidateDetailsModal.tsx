"use client";

import { CandidateDetail } from "@/types/candidate";
import ModalPortal from "@/components/common/ModalPortal";
import ModalLayout from "../common/ModalLayout";
import { getImageUrl } from "@/lib/api";

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
    <ModalLayout
      title={candidate.name}
      footer={<button className="btn-secondary" onClick={onClose}>Close</button>}
    >
      <div>
        <p className="nav-section-label">Candidate Details</p>
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

        {candidate.exams.length > 0 && candidate.exams.map((exam, examIndex) => {
          const status = statusMap[exam.status] ?? statusMap[3];

          return (
            <div key={examIndex} className="mt-6">
              <p className="nav-section-label">
                Exam Status: {exam.examTitle}
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
                      <div 
                        key={answerIndex} 
                        className={`border rounded-lg px-4 py-3 ${
                          answer.isCorrect 
                            ? "border-green-200 bg-green-50" 
                            : "border-red-200 bg-red-50"
                        }`}
                      >
                        {/* Question Section */}
                        <div className="mb-3">
                          <p className="text-sm font-medium text-gray-900 mb-1">
                            Q{answerIndex + 1}: {answer.questionText}
                          </p>
                          {answer.questionImageUrl && (
                            <img 
                              src={getImageUrl(answer.questionImageUrl)} 
                              alt="Question" 
                              className="max-w-full h-auto max-h-48 rounded mt-2"
                            />
                          )}
                        </div>

                        {/* Choice Section */}
                        <div>
                          <p className={`text-sm ${
                            answer.isCorrect 
                              ? "text-green-600" 
                              : "text-red-600"
                          }`}>
                            -&gt; {answer.choiceText}
                          </p>
                          {answer.choiceImageUrl && (
                            <img 
                              src={getImageUrl(answer.choiceImageUrl)} 
                              alt="Choice" 
                              className="max-w-full h-auto max-h-48 rounded mt-2"
                            />
                          )}
                        </div>
                      </div>
                    ))}
                  </div>
                </div>
              )}
            </div>
          );
        })}

        {candidate.exams.length === 0 && (
          <div className="pt-6">
            <p className="nav-section-label"> [ No exam assigned yet ] </p>
          </div>
        )}
      </div>
    </ModalLayout>
  );
}
