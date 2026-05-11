interface ButtonProps {
  text: string | React.ReactNode;
  onClick?: () => void;
  className?: string;
  type?: "button" | "submit";
  disabled?: boolean;
}
export default function Button({ text, onClick, className, type = "button", disabled }: ButtonProps) {
  return (
    <button
      type={type}
      onClick={onClick}
      disabled={disabled}
      // Combine the custom class with any passed in (like margins or specific widths)
      className={className}
    >
      {text}
    </button>
  );
}