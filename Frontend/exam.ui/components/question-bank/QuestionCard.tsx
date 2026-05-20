
import type { APIQuestion } from "@/types/question";
import { TrashIcon, PencilSquareIcon } from "@heroicons/react/24/outline";

import type { Question } from "@/types/question";
import {getImageUrl} from "@/lib/api";
import QuestionTag from "../common/QuestionTag";

export type QuestionCardProps = {
  question: APIQuestion;
  onEdit: (question: APIQuestion) => void;
  onDelete: (questionId: number) => void;
};

export default function QuestionCard({ question, onEdit, onDelete }: QuestionCardProps) {
  return (
    <div className="card p-4">
      <div className="flex items-start justify-between gap-4">
        <QuestionTag title={question.topicTitle} />

        <div className="flex items-center gap-3 text-gray-500">
          <button type="button" onClick={() => onEdit(question)} aria-label="Edit question" className="btn-icon-secondary">
            <PencilSquareIcon className="icon-mid"/>
          </button>
          <button type="button" onClick={() => onDelete(question.id)} aria-label="Delete question" className="btn-icon-danger">
            <TrashIcon className="icon-mid"/>
          </button>
        </div>
      </div>

      <h3 className="mb-2 text-heading">{question.text}</h3>

      {question.imageUrl && (
        <img src={getImageUrl(question.imageUrl)} alt="Question image" className="mb-4 max-h-37 w-full object-contain rounded" />
      )}

      <ul className="space-y-1">
        {question.choices.map((choice, i) => (
          <li
            key={i}
            className={`flex items-start gap-2 ${choice.isCorrect ? "text-body text-emerald-600" : "text-muted"}`}
          >
            <span
              className={`mt-0.5 h-4 w-4 shrink-0 rounded-full border ${
                choice.isCorrect ? "border-emerald-500 bg-emerald-500" : "border-gray-400 bg-white"
              }`}
            />
            <span className="min-w-0 flex-1">
              {choice.imageUrl ? (
                <img src={getImageUrl(choice.imageUrl)} alt="Choice" className="max-h-16 object-contain" />
              ) : (
                <span>{choice.text}</span>
              )}
            </span>
          </li>
        ))}
      </ul>
    </div>
  );
}