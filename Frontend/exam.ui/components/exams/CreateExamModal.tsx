'use client';
import { useState } from 'react';
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

interface Option {
  text: string;
  isCorrect: boolean;
  imageUrl?: string;
}

interface Question {
  text: string;
  topic: string;
  imageUrl?: string;
  options: Option[];
}
interface CreateExamModalProps {
  onClose: () => void;
  onSave: (data: CreateExamFormData) => void;
  initialData?: any | null; // Added this line
}
export default function CreateExamModal({ onClose, onSave, initialData }: { onClose: () => void, onSave: (data: CreateExamFormData) => void,initialData?:any|null }) {
  const [step, setStep] = useState(1);
  const [activeTab, setActiveTab] = useState<'bank' | 'manual'>('bank');
  //const [searchQuery, setSearchQuery] = useState('');
  const [filterStatus, setFilterStatus] = useState<'all' | 'chosen' | 'unchosen'>('all');
  const [selectedTopicFilter, setSelectedTopicFilter] = useState<string>('All Topics');
  const [availableTopics] = useState<string[]>(["React", "Node.js", "SQL", "Algorithms", "General"]);
  const [bankSearch, setBankSearch] = useState("");
  const [bankQuestions] = useState<Question[]>([
    { text: "Which IQ shape pattern comes next?", topic: "General", options: [{ text: "", isCorrect: true, imageUrl: "base64_url" }, { text: "", isCorrect: false, imageUrl: "base64_url" }]},
    { text: "What is the capital of France?", topic: "General", options: [{ text: "Paris", isCorrect: true }, { text: "London", isCorrect: false }]},
    { text: "Which SQL JOIN returns all rows when there is a match in one of the tables?", 
  topic: "SQL", 
  options: [{ text: "INNER JOIN", isCorrect: false }, 
    { text: "FULL OUTER JOIN", isCorrect: true },
    { text: "LEFT JOIN", isCorrect: false },
    { text: "CROSS JOIN", isCorrect: false }]
    }
  ]);

  const [formData, setFormData] = useState<CreateExamFormData>({
    title: initialData?.title || '',
    durationMins: initialData?.durationMins || 60,
    questions: (initialData?.questions as any) || []
  });
  const [errors, setErrors] = useState<Record<string, string>>({});

  const getError = (path: string) => errors[path] || "";

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
    onSave(result.data);
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
        updated[target.qIdx].options[target.oIdx].imageUrl = base64String;
      } else {
        updated[target.qIdx].imageUrl = base64String;
      }
      setFormData({ ...formData, questions: updated });
    };
  };

  const removeImage = (qIdx: number, oIdx?: number) => {
    const updated = [...formData.questions];
    if (oIdx !== undefined) {
      delete updated[qIdx].options[oIdx].imageUrl;
    } else {
      delete updated[qIdx].imageUrl;
    }
    setFormData({ ...formData, questions: updated });
  };

  const toggleBankQuestion = (q: Question) => {
    const isSelected = formData.questions.some(item => item.text === q.text);
    if (isSelected) {
      setFormData({ ...formData, questions: formData.questions.filter(item => item.text !== q.text) });
    } else {
      setFormData({ ...formData, questions: [...formData.questions, q] });
    }
  };

  const addManualQuestion = () => {
    const newQuestion: Question = {
      text: '',
      topic: '', // Now correctly initialized
      options: [
        { text: '', isCorrect: true },
        { text: '', isCorrect: false }
      ]
    };
    setFormData({ ...formData, questions: [...formData.questions, newQuestion] });
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

      {/* Content Area */}
      <div className={`p-8 overflow-y-auto custom-scrollbar ${step === 1 ? 'pb-8' : 'h-[600px] max-h-[70vh] pb-36'}`}>
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
          <div className="flex flex-col h-full">
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
                    {availableTopics.map(t => <option key={t} value={t}>{t}</option>)}
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
                  {bankQuestions
                    .filter(q => {
                      const matchesSearch = q.text.toLowerCase().includes(bankSearch.toLowerCase());
                      const matchesTopic = selectedTopicFilter === 'All Topics' || q.topic === selectedTopicFilter;
                      const isChosen = formData.questions.some(fq => fq.text === q.text);
                      const matchesStatus = !initialData || filterStatus === 'all' 
    ? true 
    : filterStatus === 'chosen' ? isChosen : !isChosen;
                      return matchesSearch && matchesTopic &&matchesStatus;
                    })
                    .map((bq, idx) => {
                      const selected = formData.questions.some(q => q.text === bq.text);
                      return (
                        <div key={idx} onClick={() => toggleBankQuestion(bq)} className={`p-5 border rounded-3xl cursor-pointer transition-all flex items-start gap-4 ${selected ? 'border-primary bg-red-50/10' : 'border-slate-100 bg-white hover:border-slate-200 shadow-sm'}`}>
                          <div className={`mt-1 w-5 h-5 rounded flex items-center justify-center border ${selected ? 'bg-primary border-primary' : 'border-slate-300'}`}>
                            {selected && <CheckIcon className="w-4 h-4 text-white stroke-[3px]" />}
                          </div>
                          <div className="flex-1">
                            <p className="text-sm font-bold text-slate-800 mb-1">{bq.text}</p>
                            <div className="flex gap-2">
                              <span className="text-[10px] px-2 py-0.5 bg-slate-100 rounded-full font-bold text-slate-500 uppercase">{bq.topic}</span>
                              <span className="text-[10px] text-slate-400 font-medium">{bq.options.length} options</span>
                            </div>
                          </div>
                        </div>
                      );
                    })}
                </div>
              </div>
            ) : (
              <div className="space-y-6">
                 {getError('questions') && <p className="text-red-500 text-xs">{getError('questions')}</p>}
                 {formData.questions.map((q, qIdx) => (
                   <div key={qIdx} className="p-6 border border-slate-100 rounded-3xl bg-slate-50/30 relative shadow-sm">
                      <div className="flex justify-between items-center mb-4">
                        <div className="flex-1">
                          <select className="text-xs font-bold p-2 bg-white border border-slate-200 rounded-lg outline-none focus:border-primary text-slate-500 w-full" value={q.topic} onChange={(e) => { const updated = [...formData.questions]; updated[qIdx].topic = e.target.value; setFormData({...formData, questions: updated}); }}>
                            <option value="">Select Topic</option>
                            {availableTopics.map(t => <option key={t} value={t}>{t}</option>)}
                          </select>
                          {getError(`questions.${qIdx}.topic`) && <p className="text-red-500 text-xs mt-1">{getError(`questions.${qIdx}.topic`)}</p>}
                        </div>
                        <div className="flex items-center gap-2">
                            <input type="file" id={`question-image-${qIdx}`} className="hidden" accept="image/*" onChange={(e) => handleImageUpload(e, { qIdx })}/>
                            <label htmlFor={`question-image-${qIdx}`} className="text-slate-400 hover:text-slate-600 cursor-pointer p-1.5 rounded-lg border border-slate-100 bg-white">
                              <PhotoIcon className="w-4 h-4" />
                            </label>
                            <button onClick={() => setFormData({...formData, questions: formData.questions.filter((_, i) => i !== qIdx)})} className="text-slate-300 hover:text-red-500 transition-colors">
                              <TrashIcon className="w-5 h-5" />
                            </button>
                        </div>
                      </div>
                      <div className="mb-4 space-y-2">
                          <textarea placeholder="Enter question text..." className="w-full p-4 bg-white border border-slate-100 rounded-2xl outline-none focus:border-primary text-sm shadow-sm" value={q.text} onChange={(e) => { const updated = [...formData.questions]; updated[qIdx].text = e.target.value; setFormData({...formData, questions: updated}); }}/>
                          {getError(`questions.${qIdx}.text`) && <p className="text-red-500 text-xs mt-1">{getError(`questions.${qIdx}.text`)}</p>}
                          {q.imageUrl && (
                            <div className="relative group w-fit">
                              <img src={q.imageUrl} alt="Question" className="w-auto max-h-40 rounded-xl border border-slate-100 bg-white" />
                              <button onClick={() => removeImage(qIdx)} className="absolute top-2 right-2 bg-slate-800/70 p-1.5 rounded-full text-white opacity-0 group-hover:opacity-100 transition-opacity">
                                  <XMarkIcon className="w-4 h-4" />
                              </button>
                            </div>
                          )}
                      </div>

                      <div className="space-y-3">
                        {/* Typed 'opt' to fix TS error */}
                        {q.options.map((opt: Option, oIdx: number) => (
                          <div key={oIdx} className="flex items-start gap-3 group">
                            <input type="radio" name={`correct-${qIdx}`} checked={opt.isCorrect} onChange={() => { const updated = [...formData.questions]; updated[qIdx].options = updated[qIdx].options.map((o, i) => ({...o, isCorrect: i === oIdx})); setFormData({...formData, questions: updated}); }} className="accent-primary w-4 h-4 cursor-pointer mt-3" />
                            <div className="flex-1 space-y-1.5">
                                <input placeholder={`Option ${oIdx + 1}`} className="w-full p-3 border border-slate-100 rounded-xl text-sm outline-none focus:border-primary bg-white shadow-sm" value={opt.text} onChange={(e) => { const updated = [...formData.questions]; updated[qIdx].options[oIdx].text = e.target.value; setFormData({...formData, questions: updated}); }}/>
                                {getError(`questions.${qIdx}.options.${oIdx}.text`) && <p className="text-red-500 text-xs mt-1">{getError(`questions.${qIdx}.options.${oIdx}.text`)}</p>}
                                {opt.imageUrl && (
                                  <div className="relative group w-fit">
                                    <img src={opt.imageUrl} alt={`Option ${oIdx + 1}`} className="w-auto max-h-32 rounded-xl border border-slate-100 bg-white" />
                                    <button onClick={() => removeImage(qIdx, oIdx)} className="absolute top-1.5 right-1.5 bg-slate-800/70 p-1 rounded-full text-white opacity-0 group-hover:opacity-100 transition-opacity">
                                        <XMarkIcon className="w-3.5 h-3.5" />
                                    </button>
                                  </div>
                                )}
                            </div>
                            <div className="flex items-center gap-1.5 mt-2.5">
                                <input type="file" id={`option-image-${qIdx}-${oIdx}`} className="hidden" accept="image/*" onChange={(e) => handleImageUpload(e, { qIdx, oIdx })}/>
                                <label htmlFor={`option-image-${qIdx}-${oIdx}`} className="text-slate-300 hover:text-slate-600 cursor-pointer p-1.5 border border-slate-100 bg-white rounded-lg">
                                  <PhotoIcon className="w-3.5 h-3.5" />
                                </label>
                                {q.options.length > 1 && (
                                  <button onClick={() => removeOption(qIdx, oIdx)} className="text-slate-300 hover:text-red-400 opacity-0 group-hover:opacity-100 transition-all">
                                    <XMarkIcon className="w-4 h-4" />
                                  </button>
                                )}
                            </div>
                          </div>
                        ))}
                        {getError(`questions.${qIdx}.options`) && <p className="text-red-500 text-xs mt-1">{getError(`questions.${qIdx}.options`)}</p>}
                        <button onClick={() => addOption(qIdx)} className="flex items-center gap-1 text-xs font-bold text-primary hover:opacity-80 transition-opacity"> <PlusIcon className="w-3 h-3 stroke-[3px]" /> Add Choice </button>
                      </div>
                   </div>
                 ))}
                 <button onClick={addManualQuestion} className="w-full py-5 border-2 border-dashed border-slate-200 rounded-3xl text-primary font-bold hover:bg-red-50 hover:border-primary/30 transition-all flex items-center justify-center gap-2"> <PlusIcon className="w-5 h-5" /> Add New Question </button>
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
