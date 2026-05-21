'use client';

import { CheckIcon } from '@heroicons/react/24/outline';
import type { QuestionForm } from '@/components/exams/types';

interface QuestionBankItemProps {
  question: QuestionForm;
  selected: boolean;
  onToggle: () => void;
}

export default function QuestionBankItem({ question, selected, onToggle }: QuestionBankItemProps) {
  return (
    <div
      onClick={onToggle}
      className={`p-5 border rounded-3xl cursor-pointer transition-all flex items-start gap-4 ${
        selected ? 'border-primary bg-red-50/10' : 'border-slate-100 bg-white hover:border-slate-200 shadow-sm'
      }`}
    >
      <div className={`mt-1 w-5 h-5 rounded flex items-center justify-center border ${selected ? 'bg-primary border-primary' : 'border-slate-300'}`}>
        {selected && <CheckIcon className="w-4 h-4 text-white stroke-[3px]" />}
      </div>
      <div className="flex-1">
        <p className="text-sm font-bold text-slate-800 mb-1">{question.text}</p>
        <div className="flex gap-2">
          <span className="text-[10px] px-2 py-0.5 bg-slate-100 rounded-full font-bold text-slate-500 uppercase">{question.topic}</span>
          {/* <span className="text-[10px] text-slate-400 font-medium">{question.options.length} options</span> */}
        </div>
      </div>
    </div>
  );
}
