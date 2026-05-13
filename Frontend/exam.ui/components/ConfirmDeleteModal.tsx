export type ConfirmDeleteModalProps = {
  onConfirm: () => void;
  onCancel: () => void;
  title:String;
  text:String;
};

export default function ConfirmDeleteModal({
  onConfirm,
  onCancel,
  title,text
}: ConfirmDeleteModalProps) {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/45 p-4">
      <section className="w-full max-w-md rounded-lg bg-white p-6 shadow-xl">
        <div className="mb-4 flex items-center justify-between gap-4">
          <h2 className="text-xl font-bold text-slate-950">
            {title}
          </h2>

          <button
            type="button"
            onClick={onCancel}
            aria-label="Close delete confirmation modal"
            className="rounded-md p-2 text-slate-500 transition hover:bg-slate-100 hover:text-slate-900"
          >
            ×
          </button>
        </div>

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
      </section>
    </div>
  );
}