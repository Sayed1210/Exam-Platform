'use client';
import { useEffect, useState } from 'react';
import { 
  PlusIcon, CheckIcon, 
  ArrowLeftIcon, PhotoIcon, XMarkIcon
} from '@heroicons/react/24/outline';
import TrashIcon from "../TrashIcon";
import SearchIcon from "../SearchIcon";
import Button from '../Button';
import { SearchBar } from './SearchBar';
import ExamModal from './ExamModal';
import { createExamSchema, createExamStepOneSchema, CreateExamFormData } from '@/schemas/requests/create-exam-request';
import { FormValidation } from '@/schemas/form-validation';
import { getQuestions, getTopics } from '@/services/questionService';
import type { APIQuestion, APITopic } from '@/types/question';
import type { QuestionForm, Option } from '@/components/exams/types';
import QuestionBankItem from '@/components/exams/QuestionBankItem';
import QuestionEditor from '@/components/exams/QuestionEditor';

type CreateExamFormValues = {
  title: string;
  durationMins: number;
  questions: QuestionForm[];
};

interface CreateExamModalProps {
  onClose: () => void;
  onSave: (data: CreateExamFormValues) => void;
  initialData?: any | null;
}

const buildQuestionForm = (question: any): QuestionForm => ({
  id: question.id,
  topicId: question.topicId,
  topic: question.topic || question.topicTitle || '',
  text: question.text || '',
  imageUrl: question.imageUrl || undefined,
  options: (question.options || question.choices || []).map((choice: any) => ({
    text: choice.text || '',
    imageUrl: choice.imageUrl || undefined,
    isCorrect: choice.isCorrect || false,
  })),
  tempId: question.id ? undefined : `${Date.now()}-${Math.random()}`,
});

const buildQuestions = (questions: any[] = []): QuestionForm[] =>
  questions.map(buildQuestionForm);

export default function CreateExamModal({ onClose, onSave, initialData }: CreateExamModalProps) {
  const [step, setStep] = useState(1);
  const [activeTab, setActiveTab] = useState<'bank' | 'manual'>('bank');
  const [filterStatus, setFilterStatus] = useState<'all' | 'chosen' | 'unchosen'>('all');
  const [selectedTopicFilter, setSelectedTopicFilter] = useState<string>('All Topics');
  const [bankSearch, setBankSearch] = useState("");
  const [bankQuestions, setBankQuestions] = useState<QuestionForm[]>([]);
  const [topics, setTopics] = useState<APITopic[]>([]);
  const [isLoadingBank, setIsLoadingBank] = useState(true);
  const [bankError, setBankError] = useState<string | null>(null);

  const topicOptions = topics.length > 0 ? topics.map((topic) => topic.title) : ["React", "Node.js", "SQL", "Algorithms", "General"];

  const [formData, setFormData] = useState<CreateExamFormValues>({
    title: initialData?.title || '',
    durationMins: initialData?.durationMins || 60,
    questions: buildQuestions(initialData?.questions || initialData?.Questions || []),
  });
  const [errors, setErrors] = useState<Record<string, string>>({});

  const getError = (path: string) => errors[path] || "";

  useEffect(() => {
    if (!initialData) return;

    setFormData({
      title: initialData.title || '',
      durationMins: initialData.durationMins || 60,
      questions: buildQuestions(initialData.questions || initialData.Questions || []),
    });
  }, [initialData]);

  useEffect(() => {
    const existingQuestions = initialData?.questions || initialData?.Questions;
    if (!existingQuestions || bankQuestions.length === 0) return;

    setFormData((prev) => ({
      ...prev,
      questions: prev.questions.map((question) => {
        if (question.topic) return question;
        const matched = bankQuestions.find((bankQuestion) => bankQuestion.id === question.id);
        return matched
          ? { ...question, topic: matched.topic, topicId: matched.topicId }
          : question;
      }),
    }));
  }, [initialData?.questions, initialData?.Questions, bankQuestions]);

  useEffect(() => {
    const loadBankData = async () => {
      try {
        setIsLoadingBank(true);
        const [questionsResponse, topicsResponse] = await Promise.all([
          getQuestions(1, 50),
          getTopics(),
        ]);

        const availableQuestions = (questionsResponse.items || questionsResponse.questions || []) as APIQuestion[];
        setBankQuestions(
          availableQuestions.map((question) => ({
            id: question.id,
            topicId: question.topicId,
            topic: question.topicTitle,
            text: question.text,
            imageUrl: question.imageUrl || undefined,
            options: question.choices.map((choice) => ({
              text: choice.text,
              imageUrl: choice.imageUrl || undefined,
              isCorrect: choice.isCorrect,
            })),
          }))
        );

        setTopics(topicsResponse || []);
        setBankError(null);
      } catch (error) {
        setBankError('Unable to load question bank. Please try again.');
      } finally {
        setIsLoadingBank(false);
      }
    };

    loadBankData();
  }, []);

  const getQuestionKey = (question: QuestionForm) =>
    question.id ? String(question.id) : question.tempId ?? `${question.text}-${Math.random()}`;

  const toggleBankQuestion = (q: QuestionForm) => {
    const isSelected = formData.questions.some(
      (item) =>
        item.id !== undefined && q.id !== undefined
          ? item.id === q.id
          : item.tempId === q.tempId
    );

    if (isSelected) {
      setFormData({
        ...formData,
        questions: formData.questions.filter(
          (item) =>
            item.id !== undefined && q.id !== undefined
              ? item.id !== q.id
              : item.tempId !== q.tempId
        ),
      });
    } else {
      setFormData({ ...formData, questions: [...formData.questions, q] });
    }
  };

  const handleNextStep = () => {
    const result = FormValidation(createExamStepOneSchema, {
      title: formData.title,
      durationMins: formData.durationMins,
    });

    if (!result.success) {
      setErrors(result.errors);
      return;
    }

    setErrors({});
    setStep(2);
  };

  const handleSaveExam = () => {
    const result = FormValidation(createExamSchema, formData);

    if (!result.success) {
      setErrors(result.errors);
      return;
    }

    setErrors({});
    onSave(formData);
  };

  const handleImageUpload = (e: React.ChangeEvent<HTMLInputElement>, target: { qIdx: number, oIdx?: number }) => {
    const file = e.target.files?.[0];
    if (!file) return;
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onloadend = () => {
      const base64String = reader.result as string;
      const updated = [...formData.questions];
      if (target.oIdx !== undefined) {
        updated[target.qIdx].options[target.oIdx] = {
          ...updated[target.qIdx].options[target.oIdx],
          imageUrl: base64String,
          text: '',
        };
      } else {
        updated[target.qIdx] = {
          ...updated[target.qIdx],
          imageUrl: base64String,
        };
      }
      setFormData({ ...formData, questions: updated });
    };
  };

  const removeImage = (qIdx: number, oIdx?: number) => {
    const updated = [...formData.questions];
    if (oIdx !== undefined) {
      updated[qIdx].options[oIdx] = {
        ...updated[qIdx].options[oIdx],
        imageUrl: undefined,
      };
    } else {
      updated[qIdx] = {
        ...updated[qIdx],
        imageUrl: undefined,
      };
    }
    setFormData({ ...formData, questions: updated });
  };

  {/* 🚀 Changed to insert at index 0 (Top of array) */}
  const addManualQuestion = () => {
    const newQuestion: QuestionForm = {
      tempId: `${Date.now()}-${Math.random()}`,
      topic: '',
      text: '',
      options: [
        { text: '', isCorrect: true },
        { text: '', isCorrect: false },
      ],
    };
    setFormData({ ...formData, questions: [newQuestion, ...formData.questions] });
  };

  const addOption = (qIdx: number) => {
    const updated = [...formData.questions];
    updated[qIdx].options.push({ text: '', isCorrect: false });
    setFormData({ ...formData, questions: updated });
  };

  const removeOption = (qIdx: number, oIdx: number) => {
    const updated = [...formData.questions];
    if (updated[qIdx].options.length > 1) {
      const removedIsCorrect = updated[qIdx].options[oIdx].isCorrect;
      updated[qIdx].options = updated[qIdx].options.filter((_, i) => i !== oIdx);
      if (removedIsCorrect && updated[qIdx].options.length > 0) {
        updated[qIdx].options[0].isCorrect = true;
      }
      setFormData({ ...formData, questions: updated });
    }
  };

  return (
    <ExamModal onClose={onClose} title={initialData ? `Edit Exam: ${initialData.title}` : "Create Exam"}>
      {/* Stepper */}
      <div className="px-12 py-6 flex items-center justify-center gap-4 sticky top-0 bg-white z-10">
        <div className="flex items-center gap-2">
          <div className={`w-8 h-8 rounded-full flex items-center justify-center font-bold ${step >= 1 ? 'bg-primary text-white' : 'bg-slate-100 text-slate-400'}`}>
             {step > 1 ? <CheckIcon className="w-5 h-5" /> : '1'}
          </div>
          <span className={`text-sm font-bold ${step >= 1 ? 'text-primary' : 'text-slate-400'}`}>Basic Info</span>
        </div>
        <div className={`h-[2px] w-20 ${step > 1 ? 'bg-primary' : 'bg-slate-100'}`} />
        <div className="flex items-center gap-2">
          <div className={`w-8 h-8 rounded-full flex items-center justify-center font-bold ${step === 2 ? 'bg-primary text-white' : 'bg-slate-100 text-slate-400'}`}>2</div>
          <span className={`text-sm font-bold ${step === 2 ? 'text-primary' : 'text-slate-400'}`}>Questions</span>
        </div>
      </div>

      {/* 🚀 Adjusted content wrapper layout classes */}
      <div className={`p-8 overflow-hidden custom-scrollbar ${step === 1 ? 'pb-8' : ''}`}>
        {step === 1 ? (
          <div className="space-y-6">
            <div>
              <label className="block mb-2 font-medium text-label">Exam Title <span className="text-primary">*</span></label>
              <input type="text" className="w-full p-4 border border-slate-200 rounded-2xl outline-none focus:border-primary shadow-sm" value={formData.title} onChange={(e) => setFormData({...formData, title: e.target.value})} placeholder="e.g. React Developer Test"/>
              {getError('title') && <p className="text-red-500 text-xs mt-1">{getError('title')}</p>}
            </div>
            <div>
              <label className="block mb-2 font-medium text-label">Duration (minutes) <span className="text-primary">*</span></label>
              <input type="number" className="w-full p-4 border border-slate-200 rounded-2xl outline-none focus:border-primary shadow-sm" value={formData.durationMins} onChange={(e) => setFormData({...formData, durationMins: parseInt(e.target.value) || 0})}/>
              {getError('durationMins') && <p className="text-red-500 text-xs mt-1">{getError('durationMins')}</p>}
            </div>
          </div>
        ) : (
          <div className="flex flex-col h-full min-h-0">
            <div className="flex p-1 bg-slate-100 rounded-2xl mb-6">
              <button onClick={() => setActiveTab('bank')} className={`flex-1 py-3 rounded-xl text-sm font-bold transition-all ${activeTab === 'bank' ? 'bg-white shadow-sm text-primary' : 'text-slate-500'}`}>📋 From Bank</button>
              <button onClick={() => setActiveTab('manual')} className={`flex-1 py-3 rounded-xl text-sm font-bold transition-all ${activeTab === 'manual' ? 'bg-white shadow-sm text-primary' : 'text-slate-500'}`}>✏️ Create New</button>
            </div>

            {activeTab === 'bank' ? (
              <div className="space-y-4">
                <div className="flex gap-3 mb-6">
                  <div className="relative flex-[2]">
                    <SearchIcon className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
                    <SearchBar 
                        placeholder="Search question bank..." 
                        value={bankSearch} 
                        onChange={setBankSearch} 
                      />
                  </div>
                  <select className="flex-1 px-4 py-3 bg-white border border-slate-200 rounded-2xl outline-none focus:border-primary text-sm text-slate-600 appearance-none cursor-pointer shadow-sm" value={selectedTopicFilter} onChange={(e) => setSelectedTopicFilter(e.target.value)}>
                    <option>All Topics</option>
                    {(topics.length > 0 ? topics.map((topic) => topic.title) : ["React", "Node.js", "SQL", "Algorithms", "General"]).map((t) => (
                      <option key={t} value={t}>{t}</option>
                    ))}
                  </select>
                  {initialData && (
    <select 
      className="flex-1 px-4 py-3 bg-white border border-slate-200 rounded-2xl outline-none text-sm font-bold text-primary shadow-sm animate-in fade-in duration-300"
      value={filterStatus}
      onChange={(e) => setFilterStatus(e.target.value as any)}
    >
      <option value="all">All Status</option>
      <option value="chosen">Chosen</option>
      <option value="unchosen">Not Chosen</option>
    </select>
  )}
                </div>
                <div className="space-y-3">
                  {getError('questions') && <p className="text-red-500 text-xs">{getError('questions')}</p>}
                  {isLoadingBank ? (
                    <div className="text-slate-500 text-sm py-10 text-center">Loading question bank…</div>
                  ) : bankError ? (
                    <div className="text-red-500 text-sm py-10 text-center">{bankError}</div>
                  ) : (
                    bankQuestions
                      .filter((q) => {
                        const matchesSearch = q.text.toLowerCase().includes(bankSearch.toLowerCase());
                        const matchesTopic = selectedTopicFilter === 'All Topics' || q.topic === selectedTopicFilter;
                        const isChosen = formData.questions.some((fq) =>
                          fq.id !== undefined && q.id !== undefined
                            ? fq.id === q.id
                            : fq.tempId === q.tempId
                        );
                        const matchesStatus = !initialData || filterStatus === 'all'
                          ? true
                          : filterStatus === 'chosen'
                          ? isChosen
                          : !isChosen;
                        return matchesSearch && matchesTopic && matchesStatus;
                      })
                      .map((bq) => {
                        const selected = formData.questions.some((q) =>
                          q.id !== undefined && bq.id !== undefined ? q.id === bq.id : q.tempId === bq.tempId
                        );
                        return (
                          <QuestionBankItem
                            key={getQuestionKey(bq)}
                            question={bq}
                            selected={selected}
                            onToggle={() => toggleBankQuestion(bq)}
                          />
                        );
                      })
                  )}
                </div>
              </div>
            ) : (
              <div className="space-y-6">
                 {getError('questions') && <p className="text-red-500 text-xs">{getError('questions')}</p>}
                 
                 {/* 🚀 New Question Trigger Button placed above the mapping stack layout */}
                 <button onClick={addManualQuestion} className="w-full py-5 border-2 border-dashed border-slate-200 rounded-3xl text-primary font-bold hover:bg-red-50 hover:border-primary/30 transition-all flex items-center justify-center gap-2"> <PlusIcon className="w-5 h-5" /> Add New Question </button>
                 
                 {formData.questions.map((q, qIdx) => (
                   <QuestionEditor
                     key={getQuestionKey(q)}
                     q={q}
                     qIdx={qIdx}
                     topicOptions={topicOptions}
                     topics={topics}
                     getError={getError}
                     onTopicChange={(value) => {
                       const updated = [...formData.questions];
                       const selectedTopic = topics.find((topic) => topic.title === value);
                       updated[qIdx].topic = value;
                       updated[qIdx].topicId = selectedTopic?.id;
                       setFormData({ ...formData, questions: updated });
                     }}
                     onRemoveQuestion={() => setFormData({ ...formData, questions: formData.questions.filter((_, i) => i !== qIdx) })}
                     onQuestionTextChange={(value) => {
                       const updated = [...formData.questions];
                       updated[qIdx].text = value;
                       setFormData({ ...formData, questions: updated });
                     }}
                     onQuestionImageUpload={(event) => handleImageUpload(event, { qIdx })}
                     onRemoveQuestionImage={() => removeImage(qIdx)}
                     onOptionAdd={() => addOption(qIdx)}
                     onOptionTextChange={(oIdx, value) => {
                       const updated = [...formData.questions];
                       updated[qIdx].options[oIdx] = {
                         ...updated[qIdx].options[oIdx],
                         text: value,
                         imageUrl: value.trim() ? undefined : updated[qIdx].options[oIdx].imageUrl,
                       };
                       setFormData({ ...formData, questions: updated });
                     }}
                     onOptionCorrectSelect={(oIdx) => {
                       const updated = [...formData.questions];
                       updated[qIdx].options = updated[qIdx].options.map((o, i) => ({ ...o, isCorrect: i === oIdx }));
                       setFormData({ ...formData, questions: updated });
                     }}
                     onOptionImageUpload={(oIdx, event) => handleImageUpload(event, { qIdx, oIdx })}
                     onOptionRemoveImage={(oIdx) => removeImage(qIdx, oIdx)}
                     onOptionRemove={(oIdx) => removeOption(qIdx, oIdx)}
                   />
                 ))}
              </div>
            )}
          </div>
        )}
      </div>

      {/* Footer - Sticky at the bottom of the modal */}
      <div className="p-6 border-t border-slate-100 flex justify-between bg-white sticky bottom-0 z-20 rounded-b-3xl">
        <Button text="Cancel" onClick={onClose} className="btn-secondary !mt-0 px-6" />
        <div className="flex gap-3">
          {step === 2 && (
            <button 
              onClick={() => setStep(1)} 
              className="flex items-center gap-2 px-6 py-2.5 rounded-2xl bg-white border border-slate-200 text-slate-600 font-bold text-sm hover:bg-slate-50 transition-all shadow-sm"
            > 
              <ArrowLeftIcon className="w-4 h-4 stroke-[2.5px]"/> Back 
            </button>
          )}
          <Button 
            text={step === 1 ? "Next Step" : "Save Exam"} 
            className="btn-primary !mt-0 px-8" 
            onClick={() => step === 1 ? handleNextStep() : handleSaveExam()} 
            disabled={step === 1 && !formData.title}
          />
        </div>
      </div>
    </ExamModal>
  );
}