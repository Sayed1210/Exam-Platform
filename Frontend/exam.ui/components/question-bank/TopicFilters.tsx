import AddTopicButton from "./AddTopicButton";
import TopicTag from "./TopicTag";
import type { APITopic } from "@/types/question";

export type TopicFiltersProps = {
  topics: APITopic[];
  selectedTags: string[];
  onToggleTag: (topic: APITopic) => void;
  onEditTopic: (topic: APITopic) => void;
  onDeleteTopic: (topic: APITopic) => void;
  onAddTopic: () => void;
};

export default function TopicFilters({
  topics,
  selectedTags,
  onToggleTag,
  onEditTopic,
  onDeleteTopic,
  onAddTopic,
}: TopicFiltersProps) {
  return (
    <div className="flex flex-wrap items-center gap-2 border-t border-gray-100 px-5 pt-3 pb-3.5">
      {topics.map((topic) => (
        <TopicTag
          key={topic.id}
          label={topic.title}
          active={selectedTags.includes(topic.title)}
          onClick={() => onToggleTag(topic)}
          onEdit={() => onEditTopic(topic)}
          onDelete={() => onDeleteTopic(topic)}
        />
      ))}
      <AddTopicButton onClick={onAddTopic} />
    </div>
  );
}
