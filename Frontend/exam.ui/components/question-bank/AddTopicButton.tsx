import Button from "@/components/Button";
export type AddTopicButtonProps = {
  onClick: () => void;
};

export default function AddTopicButton({ onClick }: AddTopicButtonProps) {
  return (
      <Button
        text={"+ Add Topic"}
        onClick={onClick}
        className={`px-4 py-2 rounded-full border border-primary/30 bg-white text-sm text-primary`}
        >
      </Button>
    );
}
