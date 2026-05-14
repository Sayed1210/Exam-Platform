export type IconProps = {
  className?: string;
  size?: number;
  strokeWidth?: number;
};
export default function SearchIcon({
  className = "",
  size = 16,
  strokeWidth = 2,
}: IconProps) {
  return (
    <svg
      width={size}
      height={size}
      viewBox="0 0 24 24"
      fill="none"
      stroke="#9ca3af"
      strokeWidth={strokeWidth}
      className={className}
    >
      <circle cx="11" cy="11" r="8" />

      <path d="M21 21l-4.35-4.35" />
    </svg>
  );
}