"use client";

import type { ReactNode } from "react";
import ModalPortal from "./ModalPortal";

export default function ModalLayout({
  title,
  children,
  footer
}: any) {

  return (
    <ModalPortal>
      <div className="modal-overlay">
        <div
          className="modal-panel max-w-[520px]"
          onClick={(e) => e.stopPropagation()}
        >
          {/* Header */}
          <div className="modal-header">
            <h2 className="modal-title">
              {title}
            </h2>
          </div>

          {/* Body */}
          <div className="modal-body px-6 py-5 flex flex-col gap-4">
            {children}
          </div>

          {/* Footer */}
          {footer && (
            <div className="flex items-center justify-end gap-3 border-t border-slate-100 px-6 py-5">
              {footer}
            </div>
          )}
        </div>
      </div>
    </ModalPortal>
  );
}