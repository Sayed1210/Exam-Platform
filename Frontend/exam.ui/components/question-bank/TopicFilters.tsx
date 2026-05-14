import AddTopicButton from "./AddTopicButton";
import TopicTag from "./TopicTag";

export type TopicFiltersProps = {
  topics: string[];
  selectedTags: string[];
  onToggleTag: (topic: string) => void;
  onAddTopic: () => void;
};

export default function TopicFilters({
  topics,
  selectedTags,
  onToggleTag,
  onAddTopic,
}: TopicFiltersProps) {
  return (
    <div className="flex flex-wrap items-center gap-2 border-t border-gray-100 px-5 pt-3 pb-3.5">
      {topics.map((topic) => (
        <TopicTag
          key={topic}
          label={topic}
          active={selectedTags.includes(topic)}
          onClick={() => onToggleTag(topic)}
        />
      ))}
      <AddTopicButton onClick={onAddTopic} />
    </div>
  );
}
