export default function Button({ text, onClick, className="", disabled = false }: any) {
  return (
    <button
      type="button"
      onClick={onClick}
      className={className}
      disabled={disabled}
    >
      {text}
    </button>
  );
}
