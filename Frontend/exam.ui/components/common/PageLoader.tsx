export default function PageLoader() {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-white">
      <div className="flex flex-col items-center gap-5">
        
        {/* Spinner */}
        <div className="relative">
          <div className="w-16 h-16 rounded-full border-4 border-gray-200"></div>

          <div className="w-16 h-16 rounded-full border-4 border-primary border-t-transparent animate-spin absolute inset-0"></div>
        </div>

        {/* Text */}
        <div className="text-center">
          <h2 className="text-lg font-semibold text-gray-800">
            Loading...
          </h2>

          <p className="text-sm text-gray-500 mt-1">
            Please wait a moment
          </p>
        </div>
      </div>
    </div>
  );
}