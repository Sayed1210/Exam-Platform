export type IconProps = {
  className?: string;
  size?: number;
  strokeWidth?: number;
};

export default function TrashIcon({
  className = "",
  size = 16,
  strokeWidth = 1.8,
}: IconProps) {
  return (
    <svg
      width={size}
      height={size}
      viewBox="0 0 24 24"
      fill="none"
      strokeWidth={strokeWidth}
      className={className}
    >
      <path
        d="M3 6h18M8 6V4h8v2M19 6l-1 14H6L5 6"
        stroke="currentColor"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
}