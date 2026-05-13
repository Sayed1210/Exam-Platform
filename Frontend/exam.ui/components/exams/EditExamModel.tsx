'use client';
import { useState, useMemo } from 'react';
import Modal from '../Modal';
import Button from '../Button';
import { SearchBar } from './SearchBar'; // Using your reusable search bar
import { ChevronLeftIcon } from '@heroicons/react/24/outline';

interface EditExamModalProps {
  exam: any; // Data from your .NET API
  onClose: () => void;
  onSave: (data: any) => void;
}

export default function EditExamModal({ exam, onClose, onSave }: EditExamModalProps) {
  const [step, setStep] = useState(1);
  const [activeTab, setActiveTab] = useState<'bank' | 'new'>('bank');
  
  // 1. Pre-fill basic info
  const [formData, setFormData] = useState({
    title: exam.name || '',
    duration: exam.durationMinutes || 60
  });

  // 2. Pre-fill selected questions
  const [selectedQuestionIds, setSelectedQuestionIds] = useState<number[]>(
    exam.questions?.map((q: any) => q.id) || []
  );

  // 3. Filter States
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedTopic, setSelectedTopic] = useState('All Topics');
  const [filterStatus, setFilterStatus] = useState<'all' | 'chosen' | 'unchosen'>('all');

  // Replace this with your actual global Question Bank data
  const questionBank = [
    { id: 1, text: "Which IQ shape pattern comes next?", topic: "General", options: 4 },
    { id: 2, text: "What is the capital of France?", topic: "General", options: 2 },
  ];

  // Logic: Combined Filtering for Edit Flow
  const filteredBank = useMemo(() => {
    return questionBank.filter(q => {
      const matchesSearch = q.text.toLowerCase().includes(searchQuery.toLowerCase());
      const matchesTopic = selectedTopic === 'All Topics' || q.topic === selectedTopic;
      const isChosen = selectedQuestionIds.includes(q.id);
      
      if (filterStatus === 'chosen') return isChosen && matchesSearch && matchesTopic;
      if (filterStatus === 'unchosen') return !isChosen && matchesSearch && matchesTopic;
      return matchesSearch && matchesTopic;
    });
  }, [searchQuery, selectedTopic, filterStatus, selectedQuestionIds]);

  return (
    <Modal onClose={onClose} title="Edit Exam">
      {/* Progress Header */}
      <div className="flex justify-center items-center gap-4 py-8">
        <div className="flex items-center gap-2">
          <span className={`w-8 h-8 rounded-full flex items-center justify-center text-sm font-bold ${step === 1 ? 'bg-red-600 text-white' : 'bg-red-50 text-red-600'}`}>1</span>
          <span className={`text-sm font-bold ${step === 1 ? 'text-slate-900' : 'text-slate-400'}`}>Basic Info</span>
        </div>
        <div className="w-16 h-[2px] bg-slate-100"></div>
        <div className="flex items-center gap-2">
          <span className={`w-8 h-8 rounded-full flex items-center justify-center text-sm font-bold ${step === 2 ? 'bg-red-600 text-white' : 'bg-slate-100 text-slate-400'}`}>2</span>
          <span className={`text-sm font-bold ${step === 2 ? 'text-slate-900' : 'text-slate-400'}`}>Questions</span>
        </div>
      </div>

      <div className="px-10 pb-10 min-h-[480px]">
        {step === 1 ? (
          /* STEP 1: EDITABLE BASIC INFO */
          <div className="space-y-8 mt-4">
            <div className="space-y-2">
              <label className="text-sm font-bold text-slate-700 ml-1">Exam Title <span className="text-red-500">*</span></label>
              <input 
                value={formData.title}
                onChange={(e) => setFormData({...formData, title: e.target.value})}
                className="w-full p-4 bg-white border border-slate-100 rounded-3xl shadow-sm outline-none focus:ring-2 focus:ring-red-500/10 transition-all"
                placeholder="e.g. React Developer Test"
              />
            </div>
            <div className="space-y-2">
              <label className="text-sm font-bold text-slate-700 ml-1">Duration (minutes) <span className="text-red-500">*</span></label>
              <input 
                type="number"
                value={formData.duration}
                onChange={(e) => setFormData({...formData, duration: parseInt(e.target.value) || 0})}
                className="w-full p-4 bg-white border border-slate-100 rounded-3xl shadow-sm outline-none focus:ring-2 focus:ring-red-500/10 transition-all"
              />
            </div>
          </div>
        ) : (
          /* STEP 2: REUSABLE TABS & FILTERS */
          <div className="space-y-6">
            <div className="flex bg-slate-50/50 p-1.5 rounded-2xl">
              <button onClick={() => setActiveTab('bank')} className={`flex-1 py-3 text-sm font-bold rounded-xl transition-all ${activeTab === 'bank' ? 'bg-white shadow-sm text-slate-900' : 'text-slate-400'}`}>📋 From Bank</button>
              <button onClick={() => setActiveTab('new')} className={`flex-1 py-3 text-sm font-bold rounded-xl transition-all ${activeTab === 'new' ? 'bg-white shadow-sm text-slate-900' : 'text-slate-400'}`}>🖊️ Create New</button>
            </div>

            {activeTab === 'bank' ? (
              <div className="space-y-4">
                {/* Modern Filter Row */}
                <div className="flex gap-3">
                  <div className="flex-[2]">
                    
                  </div>
                  <select className="flex-1 bg-white border border-slate-100 rounded-2xl px-4 text-sm font-medium outline-none" value={selectedTopic} onChange={(e) => setSelectedTopic(e.target.value)}>
                    <option>All Topics</option>
                    <option>General</option>
                  </select>
                  <select className="flex-1 bg-white border border-slate-100 rounded-2xl px-4 text-sm font-bold text-red-600 outline-none" value={filterStatus} onChange={(e) => setFilterStatus(e.target.value as any)}>
                    <option value="all">All Status</option>
                    <option value="chosen">Chosen</option>
                    <option value="unchosen">Not Chosen</option>
                  </select>
                </div>

                {/* List with Pre-checked Logic */}
                <div className="space-y-3 max-h-[320px] overflow-y-auto pr-2 custom-scrollbar">
                  {filteredBank.map(q => (
                    <div key={q.id} className={`flex items-start gap-4 p-5 border rounded-3xl transition-all ${selectedQuestionIds.includes(q.id) ? 'border-red-100 bg-red-50/30' : 'border-slate-50 bg-white hover:border-slate-100'}`}>
                      <input 
                        type="checkbox" 
                        checked={selectedQuestionIds.includes(q.id)} 
                        onChange={() => {
                           setSelectedQuestionIds(prev => prev.includes(q.id) ? prev.filter(id => id !== q.id) : [...prev, q.id])
                        }}
                        className="mt-1 w-6 h-6 rounded-lg accent-red-600 cursor-pointer" 
                      />
                      <div>
                        <p className="text-sm font-bold text-slate-800 leading-relaxed">{q.text}</p>
                        <div className="flex gap-2 mt-2">
                          <span className="text-[10px] font-black uppercase bg-slate-100 text-slate-500 px-2 py-0.5 rounded-md">{q.topic}</span>
                          <span className="text-[10px] font-bold text-slate-400">{q.options} options</span>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            ) : (
              /* Create New Sub-form */
              <div className="space-y-4 animate-in fade-in slide-in-from-bottom-2">
                <div className="p-6 border border-slate-100 rounded-3xl bg-white shadow-sm space-y-4">
                  <select className="text-xs font-bold bg-slate-50 border-none rounded-lg px-3 py-2 outline-none">
                    <option>Select Topic</option>
                  </select>
                  <textarea className="w-full p-4 bg-slate-50/50 rounded-2xl border-none text-sm outline-none" rows={3} placeholder="Enter question text..." />
                  {/* Choices logic here */}
                </div>
              </div>
            )}
          </div>
        )}
      </div>

      {/* Footer */}
      <div className="p-8 border-t border-slate-50 flex justify-between bg-white rounded-b-[40px]">
        <Button text="Cancel" onClick={onClose} className="!bg-slate-100 !text-slate-600 !w-auto px-10 rounded-2xl font-bold" />
        <div className="flex gap-3">
          {step === 2 && (
            <button onClick={() => setStep(1)} className="flex items-center gap-2 px-8 py-3 rounded-2xl border border-slate-100 text-slate-500 font-bold hover:bg-slate-50 transition-all">
              <ChevronLeftIcon className="w-4 h-4" /> Back
            </button>
          )}
          <Button 
            text={step === 1 ? "Next Step" : "Save Edits"} 
            onClick={() => step === 1 ? setStep(2) : onSave({...formData, selectedQuestionIds})} 
            className="!bg-red-600 !text-white !w-auto px-12 rounded-2xl font-bold shadow-lg shadow-red-200" 
          />
        </div>
      </div>
    </Modal>
  );
}