export default function Button({ text, onClick, className=""}: any) {
  return (
    <button
      onClick={onClick}
      className={className}
    >
      {text}
    </button>
  );
}
