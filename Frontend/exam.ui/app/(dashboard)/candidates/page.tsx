"use client";

import { getCandidates, addCandidate, deleteCandidate, getCandidateDetails } from "@/services/candidateService";
import { useEffect, useState } from "react";
import CandidatesTable from "@/components/candidates/CandidatesTable";
import AddCandidateModal from "@/components/candidates/AddCandidateModal";
import CandidateDetailsModal from "@/components/candidates/CandidateDetailsModal";
import { Candidate, CandidateDetail } from "@/types/candidate";
import DashboardPageHeader from "@/components/DashboardHeader";

export default function CandidatesPage() {
  const [candidates, setCandidates] = useState<Candidate[]>([]);
  const [emailError, setEmailError] = useState("");
  const [showAddModal, setShowAddModal] = useState(false);
  const [selectedCandidate, setSelectedCandidate] = useState<CandidateDetail | null>(null);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("All Status");

  useEffect(() => {
    getCandidates().then(setCandidates);
  }, []);

  const handleViewCandidate = async (candidate: Candidate) => {
    const details = await getCandidateDetails(Number(candidate.id));
    setSelectedCandidate(details);
  };

  const handleAddCandidate = async (data: {
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber: string;
  }) => {
    setEmailError("");
    try {
      await addCandidate(data);
      const updated = await getCandidates();
      setCandidates(updated);
      setShowAddModal(false);
    } catch (err: any) {
      if (err?.status === 409) {
        setEmailError(err.message);
      } else {
        setEmailError(err?.message || "Something went wrong");
      }
    }
  };

  const handleDelete = async (id: string) => {
    await deleteCandidate(Number(id));
    setCandidates((prev) => prev.filter((c) => c.id !== id));
  };

  const filtered = candidates.filter((c) => {
    const fullName = `${c.firstName} ${c.lastName}`.toLowerCase();
    const matchesSearch =
      fullName.includes(search.toLowerCase()) ||
      c.email.toLowerCase().includes(search.toLowerCase());
  const statusLabelToNumber: Record<string, number> = {
  Pending: 0,
  Expired: 1,
  Completed: 2,
};

const matchesStatus =
  statusFilter === "All Status" ||
  (statusFilter === "No Status"
    ? c.status === null
    : c.status === statusLabelToNumber[statusFilter]);
     return matchesSearch && matchesStatus;
});
  return (
    <div className="p-8">
      {/* <div className="flex items-center justify-between mb-6">
        <h1 className="text-title">Candidates</h1>
        <button
          className="bg-primary text-white font-semibold px-6 py-2.5 rounded-full hover:brightness-90 transition"
          onClick={() => setShowAddModal(true)}
        >
          + Add Candidate
        </button>
      </div> */}
      <DashboardPageHeader
        title="Candidates"
        buttonText="+ Add Candidate"
        onButtonClick={() => setShowAddModal(true)}
      />

      <CandidatesTable
        candidates={filtered}
        search={search}
        onSearchChange={setSearch}
        statusFilter={statusFilter}
        onStatusFilterChange={setStatusFilter}
        onViewCandidate={handleViewCandidate}
        onDeleteCandidate={handleDelete}
      />

      {showAddModal && (
        <AddCandidateModal
          onClose={() => setShowAddModal(false)}
          onSubmit={handleAddCandidate}
          backendError={emailError}
          clearBackendError={() => setEmailError("")}
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