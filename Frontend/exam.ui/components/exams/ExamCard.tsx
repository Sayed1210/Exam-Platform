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

export default function ExamCard({ exam,onAssign,onView,onEdit,onDelete}: ExamCardProps) {
  return (
    <div className="card p-6">
      
      {/* Name & Topics - Using your custom text classes */}
      <div className="mb-4">
        <h3 className="text-subtitle mb-2">{exam.title}</h3>
        <p className="text-muted mb-6 line-clamp-2">{exam.questions && exam.questions.length > 0
      ? Array.from(new Set(exam.questions.map(q => q.topicTitle)))
          .filter(Boolean) // This removes any null/undefined values
          .join(', ')
      : 'No topics assigned'}</p>
      </div>

      {/* Duration & Questions - Icons use your --muted color */}
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
      <div className="flex items-center gap-2 mt-2">
        {/* Main "Assign" button uses your --primary red*/}
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

        {/* Small icon buttons use your --foreground (black) for the icons*/}
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
  );
}
