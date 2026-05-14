"use client";

import type { Question } from "@/types/question";
import QuestionForm from "./forms/QuestionForm";
import { XMarkIcon } from "@heroicons/react/24/outline";
import ModalPortal from "@/components/ModalPortal";

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
    <ModalPortal>
      <div className="modal-overlay">
        <section className="modal-panel max-w-3xl">
          <div className="modal-header">
            <h2 className="modal-title">
              {question ? "Edit Question" : "Add Question"}
            </h2>
            <button
              type="button"
              onClick={onClose}
              aria-label="Close question modal"
              className="modal-close-button"
            >
              <XMarkIcon className="h-5 w-5" />
            </button>
          </div>
          <div className="modal-body p-6">
            <QuestionForm topics={topics} question={question} onCancel={onClose} onSubmit={onSave} />
          </div>
        </section>
      </div>
    </ModalPortal>
  );
}
