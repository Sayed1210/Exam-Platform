'use client';

import type { Option } from '@/components/exams/types';

interface OptionViewProps {
  option: Option;
  oIdx: number;
}

export default function OptionView({ option, oIdx }: OptionViewProps) {
  return (
    <div className="flex items-start gap-4 py-1">
      <div className="flex-none pt-1">
        <span className={`block rounded-full transition-colors ${
          option.isCorrect ? 'h-4 w-4 bg-emerald-600' : 'h-4 w-4 border border-slate-300 bg-white'
        }`}>
        </span>
      </div>

      <div className="flex flex-col gap-3 w-full">
        {option.text && (
          <p className={`${option.isCorrect ? 'text-emerald-600 text-sm' : 'text-slate-500 text-sm'}`}>
            {option.text}
          </p>
        )}

        {option.imageUrl && (
          <div className="rounded-2xl border border-slate-100 bg-white p-3 max-w-sm shadow-sm">
            <img
              src={option.imageUrl}
              alt={`Choice ${oIdx + 1}`}
              className="w-full max-h-40 rounded-xl object-contain"
            />
          </div>
        )}
      </div>
    </div>
  );
}
