"use client";

import { useEffect, useState } from "react";
import { getQuestions, getTopics, deleteQuestion, createQuestion, updateQuestion, createTopic, updateTopic, deleteTopic } from "@/services/questionService";
import type { APIQuestion, APITopic } from "@/types/question";

import QuestionCard from "./QuestionCard";
import SearchBar from "./SearchInput";
import TopicFilters from "./TopicFilters";
import ConfirmDeleteModal from "@/components/common/ConfirmDeleteModal";
import DashboardPageHeader from "../common/DashboardHeader";
import ModalLayout from "../common/ModalLayout";
import TopicForm from "./forms/TopicForm";
import QuestionForm from "./forms/QuestionForm";
import { ChevronLeftIcon, ChevronRightIcon, MagnifyingGlassIcon } from "@heroicons/react/16/solid";
import Pagination from "../common/Pagination";
import { toast } from "sonner";

const PAGE_SIZE = 10;

export default function QuestionsList() {
  const [questions, setQuestions] = useState<APIQuestion[]>([]);
  const [topics, setTopics] = useState<APITopic[]>([]);
  const [search, setSearch] = useState("");
  const [selectedTopicId, setSelectedTopicId] = useState<number[]>([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [selectedTags, setSelectedTags] = useState<string[]>([]);
  const [questionToDelete, setQuestionToDelete] = useState<number | null>(null);
  const [isTopicModalOpen, setIsTopicModalOpen] = useState(false);
  const [editingTopic, setEditingTopic] = useState<APITopic | null>(null);
  const [topicToDelete, setTopicToDelete] = useState<APITopic | null>(null);
  const [isQuestionModalOpen, setIsQuestionModalOpen] = useState(false);
  const [editingQuestion, setEditingQuestion] = useState<APIQuestion | null>(null);

  // ── Data fetching ──────────────────────────────────────────────
  const fetchQuestions = (page: number, searchVal: string,  topicIds: number[]) => {
    getQuestions(page, PAGE_SIZE, searchVal || undefined, topicIds.length > 0 ? topicIds : undefined).then((res) => {
      setQuestions(res.items);
      setTotalPages(res.totalPages);
    });
  };

  useEffect(() => {
    getTopics().then(setTopics);
  }, []);

  useEffect(() => {
    fetchQuestions(currentPage, search, selectedTopicId);
  }, [currentPage]);

  useEffect(() => {
    setCurrentPage(1);
    fetchQuestions(1, search, selectedTopicId);
  }, [search, selectedTopicId]);

  // ── Topic handlers ─────────────────────────────────────────────
  const openTopicModal = () => {
    setEditingTopic(null);
    setIsTopicModalOpen(true);
  };

  const openEditTopicModal = (topic: APITopic) => {
    setEditingTopic(topic);
    setIsTopicModalOpen(true);
  };

  const closeTopicModal = () => {
    setIsTopicModalOpen(false);
    setEditingTopic(null);
  };

  const handleAddTopic = async (topicName: string) => {
    try{
    if (editingTopic) {
      await updateTopic(editingTopic.id, topicName);
      toast.success("Topic Edited Successfully.");
    } else {
      await createTopic(topicName);
      toast.success("Topic Added Successfully.");
    }
    }catch(err){
      toast.error("Try again Later");
    }
    const updated = await getTopics();
    setTopics(updated);
    if (editingTopic) {
      setSelectedTags((prev) =>
        prev.map((tag) => (tag === editingTopic.title ? topicName : tag))
      );
    }
    fetchQuestions(currentPage, search, selectedTopicId);
    closeTopicModal();
  };

  const confirmDeleteTopic = async () => {
    if (!topicToDelete) return;

    try{
    await deleteTopic(topicToDelete.id);
    toast.success("Topic Deleted Successfully.");
    }catch(err){
      toast.error("Topic not deleted, Try again Later");
    }
    const updated = await getTopics();
    setTopics(updated);
    setSelectedTopicId((prev) => prev.filter((id) => id !== topicToDelete.id));
    setSelectedTags((prev) => prev.filter((tag) => tag !== topicToDelete.title));
    fetchQuestions(1, search, selectedTopicId.filter((id) => id !== topicToDelete.id));
    setCurrentPage(1);
    setTopicToDelete(null);
  };

const toggleTag = (topicId: number) => {
  setSelectedTopicId((prev) =>
    prev.includes(topicId) ? prev.filter((id) => id !== topicId) : [...prev, topicId]
  );
  const topic = topics.find((t) => t.id === topicId);
  if (!topic) return;
  setSelectedTags((prev) =>
    prev.includes(topic.title)
      ? prev.filter((t) => t !== topic.title)
      : [...prev, topic.title]
  );
};

  const clearFilters = () => {
    setSearch("");
    setSelectedTopicId([]);
    setSelectedTags([]);
  };

  // ── Question handlers ──────────────────────────────────────────
  const openAddQuestionModal = () => {
    setEditingQuestion(null);
    setIsQuestionModalOpen(true);
  };

  const openEditQuestionModal = (question: APIQuestion) => {
    setEditingQuestion(question);
    setIsQuestionModalOpen(true);
  };

  const closeQuestionModal = () => {
    setIsQuestionModalOpen(false);
    setEditingQuestion(null);
  };

const handleSaveQuestion = async (data: any) => {
   console.log("Form data received:", JSON.stringify(data, null, 2));
  const payload = {
    topicId: Number(data.topic ?? data.topicId),
    text: data.statement ?? data.text,
    imageUrl: data.imageUrl ?? null,
    choices: data.choices.map((c: any) => ({
      text: c.text ?? null,
      imageUrl: c.imageUrl ?? null,
      isCorrect: c.isCorrect,
    })),
  };

  try {
    if (editingQuestion) {
      await updateQuestion(editingQuestion.id, payload);
      toast.success("Question Edited");
    } else {
      await createQuestion(payload);
      toast.success("Question Added");
    }
    fetchQuestions(currentPage, search, selectedTopicId);
    closeQuestionModal();
  } catch (err: any) {
    toast.error("Question not edited, Try again Later");
  }
};
  const confirmDelete = async () => {
    if (!questionToDelete) return;
    try{
    await deleteQuestion(questionToDelete);
    toast.success("Question Deleted Successfully.");
    }catch(err){
      toast.error("Question not deleted, Try again Later");
    }
    fetchQuestions(currentPage, search, selectedTopicId);
    setQuestionToDelete(null);
  };

  const cancelDelete = () => setQuestionToDelete(null);

  // ── Render ─────────────────────────────────────────────────────
  return (
    <div>
      
      <DashboardPageHeader
        title="Question Bank"
        buttonText="+ Add Question"
        onButtonClick={openAddQuestionModal}
      />

      <div className="card mb-4 overflow-visible">
        <div className="px-5 py-3.5">
          <SearchBar
            value={search}
            onChange={setSearch}
            placeholder="Search questions..."
          />
        </div>

        <TopicFilters
          topics={topics}
          selectedTags={selectedTags}
          onToggleTag={(topic) => toggleTag(topic.id)}
          onEditTopic={openEditTopicModal}
          onDeleteTopic={setTopicToDelete}
          onAddTopic={() => {openTopicModal()}}
        />
      </div>

      <div className="space-y-4">
        {questions.length > 0 ? (
          questions.map((question) => (
            <QuestionCard
              key={question.id}
              question={question}
              onEdit={() => openEditQuestionModal(question)}
              onDelete={() => setQuestionToDelete(question.id)}
            />
          ))
        ) : (
          <div className="flex flex-col items-center justify-center rounded-lg border border-dashed border-gray-300 bg-white py-16 text-center">
            <p className="text-muted text-center mb-4">[ No questions found ]</p>
            <p className="text-muted py-1">Try adjusting your search or filters</p>
            {/* <ClearFiltersButton onClick={clearFilters} /> */}
            <button onClick={clearFilters} className="btn-secondary text-sm">Clear Filters</button>
          </div>
        )}
      </div>

      {/* Pagination */}
      <Pagination
        currentPage={currentPage}
        totalPages={totalPages}
        onPageChange={setCurrentPage}
      />

      {/* Modals */}
      {questionToDelete && (
        <ConfirmDeleteModal
          onConfirm={confirmDelete}
          onCancel={cancelDelete}
          title="Delete Question"
          text="Are you sure you want to delete this question?"
          yesText="Delete"
        />
      )}

      {topicToDelete && (
        <ConfirmDeleteModal
          onConfirm={confirmDeleteTopic}
          onCancel={() => setTopicToDelete(null)}
          title="Delete Topic"
          text={`Are you sure you want to delete "${topicToDelete.title}"?`}
          yesText="Delete"
        />
      )}

      {isTopicModalOpen && (
        <ModalLayout
          title={editingTopic ? "Edit Topic" : "Add Topic"}
        >
          <TopicForm 
            onSubmit={handleAddTopic}
            onCancel={closeTopicModal}
            initialValue={editingTopic?.title}
          />
        </ModalLayout>
      )}

      {isQuestionModalOpen && (
        <ModalLayout
          title="Question"
        >
          <QuestionForm 
            topics={topics}
            question={editingQuestion}
            onSubmit={handleSaveQuestion}
            onCancel={closeQuestionModal}
          />
        </ModalLayout>
      )}

    </div>
  );
}
