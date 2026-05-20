'use client';

import { PlusIcon, PhotoIcon, TrashIcon, XMarkIcon } from '@heroicons/react/24/outline';
import type { ChangeEvent } from 'react';
import type { QuestionForm, Option } from '@/components/exams/types';
import OptionRow from '@/components/exams/OptionRow';

interface QuestionEditorProps {
  q: QuestionForm;
  qIdx: number;
  topicOptions: string[];
  topics: { id: number; title: string }[];
  getError: (path: string) => string;
  onTopicChange: (value: string) => void;
  onRemoveQuestion: () => void;
  onQuestionTextChange: (value: string) => void;
  onQuestionImageUpload: (event: ChangeEvent<HTMLInputElement>) => void;
  onRemoveQuestionImage: () => void;
  onOptionAdd: () => void;
  onOptionTextChange: (oIdx: number, value: string) => void;
  onOptionCorrectSelect: (oIdx: number) => void;
  onOptionImageUpload: (oIdx: number, event: ChangeEvent<HTMLInputElement>) => void;
  onOptionRemoveImage: (oIdx: number) => void;
  onOptionRemove: (oIdx: number) => void;
}

export default function QuestionEditor({
  q,
  qIdx,
  topicOptions,
  topics,
  getError,
  onTopicChange,
  onRemoveQuestion,
  onQuestionTextChange,
  onQuestionImageUpload,
  onRemoveQuestionImage,
  onOptionAdd,
  onOptionTextChange,
  onOptionCorrectSelect,
  onOptionImageUpload,
  onOptionRemoveImage,
  onOptionRemove,
}: QuestionEditorProps) {
  return (
    <div className="p-6 border border-slate-100 rounded-3xl bg-slate-50/30 relative shadow-sm">
      <div className="flex justify-between items-center mb-4">
        <div className="flex-1">
          <select
            className="text-xs font-bold p-2 bg-white border border-slate-200 rounded-lg outline-none focus:border-primary text-slate-500 w-full"
            value={q.topic}
            onChange={(e) => onTopicChange(e.target.value)}
          >
            <option value="">Select Topic</option>
            {topicOptions.map((t) => (
              <option key={t} value={t}>{t}</option>
            ))}
          </select>
          {getError(`questions.${qIdx}.topic`) && (
            <p className="text-red-500 text-xs mt-1">{getError(`questions.${qIdx}.topic`)}</p>
          )}
        </div>
        <div className="flex items-center gap-2">
          <input
            type="file"
            id={`question-image-${qIdx}`}
            className="hidden"
            accept="image/*"
            onChange={onQuestionImageUpload}
          />
          <label
            htmlFor={`question-image-${qIdx}`}
            className="text-slate-400 hover:text-slate-600 cursor-pointer p-1.5 rounded-lg border border-slate-100 bg-white"
          >
            <PhotoIcon className="w-4 h-4" />
          </label>
          <button
            onClick={onRemoveQuestion}
            className="text-slate-300 hover:text-red-500 transition-colors"
          >
            <TrashIcon className="w-5 h-5" />
          </button>
        </div>
      </div>

      <div className="mb-4 space-y-2">
        <textarea
          placeholder="Enter question text..."
          className="w-full p-4 bg-white border border-slate-100 rounded-2xl outline-none focus:border-primary text-sm shadow-sm"
          value={q.text}
          onChange={(e) => onQuestionTextChange(e.target.value)}
        />
        {getError(`questions.${qIdx}.text`) && (
          <p className="text-red-500 text-xs mt-1">{getError(`questions.${qIdx}.text`)}</p>
        )}
        {q.imageUrl && (
          <div className="relative group w-fit">
            <img
              src={q.imageUrl}
              alt="Question"
              className="w-auto max-h-40 rounded-xl border border-slate-100 bg-white"
            />
            <button
              onClick={onRemoveQuestionImage}
              className="absolute top-2 right-2 bg-slate-800/70 p-1.5 rounded-full text-white opacity-0 group-hover:opacity-100 transition-opacity"
            >
              <XMarkIcon className="w-4 h-4" />
            </button>
          </div>
        )}
      </div>

      <div className="space-y-3">
        {q.options.map((opt: Option, oIdx: number) => (
          <OptionRow
            key={oIdx}
            qIdx={qIdx}
            oIdx={oIdx}
            option={opt}
            isChecked={opt.isCorrect}
            onCorrectSelect={() => onOptionCorrectSelect(oIdx)}
            onTextChange={(value) => onOptionTextChange(oIdx, value)}
            onImageUpload={(event) => onOptionImageUpload(oIdx, event)}
            onRemoveImage={() => onOptionRemoveImage(oIdx)}
            onRemoveOption={() => onOptionRemove(oIdx)}
            canRemove={q.options.length > 1}
          />
        ))}
        {getError(`questions.${qIdx}.options`) && (
          <p className="text-red-500 text-xs mt-1">{getError(`questions.${qIdx}.options`)}</p>
        )}
        <button
          onClick={onOptionAdd}
          className="flex items-center gap-1 text-xs font-bold text-primary hover:opacity-80 transition-opacity"
        >
          <PlusIcon className="w-3 h-3 stroke-[3px]" /> Add Choice
        </button>
      </div>
    </div>
  );
}
