"use client";
// http://localhost:3000/start-exam?token=8e709534-d2e6-4491-954e-c7848cb38a4f

import Button from "@/components/Button";
import Modal from "@/components/Modal";
import { ClipboardDocumentListIcon } from '@heroicons/react/24/outline'; 
import { useEffect, useState } from "react";
import { useSearchParams } from "next/navigation";
import { beforeStartExam, startExam } from "@/services/exam-service";
import { useRouter } from "next/navigation";
import { toast } from "sonner";

export default function StartExamPage() {
  const router = useRouter();
  const searchParams = useSearchParams();

  const token = searchParams.get("token");

  const [open, setOpen] = useState(false);
  // const [loading, setLoading] = useState(false);

  const [exam, setExam] = useState<any>(null);
  // const [pageLoading, setPageLoading] = useState(false);
  
  useEffect(() => {
    const loadExam = async () => {
      if (!token) {
        toast.error("Invalid reset link");
        return;
      }

      const result = await beforeStartExam(token);

      if (!result.success) {
        toast.error(result.message);
        return;
      }

      setExam(result.data);
      // setPageLoading(false);
    };

    loadExam();
  }, [token]);

  const handleStartExam = async () => {
    if (!exam) return;

    try {
      // setLoading(true);
      const result = await startExam(
        exam.examId,
        exam.candidateId
      );

      if (!result.success) {
        toast.error(result.message);
        return;
      }

      const fullExamData = {
      ...exam,
      questions: result.data.questions,
    };

    sessionStorage.setItem(
      "examData",
      JSON.stringify(fullExamData)
    );

    router.push("/answer-exam");

    } finally {
      // setLoading(false);
    }
  };

  return (
    <main className="min-h-screen bg-gray-100 flex items-center justify-center p-4">
      <div className="bg-white rounded-2xl shadow-md w-full max-w-md p-8">
        {/* Icon */}
        <div className="flex justify-center mb-5">
          <div className="bg-red-50 rounded-full p-4">
            <ClipboardDocumentListIcon className="w-8 h-8 opacity-70 text-primary" />
          </div>
        </div>

        {/* Title */}
        <h1 className="text-title mb-2 text-center">
          {exam?.title ?? "Undefined"}
        </h1>
        <p className="text-muted text-center mb-6">
          Enozom • Technical Screening
        </p>

        {/* Info Table */}
        <div className="border border-gray-200 rounded-2xl divide-y divide-gray-200 mb-5">
          {[
            { label: "Duration", value: <span>{`${exam?.durationMins} minutes`}</span> },
            { label: "Questions", value: <span>{`${exam?.totalQuestions} questions`}</span>},
          ].map(({ label, value }) => (
            <div key={label} className="flex justify-between items-center px-4 py-3">
              <span className="text-muted font-normal">{label}</span>
              <span className="text-sm font-semibold text-gray-900">{value}</span>
            </div>
          ))}
        </div>

        {/* Instructions */}
        <div className="bg-yellow-50 border border-yellow-200 rounded-xl px-4 py-3 mb-5 text-sm text-yellow-800">
          <span className="font-semibold">⚠ Instructions:</span> 
          <br></br>• Once started, the timer cannot be paused. 
          <br></br>• Do not refresh the page during the exam. 
          <br></br>• You may navigate between questions freely.
          <br></br>• Submit before the timer runs out.
        </div>

        {/* Start Button */}
        <Button text="Start Exam" onClick={() => setOpen(true)} className="btn-primary"/>
        {/* Popup Window */}
        <Modal open={open} onClose={() => setOpen(false)}>
            <div className="flex flex-col gap-2 text-center">
                <h3 className="text-heading mb-2">Are you sure you want to proceed?</h3>
                <p className="text-muted">The timer will start immediately and <br></br> cannot be paused once exam begins</p>
            </div>
            
            <div className="flex gap-3">  
                <Button 
                text="Proceed" 
                onClick={handleStartExam}
                className="btn-primary flex-1"
                // loading={loading}
                // loadingText="Opening..." 
                />
                <Button text="Cancel" onClick={() => setOpen(false)} className="btn-secondary w-full flex-1" />
            </div>
        </Modal>    

        {/* Footer */}
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