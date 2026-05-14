'use client';
import { useState } from 'react'; 
import ExamCard from '@/components/exams/ExamCard';
import Button from '@/components/Button';
import AssignModal from '@/components/exams/AssignModal';
import { PlusIcon } from '@heroicons/react/24/outline';
import Message from '@/components/Message'; 
import CreateExamModal from '@/components/exams/CreateExamModal';
import { SearchBar } from '@/components/exams/SearchBar';
import ViewExamModal from '@/components/exams/ViewExamModal';
import DashboardPageHeader from '@/components/DashboardHeader';
import ConfirmDeleteModal from '@/components/ConfirmDeleteModal';
// Interfaces remain the same
interface Question {
  text: string;
  topic: string;
  options: Array<{ text: string; isCorrect: boolean }>;
}

interface Exam {
  id: number;
  name: string;
  topics: string;
  durationMinutes: number;
  totalQuestions: number;
  questions?: Question[];
}

export default function ExamsPage() {
  const [exams, setExams] = useState<Exam[]>([
  { 
    id: 1, 
    name: "Frontend Developer Test", 
    topics: "React, TypeScript", 
    durationMinutes: 60, 
    totalQuestions: 1,
    // Add the actual question data here
    questions: [
      { 
        text: "What is a Hook in React?", 
        topic: "React", 
        options: [
          { text: "A function", isCorrect: true },
          { text: "A CSS property", isCorrect: false }
        ] 
      }
    ] 
  },
  // ... do the same for other mock exams
]);

  const [examToAssign, setExamToAssign] = useState<Exam | null>(null);
  const [examToView, setExamToView] = useState<Exam | null>(null);
  const [examToEdit, setExamToEdit] = useState<any | null>(null);
  const [examToDelete, setExamToDelete] = useState<Exam | null>(null);
  const [fullExamData, setFullExamData] = useState<any | null>(null);
  const [isLoadingView, setIsLoadingView] = useState(false);
  const [message, setMessage] = useState<{ message: string; type: 'success' | 'error' } | null>(null);
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');

  // --- NEW: Missing Handler Functions ---

  const handleCreateExam = async (newData: any) => {
   const newExam: Exam = {
    // Generate a unique ID (use timestamp if you don't have a DB yet)
    id: Date.now(), 
    name: newData.title,
    // Extract unique topics from the questions added
    topics: Array.from(new Set(newData.questions.map((q: any) => q.topic))).join(', '),
    durationMinutes: newData.durationMins,
    totalQuestions: newData.questions.length,
    // CRITICAL: Save the actual questions array here!
    questions: newData.questions};
    setExams(prev => [...prev, newExam]);
    setMessage({ message: 'Exam created with ' + newExam.totalQuestions + ' questions', type: 'success' });
    setTimeout(() => {
      setMessage(null);
    }, 3000);
    };

  const handleUpdateExam = async (id: number, updatedData: any) => {
    setExams((prevExams) => 
    prevExams.map((exam) => 
      exam.id === id 
        ? { 
            ...exam, 
            name: updatedData.title, 
            durationMinutes: updatedData.durationMins,
            // Capture the questions from the modal
            questions: updatedData.questions, 
            totalQuestions: updatedData.questions.length,
            topics: Array.from(new Set(updatedData.questions.map((q: any) => q.topic))).join(', ')
          } 
        : exam
    )
  );
  setMessage({ message: 'Changes saved!', type: 'success' });
  setTimeout(() => {
    setMessage(null);
  }, 3000);
  };
  const handleDeleteExam = async (id: number) => {
    if (!examToDelete) return;

    try {
      setExams(prev => prev.filter(exam => exam.id !== examToDelete.id));
      setMessage({ message: 'Exam deleted successfully', type: 'success' });
      
      // Auto-hide success message
      setTimeout(() => setMessage(null), 3000);
    } catch (error) {
      setMessage({ message: 'Failed to delete exam', type: 'error' });
    }
    
    setExamToDelete(null); // Close the modal
  };
  const confirmDelete = () => {
    if (!examToDelete) return;

    try {
      // Update local state
      setExams(prev => prev.filter(exam => exam.id !== examToDelete.id));
      
      // Feedback to user
      setMessage({ message: 'Exam deleted successfully', type: 'success' });
      
      // Clean up popup
      setTimeout(() => setMessage(null), 3000);
    } catch (error) {
      setMessage({ message: 'Failed to delete exam', type: 'error' });
    }
    
    // Close the modal
    setExamToDelete(null); 
  };
    // --- Fixed Filter Logic ---
  const filteredExams = exams.filter((exam) => {
    const searchTerm = searchQuery.toLowerCase();
    return (
      exam.name.toLowerCase().includes(searchTerm) ||
      exam.topics.toLowerCase().includes(searchTerm)
    );
  });

  const handleViewExam = async (exam: Exam) => {
    // Find the most up-to-date version of this exam from our state
  const currentExam = exams.find(e => e.id === exam.id);
  
  if (currentExam) {
    setFullExamData(currentExam); // This now contains the REAL questions
    setExamToView(currentExam);
  }
  };

  const handleAssignment = async (data: any) => {
    setExamToAssign(null);
    setMessage({ message: 'Invitations sent successfully!', type: 'success' });
    console.log(data);
    setTimeout(() => setMessage(null), 3000);
  };

  return (
    <main>
      {/* Header */}
      {/* <div className="flex items-center justify-between mb-8">
        <h2 className="text-heading text-2xl font-bold">Exams</h2>
      </div> */}
      <DashboardPageHeader
        title="Exams"
        buttonText="+ Add Exam"
        onButtonClick={() => setIsCreateOpen(true)}
      />
      

      {/* Action Bar */}
      <div className="mb-4 flex flex-col justify-between gap-4 md:flex-row md:items-center">
        <div className="relative flex-1 max-w-md">
          <SearchBar placeholder="Search exams..." value={searchQuery} onChange={setSearchQuery} />
        </div>
        {/* <Button 
          text={<span className="flex items-center gap-2"><PlusIcon className="w-5 h-5 stroke-[2.5px]" /> Create Exam</span>}
          onClick={() => setIsCreateOpen(true)}
          className="btn-primary !w-auto px-8"
        /> */}
      </div>

      {/* Grid */}
      <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
        {filteredExams.map((exam) => (
          <ExamCard 
            key={exam.id} 
            exam={exam}
            onAssign={(e) => setExamToAssign(e)}
            onView={() => handleViewExam(exam)}
            onEdit={((clickedExam) => setExamToEdit(clickedExam))}
            onDelete={(id) => setExamToDelete(exam)}
          />
        ))}
      </div>

      {/* Unified Create/Edit Modal */}
      {(isCreateOpen || examToEdit) && (
        <CreateExamModal 
          initialData={examToEdit} 
          onClose={() => {
            setIsCreateOpen(false);
            setExamToEdit(null);
          }}
          onSave={(data) => {
            console.log(data);
            if (examToEdit) {
              handleUpdateExam(examToEdit.id, data);
            } else {
              handleCreateExam(data);
            }
            setIsCreateOpen(false);
            setExamToEdit(null);
          }} 
        />
      )}

      {/* View Modal */}
      {examToView && (
        <ViewExamModal 
          exam={fullExamData || examToView} 
          isLoading={isLoadingView}
          onClose={() => { setExamToView(null); setFullExamData(null); }} 
          onEdit={() => {
            const data = fullExamData || examToView;
            setExamToView(null); 
            setExamToEdit(data);
          }} 
        />
      )}
      {examToDelete && (
  <ConfirmDeleteModal 
    title={examToDelete.name}
    onCancel={() => setExamToDelete(null)}
    onConfirm={confirmDelete}
    text="Are you sure you want to delete this exam?"
  />
)}

      {/* Assign Modal */}
      {examToAssign && (
        <AssignModal 
          exam={examToAssign} 
          onClose={() => setExamToAssign(null)}
          onConfirm={handleAssignment} 
        />
      )}

      {message && <Message message={message.message} type={message.type} onClose={() => setMessage(null)} />}
    </main>
  );
}
