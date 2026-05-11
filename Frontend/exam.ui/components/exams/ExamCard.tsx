'use client';
import Button from '../Button';
import { 
  ClockIcon, 
  ClipboardDocumentListIcon, 
  PaperAirplaneIcon, 
  EyeIcon, 
  PencilSquareIcon, 
  TrashIcon 
} from '@heroicons/react/24/outline'; 
interface ExamCardProps {
  exam: any;
  onAssign: (exam: any) => void; 
}
export default function ExamCard({ exam,onAssign}: ExamCardProps) {
  return (
    <div className="bg-white p-6 rounded-2xl border border-slate-100 shadow-sm hover:shadow-md transition-shadow">
      
      {/* Name & Topics - Using your custom text classes */}
      <div className="mb-4">
        <h3 className="text-subtitle mb-2">{exam.name}</h3>
        <p className="text-muted mb-6 line-clamp-2">{exam.topics}</p>
      </div>

      {/* Duration & Questions - Icons use your --muted color */}
      <div className="flex gap-5 py-4 border-t border-slate-50">
        <div className="flex items-center gap-1.5 text-muted">
          <ClockIcon className="w-5 h-5 opacity-70" />
          <span className="text-sm">{exam.durationMinutes} mins</span>
        </div>
        <div className="flex items-center gap-1.5 text-muted">
          <ClipboardDocumentListIcon className="w-5 h-5 opacity-70" />
          <span className="text-sm">{exam.totalQuestions} Questions</span>
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
        />
        <Button 
          text={<PencilSquareIcon className="w-5 h-5" />} 
          className="btn-icon-secondary" 
        />
        <Button 
          text={<TrashIcon className="w-5 h-5" />} 
          className="btn-icon-danger" 
        />
      </div>
    </div>
  );
}