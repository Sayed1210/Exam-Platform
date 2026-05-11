'use client';
import { useState } from 'react'; 
import ExamCard from '@/components/exams/ExamCard';
import Button from '@/components/Button';
import AssignModal from '@/components/exams/AssignModal';
import { PlusIcon, MagnifyingGlassIcon } from '@heroicons/react/24/outline';
import Message from '@/components/Message'; 

// Example data structure
//const examList = [
  //{ id: 1, name: "Frontend Developer Test", topics: "React, TypeScript, and CSS fundamentals", durationMinutes: 60, totalQuestions: 25 },
  //{ id: 2, name: "Backend Engineer Test", topics: "Node.js, databases, and API design", durationMinutes: 90, totalQuestions: 30 },
  //{ id: 3, name: "Full Stack Assessment", topics: "Comprehensive full-stack development test", durationMinutes: 120, totalQuestions: 40 }
//];

//Define the interface to match .NET DTO
interface Exam {
  id: number;
  name: string;
  topics: string;
  durationMinutes: number;
  totalQuestions: number;
}

export default function ExamsPage() {
  const [selectedExam, setSelectedExam] = useState<any>(null);
  const [message, setMessage] = useState<{ message: string; type: 'success' | 'error' } | null>(null);
  // for the search query
  const [searchQuery, setSearchQuery] = useState('');
  // 2. Initialize state with mock data (Preparation for DB)
  // These property names (name, topics, etc.) match standard .NET JSON serialization
  const [exams, setExams] = useState<Exam[]>([
    { id: 1, name: "Frontend Developer Test", topics: "React, TypeScript, and CSS", durationMinutes: 60, totalQuestions: 25 },
    { id: 2, name: "Backend Engineer Test", topics: "Node.js, SQL, and API Design", durationMinutes: 90, totalQuestions: 30 },
    { id: 3, name: "Full Stack Assessment", topics: "Full-stack development lifecycle", durationMinutes: 120, totalQuestions: 40 }
  ]);
  // Filter the exam list dynamically
  // This looks at both the exam name and the topics
    const filteredExams = exams.filter((exam) => {
    const searchTerm = searchQuery.toLowerCase();
    
    return (
      exam.name.toLowerCase().includes(searchTerm) ||
      exam.topics.toLowerCase().includes(searchTerm)
    );
  });
  /**
   * ADJUSTED: handleAssignment now receives the structured data object.
   * This object matches your .NET DTO: { examId, candidateIds, deadline }
   */
  const handleAssignment = async (data: { examId: number, candidateIds: number[], deadline: string }) => {
    // 1. Close the modal immediately for a smooth UX
    setSelectedExam(null); 

    try {
      // In the future, this is where you'll call: 
      // const response = await sendInvitations(data);
      
      // Simulating a successful backend response for now
      const isApiSuccessful = true; 

      if (isApiSuccessful) {
        setMessage({ message: 'Invitations sent successfully!', type: 'success' });
        console.log("Data ready for .NET Backend:", data);
      } else {
        setMessage({ message: 'Server error: Could not process invitations.', type: 'error' });
      }
    } catch (error) {
      setMessage({ message: 'Connection failed. Please check your internet.', type: 'error' });
    }

    // Auto-hide the success/error message
    setTimeout(() => setMessage(null), 4000);
  };

  return (
    <main className="p-8 max-w-7xl mx-auto">
      
      {/* Header Section */}
      <div className="flex items-center justify-between mb-8">
        <h2 className="text-heading text-2xl">Exams</h2>
      </div>

      {/* Search and Action Bar */}
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 mb-10 pb-8 border-b border-slate-50">
          <div className="relative flex-1 max-w-md">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <MagnifyingGlassIcon className="w-5 h-5 text-muted" />
            </div>
            {/* 3. Connect the input to the state */}
            <input
              type="text"
              placeholder="Search exams..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="w-full bg-white pl-10 pr-4 py-2.5 rounded-xl border border-slate-200 focus:outline-none focus:ring-2 focus:ring-primary/20 transition-all text-body"
            />
          </div>

          <Button 
            text={
              <span className="flex items-center gap-2">
                <PlusIcon className="w-5 h-5 stroke-[2.5px]" />
                Create Exam
              </span>
            }
            className="btn-primary !w-auto px-8"
          />
        </div>

        <div className="mb-6">
          <p className="text-muted">
            {/* 4. Use the filtered list length */}
            {filteredExams.length} {filteredExams.length === 1 ? 'Exam' : 'Exams'} Found
          </p>
        </div>

        {/* 5. Map over filteredExams instead of examList */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          {filteredExams.length > 0 ? (
            filteredExams.map((exam) => (
              <ExamCard 
                key={exam.id} 
                exam={exam}
                onAssign={(clickedExam) => setSelectedExam(clickedExam)}
              />
            ))
          ) : (
            <div className="col-span-full py-20 text-center">
              <p className="text-muted font-medium">No exams found matching "{searchQuery}"</p>
            </div>
          )}
        </div>

        {/* Notification */}
        {message && (
          <Message 
            message={message.message} 
            type={message.type} 
            onClose={() => setMessage(null)} 
          />
        )}

        {/* ADJUSTED: AssignModal now passes the data object to onConfirm */}
        {selectedExam && (
          <AssignModal 
            exam={selectedExam} 
            onClose={() => setSelectedExam(null)}
            onConfirm={(data) => handleAssignment(data)} 
          />
        )}
    </main>
  );
}