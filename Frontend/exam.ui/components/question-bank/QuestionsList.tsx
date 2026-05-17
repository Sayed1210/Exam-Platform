"use client";

import { useEffect, useState } from "react";
import { getQuestions, getTopics, deleteQuestion, createQuestion, updateQuestion, createTopic } from "@/services/questionService";
import type { APIQuestion, APITopic } from "@/types/question";

import ClearFiltersButton from "@/components/ClearFiltersButton";
import QuestionCard from "./QuestionCard";
import SearchBar from "./SearchBar";
import TopicFilters from "./TopicFilters";
import QuestionModal from "./QuestionModal";
import TopicModal from "./TopicModal";
import ConfirmDeleteModal from "@/components/ConfirmDeleteModal";
import DashboardPageHeader from "../DashboardHeader";

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
  const openTopicModal = () => setIsTopicModalOpen(true);
  const closeTopicModal = () => setIsTopicModalOpen(false);

  const handleAddTopic = async (topicName: string) => {
    await createTopic(topicName);
    const updated = await getTopics();
    setTopics(updated);
    closeTopicModal();
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
    } else {
      await createQuestion(payload);
    }
    fetchQuestions(currentPage, search, selectedTopicId);
    closeQuestionModal();
  } catch (err: any) {
    console.error("Save question error:", err);
  }
};
  const confirmDelete = async () => {
    if (!questionToDelete) return;
    await deleteQuestion(questionToDelete);
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
            placeholder="Search questions by text or topic..."
          />
        </div>

        <TopicFilters
          topics={topics.map((t) => t.title)}
          selectedTags={selectedTags}
          onToggleTag={(title) => {
            const topic = topics.find((t) => t.title === title);
            if (topic) toggleTag(topic.id);
  }}
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
            <p className="text-lg font-medium text-gray-700">No questions found</p>
            <p className="mt-2 text-sm text-gray-500">Try adjusting your search or filters.</p>
            <ClearFiltersButton onClick={clearFilters} />
          </div>
        )}
      </div>

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="flex items-center justify-between px-1 py-4 mt-4">
          <p className="text-sm text-gray-500">
            Page {currentPage} of {totalPages}
          </p>
          <div className="flex items-center gap-2">
            <button
              onClick={() => setCurrentPage((p) => p - 1)}
              disabled={currentPage === 1}
              className="flex items-center gap-1.5 px-3.5 py-2 rounded-lg border border-gray-200 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-40 disabled:cursor-not-allowed transition"
            >
              <svg width="14" height="14" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
                <path d="M15 18l-6-6 6-6" />
              </svg>
              Previous
            </button>
            <button
              onClick={() => setCurrentPage((p) => p + 1)}
              disabled={currentPage === totalPages}
              className="flex items-center gap-1.5 px-3.5 py-2 rounded-lg border border-gray-200 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-40 disabled:cursor-not-allowed transition"
            >
              Next
              <svg width="14" height="14" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
                <path d="M9 18l6-6-6-6" />
              </svg>
            </button>
          </div>
        </div>
      )}

      {/* Modals */}
      {questionToDelete && (
        <ConfirmDeleteModal
             title="Delete Question"
             text="Are you sure you want to delete this question? This action cannot be undone."
             onConfirm={confirmDelete}
             onCancel={cancelDelete}
        />
      )}

  <TopicModal
  isOpen={isTopicModalOpen}
  onClose={closeTopicModal}
  onSave={handleAddTopic}
/>

<QuestionModal
  isOpen={isQuestionModalOpen}
  topics={topics}
  question={editingQuestion}
  onClose={closeQuestionModal}
  onSave={handleSaveQuestion}
/>
</div>

  );
}
