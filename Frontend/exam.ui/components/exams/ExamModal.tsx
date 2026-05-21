"use client";

import { ReactNode } from 'react';
import { XMarkIcon } from '@heroicons/react/24/outline';
import ModalPortal from '../common/ModalPortal';

interface ModalProps {
  isOpen?: boolean; // Optional if you control visibility from the parent
  onClose: () => void;
  title: string;
  children: ReactNode;
  disableClose?: boolean;
}

export default function ExamModal({ onClose, title, children, disableClose }: ModalProps) {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
      {/* Container matches your original 3xl rounded shadow style */}
      <div className="bg-white w-full max-w-2xl rounded-3xl shadow-2xl flex flex-col max-h-[90vh]">
        
        {/* Common Header */}
        <div className="pt-6 pl-6 border-b border-slate-50 flex justify-between items-center">
          <h2 className="text-xl font-bold text-slate-800">{title}</h2>
        </div>

          <div className="modal-body overflow-hidden">
            {children}
          </div>
        </div>
      </div>
    
  );
}
