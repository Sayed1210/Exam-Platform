import Button from "@/components/Button";
export type ClearFiltersButtonProps = {
  onClick: () => void;
};

export default function ClearFiltersButton({ onClick }: ClearFiltersButtonProps) {
  return (
      <Button
          text={"Clear Filters"}
          onClick={onClick}
          className={`px-4 py-2 rounded-full text-sm bg-primary text-white btn-primary rounded-sm`}
              >
        </Button>
  );
}
