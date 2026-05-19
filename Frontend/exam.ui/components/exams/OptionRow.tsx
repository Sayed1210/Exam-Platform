'use client';

import { PhotoIcon, XMarkIcon } from '@heroicons/react/24/outline';
import type { ChangeEvent } from 'react';
import type { Option } from '@/components/exams/types';

interface OptionRowProps {
  qIdx: number;
  oIdx: number;
  option: Option;
  isChecked: boolean;
  onCorrectSelect: () => void;
  onTextChange: (value: string) => void;
  onImageUpload: (event: ChangeEvent<HTMLInputElement>) => void;
  onRemoveImage: () => void;
  onRemoveOption: () => void;
  canRemove: boolean;
}

export default function OptionRow({
  qIdx,
  oIdx,
  option,
  isChecked,
  onCorrectSelect,
  onTextChange,
  onImageUpload,
  onRemoveImage,
  onRemoveOption,
  canRemove,
}: OptionRowProps) {
  return (
    <div className="flex items-start gap-3 group">
      <input
        type="radio"
        name={`correct-${qIdx}`}
        checked={isChecked}
        onChange={onCorrectSelect}
        className="accent-primary w-4 h-4 cursor-pointer mt-3"
      />
      <div className="flex-1 space-y-1.5">
        <input
          placeholder={`Option ${oIdx + 1}`}
          className="w-full p-3 border border-slate-100 rounded-xl text-sm outline-none focus:border-primary bg-white shadow-sm"
          value={option.text ?? ''}
          onChange={(e) => onTextChange(e.target.value)}
        />
        {option.imageUrl && (
          <div className="relative group w-fit">
            <img
              src={option.imageUrl}
              alt={`Option ${oIdx + 1}`}
              className="w-auto max-h-32 rounded-xl border border-slate-100 bg-white"
            />
            <button
              onClick={onRemoveImage}
              className="absolute top-1.5 right-1.5 bg-slate-800/70 p-1 rounded-full text-white opacity-0 group-hover:opacity-100 transition-opacity"
            >
              <XMarkIcon className="w-3.5 h-3.5" />
            </button>
          </div>
        )}
      </div>
      <div className="flex items-center gap-1.5 mt-2.5">
        <input
          type="file"
          id={`option-image-${qIdx}-${oIdx}`}
          className="hidden"
          accept="image/*"
          onChange={onImageUpload}
        />
        <label
          htmlFor={`option-image-${qIdx}-${oIdx}`}
          className="text-slate-300 hover:text-slate-600 cursor-pointer p-1.5 border border-slate-100 bg-white rounded-lg"
        >
          <PhotoIcon className="w-3.5 h-3.5" />
        </label>
        {canRemove && (
          <button
            onClick={onRemoveOption}
            className="text-slate-300 hover:text-red-400 opacity-0 group-hover:opacity-100 transition-all"
          >
            <XMarkIcon className="w-4 h-4" />
          </button>
        )}
      </div>
    </div>
  );
}
