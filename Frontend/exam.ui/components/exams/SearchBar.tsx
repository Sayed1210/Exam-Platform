import React from 'react';
import { MagnifyingGlassIcon } from '@heroicons/react/24/outline';

interface SearchBarProps {
  placeholder?: string;
  value: string;
  onChange: (val: string) => void;
}

export const SearchBar = ({ placeholder = "Search...", value, onChange }: SearchBarProps) => {
  return (
    <div className="relative flex-grow max-w-xl group">
      {/* Icon: Using pointer-events-none ensures you can click "through" it to the input */}
      <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none z-10">
        <MagnifyingGlassIcon className="w-5 h-5 text-slate-400 group-focus-within:text-blue-500 transition-colors" />
      </div>
      <input
        type="text"
        placeholder={placeholder}
        value={value} 
        onChange={(e) => onChange(e.target.value)}
        // Added z-0 and relative to ensure the input is in the correct stacking context
        className="relative z-0 w-full bg-slate-50 pl-12 pr-4 py-3 rounded-xl border border-slate-200 focus:border-blue-400 focus:ring-4 focus:ring-blue-50 transition-all outline-none text-sm text-slate-700"
      />
    </div>
  );
};
