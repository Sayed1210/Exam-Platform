"use client";

import { Candidate } from "@/types/candidate";
import { TrashIcon, EyeIcon, MagnifyingGlassIcon } from "@heroicons/react/24/outline";
import { useState } from "react";
import ModalPortal from "@/components/common/ModalPortal";
import { ChevronLeftIcon, ChevronRightIcon, ChevronDownIcon } from "@heroicons/react/16/solid";
import ConfirmDeleteModal from "../common/ConfirmDeleteModal";
import Pagination from "../common/Pagination";
import SearchBar from "../question-bank/SearchInput";

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
      <div className="flex gap-3 items-center justify-between px-5 py-4 border-b border-gray-100">
        <SearchBar
          value={search}
          onChange={onSearchChange}
          placeholder="Search candidates..."
        />
        <div className="relative flex items-center">
          <select
            value={statusFilter}
            onChange={(e) => onStatusFilterChange(e.target.value)}
            className="appearance-none border border-gray-200 rounded-lg pl-3.5 pr-9 py-2 text-gray bg-white cursor-pointer outline-none"
          >
            {STATUS_OPTIONS.map((s) => (
              <option key={s} value={s}>{s}</option>
            ))}
          </select>
          <ChevronDownIcon className="pointer-events-none absolute right-2.5 icon-small text-gray"/>
        </div>
      </div>

      {/* Table */}
      <table className="w-full border-collapse">
        <thead>
          <tr className="border-b border-gray-100">
            {["NAME", "EMAIL", "STATUS", "SCORE", "ACTIONS"].map((h) => (
              <th key={h} className="nav-section-label text-gray-400 px-5 py-3">
                {h}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {(!candidates || candidates.length === 0) && (
            <tr>
              <td colSpan={5} className="text-muted text-center py-8">
                [ No candidates found ]
              </td>
            </tr>
          )}
          {candidates.map((c, i) => {
            return (
              <tr key={c.id} className={`${i !== candidates.length - 1 ? "border-b border-gray-50" : ""}`}>
                <td className="px-5 py-4 text-label text-center">
                  {c.firstName} {c.lastName}
                </td>
                <td className="px-5 py-4 text-muted text-center">{c.email}</td>
                {/* Status */}
                <td className="px-5 py-4 text-center">
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
                {/* Score */}
                <td className="px-5 py-4 text-sm text-label text-center">
                  {c.score !== null ? `${c.score}%` : "-"}
                </td>
                {/* Actions */}
                <td className="px-5 py-4">
                  <div className="flex items-center justify-center gap-2">
                    {/* view details button */}
                    <button
                      className="btn-icon-secondary flex items-center gap-1.5 text-muted text-[13px]"
                      onClick={() => onViewCandidate(c)}
                    >
                      <EyeIcon className="icon-mid"/>
                      View Details
                    </button>
                    {/* delete button */}
                    <button
                      title="Delete"
                      className="btn-icon-danger"
                      onClick={() => setPendingDeleteId(c.id)}
                    >
                      <TrashIcon className="icon-mid"/>
                    </button>
                  </div>
                </td>
              </tr>
            );
          })}
        </tbody>
      </table>

      {/* Pagination */}
      {/* {totalPages > 1 && (
        <div className="flex items-center justify-between px-5 py-4 border-t border-gray-100">
          <p className="text-gray">
            Page {currentPage} of {totalPages}
          </p>
          <div className="flex items-center gap-2">
            <button
              onClick={() => onPageChange(currentPage - 1)}
              disabled={currentPage === 1}
              className="btn-nav"
            >
              <ChevronLeftIcon className="icon-small" />
              Previous
            </button>
            <button
              onClick={() => onPageChange(currentPage + 1)}
              disabled={currentPage === totalPages}
              className="btn-nav"
            >
              Next
              <ChevronRightIcon className="icon-small" />
            </button>
          </div>
        </div>
      )} */}

      <Pagination
        currentPage={currentPage}
        totalPages={totalPages}
        onPageChange={onPageChange}
      />

      {/* Delete Confirmation Modal */}
      {pendingDeleteId && (
        <ConfirmDeleteModal
          onConfirm={() => {
                      onDeleteCandidate(pendingDeleteId);
                      setPendingDeleteId(null);
                    }}
          onCancel={() => setPendingDeleteId(null)}
          title="Delete Candidate"
          text={
          <>
            Are you sure you want to delete{" "}
            <span className="font-semibold italic">
              {target?.firstName} {target?.lastName}
            </span>
            ?
          </>
          }
            yesText="Delete"
            noText="Cancel"
          >
        </ConfirmDeleteModal>
      )}
    </div>
  );
}
