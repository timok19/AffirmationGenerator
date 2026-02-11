type ErrorMessageProps = {
  message: string;
};

function AffirmationErrorMessage({message}: ErrorMessageProps) {
  if (!message) return null;

  return (
    <div className="relative mt-4 w-full flex justify-center animate-slide-down z-0">
      <div className="alert bg-blue-400 text-white border-none shadow-lg flex items-center gap-3 w-auto px-6 py-3 rounded-xl">
        <span className="font-medium text-lg">{message}</span>
      </div>
    </div>
  );
}


export default AffirmationErrorMessage;
