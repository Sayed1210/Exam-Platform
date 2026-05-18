'use client';
import { useState } from 'react'; 
import { useEffect } from 'react';
import { createExamBackendSchema } from '@/schemas/requests/create-exam-request';
import { getExams, deleteExam, createExam } from '@/services/examService';
import { Exam } from '@/types/exam';
import ExamCard from '@/components/exams/ExamCard';
import AssignModal from '@/components/exams/AssignModal';
import Message from '@/components/Message'; 
import CreateExamModal from '@/components/exams/CreateExamModal';
import { SearchBar } from '@/components/exams/SearchBar';
import ViewExamModal from '@/components/exams/ViewExamModal';
import DashboardPageHeader from '@/components/DashboardHeader';
import ConfirmDeleteModal from '@/components/ConfirmDeleteModal';
// Interfaces remain the same

export default function ExamsPage() {
  const [exams, setExams] = useState<Exam[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [examToAssign, setExamToAssign] = useState<Exam | null>(null);
  const [examToView, setExamToView] = useState<Exam | null>(null);
  const [examToEdit, setExamToEdit] = useState<any | null>(null);
  const [examToDelete, setExamToDelete] = useState<Exam | null>(null);
  const [fullExamData, setFullExamData] = useState<any | null>(null);
  const [isLoadingView, setIsLoadingView] = useState(false);
  const [message, setMessage] = useState<{ message: string; type: 'success' | 'error' } | null>(null);
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');  

  useEffect(()=>{
    const loadExams=async()=>{
      try{
        const data=await getExams();
        setExams(data);
      }catch(error){
        setMessage({ message: 'Failed to sync with backend.', type: 'error' });
      }finally{
        setIsLoading(false);
      }
    };
    loadExams();
  },[]);
//   { 
//     id: 1, 
//     name: "Frontend Developer Test", 
//     topics: "React, TypeScript", 
//     durationMinutes: 60, 
//     totalQuestions: 1,
//     // Add the actual question data here
//     questions: [
//       { 
//         text: "What is a Hook in React?", 
//         topic: "React", 
//         options: [
//           { text: "A function", isCorrect: true },
//           { text: "A CSS property", isCorrect: false }
//         ] 
//       }
//     ] 
//   },
//   // ... do the same for other mock exams
// ]);


  // --- NEW: Missing Handler Functions ---

  const handleCreateExam = async (newData: any) => {
   const newExam: Exam = {
    id: Date.now(), 
    title: newData.title,
    topics: Array.from(new Set(newData.questions.map((q: any) => q.topic))).join(', '),
    durationMins: newData.durationMins,
    totalQuestions: newData.questions.length,
    createdAt: new Date().toISOString(),
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
            title: updatedData.title, 
            durationMins: updatedData.durationMins,
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
  const confirmDelete = async() => {
    if (!examToDelete) return;

    try {
      await deleteExam(examToDelete.id);
      setExams(prev => prev.filter(e => e.id !== examToDelete.id));
      setMessage({ message: 'Exam deleted successfully', type: 'success' });
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
      exam.title.toLowerCase().includes(searchTerm) ||
      (exam.topics ?? '').toLowerCase().includes(searchTerm)
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
    <main className="p-8">
      {/* Header */}
      {/* <div className="flex items-center justify-between mb-8">
        <h2 className="text-heading text-2xl font-bold">Exams</h2>
      </div> */}
      <DashboardPageHeader
        title="Exams"
        buttonText="+ Create Exam"
        onButtonClick={() => setIsCreateOpen(true)}
      />
      

      {/* Action Bar */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 mb-10 pb-8 border-b border-slate-50">
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
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
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
          onSave={async (data) => {
    try {
      // transform form data to the IDs your backend expects
      const payload = {
        title: data.title,
        durationMins: data.durationMins,
        questionIds: data.questions.map((q: any) => q.id)
      };

      if (examToEdit) {
        // 1. Call PATCH /api/exams/{id}
       // const updated = await examService.updateExam(examToEdit.id, payload);
        //setExams(prev => prev.map(e => e.id === examToEdit.id ? updated : e));
        //setMessage({ message: 'Exam updated in database!', type: 'success' });
      } else {
        // 2. Call POST /api/exams
        const created = await createExam(payload);
        setExams(prev => [...prev, created]);
        setMessage({ message: 'Exam saved to database!', type: 'success' });
      }
    } catch (error) {
      setMessage({ message: 'Sync failed. Ensure dummy question IDs exist.', type: 'error' });
    } finally {
      setIsCreateOpen(false);
      setExamToEdit(null);
      setTimeout(() => setMessage(null), 3000);
    }
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
    title={examToDelete.title}
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
