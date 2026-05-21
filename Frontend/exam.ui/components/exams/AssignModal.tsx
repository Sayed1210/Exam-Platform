'use client';
import { useEffect, useState } from 'react';
import { 
  ClockIcon, 
  QuestionMarkCircleIcon, 
  ClipboardDocumentIcon, 
  InformationCircleIcon 
} from '@heroicons/react/24/outline';
import Button from '../Button';
import SearchInput from '../question-bank/SearchInput';
import ExamModal from './ExamModal';
import { Exam } from '@/types/exam';
import type { Candidate } from '@/types/candidate';
import { getUnassignedCandidates } from '@/services/candidateService';
import { FormValidation } from '@/schemas/form-validation';
import { assignExamSchema } from '@/schemas/requests/assign-exam-request';

interface AssignModalProps {
  exam: Exam;
  onClose: () => void;
  onConfirm: (payload: { examId: number; candidateIds: number[]; deadline: string }) => Promise<void>;
}

type CandidateSummary = {
  id: string | number;
  firstName: string;
  lastName: string;
  email: string;
};

// type AssignExamFormValues = {
//   candidateIds: string[];
//   deadline: string;
// };

export default function AssignModal({ exam, onClose, onConfirm }: AssignModalProps) {
  const [selectedIds, setSelectedIds] = useState<string[]>([]);
  const [deadlineDate, setDeadlineDate] = useState<string>('');
  const [selectedHour, setSelectedHour] = useState<string>('12:00');
  const [searchQuery, setSearchQuery] = useState('');
  const [candidates, setCandidates] = useState<CandidateSummary[]>([]);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [submissionError, setSubmissionError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    const loadCandidates = async () => {
      try {
        const data = (await getUnassignedCandidates()) as CandidateSummary[];
        setCandidates(Array.isArray(data) ? data : []);
      } catch (error) {
        setLoadError('Unable to load candidates. Please try again.');
      } finally {
        setIsLoading(false);
      }
    };

    loadCandidates();
  }, []);

  const toggleCandidate = (id: string) => {
    setSelectedIds((prev) =>
      prev.includes(id) ? prev.filter((item) => item !== id) : [...prev, id]
    );
  };

  const filteredCandidates = candidates.filter((candidate) =>
    `${candidate.firstName} ${candidate.lastName}`.toLowerCase().includes(searchQuery.toLowerCase()) ||
    candidate.email.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const deadlineIso = `${deadlineDate}T${selectedHour}:00`;
  const validation = FormValidation(assignExamSchema, {
    candidateIds: selectedIds,
    deadline: deadlineIso,
  });

  const validationErrors = validation.success ? {} : validation.errors;
  const isValid = validation.success && !isLoading;

  return (
    <ExamModal onClose={onClose} title="Assign to Candidate" disableClose={isSubmitting}>
      {/* Metrics Header */}
      <div className="px-7 bg-slate-50/50 border-b border-slate-100 flex items-center gap-4 text-slate-500 text-sm shrink-0">
        <span className="flex items-center gap-1.5"><ClockIcon className="w-4 h-4" /> {exam.durationMins} min</span>
        <span className="text-slate-300">|</span>
        <span className="flex items-center gap-1.5"><QuestionMarkCircleIcon className="w-4 h-4" /> {exam.totalQuestions} questions</span>
      </div>

      {/* Scrollable Body - pb-20 ensures content clears the sticky footer */}
      <div className="p-7 space-y-6 overflow-y-auto custom-scrollbar pb-20">
        <div className="bg-blue-50 text-blue-600 px-5 py-4 rounded-2xl flex items-center gap-3.5 font-bold">
          <ClipboardDocumentIcon className="w-5 h-5" /> {exam.title}
        </div>
        <div className="mb-4">
          <SearchInput 
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
              filteredCandidates.map((candidate) => {
                const candidateId = String(candidate.id);
                return (
                  <label 
                    key={candidateId} // Retains candidate.id for backend integration
                    className={`flex items-center gap-4 p-4 rounded-2xl border transition-all cursor-pointer ${
                      selectedIds.includes(candidateId) 
                        ? 'border-primary bg-red-50/10' 
                        : 'border-slate-100 bg-white hover:border-slate-200'
                    }`}
                  >
                    <input 
                      type="checkbox" 
                      className="w-5 h-5 rounded text-primary focus:ring-primary accent-primary"
                      checked={selectedIds.includes(candidateId)} // Functionality preserved
                      onChange={() => toggleCandidate(candidateId)}
                    />
                    <div>
                      <p className="font-bold text-sm text-slate-800">
                        {candidate.firstName} {candidate.lastName}
                      </p>
                      <p className="text-slate-400 text-xs">{candidate.email}</p>
                    </div>
                  </label>
                );
              })
            ) : (
              <div className="text-center py-8 text-slate-400 text-sm italic">
                No candidates found matching "{searchQuery}"
              </div>
            )}
          </div>
          {(loadError || validationErrors.candidateIds) && (
            <p className="text-red-500 text-xs mt-1">{loadError ?? validationErrors.candidateIds}</p>
          )}
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

        {validationErrors.deadline && (
           <p className="text-red-500 text-xs italic font-medium">{validationErrors.deadline}</p>
        )}

        {submissionError && (
          <div className="bg-red-50 border border-red-200 text-red-700 p-4 rounded-2xl text-sm">
            {submissionError}
          </div>
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
          onClick={isSubmitting ? undefined : onClose}
          className={`btn-secondary px-6 ${isSubmitting ? 'opacity-50 cursor-not-allowed' : ''}`} 
          disabled={isSubmitting}
        />
        <Button 
          text={isSubmitting ? 'Sending...' : 'Assign Exam'}
          className={`btn-primary px-8 ${(!isValid || isSubmitting) ? 'opacity-50' : ''}`}
          disabled={!isValid || isSubmitting}
          onClick={async () => {
            if (!isValid || isSubmitting) return;

            setSubmissionError(null);
            setIsSubmitting(true);

            const localDateTimeString = `${deadlineDate}T${selectedHour}:00`;
            try {
              await onConfirm({
                examId: exam.id,
                candidateIds: selectedIds
                  .map((id) => Number(id))
                  .filter((id) => !Number.isNaN(id)),
                deadline: localDateTimeString,
              });
            } catch (error: any) {
              setSubmissionError(error?.message || 'Failed to send invitations.');
            } finally {
              setIsSubmitting(false);
            }
          }}  
        />
      </div>
    </ExamModal>
  );
}