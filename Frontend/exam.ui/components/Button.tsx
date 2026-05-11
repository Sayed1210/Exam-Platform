export type ButtonProps = {
  text: string;
  onClick?: () => void;
  className?: string;
};

export default function Button({ text, onClick, className = "" }: ButtonProps) {
  return (
    <button
      type="button"
      onClick={onClick}
      className={className}
    >
      {text}
    </button>
  );
}
