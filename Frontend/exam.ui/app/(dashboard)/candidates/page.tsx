"use client";

import { getCandidates, addCandidate, deleteCandidate, getCandidateDetails } from "@/services/candidateService";
import { useEffect, useState } from "react";
import CandidatesTable from "@/components/candidates/CandidatesTable";
import AddCandidateModal from "@/components/candidates/AddCandidateModal";
import CandidateDetailsModal from "@/components/candidates/CandidateDetailsModal";
import { Candidate, CandidateDetail } from "@/types/candidate";
import DashboardPageHeader from "@/components/DashboardHeader";

const ITEMS_PER_PAGE = 8;
const statusLabelToNumber: Record<string, number | undefined> = {
  "All Status": undefined,
  "No Status": undefined,
  Pending: 0,
  Expired: 1,
  "In Progress": 2,
  Done: 3,
};

export default function CandidatesPage() {
  const [candidates, setCandidates] = useState<Candidate[]>([]);
  const [emailError, setEmailError] = useState("");
  const [showAddModal, setShowAddModal] = useState(false);
  const [selectedCandidate, setSelectedCandidate] = useState<CandidateDetail | null>(null);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("All Status");
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

const fetchCandidates = (page: number, searchVal: string, statusVal: string) => {
  const isNoStatus = statusVal === "No Status";
  const status = isNoStatus ? undefined : statusLabelToNumber[statusVal];

  getCandidates(page, ITEMS_PER_PAGE, searchVal || undefined, status, isNoStatus)
    .then((res) => {
      setCandidates(res.items);
      setTotalPages(res.totalPages);
    });
};

  // fetch when page changes
  useEffect(() => {
    fetchCandidates(currentPage, search, statusFilter);
  }, [currentPage]);

  // reset to page 1 when search or filter changes
  useEffect(() => {
    setCurrentPage(1);
    fetchCandidates(1, search, statusFilter);
  }, [search, statusFilter]);

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
      fetchCandidates(currentPage, search, statusFilter);
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
    fetchCandidates(currentPage, search, statusFilter);
  };

  return (
    <div className="p-8">
      <DashboardPageHeader
        title="Candidates"
        buttonText="+ Add Candidate"
        onButtonClick={() => setShowAddModal(true)}
      />

      <CandidatesTable
        candidates={candidates}
        search={search}
        onSearchChange={setSearch}
        statusFilter={statusFilter}
        onStatusFilterChange={setStatusFilter}
        onViewCandidate={handleViewCandidate}
        onDeleteCandidate={handleDelete}
        currentPage={currentPage}
        totalPages={totalPages}
        onPageChange={setCurrentPage}
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
