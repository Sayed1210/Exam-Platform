'use client';
import Button from '../Button';
import TrashIcon from "../TrashIcon";
import EditIcon from "../EditIcon";
import { 
  ClockIcon, 
  ClipboardDocumentListIcon, 
  PaperAirplaneIcon, 
  EyeIcon,   
} from '@heroicons/react/24/outline'; 
import { Exam } from '@/types/exam';

interface ExamCardProps {
  exam: Exam;
  onAssign: (exam: Exam) => void; 
  onView: (exam: Exam) => void;
  onEdit: (exam: Exam) => void;
  onDelete: (exam: Exam) => void;
}

export default function ExamCard({ exam, onAssign, onView, onEdit, onDelete }: ExamCardProps) {
  return (
    /* 🚀 FIX 1: Add 'flex flex-col h-full' to force the card to fill the grid height vertically */
    <div className="card p-6 flex flex-col h-full">
      
      {/* Name & Topics */}
      <div>
        <h3 className="text-subtitle mb-2">{exam.title}</h3>
        {/* Removed mb-6 so multiple topics can expand naturally downward without adding extra dead space */}
        <p className="text-muted line-clamp-3">
          {(() => {
            const topicTitles = exam.questions?.map((q) => q.topicTitle).filter(Boolean) ?? [];
            const uniqueTopics = Array.from(new Set(topicTitles));
            if (uniqueTopics.length > 0) {
              return uniqueTopics.join(', ');
            }
            return exam.topics?.trim() || 'No topics assigned';
          })()}
        </p>
      </div>

      {/* 🚀 FIX 2: Add 'mt-auto' to this wrapper container. 
          This cleanly eats up any leftover empty vertical space and locks everything below it to the card floor. */}
      <div className="mt-auto pt-4">
        
        {/* Duration & Questions */}
        <div className="flex gap-5 py-4 border-t border-slate-50">
          <div className="flex items-center gap-1.5 text-muted">
            <ClockIcon className="w-5 h-5 opacity-70" />
            <span className="text-sm">{exam.durationMins} mins</span>
          </div>
          <div className="flex items-center gap-1.5 text-muted">
            <ClipboardDocumentListIcon className="w-5 h-5 opacity-70" />
            <span className="text-sm">{exam.totalQuestions} Question(s)</span>
          </div>
        </div>

        {/* Action Buttons Row */}
        <div className="flex items-center gap-2">
          {/* Main "Assign" button */}
          <Button 
            text={
              <span className="flex items-center gap-2">
                <PaperAirplaneIcon className="w-5 h-5 -rotate-45" /> 
                Assign
              </span>
            } 
            className="btn-primary !mt-0 !w-auto px-6 py-2"
            onClick={() => onAssign(exam)}
          />

          {/* Small icon buttons */}
          <Button 
            text={<EyeIcon className="w-5 h-5" />} 
            className="btn-icon-secondary" 
            onClick={() => onView(exam)}
          />
          <Button 
            text={<EditIcon className="text-gray-400 transition group-hover:text-blue-500" />} 
            className="btn-icon-secondary" 
            onClick={() => onEdit(exam)}
          />
          <Button 
            text={<TrashIcon className="text-gray-400 transition group-hover:text-red-500" />} 
            className="btn-icon-danger" 
            onClick={() => onDelete(exam)}
          />
        </div>
        
      </div>
    </div>
  );
}