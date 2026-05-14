"use client";

import type { ReactNode } from "react";
import ModalPortal from "./ModalPortal";

type ModalProps = {
  open: boolean;
  onClose: () => void;
  children: ReactNode;
};

export default function Modal({ open, onClose, children }: ModalProps) {
  if (!open) return null;

  return (
    <ModalPortal>
      <div className="modal-overlay text-center" onClick={onClose}>
        <div
          className="modal-panel max-w-lg p-8"
          onClick={(event) => event.stopPropagation()}
        >
          {children}
        </div>
      </div>
    </ModalPortal>
  );
}
