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
        if (!normalizedTopicId) {
          throw new Error(`Missing topic id for existing question: ${question.text}`);
        }

        await updateQuestion(Number(question.id), {
          topicId: normalizedTopicId,
          ...payload,
        });
        ids.push(Number(question.id));
        continue;
      }

      if (!normalizedTopicId) {
        throw new Error(`Missing topic id for new question: ${question.text}`);
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
      setMessage({ message: 'Exam created successfully.', type: 'success' });
    } catch (error: any) {
      setMessage({ message: error?.message || 'Failed to create exam.', type: 'error' });
    } finally {
      setTimeout(() => setMessage(null), 3000);
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
      setMessage({ message: 'Exam updated successfully.', type: 'success' });
    } catch (error: any) {
      setMessage({ message: error?.message || 'Failed to update exam.', type: 'error' });
    } finally {
      setTimeout(() => setMessage(null), 3000);
    }
  };
  const confirmDelete = async() => {
    if (!examToDelete) return;

    try {
      await deleteExam(examToDelete.id);
      setExams(prev => prev.filter(e => e.id !== examToDelete.id));
      setMessage({ message: 'Exam deleted successfully', type: 'success' });
      setTimeout(() => {
      setMessage(null); // or setMessage({ message: '', type: '' }) depending on your state setup
    }, 3000);
    } catch (error) {
      setMessage({ message: 'Failed to delete exam', type: 'error' });
      setTimeout(() => {
      setMessage(null);
    }, 3000);
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
      setMessage({ message: 'Unable to load exam details.', type: 'error' });
    } finally {
      setIsLoadingView(false);
    }
  };

  const handleEditExam = async (exam: Exam) => {
    try {
      const details = await getExamById(exam.id);
      setExamToEdit(details);
    } catch (error) {
      setMessage({ message: 'Unable to load exam for editing.', type: 'error' });
      setTimeout(() => setMessage(null), 3000);
    }
  };

 const handleAssignment = async (data: { examId: number; candidateIds: number[]; deadline: string }): Promise<void> => {
  try {
    // Convert the string generated by the modal into a clean ISO UTC timestamp
    const utcExpiryDate = new Date(data.deadline).toISOString();

    const payload = {
      examId: data.examId,
      candidateIds: data.candidateIds,
      expiryDate: utcExpiryDate,
    };
    console.log("Payload for invitations:", payload); // Debug log to verify payload structure
    // Call POST /api/invitations/send via your apiFetch client
    await invitationService.sendExamInvitations(payload);

    setMessage({ message: 'Invitations sent successfully!', type: 'success' });
    setExamToAssign(null);
  } catch (error: any) {
    setMessage({ 
      message: error?.message || 'Failed to dispatch examination links.', 
      type: 'error' 
    });
    throw error;
  } finally {
    setTimeout(() => setMessage(null), 4000);
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
      setMessage({ message: 'Sync failed. Ensure the exam questions are valid.', type: 'error' });
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
