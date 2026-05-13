"use client";

import Button from "@/components/Button";
import Modal from "@/components/Modal";
import { useState, useEffect } from "react";

type Question = {
  id: number;
  text: string;
  options: {
    id: number;
    text: string;
  }[];
};
const questions: Question[] = [
  {
    id: 101,
    text: "Select the correct React statement.",
    options: [
      { id: 11, text: "Option A" },
      { id: 12, text: "Option B" },
      { id: 13, text: "Option C" },
      { id: 14, text: "Option D" },
    ],
  },
  {
    id: 102,
    text: "Select the correct React statement.",
    options: [
      { id: 15, text: "Option A" },
      { id: 16, text: "Option B" },
      { id: 17, text: "Option C" },
      { id: 18, text: "Option D" },
    ],
  },
  {
    id: 103,
    text: "Select the correct React statement.",
    options: [
      { id: 19, text: "Option A" },
      { id: 20, text: "Option B" },
      { id: 21, text: "Option C" },
      { id: 22, text: "Option D" },
    ],
  },
];

export default function ExamPage() {
  const [current, setCurrent] = useState(0);
  const [answers, setAnswers] = useState<{ [questionId: number]: number }>({});
  const [timeLeft, setTimeLeft] = useState(310); // seconds
  const [open, setOpen] = useState(false);

  // Timer
  useEffect(() => {
    const interval = setInterval(() => {
      setTimeLeft((prev) => (prev > 0 ? prev - 1 : 0));
    }, 1000);
    return () => clearInterval(interval);
  }, []);

  const formatTime = (sec: number) => {
    const m = Math.floor(sec / 60);
    const s = sec % 60;
    return `${m}:${s.toString().padStart(2, "0")}`;
  };

  const answeredCount = Object.keys(answers).length;
  const question = questions[current];
  const TOTAL_QUESTIONS = questions.length; // will take from backend

  return (
    // flex-col lg:flex-row
    <div className="flex min-h-screen bg-gray-50"> 
      {/* Sidebar */}
      <div className="w-64 bg-white border-0 p-4 flex flex-col justify-between shadow-lg">
        <div>
          <h2 className="text-heading mb-2">Questions</h2>
          <p className="text-muted mb-4">{TOTAL_QUESTIONS} Total</p>

          <div className="grid grid-cols-5 gap-2 text-body">
            {questions.map((q, i) => {
              const num = i + 1;
              const isCurrent = i === current;
              const isAnswered = answers[q.id];

              return (
                <Button key={q.id} text={num.toString()} onClick={() => setCurrent(i)}
                  className={`h-10 rounded border text-sm hover:brightness-90 transition
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
        
        {/* Submit Modal */}
        <Modal open={open} onClose={() => setOpen(false)}>
          {/* Icon */}
          <div className="flex justify-center">
            <div className="w-18 h-18 rounded-full bg-yellow-100 flex items-center justify-center">
              <span className="text-4xl text-yellow-500">⚠</span>
            </div>
          </div>

          {/* Title */}
          <h2 className="text-heading text-center">
            Submit your exam?
          </h2>

          {/* Description */}
          <p className="text-center text-muted">
            You answered <span className="font-bold">{answeredCount}</span> of{" "}
            {TOTAL_QUESTIONS} <br></br>This cannot be undone
          </p>
          <div className="flex gap-3">   
              <Button text="Submit" onClick={() => setOpen(false)} className="btn-primary flex-1" />
              <Button text="Go Back" onClick={() => setOpen(false)} className="btn-secondary flex-1" />
          </div>
        </Modal>
      </div>

      {/* Main Content */}
      <div className="flex-1 p-8">
        {/* Header */}
        <div className="flex justify-between items-center mb-6">
          <div>
            <h1 className="text-lg font-semibold">
              React Developer Assessment
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

        {/* Question Card */}
        <div className="bg-white p-6 rounded-xl shadow-sm">
          <h2 className="text-lg font-semibold mb-4">
            Question {current}: {question.text}
          </h2>

          <div className="space-y-3">
            {question.options.map((opt, index) => (
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

        {/* Navigation */}
        <div className="flex justify-between mt-6">
          <Button text="← Previous" onClick={() => setCurrent((p) => Math.max(1, p - 1))} className="btn-secondary" />
          <Button text="Next →" onClick={() => setCurrent((p) => Math.min(TOTAL_QUESTIONS, p + 1))} className="btn-secondary" />
        </div>
      </div>
    </div>
  );
}