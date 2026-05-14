"use client";

import TopicForm from "./forms/TopicForm";
import { XMarkIcon } from "@heroicons/react/24/outline";
import ModalPortal from "@/components/ModalPortal";

export type TopicModalProps = {
  isOpen: boolean;
  onClose: () => void;
  onSave: (topicName: string) => void;
};

export default function TopicModal({ isOpen, onClose, onSave }: TopicModalProps) {
  if (!isOpen) {
    return null;
  }

  return (
    <ModalPortal>
      <div className="modal-overlay">
        <section className="modal-panel max-w-md">
          <div className="modal-header">
            <h2 className="modal-title">Add Topic</h2>
            <button
              type="button"
              onClick={onClose}
              aria-label="Close add topic modal"
              className="modal-close-button"
            >
              <XMarkIcon className="h-5 w-5" />
            </button>
          </div>
          <div className="modal-body p-6">
            <TopicForm onSubmit={onSave} onCancel={onClose} />
          </div>
        </section>
      </div>
    </ModalPortal>
  );
}
