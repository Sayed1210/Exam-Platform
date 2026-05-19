'use client';
import Button from '../Button';
import ExamModal from './ExamModal';

interface Option {
  text: string;
  isCorrect: boolean;
  imageUrl?: string | null;
}

interface Question {
  text: string;
  imageUrl?: string | null;
  options: Option[];
}

interface RemoteQuestion {
  id: number;
  text: string;
  imageUrl?: string | null;
  choices: Option[];
}

interface ViewExamModalProps {
  isLoading: boolean;
  exam: {
    id: number;
    title: string;
    durationMins: number;
    totalQuestions: number;
    questions?: RemoteQuestion[];
  };
  onClose: () => void;
  onEdit: (exam: any) => void;
}

export default function ViewExamModal({ exam, isLoading, onClose, onEdit }: ViewExamModalProps) {
  const questionsToDisplay: Question[] =
    exam.questions?.map((question) => ({
      text: question.text,
      options: question.choices ?? [],
      imageUrl: question.imageUrl,
    })) ?? [];

  return (
    <ExamModal onClose={onClose} title={exam.title}>
      {/* Summary Header */}
      <div className="px-8 py-4 bg-slate-50/50 border-b border-slate-100 flex items-center justify-between sticky top-0 z-10">
        <div className="flex gap-6 items-center text-slate-500">
          <span className="text-xs font-bold">{exam.durationMins} mins</span>
          <span className="text-xs font-bold">{exam.totalQuestions} Questions</span>
        </div>
        {/* Re-added the Edit button to match your initial requirement */}
        <button 
          onClick={() => onEdit(exam)}
          className="text-primary hover:underline text-xs font-bold"
        >
          Edit Exam
        </button>
      </div>

      {/* Content Area */}
      <div className="p-8 space-y-8 overflow-y-auto custom-scrollbar h-[500px] max-h-[60vh]">
        {isLoading ? (
          <div className="flex flex-col items-center justify-center h-full space-y-4">
            <div className="w-8 h-8 border-4 border-primary border-t-transparent rounded-full animate-spin"></div>
            <p className="text-slate-400 text-sm italic font-medium">Fetching exam from database...</p>
          </div>
        ) : questionsToDisplay.length === 0 ? (
          <div className="text-center py-20 text-slate-500 text-sm">
            No questions were returned by the database for this exam.
          </div>
        ) : (
          questionsToDisplay.map((q, idx) => (
            <div key={idx} className="space-y-4">
              {/* Question Heading */}
              <div className="flex gap-3 items-start">
                <span className="bg-primary/10 text-primary text-[10px] font-black px-2 py-1 rounded-lg mt-0.5">
                  Q{idx + 1}
                </span>
                <div>
                  <p className="text-sm font-bold text-slate-800 leading-relaxed">
                    {q.text}
                  </p>
                  {q.imageUrl && (
                    <div className="mt-3">
                      <img src={q.imageUrl} alt={`Question ${idx + 1}`} className="w-full max-h-56 rounded-3xl border border-slate-100 bg-white object-contain" />
                    </div>
                  )}
                </div>
              </div>

              {/* Options Grid */}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-3 pl-10">
                {q.options.map((opt, oIdx) => (
                  <div 
                    key={oIdx}
                    className={`p-3 rounded-xl border text-xs transition-all ${
                      opt.isCorrect 
                        ? 'border-emerald-200 bg-emerald-50 text-emerald-700 font-bold' 
                        : 'border-slate-100 bg-white text-slate-500'
                    }`}
                  >
                    <div className="flex items-center gap-2 mb-2">
                      <span className="opacity-50">{String.fromCharCode(65 + oIdx)}.</span>
                      <span className="font-semibold">{opt.isCorrect ? 'Correct' : 'Choice'}</span>
                    </div>
                    {opt.imageUrl ? (
                      <img src={opt.imageUrl} alt={`Option ${oIdx + 1}`} className="w-full max-h-44 rounded-2xl object-contain" />
                    ) : (
                      <p>{opt.text}</p>
                    )}
                  </div>
                ))}
              </div>
            </div>
          ))
        )}
      </div>

      <div className="p-6 border-t border-slate-100 flex justify-end bg-white sticky bottom-0 z-10 rounded-b-3xl">
        <Button text="Close" onClick={onClose} className="btn-secondary !mt-0 px-8" />
      </div>
    </ExamModal>
  );
}