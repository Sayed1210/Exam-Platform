'use client';

interface TopicBadgeProps {
  title?: string | null;
}

export default function TopicBadge({ title }: TopicBadgeProps) {
  if (!title) return <div />; // keep layout alignment when no topic

  return (
    <span className="rounded-full bg-primary/10 px-3 py-1 text-xs font-medium text-primary">
      {title}
    </span>
  );
}
