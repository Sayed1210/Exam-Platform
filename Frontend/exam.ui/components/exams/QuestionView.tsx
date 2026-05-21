'use client';

import TopicBadge from '@/components/exams/TopicBadge';
import OptionView from '@/components/exams/OptionView';
import { getImageUrl } from '@/lib/api';


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

interface QuestionViewProps {
  question: Question;
  idx: number;
}

export default function QuestionView({ question, idx }: QuestionViewProps) {
  return (
        <div className="card p-4">
        <div className="flex flex-wrap items-center justify-between gap-3 mb-3">
          <TopicBadge title={question.topicTitle} />

          <span className="text-xs font-semibold uppercase tracking-[0.18em] text-slate-400">
            Q{idx + 1}
          </span>
        </div>

        <p className="mb-2 text-body font-semibold text-gray-900 ">
          {question.text}
        </p>

        {question.imageUrl && (
          <div className="mt-2 rounded-[28px] border border-slate-200 bg-slate-50 p-4">
            <img
              src={getImageUrl(question.imageUrl)}
              alt={`Question ${idx + 1}`}
              className="w-full max-h-56 rounded-3xl object-contain"
            />
          </div>
        )}

        <div className="mt-5 ">
          {question.options.map((opt, oIdx) => (
            <OptionView key={oIdx} option={opt} oIdx={oIdx} />
          ))}
        </div>
      
    </div>
  );
}
