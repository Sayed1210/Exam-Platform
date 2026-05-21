"use client";

import { useEffect, useRef, useState } from "react";
import type { MouseEvent } from "react";
import { PencilSquareIcon, TrashIcon } from "@heroicons/react/24/outline";

export type TopicTagProps = {
  label: string;
  active?: boolean;
  onClick?: () => void;
  onEdit?: () => void;
  onDelete?: () => void;
};

export default function TopicTag({
  label,
  active = false,
  onClick,
  onEdit,
  onDelete,
}: TopicTagProps) {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!isMenuOpen) return;

    function handlePointerDown(event: PointerEvent) {
      if (!menuRef.current?.contains(event.target as Node)) {
        setIsMenuOpen(false);
      }
    }

    document.addEventListener("pointerdown", handlePointerDown);
    return () => document.removeEventListener("pointerdown", handlePointerDown);
  }, [isMenuOpen]);

  function handleContextMenu(event: MouseEvent<HTMLButtonElement>) {
    event.preventDefault();
    setIsMenuOpen(true);
  }

  return (
    <div ref={menuRef} className="relative inline-flex">
      <button
        type="button"
        onClick={onClick}
        onContextMenu={handleContextMenu}
        className={`cursor-pointer select-none whitespace-nowrap rounded-full border-[1.5px] px-3.5 py-[5px] text-[12.5px] font-medium transition-all duration-150 ${
          active
            ? "border-primary bg-primary text-white"
            : "border-gray-200 bg-white text-gray-600 hover:border-primary hover:text-primary"
        }`}
      >
        {label}
      </button>

      {isMenuOpen && (
        <div className="absolute left-0 top-full z-30 mt-2 min-w-32 overflow-hidden rounded-lg border border-slate-100 bg-white py-1 shadow-lg">
          <button
            type="button"
            onClick={() => {
              setIsMenuOpen(false);
              onEdit?.();
            }}
            className="flex w-full items-center gap-2 px-3 py-2 text-left text-sm text-slate-600 hover:bg-slate-50 hover:text-primary"
          >
            <PencilSquareIcon className="h-4 w-4" />
            Edit
          </button>
          <button
            type="button"
            onClick={() => {
              setIsMenuOpen(false);
              onDelete?.();
            }}
            className="flex w-full items-center gap-2 px-3 py-2 text-left text-sm text-red-600 hover:bg-red-50"
          >
            <TrashIcon className="h-4 w-4" />
            Delete
          </button>
        </div>
      )}
    </div>
  );
}
