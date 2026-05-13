export default function Input({
  placeholder,
  value,
  onChange,
  type = "text",
  onBlur,
}: any) {
  return (
    <input
      type={type}
      placeholder={placeholder}
      value={value}
      onChange={(e) => onChange(e.target.value)}
      className="w-full px-3 py-2 border border-gray-300 rounded-md 
                 focus:outline-none focus:ring-1 focus:ring-primary focus:border-primary
                 transition"
      onBlur={onBlur}
    />
  );
}