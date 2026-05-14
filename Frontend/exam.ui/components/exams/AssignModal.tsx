'use client';
import { useState } from 'react';
import { 
  ClockIcon, 
  QuestionMarkCircleIcon, 
  ClipboardDocumentIcon, 
  InformationCircleIcon 
} from '@heroicons/react/24/outline';
import Button from '../Button';
import { SearchBar } from './SearchBar';
import ExamModal from './ExamModal';
interface AssignModalProps {
  exam: { id: number; name: string; durationMinutes: number; totalQuestions: number };
  onClose: () => void;
  onConfirm: (payload: { examId: number, candidateIds: number[], deadline: string }) => void;
}

interface Candidate {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
}

export default function AssignModal({ exam, onClose, onConfirm }: AssignModalProps) {
  const [selectedIds, setSelectedIds] = useState<number[]>([]);
  const [deadlineDate, setDeadlineDate] = useState<string>("");
  const [selectedHour, setSelectedHour] = useState<string>("12:00");
  const [searchQuery, setSearchQuery] = useState("");

  const [candidates] = useState<Candidate[]>([
    { id: 1, firstName: "Ahmed", lastName: "Mohamed", email: "ahmed@example.com" },
    { id: 2, firstName: "Sara", lastName: "Ali", email: "sara@example.com" },
    { id: 3, firstName: "Omar", lastName: "Hassan", email: "omar@example.com" },
  ]);

  const toggleCandidate = (id: number) => {
    setSelectedIds(prev => 
      prev.includes(id) ? prev.filter(item => item !== id) : [...prev, id]
    );
  };
  const filteredCandidates = candidates.filter(c => 
    `${c.firstName} ${c.lastName}`.toLowerCase().includes(searchQuery.toLowerCase()) ||
    c.email.toLowerCase().includes(searchQuery.toLowerCase())
  );
  const isFutureDateTime = () => {
    if (!deadlineDate) return false;
    const selected = new Date(`${deadlineDate}T${selectedHour}:00`);
    return selected > new Date();
  };

  const isValid = selectedIds.length > 0 && deadlineDate !== "" && isFutureDateTime();

  return (
    <ExamModal onClose={onClose} title="Assign to Candidate">
      {/* Metrics Header */}
      <div className="px-7 py-4 bg-slate-50/50 border-b border-slate-100 flex items-center gap-4 text-slate-500 text-sm shrink-0">
        <span className="flex items-center gap-1.5"><ClockIcon className="w-4 h-4" /> {exam.durationMinutes} min</span>
        <span className="text-slate-300">|</span>
        <span className="flex items-center gap-1.5"><QuestionMarkCircleIcon className="w-4 h-4" /> {exam.totalQuestions} questions</span>
      </div>

      {/* Scrollable Body - pb-20 ensures content clears the sticky footer */}
      <div className="p-7 space-y-6 overflow-y-auto custom-scrollbar pb-20">
        <div className="bg-blue-50 text-blue-600 px-5 py-4 rounded-2xl flex items-center gap-3.5 font-bold">
          <ClipboardDocumentIcon className="w-5 h-5" /> {exam.name}
        </div>
        <div className="mb-4">
          <SearchBar 
            placeholder="Search candidates by name or email..." 
            value={searchQuery} 
            onChange={setSearchQuery} 
          />
        </div>
        <div className="space-y-3">
          <label className="block font-bold text-slate-700">Select Candidates</label>
          <div className="space-y-2">
            {/* 4. Use filteredCandidates here instead of candidates */}
            {filteredCandidates.length > 0 ? (
              filteredCandidates.map((candidate) => (
                <label 
                  key={candidate.id} // Retains candidate.id for backend integration
                  className={`flex items-center gap-4 p-4 rounded-2xl border transition-all cursor-pointer ${
                    selectedIds.includes(candidate.id) 
                      ? 'border-primary bg-red-50/10' 
                      : 'border-slate-100 bg-white hover:border-slate-200'
                  }`}
                >
                  <input 
                    type="checkbox" 
                    className="w-5 h-5 rounded text-primary focus:ring-primary accent-primary"
                    checked={selectedIds.includes(candidate.id)} // Functionality preserved
                    onChange={() => toggleCandidate(candidate.id)}
                  />
                  <div>
                    <p className="font-bold text-sm text-slate-800">
                      {candidate.firstName} {candidate.lastName}
                    </p>
                    <p className="text-slate-400 text-xs">{candidate.email}</p>
                  </div>
                </label>
              ))
            ) : (
              <div className="text-center py-8 text-slate-400 text-sm italic">
                No candidates found matching "{searchQuery}"
              </div>
            )}
          </div>
        </div>

        <div className="flex gap-4">
          <div className="flex-[2]">
            <label className="block text-sm font-bold mb-2 text-label">Expiry Date</label>
            <input 
              type="date" 
              min={new Date().toISOString().split("T")[0]}
              value={deadlineDate}
              onChange={(e) => setDeadlineDate(e.target.value)}
              className="w-full px-5 py-3 border border-slate-200 rounded-2xl outline-none text-sm focus:border-primary transition-all" 
            />
          </div>
          <div className="flex-1">
            <label className="block text-sm font-bold mb-2 text-label">Hour</label>
            <select 
              value={selectedHour}
              onChange={(e) => setSelectedHour(e.target.value)}
              className="w-full px-5 py-3 border border-slate-200 rounded-2xl outline-none text-sm bg-white cursor-pointer focus:border-primary transition-all"
            >
              {Array.from({ length: 24 }).map((_, i) => (
                <option key={i} value={`${i < 10 ? '0'+i : i}:00`}>
                  {i < 10 ? '0'+i : i}:00
                </option>
              ))}
            </select>
          </div>
        </div>

        {!isFutureDateTime() && deadlineDate && (
           <p className="text-red-500 text-xs italic font-medium">Please select a future date and time.</p>
        )}

        <div className="bg-blue-50 border border-blue-100 text-blue-600 p-4 rounded-2xl flex items-start gap-3.5 text-xs font-medium">
          <InformationCircleIcon className="w-5 h-5 shrink-0" />
          <p>A unique exam link will be emailed to all {selectedIds.length} chosen candidates.</p>
        </div>
      </div>

      {/* Fixed Footer */}
      <div className="p-7 border-t border-slate-100 flex justify-end items-center gap-4 bg-white sticky bottom-0 z-10">
        {!isValid && (
          <p className="text-slate-400 text-[10px] uppercase font-bold tracking-widest mr-auto">
            Fill all fields
          </p>
        )}
        <Button 
          text="Cancel"
          onClick={onClose} // Fixed: was onClose={onClose} which caused TS error
          className="btn-secondary px-6" 
        />
        <Button 
          text="Assign Exam"
          className={`btn-primary px-8 ${!isValid ? "opacity-50" : ""}`}
          disabled={!isValid}
          onClick={() => {
            if (isValid) {
              onConfirm({
                examId: exam.id,
                candidateIds: selectedIds,
                deadline: `${deadlineDate}T${selectedHour}:00`
              });
            }
          }}
        />
      </div>
    </ExamModal>
  );
}