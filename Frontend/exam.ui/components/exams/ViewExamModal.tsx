'use client';
import Button from '../Button';
import ExamModal from './ExamModal';
import QuestionCard from '@/components/question-bank/QuestionCard';
import type { APIQuestion } from '@/types/question';
import { ClockIcon, QuestionMarkCircleIcon } from '@heroicons/react/24/outline';

interface RemoteChoice {
  id?: number;
  text?: string | null;
  isCorrect: boolean;
  imageUrl?: string | null;
}

interface RemoteQuestion {
  id: number;
  topicId?: number;
  text: string;
  imageUrl?: string | null;
  choices: RemoteChoice[];
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
  const questionsToDisplay: APIQuestion[] =
    exam.questions?.map((question) => ({
      id: question.id,
      topicId: question.topicId ?? 0,
      text: question.text,
      imageUrl: question.imageUrl,
      topicTitle: question.topicTitle ?? "",
      choices: (question.choices ?? []).map((choice, index) => ({
        id: choice.id ?? index,
        text: choice.text ?? "",
        imageUrl: choice.imageUrl ?? null,
        isCorrect: choice.isCorrect,
      })),
    })) ?? [];

  return (
    <ExamModal onClose={onClose} title={exam.title}>
      {/* Summary Header */}
      <div className="px-7 py-4 bg-slate-50/50 border-b border-slate-100 flex items-center justify-between sticky top-0 z-10">
        <div className="flex items-center gap-4 text-slate-500 text-sm">
          <span className="flex items-center gap-1.5"><ClockIcon className="w-4 h-4" /> {exam.durationMins} min</span>
          <span className="text-slate-300">|</span>
          <span className="flex items-center gap-1.5"><QuestionMarkCircleIcon className="w-4 h-4" /> {exam.totalQuestions} questions</span>
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
          <div className="text-center nav-section-label">
            [ No questions in this exam ]
          </div>
          ) : (
          questionsToDisplay.map((question) => (
            <QuestionCard key={question.id} question={question} />
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
