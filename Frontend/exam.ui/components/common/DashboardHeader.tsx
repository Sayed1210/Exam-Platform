type DashboardPageHeaderProps = {
  title: string;
  buttonText?: string;
  onButtonClick?: () => void;
};

export default function DashboardPageHeader({
  title,
  buttonText,
  onButtonClick,
}: DashboardPageHeaderProps) {
  return (
    <div className="flex items-center justify-between mb-6">
      <h1 className="text-title">{title}</h1>

      {buttonText && (
        <button
          onClick={onButtonClick}
          className=" btn-primary px-6"
        >
          {buttonText}
        </button>
      )}
    </div>
  );
}