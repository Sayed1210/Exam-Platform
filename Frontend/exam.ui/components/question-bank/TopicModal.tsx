import TopicForm from "./forms/TopicForm";

export type TopicModalProps = {
  isOpen: boolean;
  onClose: () => void;
  onSave: (topicName: string) => void;
};

export default function TopicModal({ isOpen, onClose, onSave }: TopicModalProps) {
  if (!isOpen) {
    return null;
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/45 p-4">
      <section className="w-full max-w-md rounded-lg bg-white p-6 shadow-xl">
        <div className="mb-6 flex items-center justify-between gap-4">
          <h2 className="text-xl font-bold text-slate-950">Add Topic</h2>
          <button
            type="button"
            onClick={onClose}
            aria-label="Close add topic modal"
            className="rounded-md p-2 text-slate-500 transition hover:bg-slate-100 hover:text-slate-900"
          >
            ×
          </button>
        </div>
        <TopicForm onSubmit={onSave} onCancel={onClose} />
      </section>
    </div>
  );
}
