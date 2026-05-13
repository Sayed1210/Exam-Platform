"use client";

import { Candidate } from "@/types/candidate";
import { useState } from "react";

const STATUS_OPTIONS = ["All Status", "Completed", "In Progress", "Invited", "Expired", "No Status"];

const statusStyles: Record<string, { bg: string; color: string }> = {
  Completed: { bg: "#d1fae5", color: "#065f46" },
  "In Progress": { bg: "#fef9c3", color: "#854d0e" },
  Invited: { bg: "#eff6ff", color: "#1d4ed8" },
  Expired: { bg: "#fee2e2", color: "#991b1b" },
  "No Status": { bg: "#f3f4f6", color: "#6b7280" },
};

interface Props {
  candidates: Candidate[];
  search: string;
  onSearchChange: (v: string) => void;
  statusFilter: string;
  onStatusFilterChange: (v: string) => void;
  onViewCandidate: (c: Candidate) => void;
  onDeleteCandidate: (id: string) => void;
}

export default function CandidatesTable({
  candidates,
  search,
  onSearchChange,
  statusFilter,
  onStatusFilterChange,
  onViewCandidate,
  onDeleteCandidate,
}: Props) {
  const [pendingDeleteId, setPendingDeleteId] = useState<string | null>(null);
  const target = candidates.find((c) => c.id === pendingDeleteId);

  return (
    <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
      {/* Toolbar */}
      <div className="flex items-center justify-between px-5 py-4 border-b border-gray-100">
        <div className="flex items-center gap-2 bg-gray-50 border border-gray-200 rounded-lg px-3 py-2 w-64">
          <svg width="16" height="16" fill="none" viewBox="0 0 24 24" stroke="#9ca3af" strokeWidth="2">
            <circle cx="11" cy="11" r="8" />
            <path d="M21 21l-4.35-4.35" />
          </svg>
          <input
            type="text"
            placeholder="Search candidates..."
            value={search}
            onChange={(e) => onSearchChange(e.target.value)}
            className="bg-transparent border-none outline-none text-sm text-gray-700 w-full placeholder:text-gray-400"
          />
        </div>

        <div className="relative flex items-center">
          <select
            value={statusFilter}
            onChange={(e) => onStatusFilterChange(e.target.value)}
            className="appearance-none border border-gray-200 rounded-lg pl-3.5 pr-9 py-2 text-sm text-gray-700 bg-white cursor-pointer outline-none"
          >
            {STATUS_OPTIONS.map((s) => (
              <option key={s} value={s}>{s}</option>
            ))}
          </select>
          <svg className="absolute right-2.5 pointer-events-none" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="#6b7280" strokeWidth="2">
            <path d="M6 9l6 6 6-6" />
          </svg>
        </div>
      </div>

      {/* Table */}
      <table className="w-full border-collapse">
        <thead>
          <tr className="border-b border-gray-100">
            {["NAME", "EMAIL", "STATUS", "SCORE", "ACTIONS"].map((h) => (
              <th key={h} className="text-left text-[11px] font-semibold tracking-wider text-gray-400 px-5 py-3">
                {h}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {candidates.length === 0 && (
            <tr>
              <td colSpan={5} className="text-center text-gray-400 py-8 text-sm">
                No candidates found.
              </td>
            </tr>
          )}
          {candidates.map((c, i) => {
            const statusKey = c.status ?? "No Status";
            const style = statusStyles[statusKey];
            return (
              <tr key={c.id} className={`${i !== candidates.length - 1 ? "border-b border-gray-50" : ""}`}>
                <td className="px-5 py-4 text-sm font-semibold text-gray-900">
                  {c.firstName} {c.lastName}
                </td>
                <td className="px-5 py-4 text-sm text-gray-700">{c.email}</td>
                <td className="px-5 py-4">
                  {c.status ? (
                    <span
                      className="inline-block px-3 py-1 rounded-full text-[13px] font-medium"
                      style={{ background: style.bg, color: style.color }}
                    >
                      {c.status}
                    </span>
                  ) : (
                    <span className="text-gray-400">—</span>
                  )}
                </td>
                <td className="px-5 py-4 text-sm text-gray-700">
                  {c.score !== null ? `${c.score}%` : "—"}
                </td>
                <td className="px-5 py-4">
                  <div className="flex items-center gap-2">
                    <button
                      className="flex items-center gap-1.5 bg-white border border-gray-200 rounded-lg px-3.5 py-1.5 text-[13px] font-medium text-gray-700 cursor-pointer hover:bg-gray-50 hover:border-gray-300 transition"
                      onClick={() => onViewCandidate(c)}
                    >
                      <svg width="15" height="15" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
                        <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" />
                        <circle cx="12" cy="12" r="3" />
                      </svg>
                      View Details
                    </button>
                    <button
                      title="Delete"
                      className="p-1.5 rounded-md cursor-pointer hover:bg-gray-100 transition group"
                      onClick={() => setPendingDeleteId(c.id)}
                    >
                      <svg width="16" height="16" fill="none" viewBox="0 0 24 24" strokeWidth="1.8"
                        className="stroke-gray-400 group-hover:stroke-red-500 transition">
                        <path d="M3 6h18M8 6V4h8v2M19 6l-1 14H6L5 6" />
                      </svg>
                    </button>
                  </div>
                </td>
              </tr>
            );
          })}
        </tbody>
      </table>

      {/* Delete Confirmation Modal */}
      {pendingDeleteId && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl p-7 w-[380px] shadow-2xl">
            <h3 className="text-[17px] font-bold text-gray-900 mb-2">Delete Candidate</h3>
            <p className="text-sm text-gray-500 mb-6 leading-relaxed">
              Are you sure you want to delete{" "}
              <strong className="text-gray-700">{target?.firstName} {target?.lastName}</strong>?
              {" "}This action cannot be undone.
            </p>
            <div className="flex justify-end gap-2.5">
              <button
                className="px-4 py-2 rounded-lg border border-gray-200 bg-white text-sm font-medium text-gray-700 cursor-pointer hover:bg-gray-50 transition"
                onClick={() => setPendingDeleteId(null)}
              >
                Cancel
              </button>
              <button
                className="px-4 py-2 rounded-lg bg-primary text-white text-sm font-medium cursor-pointer hover:brightness-90 transition"
                onClick={() => {
                  onDeleteCandidate(pendingDeleteId);
                  setPendingDeleteId(null);
                }}
              >
                Delete
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}