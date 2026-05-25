"use client";

import Button from "@/components/Button";
import ConfirmDeleteModal from "@/components/common/ConfirmDeleteModal";
import { beforeStartExam, startExam } from "@/services/exam-service";
import { ClipboardDocumentListIcon } from "@heroicons/react/24/outline";
import { useRouter, useSearchParams } from "next/navigation";
import { useEffect, useState } from "react";
import { toast } from "sonner";

type BeforeStartExam = {
  candidateId: number;
  examId: number;
  title: string;
  durationMins: number;
  totalQuestions: number;
  startDate: string;
  expiryDate: string;
  status: string;
};

const getRemainingMs = (date?: string) => {
  if (!date) return 0;

  const targetTime = new Date(date).getTime();

  if (Number.isNaN(targetTime)) return 0;

  return Math.max(0, targetTime - Date.now());
};

const formatCountdown = (milliseconds: number) => {
  const totalSeconds = Math.max(0, Math.floor(milliseconds / 1000));
  const days = Math.floor(totalSeconds / 86400);
  const hours = Math.floor((totalSeconds % 86400) / 3600);
  const minutes = Math.floor((totalSeconds % 3600) / 60);
  const seconds = totalSeconds % 60;
  const pad = (value: number) => value.toString().padStart(2, "0");

  return days > 0
    ? `${days}d ${pad(hours)}:${pad(minutes)}:${pad(seconds)}`
    : `${pad(hours)}:${pad(minutes)}:${pad(seconds)}`;
};

const formatWindowDate = (date?: string) => {
  if (!date) return "Not scheduled";

  const parsedDate = new Date(date);

  if (Number.isNaN(parsedDate.getTime())) return "Not scheduled";

  return parsedDate.toLocaleString(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
  });
};

export default function StartExamPage() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const token = searchParams.get("token");

  const [open, setOpen] = useState(false);
  const [exam, setExam] = useState<BeforeStartExam | null>(null);
  const [remainingMs, setRemainingMs] = useState(0);
  const [nowMs, setNowMs] = useState(() => Date.now());
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadExam = async () => {
      if (!token) {
        toast.error("Invalid Exam Link");
        setLoading(false);
        return;
      }

      const result = await beforeStartExam(token);

      if (!result.success) {
        toast.error(result.message);
        setLoading(false);
        return;
      }

      setExam(result.data);
      setRemainingMs(getRemainingMs(result.data.startDate));
      setLoading(false);
    };

    loadExam();
  }, [token]);

  useEffect(() => {
    if (!exam) return;

    const updateRemainingTime = () => {
      setNowMs(Date.now());
      setRemainingMs(getRemainingMs(exam.startDate));
    };

    updateRemainingTime();
    const timerId = window.setInterval(updateRemainingTime, 1000);

    return () => window.clearInterval(timerId);
  }, [exam]);

  const handleStartExam = async () => {
    if (!exam) return;

    const result = await startExam(exam.examId, exam.candidateId);

    if (!result.success) {
      toast.error(result.message);
      return;
    }

    const fullExamData = {
      ...exam,
      questions: result.data.questions,
    };

    sessionStorage.setItem("examData", JSON.stringify(fullExamData));

    router.push("/answer-exam");
  };

  const isBeforeStart = remainingMs > 0;
  const isAfterEnd = exam?.expiryDate
    ? new Date(exam.expiryDate).getTime() <= nowMs
    : false;
  const canStartExam = !!exam && !isBeforeStart && !isAfterEnd;

  return (
    <main className="min-h-screen bg-gray-100 flex items-center justify-center p-4">
      <div className="bg-white rounded-2xl shadow-md w-full max-w-md p-8">
        <div className="flex justify-center mb-5">
          <div className="bg-red-50 rounded-full p-4">
            <ClipboardDocumentListIcon className="w-8 h-8 opacity-70 text-primary" />
          </div>
        </div>

        <h1 className="text-title mb-2 text-center">
          {loading ? "Loading exam..." : exam?.title ?? "Undefined"}
        </h1>
        <p className="text-muted text-center mb-6">
          Enozom - Technical Screening
        </p>

        {isBeforeStart && (
          <div className="bg-blue-50 border border-blue-200 rounded-xl px-4 py-4 mb-5 text-center">
            <p className="text-sm font-medium text-blue-800 mb-2">
              Exam starts in
            </p>
            <p className="text-3xl font-bold text-blue-950 tabular-nums">
              {formatCountdown(remainingMs)}
            </p>
          </div>
        )}

        <div className="border border-gray-200 rounded-2xl divide-y divide-gray-200 mb-5">
          {[
            {
              label: "Duration",
              value: <span>{`${exam?.durationMins ?? 0} minutes`}</span>,
            },
            {
              label: "Questions",
              value: <span>{`${exam?.totalQuestions ?? 0} questions`}</span>,
            },
            {
              label: "Starts",
              value: <span>{formatWindowDate(exam?.startDate)}</span>,
            },
            {
              label: "Ends",
              value: <span>{formatWindowDate(exam?.expiryDate)}</span>,
            },
          ].map(({ label, value }) => (
            <div
              key={label}
              className="flex justify-between items-center gap-4 px-4 py-3"
            >
              <span className="text-muted font-normal">{label}</span>
              <span className="text-sm font-semibold text-gray-900 text-right">
                {value}
              </span>
            </div>
          ))}
        </div>

        <div className="bg-yellow-50 border border-yellow-200 rounded-xl px-4 py-3 mb-5 text-sm text-yellow-800">
          <span className="font-semibold">Instructions:</span>
          <br />
          - Once started, the timer cannot be paused.
          <br />
          - Do not refresh the page during the exam.
          <br />
          - You may navigate between questions freely.
          <br />- Submit before the timer runs out.
        </div>

        <Button
          text={
            isBeforeStart
              ? `Available in ${formatCountdown(remainingMs)}`
              : isAfterEnd
                ? "Exam Window Ended"
                : "Start Exam"
          }
          onClick={() => setOpen(true)}
          className="btn-primary w-full"
          disabled={!canStartExam}
        />

        {open && (
          <ConfirmDeleteModal
            onConfirm={handleStartExam}
            onCancel={() => setOpen(false)}
            title="Are you sure you want to proceed?"
            yesText="Proceed"
          />
        )}

        <p className="text-center text-xs text-gray-400 mt-4">
          Having trouble? Contact{" "}
          <a
            href="mailto:hr@enozom.com"
            className="text-blue-600 underline hover:text-blue-700"
          >
            hr@enozom.com
          </a>
        </p>
      </div>
    </main>
  );
}
