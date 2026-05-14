"use client";

import { XMarkIcon } from "@heroicons/react/24/outline";
import ModalPortal from "./ModalPortal";

export type ConfirmDeleteModalProps = {
  onConfirm: () => void;
  onCancel: () => void;
  title: string;
  text: string;
};

export default function ConfirmDeleteModal({
  onConfirm,
  onCancel,
  title,
  text,
}: ConfirmDeleteModalProps) {
  return (
    <ModalPortal>
      <div className="modal-overlay">
        <section className="modal-panel max-w-md">
          <div className="modal-header">
            <h2 className="modal-title">{title}</h2>

            <button
              type="button"
              onClick={onCancel}
              aria-label="Close delete confirmation modal"
              className="modal-close-button"
            >
              <XMarkIcon className="h-5 w-5" />
            </button>
          </div>

          <div className="modal-body p-6">
            <p className="text-sm text-slate-600">
              {text}
            </p>

            <div className="mt-6 flex justify-end gap-3">
              <button
                type="button"
                onClick={onCancel}
                className="bg-white text-black font-semibold px-6 py-2.5 rounded-full hover:brightness-90 transition"
              >
                Cancel
              </button>

              <button
                type="button"
                onClick={onConfirm}
                className="bg-primary text-white font-semibold px-6 py-2.5 rounded-full hover:brightness-90 transition"
              >
                Yes
              </button>
            </div>
          </div>
        </section>
      </div>
    </ModalPortal>
  );
}
