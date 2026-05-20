"use client";

// import { XMarkIcon } from "@heroicons/react/24/outline";
import ModalPortal from "./ModalPortal";

// export type ConfirmDeleteModalProps = {
//   onConfirm: () => void;
//   onCancel: () => void;
//   title: string;
//   text: string;
// };

export default function ConfirmDeleteModal({
  onConfirm,
  onCancel,
  title,
  text,
  yesText="Yes",
  noText="Cancel"
}: any) {
  return (
    <ModalPortal>
      <div className="modal-overlay">
        <section className="modal-panel max-w-[400px]">
          <div className="modal-body p-6">
            <h2 className="text-subtitle mb-2">{title}</h2>
            <div  className="text-body text-gray-900 mb-2">
              {text} 
            </div >
            <p className="text-muted mb-2">This action cannot be undone.</p>

            <div className="flex justify-center gap-2">
              <button
                type="button"
                onClick={onConfirm}
                className="btn-primary w-full"
              >
                {yesText}
              </button>
              <button
                type="button"
                onClick={onCancel}
                className="btn-secondary w-full"
              >
                {noText}
              </button>
            </div>
          </div>
        </section>
      </div>
    </ModalPortal>
  );
}
