import type { Question } from "@/types/question";
import QuestionForm from "./forms/QuestionForm";

export type QuestionModalProps = {
  isOpen: boolean;
  topics: string[];
  question?: Question | null;
  onClose: () => void;
  onSave: (question: Question) => void;
};

export default function QuestionModal({
  isOpen,
  topics,
  question,
  onClose,
  onSave,
}: QuestionModalProps) {
  if (!isOpen) {
    return null;
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center overflow-y-auto bg-slate-950/45 p-4">
      <section className="my-8 w-full max-w-3xl rounded-lg bg-white p-6 shadow-xl">
        <div className="mb-6 flex items-center justify-between gap-4">
          <h2 className="text-xl font-bold text-slate-950">
            {question ? "Edit Question" : "Add Question"}
          </h2>
          <button
            type="button"
            onClick={onClose}
            aria-label="Close question modal"
            className="rounded-md p-2 text-slate-500 transition hover:bg-slate-100 hover:text-slate-900"
          >
            ×
          </button>
        </div>
        <QuestionForm topics={topics} question={question} onCancel={onClose} onSubmit={onSave} />
      </section>
    </div>
  );
}
