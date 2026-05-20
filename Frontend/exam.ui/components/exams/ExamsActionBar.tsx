'use client';

import SearchInput from "../question-bank/SearchInput";

interface ExamsActionBarProps {
  searchQuery: string;
  setSearchQuery: (v: string) => void;
  onCreate: () => void;
}

export default function ExamsActionBar({ searchQuery, setSearchQuery, onCreate }: ExamsActionBarProps) {
  return (
    <div className="mb-4 flex flex-col justify-between gap-4 md:flex-row md:items-center">
      <div className="relative flex-1 max-w-md">
        <SearchInput placeholder="Search by title or topic..." value={searchQuery} onChange={setSearchQuery} />
      </div>
      {/* keep commented Create button placeholder to preserve UI details */}
      {/* <Button 
        text={<span className="flex items-center gap-2"><PlusIcon className="w-5 h-5 stroke-[2.5px]" /> Create Exam</span>}
        onClick={onCreate}
        className="btn-primary !w-auto px-8"
      /> */}
    </div>
  );
}
