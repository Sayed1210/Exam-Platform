"use client";

import {
  ChevronLeftIcon,
  ChevronRightIcon,
} from "@heroicons/react/24/outline";

interface PaginationProps {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}

export default function Pagination({
  currentPage,
  totalPages,
  onPageChange,
}: PaginationProps) {
  return (
    <div className="flex items-center justify-between px-5 py-4 border-t border-gray-100">
      <p className="text-gray">
        Page {totalPages==0 ? 0 : currentPage} of {totalPages}
      </p>

      <div className="flex items-center gap-2">
        <button
          onClick={() => onPageChange(currentPage - 1)}
          disabled={totalPages==0 || currentPage === 1}
          className="btn-nav"
        >
          <ChevronLeftIcon className="icon-small" />
          Previous
        </button>

        <button
          onClick={() => onPageChange(currentPage + 1)}
          disabled={totalPages==0 || currentPage === totalPages}
          className="btn-nav"
        >
          Next
          <ChevronRightIcon className="icon-small" />
        </button>
      </div>
    </div>
  );
}