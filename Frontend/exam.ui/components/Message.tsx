'use client';
import { CheckCircleIcon, XCircleIcon, XMarkIcon } from '@heroicons/react/24/outline';

interface MessageProps {
  message: string;
  type: 'success' | 'error';
  onClose: () => void;
}

export default function Message({ message, type, onClose }: MessageProps) {
  const isSuccess = type === 'success';

  return (
    <div className="fixed top-6 left-1/2 -translate-x-1/2 z-[100] animate-in fade-in slide-in-from-top-4 duration-300">
      <div className={`
        flex items-center gap-3 px-6 py-4 rounded-2xl shadow-2xl border min-w-[320px]
        ${isSuccess 
          ? 'bg-green-50 border-green-100 text-green-700' 
          : 'bg-red-50 border-red-100 text-red-700'} 
      `}>
        {isSuccess ? (
          <CheckCircleIcon className="w-6 h-6 shrink-0" />
        ) : (
          <XCircleIcon className="w-6 h-6 shrink-0" />
        )}
        
        <p className="font-bold text-sm flex-1">{message}</p>

        <button onClick={onClose} className="p-1 hover:bg-black/5 rounded-full transition-colors">
          <XMarkIcon className="w-5 h-5 opacity-70" />
        </button>
      </div>
    </div>
  );
}