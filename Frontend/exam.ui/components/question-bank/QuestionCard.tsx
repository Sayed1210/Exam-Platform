import ChoiceItem from "./ChoiceItem";
import TrashIcon from "../TrashIcon";
import EditIcon from "../EditIcon";
import type { Question } from "@/types/question";

export type QuestionCardProps = {
  question: Question;
  onEdit: (question: Question) => void;
  onDelete: (questionId: string) => void;
};

export default function QuestionCard({ question, onEdit, onDelete }: QuestionCardProps) {
  return (
    <div className="card p-4">
      <div className="mb-4 flex items-start justify-between gap-4">
        <div className="flex flex-wrap items-center gap-2">
          <span className="rounded-full bg-primary/10 px-3 py-1 text-xs font-medium text-primary">
            {question.topic}
          </span>
        </div>

        <div className="flex items-center gap-3 text-gray-500">
          <button type="button" onClick={() => onEdit(question)} aria-label="Edit question"className="btn-icon-secondary">
  <EditIcon className="text-gray-400 transition group-hover:text-blue-500" />
</button>
          <button type="button" onClick={() => onDelete(question.id)} aria-label="Delete question" className="btn-icon-danger">
            <TrashIcon className="text-gray-400 transition group-hover:text-red-500"/>
          </button>
        </div>
      </div>

      <h3 className="mb-3 text-body font-semibold text-gray-900">{question.statement}</h3>

      {question.imageUrl && (
        <img src={question.imageUrl} alt="Loading image..." className="mb-4 max-h-37 w-full object-contain rounded" />
      )}

      <ul className="space-y-1">
        {question.choices.map((choice) => (
          <ChoiceItem key={choice.id} choice={choice} />
        ))}
      </ul>
    </div>
  );
}
