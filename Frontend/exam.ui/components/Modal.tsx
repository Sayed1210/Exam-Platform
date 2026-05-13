export default function Modal({ open, onClose, children }: any) {
  if (!open) return null;

  return (
    <div className="fixed inset-0 bg-black/40 flex items-center justify-center text-center z-50 p-4">
      
      {/* Modal Card */}
      <div className="bg-white rounded-3xl max-w-lg p-8 relative flex flex-col gap-3 
      min-w-70 shadow-2xl animate-in fade-in zoom-in duration-200">
        
        {/* Close button (top right) */}
        {/* <button
          onClick={onClose}
          className="absolute top-3 right-4 font-bold text-gray-400 hover:text-black"
        >
          ✕
        </button> */}

        {/* Content */}
        {children}
      </div>
    </div>
  );
}