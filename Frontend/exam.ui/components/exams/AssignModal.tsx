'use client';
import { useState } from 'react';
import { XMarkIcon, ClockIcon, QuestionMarkCircleIcon, MagnifyingGlassIcon, ClipboardDocumentIcon, InformationCircleIcon } from '@heroicons/react/24/outline';
import Button from '../Button';

interface AssignModalProps {
exam: { id: number; name: string; durationMinutes: number; totalQuestions: number };
  onClose: () => void;
  // This now explicitly requires the examId along with the other data
  onConfirm: (payload: { examId: number, candidateIds: number[], deadline: string }) => void;
}
interface Candidate {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
}
export default function AssignModal({ exam, onClose, onConfirm }: AssignModalProps) {
  // 1. State for selected candidate IDs
  const [selectedIds, setSelectedIds] = useState<number[]>([]);
  const [deadlineDate, setDeadlineDate] = useState<string>("");
  const [selectedHour, setSelectedHour] = useState<string>("12:00");



  const toggleCandidate = (id: number) => {
    setSelectedIds(prev => 
      prev.includes(id) ? prev.filter(item => item !== id) : [...prev, id]
    );
  };

  const isFutureDateTime = () => {
    if (!deadlineDate) return false;
    const selected = new Date(`${deadlineDate}T${selectedHour}:00`);
    return selected > new Date();
  };
  // 2. Initialize the state with mock data (this will later be a fetch call)
  const [candidates, setCandidates] = useState<Candidate[]>([
    { id: 1, firstName: "Ahmed", lastName: "Mohamed", email: "ahmed@example.com" },
    { id: 2, firstName: "Sara", lastName: "Ali", email: "sara@example.com" },
    { id: 3, firstName: "Omar", lastName: "Hassan", email: "omar@example.com" },
  ]);
  const isValid = selectedIds.length > 0 && deadlineDate !== "" && isFutureDateTime();

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm p-4 text-body">
      <div className="bg-white w-full max-w-lg rounded-3xl shadow-2xl flex flex-col max-h-[90vh] overflow-hidden">
        
        {/* Header (Exam Info) */}
        <div className="p-7 border-b border-slate-50 shrink-0">
          <div className="flex justify-between items-start mb-2">
            <h2 className="text-title">Assign to Candidate</h2>
            <button onClick={onClose} className="text-slate-400 hover:text-foreground"><XMarkIcon className="w-7 h-7" /></button>
          </div>
          <div className="flex items-center gap-3 text-muted">
            <span className="flex items-center gap-1.5"><ClockIcon className="w-4 h-4" /> {exam.durationMinutes} min</span>
            <span>•</span>
            <span className="flex items-center gap-1.5"><QuestionMarkCircleIcon className="w-4 h-4" /> {exam.totalQuestions} questions</span>
          </div>
        </div>

        <div className="p-7 space-y-6 overflow-y-auto custom-scrollbar">
          {/* Selected Exam Display */}
          <div className="bg-blue-50 text-blue-600 px-5 py-4 rounded-2xl flex items-center gap-3.5 font-bold">
            <ClipboardDocumentIcon className="w-5 h-5" /> {exam.name}
          </div>

          {/* Candidate List Selection */}
          <div className="space-y-3">
            <label className="text-label block mb-2">Select Candidates</label>
            <div className="space-y-2 max-h-48 overflow-y-auto pr-2">
              {/* This will later be replaced by your backend data */}

              {candidates.map((candidate) => (
                <label key={candidate.id} className={`flex items-center gap-4 p-4 rounded-2xl border transition-all cursor-pointer ${
                    selectedIds.includes(candidate.id) ? 'border-primary bg-red-50/30' : 'border-slate-100'
                  }`}>
                  <input 
                    type="checkbox" className="w-5 h-5 rounded text-primary focus:ring-primary"
                    checked={selectedIds.includes(candidate.id)}
                    onChange={() => toggleCandidate(candidate.id)}
                  />
                  <div>
                    <p className="font-bold text-sm">{candidate.firstName} {candidate.lastName}</p>
                    <p className="text-muted text-xs">{candidate.email}</p>
                  </div>
                </label>
              ))}
            </div>
          </div>

          {/* DateTime Selection */}
          <div className="flex gap-4">
            <div className="flex-[2]">
              <label className="block text-sm font-bold mb-2">Expiry Date</label>
              <input 
                type="date" 
                min={new Date().toISOString().split("T")[0]}
                value={deadlineDate}
                onChange={(e) => setDeadlineDate(e.target.value)}
                className="w-full px-5 py-3 border border-slate-200 rounded-2xl outline-none text-sm" 
              />
            </div>
            <div className="flex-1">
              <label className="block text-sm font-bold mb-2">Hour</label>
              <select 
                value={selectedHour}
                onChange={(e) => setSelectedHour(e.target.value)}
                className="w-full px-5 py-3 border border-slate-200 rounded-2xl outline-none text-sm bg-white"
              >
                {Array.from({ length: 24 }).map((_, i) => (
                  <option key={i} value={`${i < 10 ? '0'+i : i}:00`}>{i < 10 ? '0'+i : i}:00</option>
                ))}
              </select>
            </div>
          </div>

          {!isFutureDateTime() && deadlineDate && (
             <p className="text-error italic">Please select a future time.</p>
          )}

          <div className="bg-blue-50 border border-blue-100 text-blue-600 p-4 rounded-2xl flex items-start gap-3.5 text-sm font-medium">
            <InformationCircleIcon className="w-5 h-5 shrink-0" />
            <p>A unique exam link will be emailed to all {selectedIds.length} chosen candidates.</p>
          </div>
        </div>

        {/* Footer */}
        <div className="p-7 pt-5 border-t border-slate-50 flex justify-end items-center gap-4">
           {!isValid && <p className="text-muted text-[11px] uppercase font-bold tracking-wider">Fill all fields</p>}
          <Button 
            text="Cancel"
            onClick={onClose}
            className="btn-secondary" 
          />
          <Button 
              text="Done"
              className={`btn-primary ${!isValid ? "opacity-50 cursor-not-allowed" : ""}`}
              disabled={!isValid}
              onClick={() => {
                if (isValid) {
                  onConfirm({
                    examId: exam.id,              // <--- Include the Exam ID here
                    candidateIds: selectedIds,    // <--- Array of IDs [1, 2, 3]
                    deadline: `${deadlineDate}T${selectedHour}:00` // <--- ISO string for .NET DateTime
                  });
                }
              }}
            />
        </div>
      </div>
    </div>
  );
}