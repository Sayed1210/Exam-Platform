"use client";
import { useMemo, useState } from "react";

import AddQuestionButton from "./AddQuestionButton";
import ClearFiltersButton from "@/components/ClearFiltersButton";
import QuestionCard from "./QuestionCard";
import SearchBar from "./SearchBar";
import TopicFilters from "./TopicFilters";
import QuestionModal from "./QuestionModal";
import TopicModal from "./TopicModal";
import ConfirmDeleteModal from "@/components/ConfirmDeleteModal";
import type { Question } from "@/types/question";


const initialTopics = ["React", "Node.js", "Algorithms", "QA"];

const initialQuestions: Question[] = [
  {
    id: "question-1",
    topic: "React",
    statement: "What is the purpose of React's useEffect hook?",
    imageUrl: "https://illustoon.com/photo/dl/7416.png",
    choices: [
      { id: "q1-a", text: "To manage component state", isCorrect: false },
      { id: "q1-b", text: "To handle side effects in functional components", isCorrect: true },
      { id: "q1-c", text: "To create reusable components", isCorrect: false },
      { id: "q1-d", text: "To optimize rendering performance", isCorrect: false },
    ],
  },
  {
    id: "question-2",
    topic: "Algorithms",
    statement: "What is the time complexity of binary search?",
    choices: [
      { id: "q2-a", text: "O(n)", isCorrect: false },
      { id: "q2-b", text: "O(log n)", isCorrect: true },
      { id: "q2-c", text: "O(n²)", isCorrect: false },
      { id: "q2-d", text: "O(1)", isCorrect: false },
    ],
  },
  {
    id: "question-3",
    topic: "Node.js",
    statement: "Which runtime feature allows Node.js to handle many I/O operations efficiently?",
    choices: [
      { id: "q3-a", text: "The event loop", isCorrect: true },
      { id: "q3-b", text: "Browser rendering", isCorrect: false },
      { id: "q3-c", text: "CSS cascade layers", isCorrect: false },
      { id: "q3-d", text: "Static site maps", isCorrect: false },
    ],
  },
];

export default function QuestionsList() {
  const [search, setSearch] = useState("");
  const [selectedTags, setSelectedTags] = useState<string[]>([]);
  const [questions, setQuestions] = useState<Question[]>(initialQuestions);
  const [topics, setTopics] = useState<string[]>(initialTopics);
  const [questionToDelete, setQuestionToDelete] = useState<string | null>(null);
  const [isTopicModalOpen, setIsTopicModalOpen] = useState(false);
  const [isQuestionModalOpen, setIsQuestionModalOpen] = useState(false);
  const [editingQuestion, setEditingQuestion] = useState<Question | null>(null);
  const filteredQuestions = useMemo(() => {
  const value = search.toLowerCase();

  return questions.filter((question) => {
    const matchesSearch =
      question.statement.toLowerCase().includes(value);

    const matchesTags =
      selectedTags.length === 0 ||
      selectedTags.includes(question.topic);

    return matchesSearch && matchesTags;
  });
}, [questions, search, selectedTags]);


const toggleTag = (tag: string) => {
  setSelectedTags((prev) => {
    if (prev.includes(tag)) {
      return prev.filter((t) => t !== tag);
    }

    return [...prev, tag];
  });
};

const clearFilters = () => {
  setSearch("");
  setSelectedTags([]);
};

const confirmDelete = () => {
  if (!questionToDelete) {
    return;
  }

  setQuestions((prev) =>
    prev.filter(
      (question) => question.id !== questionToDelete
    )
  );

  setQuestionToDelete(null);
};

const cancelDelete = () => {
  setQuestionToDelete(null);
};

const openTopicModal = () => {
  setIsTopicModalOpen(true);
};

const closeTopicModal = () => {
  setIsTopicModalOpen(false);
};

const addTopic = (topicName: string) => {
  setTopics((prev) => [...prev, topicName]);

  closeTopicModal();
};

const openAddQuestionModal = () => {
  setEditingQuestion(null);

  setIsQuestionModalOpen(true);
};

const openEditQuestionModal = (question: Question) => {
  setEditingQuestion(question);

  setIsQuestionModalOpen(true);
};

const closeQuestionModal = () => {
  setIsQuestionModalOpen(false);

  setEditingQuestion(null);
};

const saveQuestion = (question: Question) => {
  setQuestions((prev) => {
    const exists = prev.some(
      (item) => item.id === question.id
    );

    if (exists) {
      return prev.map((item) =>
        item.id === question.id ? question : item
      );
    }

    return [...prev, question];
  });

  closeQuestionModal();
};

  return (
    <div className="w-full p-6 space-y-6">
      <div className="flex w-full items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900">Question Bank</h1>
        
        <AddQuestionButton onClick={() => {openAddQuestionModal()}} />
      </div>

      <div className="rounded-md border border-gray-200 bg-white p-4 space-y-4">
        <SearchBar
          value={search}
          onChange={setSearch}
          placeholder="Search questions by text or topic..."
        />

        <div className="border-t border-gray-200 pt-4">
          <TopicFilters
            topics={topics}
            selectedTags={selectedTags}
            onToggleTag={toggleTag}
            onAddTopic={() => {openTopicModal()}}
          />
        </div>
      </div>

      <div className="space-y-4">
  {filteredQuestions.length > 0 ? (
    filteredQuestions.map((question) => (
      <QuestionCard
        key={question.id}
        question={question}
        onEdit={() => {openEditQuestionModal(question);}}
        onDelete={() => {setQuestionToDelete(question.id);}}
      />
    ))
  ) : (
    <div className="flex flex-col items-center justify-center rounded-lg border border-dashed border-gray-300 bg-white py-16 text-center">
      <p className="text-lg font-medium text-gray-700">
        No questions found
      </p>

      <p className="mt-2 text-sm text-gray-500">
        Try adjusting your search or filters.
      </p>

      <ClearFiltersButton onClick={clearFilters} />
    </div>
  )}
</div>


  {questionToDelete && (
  <ConfirmDeleteModal
    onConfirm={confirmDelete}
    onCancel={cancelDelete}
  />
)}

<TopicModal
  isOpen={isTopicModalOpen}
  onClose={closeTopicModal}
  onSave={addTopic}
/>

<QuestionModal
  isOpen={isQuestionModalOpen}
  topics={topics}
  question={editingQuestion}
  onClose={closeQuestionModal}
  onSave={saveQuestion}
/>
    </div>
  );
}
