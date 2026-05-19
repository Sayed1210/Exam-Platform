'use client';
import Button from '../Button';
import ExamModal from './ExamModal';
import QuestionView from '@/components/exams/QuestionView';

interface Option {
  text: string;
  isCorrect: boolean;
  imageUrl?: string | null;
}

interface Question {
  text: string;
  imageUrl?: string | null;
  options: Option[];
  topicTitle?: string;
}

interface RemoteQuestion {
  id: number;
  text: string;
  imageUrl?: string | null;
  choices: Option[];
  topicTitle?: string;
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
      topicTitle: question.topicTitle,
    })) ?? [];

  return (
    <ExamModal onClose={onClose} title={exam.title}>
      {/* Summary Header */}
      <div className="px-8 py-4 bg-slate-50/50 border-b border-slate-100 flex items-center justify-between sticky top-0 z-10">
        <div className="flex gap-6 items-center text-slate-500">
          <span className="text-xs font-bold">{exam.durationMins} mins</span>
          <span className="text-xs font-bold">{exam.totalQuestions} Questions</span>
        </div>
        <button 
          onClick={() => onEdit(exam)}
          className="text-primary hover:underline text-xs font-bold"
        >
          Edit Exam
        </button>
      </div>

      {/* Content Area */}
      <div className="py-4 px-8 space-y-8 overflow-y-auto custom-scrollbar h-[500px] max-h-[60vh]">
        {isLoading ? (
          <div className="flex flex-col items-center justify-center h-full space-y-4">
            <div className="w-8 h-8 border-4 border-primary border-t-transparent rounded-full animate-spin"></div>
            <p className="text-slate-400 text-sm italic font-medium">Fetching exam from database...</p>
          </div>
        ) : questionsToDisplay.length === 0 ? (
          <div className="text-center py-20 text-slate-500 text-sm">
            No questions for this exam.
          </div>
          ) : (
          questionsToDisplay.map((q, idx) => (
            <QuestionView key={idx} question={q} idx={idx} />
          ))
        )}
      </div>

      {/* Fixed Footer */}
      <div className="p-6 border-t border-slate-100 flex justify-end bg-white sticky bottom-0 z-10 rounded-b-3xl">
        <Button text="Close" onClick={onClose} className="btn-secondary !mt-0 px-8" />
      </div>
    </ExamModal>
  );
}