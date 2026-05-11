import Button from "@/components/Button";


export type TopicTagProps = {
  label: string;
  active?: boolean;
  onClick?: () => void;
};

export default function TopicTag({ label, active = false, onClick }: TopicTagProps) {
  return (
    <Button
      text={label}
      onClick={onClick}
      className={`px-4 py-2 rounded-full text-sm ${
        active ? "bg-primary text-white" : "bg-primary/10 text-primary"
      }`}
      >
    </Button>
  );
}
