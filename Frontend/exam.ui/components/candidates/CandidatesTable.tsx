"use client";

import { Candidate } from "@/types/candidate";
import TrashIcon from "../TrashIcon";
import { useState } from "react";
import SearchIcon from "../SearchIcon";
import ModalPortal from "@/components/ModalPortal";

const STATUS_OPTIONS = ["All Status", "Pending", "Expired", "In Progress", "Done", "No Status"];
const statusMap: Record<number, { label: string; bg: string; color: string }> = {
  0: { label: "Pending",     bg: "#eff6ff", color: "#1d4ed8" },
  1: { label: "Expired",     bg: "#fee2e2", color: "#991b1b" },
  2: { label: "In Progress", bg: "#fef9c3", color: "#854d0e" },
  3: { label: "Done",        bg: "#d1fae5", color: "#065f46" },
};
interface Props {
  candidates: Candidate[];
  search: string;
  onSearchChange: (v: string) => void;
  statusFilter: string;
  onStatusFilterChange: (v: string) => void;
  onViewCandidate: (c: Candidate) => void;
  onDeleteCandidate: (id: string) => void;
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}

export default function CandidatesTable({
  candidates,
  search,
  onSearchChange,
  statusFilter,
  onStatusFilterChange,
  onViewCandidate,
  onDeleteCandidate,
  currentPage,
  totalPages,
  onPageChange,
}: Props) {
  const [pendingDeleteId, setPendingDeleteId] = useState<string | null>(null);
  const target = candidates?.find((c) => c.id === pendingDeleteId);

  return (
    <div className="card overflow-hidden">
      {/* Toolbar */}
      <div className="flex items-center justify-between px-5 py-4 border-b border-gray-100">
        <div className="flex items-center gap-2 bg-gray-50 border border-gray-200 rounded-lg px-3 py-2 w-64">
          <SearchIcon/>
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
          {(!candidates || candidates.length === 0) && (
            <tr>
              <td colSpan={5} className="text-center text-gray-400 py-8 text-sm">
                No candidates found.
              </td>
            </tr>
          )}
          {candidates.map((c, i) => {
            return (
              <tr key={c.id} className={`${i !== candidates.length - 1 ? "border-b border-gray-50" : ""}`}>
                <td className="px-5 py-4 text-sm font-semibold text-gray-900">
                  {c.firstName} {c.lastName}
                </td>
                <td className="px-5 py-4 text-sm text-gray-700">{c.email}</td>
         <td className="px-5 py-4">
  {c.status !== null && c.status !== undefined ? (
    <span
      className="inline-block px-3 py-1 rounded-full text-[13px] font-medium"
      style={{
        background: statusMap[c.status as unknown as number]?.bg ?? "#f3f4f6",
        color: statusMap[c.status as unknown as number]?.color ?? "#6b7280",
      }}
    >
      {statusMap[c.status as unknown as number]?.label ?? c.status}
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
                      className="btn-icon-secondary"
                      onClick={() => setPendingDeleteId(c.id)}
                    >
                      <TrashIcon className="text-gray-400 transition group-hover:text-red-500"/>
                    </button>
                  </div>
                </td>
              </tr>
            );
          })}
        </tbody>
      </table>

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="flex items-center justify-between px-5 py-4 border-t border-gray-100">
          <p className="text-sm text-gray-500">
            Page {currentPage} of {totalPages}
          </p>
          <div className="flex items-center gap-2">
            <button
              onClick={() => onPageChange(currentPage - 1)}
              disabled={currentPage === 1}
              className="flex items-center gap-1.5 px-3.5 py-2 rounded-lg border border-gray-200 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-40 disabled:cursor-not-allowed transition"
            >
              <svg width="14" height="14" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
                <path d="M15 18l-6-6 6-6" />
              </svg>
              Previous
            </button>
            <button
              onClick={() => onPageChange(currentPage + 1)}
              disabled={currentPage === totalPages}
              className="flex items-center gap-1.5 px-3.5 py-2 rounded-lg border border-gray-200 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-40 disabled:cursor-not-allowed transition"
            >
              Next
              <svg width="14" height="14" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
                <path d="M9 18l6-6-6-6" />
              </svg>
            </button>
          </div>
        </div>
      )}

      {/* Delete Confirmation Modal */}
      {pendingDeleteId && (
        <ModalPortal>
          <div className="modal-overlay">
            <div className="modal-panel max-w-[380px]">
              <div className="modal-body p-7">
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
          </div>
        </ModalPortal>
      )}

    </div>
  );
}
