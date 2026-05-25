'use client';
import { useState } from 'react'; 
import { useEffect } from 'react';
import { getExams, deleteExam, createExam, updateExam, getExamById } from '@/services/examService';
import { createQuestion, updateQuestion } from '@/services/questionService';
import { Exam } from '@/types/exam';
import ExamsActionBar from '@/components/exams/ExamsActionBar';
import ExamsGrid from '@/components/exams/ExamsGrid';
import AssignModal from '@/components/exams/AssignModal';
import Message from '@/components/Message'; 
import CreateExamModal from '@/components/exams/CreateExamModal';
import ViewExamModal from '@/components/exams/ViewExamModal';
import DashboardPageHeader from '@/components/common/DashboardHeader';
import ConfirmDeleteModal from '@/components/common/ConfirmDeleteModal';
import { invitationService } from '@/services/invitationService';
import SearchInput from '@/components/question-bank/SearchInput';
import { toast } from 'sonner';
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
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');  

  useEffect(()=>{
    const loadExams=async()=>{
      try{
        const data=await getExams();
        setExams(data);
      }catch(error){
        toast.error("Failed to sync with backend")
      }finally{
        setIsLoading(false);
      }
    };
    loadExams();
  },[]);

  // --- NEW: Missing Handler Functions ---

  const resolveQuestionIds = async (questions: any[]) => {
    const ids: number[] = [];

    for (const question of questions) {
      const normalizedTopicId = Number(question.topicId ?? question.topicId);
      const payload = {
        text: question.text,
        imageUrl: question.imageUrl,
        choices: question.options.map((option: any) => ({
          text: option.text,
          imageUrl: option.imageUrl,
          isCorrect: option.isCorrect,
        })),
      };

      if (question.id && Number(question.id) > 0) {
        await updateQuestion(Number(question.id), {
          topicId: normalizedTopicId,
          ...payload,
        });
        ids.push(Number(question.id));
        continue;
      }

      const createdQuestion = await createQuestion({
        topicId: normalizedTopicId,
        ...payload,
      });

      ids.push(Number(createdQuestion.id));
    }

    return ids;
  };

  const handleCreateExam = async (newData: any) => {
    try {
      const questionIds = await resolveQuestionIds(newData.questions);
      const created = await createExam({
        title: newData.title,
        durationMins: newData.durationMins,
        questionIds,
      });

      setExams((prev) => [...prev, created]);
      toast.success("Exam created successfully");
    } catch (error: any) {
      toast.error(error.message);
    } 
  };

  const handleUpdateExam = async (id: number, updatedData: any) => {
    try {
      const questionIds = await resolveQuestionIds(updatedData.questions);
      const updated = await updateExam(id, {
        title: updatedData.title,
        durationMins: updatedData.durationMins,
        questionIds,
      });

      setExams((prevExams) => prevExams.map((exam) => (exam.id === updated.id ? updated : exam)));
      toast.success("Exam updated successfully")
    } catch (error: any) {
      toast.error(error?.message || "Failed to update exam");
    } 
  };
  const confirmDelete = async() => {
    if (!examToDelete) return;

    try {
      await deleteExam(examToDelete.id);
      setExams(prev => prev.filter(e => e.id !== examToDelete.id));
      toast.success("Exam deleted successfully")
    } catch (error: any) {
      toast.error(error.message);
    }
    // Close the modal
    setExamToDelete(null); 
  };
    // --- Fixed Filter Logic ---
  const filteredExams = exams.filter((exam) => {
    const searchTerm = searchQuery.toLowerCase();
    const matchesTitle = exam.title.toLowerCase().includes(searchTerm);
    const matchesTopics = (exam.topics ?? '').toLowerCase().includes(searchTerm);
    const matchesQuestionTopics = exam.questions?.some((question) =>
      (question.topicTitle ?? '').toLowerCase().includes(searchTerm)
    );

    return matchesTitle || matchesTopics || matchesQuestionTopics;
  });

  const handleViewExam = async (exam: Exam) => {
    setIsLoadingView(true);
    try {
      const details = await getExamById(exam.id);
      setFullExamData(details);
      setExamToView(details);
    } catch (error) {
      toast.error("Unable to load exam details");
    } finally {
      setIsLoadingView(false);
    }
  };

  const handleEditExam = async (exam: Exam) => {
    try {
      const details = await getExamById(exam.id);
      setExamToEdit(details);
    } catch (error) {
      toast.error("Unable to load exam for editing");
    }
  };

 const handleAssignment = async (data: { examId: number; candidateIds: number[]; startDeadline: string; endDeadline: string }): Promise<void> => {
  try {
    // Convert the string generated by the modal into a clean ISO UTC timestamp
    const utcStartExpiryDate = new Date(data.startDeadline).toISOString();
    const utcEndExpiryDate = new Date(data.endDeadline).toISOString();

    const payload = {
      examId: data.examId,
      candidateIds: data.candidateIds,
      startDate: utcStartExpiryDate,
      expiryDate: utcEndExpiryDate,
    };
    console.log("Payload for invitations:", payload); // Debug log to verify payload structure

    // Call POST /api/invitations/send via your apiFetch client
    await invitationService.sendExamInvitations(payload);
    // if (!result.success) {
    //   toast.error(result.message || "Failed to dispatch examination links");
    //   return;
    // }

    toast.success("Invitations sent successfully");
    setExamToAssign(null);
  } 
  catch (error: any) {
    toast.error(error?.message || "Failed to dispatch examination links");
  } 
};

  return (
    <main>
      {/* Header */}
      <DashboardPageHeader
        title="Exams"
        buttonText="+ Create Exam"
        onButtonClick={() => setIsCreateOpen(true)}
      />
        <SearchInput 
          placeholder="Search by title or topic..." 
          value={searchQuery} 
          onChange={setSearchQuery} 
        />
      <ExamsGrid
        exams={filteredExams}
        onAssign={(e) => setExamToAssign(e)}
        onView={(e) => handleViewExam(e)}
        onEdit={(e) => handleEditExam(e)}
        onDelete={(e) => setExamToDelete(e)}
      />

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
      if (examToEdit) {
        await handleUpdateExam(examToEdit.id, data);
      } else {
        await handleCreateExam(data);
      }
    } catch (error) {
      toast.error("Sync failed. Ensure the exam questions are valid");
    } finally {
      setIsCreateOpen(false);
      setExamToEdit(null);
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

    </main>
  );
}
