"use client";

import { Loader2 } from "lucide-react";

export default function Button({ 
  text, 
  onClick, 
  className="",
  loading = false,
  loadingText,
  disabled = false
}: any) {
  return (
    <button
      type="button"
      onClick={onClick}
      className={` 
        disabled:opacity-70
        disabled:cursor-not-allowed 
        flex items-center justify-center gap-2
        ${className}`
      }
      disabled={disabled}
    >
      {loading && (
        <Loader2 className="w-4 h-4 animate-spin" />
      )}

      {loading ? loadingText || "Loading..." : text}
    </button>
  );
}
