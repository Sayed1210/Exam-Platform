import Button from "@/components/Button";
export type AddQuestionButtonProps = {
  onClick: () => void;
};

export default function AddQuestionButton({ onClick }: AddQuestionButtonProps) {
  return (
      <Button
          text={"+ Add Question"}
          onClick={onClick}
          className="bg-primary text-white font-semibold px-6 py-2.5 rounded-full hover:brightness-90 transition"
              >
        </Button>
  );
}
