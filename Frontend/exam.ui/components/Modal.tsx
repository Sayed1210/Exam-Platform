export default function Modal({ open, onClose, children }: any) {
  if (!open) return null;

  return (
    <div className="fixed inset-0 bg-black/30 flex items-center justify-center text-center">
      
      {/* Modal Card */}
      <div className="bg-white rounded-2xl shadow-xl max-w-md p-8 relative relative flex flex-col gap-6">
        
        {/* Close button (top right) */}
        <button
          onClick={onClose}
          className="absolute top-3 right-4 font-bold text-gray-400 hover:text-black"
        >
          ✕
        </button>

        {/* Content */}
        {children}
      </div>
    </div>
  );
}