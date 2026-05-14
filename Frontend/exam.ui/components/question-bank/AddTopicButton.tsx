import Button from "@/components/Button";
export type AddTopicButtonProps = {
  onClick: () => void;
};

export default function AddTopicButton({ onClick }: AddTopicButtonProps) {
  return (
    <Button
      text={"+ Add Topic"}
      onClick={onClick}
      className="cursor-pointer select-none whitespace-nowrap rounded-full border-[1.5px] border-dashed border-primary-300 bg-primary/10 px-3.5 py-[5px] text-[12.5px] font-medium text-primary transition-all duration-150 hover:border-solid hover:border-primary hover:bg-primary hover:text-white"
    />
  );
}
