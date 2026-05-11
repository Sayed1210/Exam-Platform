"use client";

import { useEffect, useState } from "react";
import CandidatesTable from "@/components/candidates/CandidatesTable";
import AddCandidateModal from "@/components/candidates/AddCandidateModal";
import CandidateDetailsModal from "@/components/candidates/CandidateDetailsModal";
import { Candidate } from "@/types/candidate";
import { initialCandidates } from "@/lib/data/candidates";

export default function CandidatesPage() {
  const [candidates, setCandidates] = useState<Candidate[]>(initialCandidates);

  useEffect(() => {
    fetch("/api/candidates")
      .then((res) => res.json())
      .then(setCandidates);
  }, []);

  const [showAddModal, setShowAddModal] = useState(false);
  const [selectedCandidate, setSelectedCandidate] = useState<Candidate | null>(null);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("All Status");

  const handleAddCandidate = (data: {
    firstName: string;
    lastName: string;
    email: string;
  }) => {
    const newCandidate: Candidate = {
      id: String(Date.now()),
      ...data,
      status: null,
      score: null,
      invitedAt: null,
      startedAt: null,
      answers: [],
    };
    setCandidates((prev) => [newCandidate, ...prev]);
    setShowAddModal(false);
  };

  const handleDelete = (id: string) => {
    setCandidates((prev) => prev.filter((c) => c.id !== id));
  };

  const filtered = candidates.filter((c) => {
    const fullName = `${c.firstName} ${c.lastName}`.toLowerCase();
    const matchesSearch =
      fullName.includes(search.toLowerCase()) ||
      c.email.toLowerCase().includes(search.toLowerCase());
    const matchesStatus =
      statusFilter === "All Status" ||
      (statusFilter === "No Status" ? c.status === null : c.status === statusFilter);
    return matchesSearch && matchesStatus;
  });

  return (
    <div className="min-h-screen bg-[#f4f6f9] p-8">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Candidates</h1>
        <button
          className="bg-primary text-white font-semibold px-6 py-2.5 rounded-full hover:brightness-90 transition"
          onClick={() => setShowAddModal(true)}
        >
          + Add Candidate
        </button>
      </div>

      <CandidatesTable
        candidates={filtered}
        search={search}
        onSearchChange={setSearch}
        statusFilter={statusFilter}
        onStatusFilterChange={setStatusFilter}
        onViewCandidate={setSelectedCandidate}
        onDeleteCandidate={handleDelete}
      />

      {showAddModal && (
        <AddCandidateModal
          onClose={() => setShowAddModal(false)}
          onSubmit={handleAddCandidate}
        />
      )}

      {selectedCandidate && (
        <CandidateDetailsModal
          candidate={selectedCandidate}
          onClose={() => setSelectedCandidate(null)}
        />
      )}
    </div>
  );
}