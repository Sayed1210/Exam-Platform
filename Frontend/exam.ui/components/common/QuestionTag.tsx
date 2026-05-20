interface QuestionTagProps {
  title: string;
}

export default function QuestionTag({ title }: QuestionTagProps) {
  return (
    <div className="flex flex-wrap items-center gap-2">
      <span className="rounded-full bg-primary/10 px-3 py-1 text-xs font-medium text-primary">
        {title}
      </span>
    </div>
  );
}