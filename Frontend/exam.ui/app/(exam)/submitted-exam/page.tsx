"use client";

export default function SubmittedExamPage() {
    
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
                d="M5 13l4 4L19 7"
              />
            </svg>
          </div>
        </div>

        {/* Title */}
        <h1 className="text-title mb-4 text-center">
          Exam Submitted
        </h1>

        {/* Instructions */}
        <div className="bg-red-50 border-red-200 rounded-xl px-4 py-3 mb-5 text-sm text-black">
          <span className="font-semibold">What's Next?</span> 
          <br></br>• Review of your exam by the HR team
          <br></br>• Technical interview if shortlisted
          <br></br>• Final feedback communicated via email
        </div> 

        {/* Footer */}
        <p className="text-center text-xs text-gray-400 mt-4">
          Having trouble? Contact{" "}
          <a
            href="mailto:hr@enozom.com"
            className="text-blue-600 underline hover:brightness-85"
          >
            hr@enozom.com
          </a>
        </p>
      </div>
    </main>
  );
}