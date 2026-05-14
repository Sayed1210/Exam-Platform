"use client";

import { ReactNode } from 'react';
import { XMarkIcon } from '@heroicons/react/24/outline';
import ModalPortal from '../ModalPortal';

interface ModalProps {
  isOpen?: boolean; // Optional if you control visibility from the parent
  onClose: () => void;
  title: string;
  children: ReactNode;
}

export default function ExamModal({ onClose, title, children }: ModalProps) {
  return (
    <ModalPortal>
      <div className="modal-overlay">
        <div className="modal-panel max-w-2xl">
          <div className="modal-header">
            <h2 className="modal-title">{title}</h2>
            <button 
              onClick={onClose} 
              className="modal-close-button"
              aria-label="Close modal"
            >
              <XMarkIcon className="w-6 h-6" />
            </button>
          </div>

          <div className="modal-body">
            {children}
          </div>
        </div>
      </div>
    </ModalPortal>
  );
}
