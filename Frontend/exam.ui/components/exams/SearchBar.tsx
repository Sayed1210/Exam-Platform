// components/exams/SearchBar.tsx
import React from 'react';
import { MagnifyingGlassIcon } from '@heroicons/react/24/outline';

export const SearchBar = () => {
  return (
    <div className="relative flex-grow max-w-xl">
      <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
        <MagnifyingGlassIcon className="w-5 h-5 text-slate-400" />
      </div>
      <input
        type="text"
        placeholder="Search exams..."
        className="w-full bg-slate-50 pl-12 pr-4 py-3 rounded-xl border border-slate-200 focus:border-blue-300 focus:ring-1 focus:ring-blue-100 transition duration-150 outline-none"
      />
    </div>
  );
};