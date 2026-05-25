import type { APIQuestion } from "@/types/question";
import { TrashIcon, PencilSquareIcon, XMarkIcon } from "@heroicons/react/24/outline";

// import type { Question } from "@/types/question";
import {getImageUrl} from "@/lib/api";
import QuestionTag from "../common/QuestionTag";
import { useState } from "react";

export type QuestionCardProps = {
  question: APIQuestion;
  onEdit?: (question: APIQuestion) => void;
  onDelete?: (questionId: number) => void;
};

export default function QuestionCard({ question, onEdit, onDelete }: QuestionCardProps) {
  const [previewImage, setPreviewImage] = useState<string | null>(null);

  return (
    <div className="card p-4">
      <div className="flex items-start justify-between gap-4">
        <QuestionTag title={question.topicTitle} />

        {(onEdit || onDelete) && (
          <div className="flex items-center gap-3 text-gray-500">
            {onEdit && (
              <button type="button" onClick={() => onEdit(question)} aria-label="Edit question" className="btn-icon-secondary">
                <PencilSquareIcon className="icon-mid"/>
              </button>
            )}
            {onDelete && (
              <button type="button" onClick={() => onDelete(question.id)} aria-label="Delete question" className="btn-icon-danger">
                <TrashIcon className="icon-mid"/>
              </button>
            )}
          </div>
        )}
      </div>

      <h3 className="mb-2 text-heading">{question.text}</h3>

      {question.imageUrl && (
        <div className="mb-4 flex justify-center">
          <img 
            src={getImageUrl(question.imageUrl)} 
            alt="Question image" 
            className="h-30 w-50 object-contain rounded-lg cursor-zoom-in transition hover:opacity-90" 
            onClick={() => setPreviewImage(getImageUrl(question.imageUrl))}
          />
        </div>
      )}

      <ul className="space-y-1">
        {question.choices.map((choice, i) => (
          <li
            key={i}
            className={`flex items-start gap-2 ${
              choice.isCorrect
                ? "text-body rounded-xl border border-emerald-200 bg-emerald-50 px-1 py-1.5"
                : "text-muted px-1"
            }`}
          >
            <span
              className={`mt-1 h-4 w-4 shrink-0 rounded-full border ${
                choice.isCorrect
                  ? "border-emerald-500 bg-emerald-500"
                  : "border-gray-400 bg-white"
              }`}
            />
            <span className="min-w-0 flex-1">
              {choice.text ? (
                <span className="block break-words">{choice.text}</span>
              ) : null}

              {choice.imageUrl ? (
                <img
                  src={getImageUrl(choice.imageUrl)}
                  alt="Choice"
                  className="mt-2 h-18 w-24 cursor-zoom-in rounded-lg object-cover transition hover:opacity-90"
                  onClick={() => setPreviewImage(getImageUrl(choice.imageUrl))}
                />
              ) : null}
            </span>
          </li>
        ))}
      </ul>
      {previewImage && (
      <div
        onClick={() => setPreviewImage(null)}
        className="fixed inset-0 z-50 flex items-center justify-center bg-black/90 p-4"
      >
        <img
          src={previewImage}
          alt="Preview"
          className="max-h-[90vh] max-w-[90vw] rounded-xl object-contain"
          onClick={(e) => e.stopPropagation()}
        />

        <button
          onClick={() => setPreviewImage(null)}
          className="cursor-pointer absolute top-5 right-5 text-3xl text-gray-200 transition hover:opacity-80"
        >
          <XMarkIcon className="w-[22px] h-[22px] stroke-[3] shadow-2xl"/>
        </button>
      </div>
    )}
    </div>
  );
}
