"use client";

import Button from "@/components/Button";
import Modal from "@/components/Modal";
import { useState } from "react";

export default function StartExamPage() {
    const [open, setOpen] = useState(false);
    
  return (
    <main className="min-h-screen bg-gray-100 flex items-center justify-center p-4">
      <div className="bg-white rounded-2xl shadow-md w-full max-w-md p-8">
        {/* Icon */}
        <div className="flex justify-center mb-5">
          <div className="bg-red-50 rounded-full p-4">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              className="w-8 h-8 text-red-600"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
              strokeWidth={1.5}
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"
              />
            </svg>
          </div>
        </div>

        {/* Title */}
        <h1 className="text-title mb-2 text-center">
          React Developer Assessment
        </h1>
        <p className="text-muted text-center mb-6">
          Enozom Software Engineering • Technical Screening
        </p>

        {/* Info Table */}
        <div className="border border-gray-200 rounded-2xl divide-y divide-gray-200 mb-5">
          {[
            { label: "Duration", value: "60 minutes" },
            { label: "Questions", value: "25 questions" },
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
        {/* <button className="w-full bg-blue-600 hover:bg-blue-700 active:bg-blue-800 transition-colors text-white font-semibold text-base rounded-xl py-4 flex items-center justify-center gap-2">
          Start Exam
          <span className="text-lg">→</span>
        </button> */}
        <Button text="Start Exam" onClick={() => setOpen(true)} className="btn-primary"/>
        {/* Popup Window */}
        <Modal open={open} onClose={() => setOpen(false)}>
            <div className="flex flex-col gap-2 text-center">
                <h3 className="text-heading mb-2">Are you sure you want to proceed?</h3>
                <p className="text-muted">The timer will start immediately and <br></br> cannot be paused once exam begins</p>
            </div>
            
            <div className="flex gap-3">  
                <Button text="Proceed" onClick={() => setOpen(false)} className="btn-primary flex-1" />
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