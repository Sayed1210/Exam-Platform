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
      className={`cursor-pointer select-none whitespace-nowrap rounded-full border-[1.5px] px-3.5 py-[5px] text-[12.5px] font-medium transition-all duration-150 ${
        active
          ? "border-primary bg-primary text-white"
          : "border-gray-200 bg-white text-gray-600 hover:border-primary hover:text-primary"
      }`}
    />
  );
}
