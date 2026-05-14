"use client";

import SearchIcon from "../SearchIcon";
export type SearchBarProps = {
  value: string;
  onChange: (newValue: string) => void;
  placeholder?: string;
};

export default function SearchBar({ value, onChange, placeholder }: SearchBarProps) {
  return (
    <div className="relative w-full">
      <SearchIcon
        className="
          pointer-events-none
          absolute
          left-3
          top-1/2
          -translate-y-1/2
          text-gray-400
        "
      />

      <input
        type="text"
        value={value}
        onChange={(e) => onChange(e.target.value)}
        placeholder={placeholder}
        className="
          w-full
          rounded-md
          border
          border-gray-300
          bg-white
          py-2
          pl-10
          pr-3
          text-sm
          text-foreground
          placeholder:text-muted
          focus:outline-none
          focus:ring-0
          focus:border-slate-400
        "
      />
    </div>
  );
}
