// components/Modal.tsx
import { ReactNode } from 'react';
import { XMarkIcon } from '@heroicons/react/24/outline';

interface ModalProps {
  isOpen?: boolean; // Optional if you control visibility from the parent
  onClose: () => void;
  title: string;
  children: ReactNode;
  disableClose?: boolean;
}

export default function ExamModal({ onClose, title, children, disableClose }: ModalProps) {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm p-4">
      {/* Container matches your original 3xl rounded shadow style */}
      <div className="bg-white w-full max-w-2xl rounded-3xl shadow-2xl flex flex-col max-h-[90vh]">
        
        {/* Common Header */}
        <div className="p-6 border-b border-slate-50 flex justify-between items-center">
          <h2 className="text-xl font-bold text-slate-800">{title}</h2>
          <button 
            onClick={disableClose ? undefined : onClose} 
            disabled={disableClose}
            className={`text-slate-400 hover:text-slate-600 transition-colors ${disableClose ? 'cursor-not-allowed opacity-50' : ''}`}
          >
            <XMarkIcon className="w-6 h-6" />
          </button>
        </div>

        {/* This is where your CreateExam content will be injected */}
        <div className="flex-1 overflow-y-auto">
          {children}
        </div>
      </div>
    </div>
  );
}