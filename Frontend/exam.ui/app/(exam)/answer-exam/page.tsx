"use client";

import Button from "@/components/Button";
import Modal from "@/components/Modal";
import ConfirmDeleteModal from "@/components/common/ConfirmDeleteModal";
import PageLoader from "@/components/common/PageLoader";
import { submitExam } from "@/services/exam-service";
import { useRouter } from "next/navigation";
import { useState, useEffect } from "react";
import { toast } from "sonner";


export default function ExamPage() {
  const router = useRouter();
  const [current, setCurrent] = useState(0);
  const [answers, setAnswers] = useState<{ [questionId: number]: number }>({});
  const [timeLeft, setTimeLeft] = useState<number | null>(null);
  const [open, setOpen] = useState(false);
  const [exam, setExam] = useState<any>(null);
  
  const handleSubmitExam = async () => {
    const formattedAnswers = Object.entries(answers).map(
      ([questionId, choiceId]) => ({
        questionId: Number(questionId),
        choiceId: Number(choiceId),
      })
    );

    const result = await submitExam(
      exam.examId,
      exam.candidateId,
      formattedAnswers
    );

    if (!result.success) {
      toast.error(result.message);
      return;
    }

    setOpen(false);
    toast.success("Exam submitted successfully");
    localStorage.removeItem(`exam-end-time`);
    localStorage.removeItem("exam-answers");
    sessionStorage.removeItem("examData");
    router.push("/submitted-exam");
  };

  // get exam info with questions from beforeStartPage
  useEffect(() => {
    const storedExam = sessionStorage.getItem("examData");

    if (!storedExam) return;

    setExam(JSON.parse(storedExam));
  }, []);

  // timer handling
  useEffect(() => {
    if (!exam) return;

    // Create localStorage key
    const storageKey = "exam-end-time";

    // check if we saved exam ending time before
    let endTime = localStorage.getItem(storageKey);

    // First time opening exam (calculate exam duration/save final end timestamp)
    if (!endTime) {
      const durationInSeconds = exam.durationMins * 60;

      endTime = (
        Date.now() + durationInSeconds * 1000
      ).toString();

      localStorage.setItem(storageKey, endTime);
    }
    
    // SET TIMER IMMEDIATELY
    // const initialRemaining = Math.max(
    //   0,
    //   Math.floor((Number(endTime) - Date.now()) / 1000)
    // );
    // setTimeLeft(initialRemaining);

    // Start ticking every second
    const interval = setInterval(() => {
      // Calculate remaining time
      const remaining = Math.max(
        0,
        Math.floor((Number(endTime) - Date.now()) / 1000)
      );

      // Update UI timer
      setTimeLeft(remaining);

      // Auto submit
      if (remaining <= 0) {
        clearInterval(interval);

        handleSubmitExam();
      }
    }, 1000);

    return () => clearInterval(interval);
  }, [exam]);
  // load saved answers
  useEffect(() => {
    const savedAnswers = localStorage.getItem("exam-answers");

    if (savedAnswers) {
      setAnswers(JSON.parse(savedAnswers));
    }
  }, []);
  // save answers
  useEffect(() => {
    localStorage.setItem(
      "exam-answers",
      JSON.stringify(answers)
    );
  }, [answers]);

  if (!exam || timeLeft === null) {
    return <PageLoader />;
  }

  const formatTime = (sec: number) => {
    const m = Math.floor(sec / 60);
    const s = sec % 60;
    return `${m}:${s.toString().padStart(2, "0")}`;
  };

  const answeredCount = Object.keys(answers).length;
  const question = exam.questions[current];
  const TOTAL_QUESTIONS = exam.totalQuestions;

  return (
    // flex-col lg:flex-row
    <div className="flex min-h-screen bg-gray-50"> 
      {/* Sidebar */}
      <div className="w-64 bg-white border-0 p-4 flex flex-col justify-between shadow-lg">
        <div>
          <h2 className="text-heading mb-2">Questions</h2>
          <p className="text-muted mb-4">{TOTAL_QUESTIONS} Total</p>

          <div className="grid grid-cols-5 gap-2 text-body">
            {exam.questions.map((q: any, i: number) => {
              const num = i + 1;
              const isCurrent = i === current;
              const isAnswered = answers[q.id];

              return (
                <Button key={q.id} text={num.toString()} onClick={() => setCurrent(i)}
                  className={`cursor-pointer h-10 rounded border text-sm hover:brightness-90 transition
                    ${
                      isCurrent
                        ? "bg-blue-400 text-white border-0 shadow-md"
                        : isAnswered
                        ? "bg-green-200 border-0"
                        : "border-gray-300"
                    }
                  `}
                />
              );
            })}
          </div>

          {/* info */}
          <div className="mt-6 text-muted space-y-2">
            <div className="flex items-center gap-2">
              <span className="w-3 h-3 bg-blue-400 rounded shadow-sm" /> Current
            </div>
            <div className="flex items-center gap-2">
              <span className="w-3 h-3 bg-green-200 rounded shadow-sm" /> Answered
            </div>
            <div className="flex items-center gap-2">
              <span className="w-3 h-3 border rounded shadow-sm" /> Unanswered
            </div>
          </div>
        </div>

        {/* Submit Button */}
        <Button text="Submit Exam" className="btn-primary w-full" onClick={() => setOpen(true)} />
        {open && (<ConfirmDeleteModal 
          onConfirm={handleSubmitExam}
          onCancel={() => setOpen(false)}
          title="Submit your exam?"
          text={
            <p className="text-body">
              You answered <span className="font-bold text-primary">{answeredCount}</span> 
              {" "} of{" "} {TOTAL_QUESTIONS} questions
              <br />
            </p>
          }
          yesText="Submit"
          noText="Go Back"
        >
        </ConfirmDeleteModal>
        )}
      </div>

      {/* Main Content */}
      <div className="flex-1 h-screen p-8 flex flex-col overflow-hidden">
        {/* Header */}
        <div className="flex justify-between items-center mb-6">
          <div>
            <h1 className="text-lg font-semibold">
              {exam.title} 
            </h1>
            <p className="text-muted">
              Question {current + 1} of {TOTAL_QUESTIONS}
            </p>
          </div>

          {/* Timer Box */}
      <div
        className={`
          w-42 h-18 flex items-center justify-center rounded-xl border
          text-title transition-colors shadow-sm
          ${
            timeLeft <= 300
              ? "bg-red-100 border-red-200 text-primary"
              : "bg-green-100 border-green-200 text-black"
          }
        `}
      >
        {formatTime(timeLeft)}
      </div>
        </div>

        {/* Progress */}
        <div className="mb-6">
          <div className="h-2 bg-gray-200 rounded shadow-sm overflow-hidden">
            <div
              className="h-2 bg-blue-500 rounded transition-all duration-500 ease-in-out"
              style={{
                width: `${(answeredCount / TOTAL_QUESTIONS) * 100}%`,
              }}
            />
          </div>
          <p className="text-muted mt-2">
            {answeredCount} of {TOTAL_QUESTIONS} answered
          </p>
        </div>

        <div className="flex-1 overflow-y-auto pr-2 rounded-xl">
          {/* Question Card */}
          <div className="bg-white p-6 rounded-xl shadow-sm">
            <h2 className="text-lg font-semibold mb-4">
              Question {current + 1}: {question.text}
            </h2>

            <div className="space-y-3">
              {question.choices.map((opt: any) => (
                <label
                  key={opt.id}
                  className={`text-body flex items-center gap-3 border rounded-2xl p-3 cursor-pointer
                    ${
                      answers[question.id] === opt.id
                        ? "border-blue-100 bg-blue-100"
                        : "border-gray-300"
                    }
                  `}
                >
                  <input
                    type="radio"
                    name={`q-${question.id}`}
                    checked={answers[question.id] === opt.id}
                    onChange={() =>
                      setAnswers((prev) => ({
                        ...prev,
                        [question.id]: opt.id,
                      }))
                    }
                  />
                  {opt.text}
                </label>
              ))}
            </div>
          </div>
        </div>
        {/* Navigation */}
        <div className="flex justify-end gap-3 mt-2">
          <button onClick={() => setCurrent((p) => Math.max(0, p - 1))} className="btn-secondary">← Previous</button>
          <button onClick={() => setCurrent((p) => Math.min(TOTAL_QUESTIONS - 1, p + 1))} className="btn-secondary">Next →</button>
        </div>
      </div>
    </div>
  );
}