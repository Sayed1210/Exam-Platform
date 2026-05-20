'use client';

import { Exam } from '@/types/exam';
import ExamCard from '@/components/exams/ExamCard';

interface ExamsGridProps {
  exams: Exam[];
  onAssign: (exam: Exam) => void;
  onView: (exam: Exam) => void;
  onEdit: (exam: Exam) => void;
  onDelete: (exam: Exam) => void;
}

export default function ExamsGrid({ exams, onAssign, onView, onEdit, onDelete }: ExamsGridProps) {
  return (
    <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
      {exams.map((exam) => (
        <ExamCard
          key={exam.id}
          exam={exam}
          onAssign={(e) => onAssign(e)}
          onView={() => onView(exam)}
          onEdit={() => onEdit(exam)}
          onDelete={() => onDelete(exam)}
        />
      ))}
    </div>
  );
}
