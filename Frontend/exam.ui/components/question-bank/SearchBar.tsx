"use client";

import {Search} from "lucide-react";
export type SearchBarProps = {
  value: string;
  onChange: (newValue: string) => void;
  placeholder?: string;
};

export default function SearchBar({ value, onChange, placeholder }: SearchBarProps) {
  return (
    <div className="w-full flex items-center gap-2 rounded-md border border-gray-300 bg-white px-3 py-2"> 
    <Search size={18} className="text-muted" />
    <input
      type="text"
      value={value}
      onChange={(e) => onChange(e.target.value)}
      placeholder={placeholder}
      className="
       w-full
       border-none 
       focus:ring-0 
       text-sm 
       text-foreground 
       placeholder:text-muted 
       focus:outline-none 
       focus:ring-0
       focus:border-slate-200"
    />
    </div>
  );
}
