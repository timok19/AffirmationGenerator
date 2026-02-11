type RemainingAffirmationsTextProps = {
  count: number;
};

function RemainingAffirmationsText({count}: RemainingAffirmationsTextProps) {
  const getCountColor = (count: number) => {
    if (count >= 4) return 'text-green-600';
    if (count >= 2) return 'text-yellow-500';
    return 'text-red-600';
  };

  return (
    <div className="absolute bottom-8 left-8">
      <div className="flex items-center justify-center w-52 h-12 rounded-lg glass border border-white/20 text-black font-medium">
        Available affirmations: <span className={`ml-1 font-semibold ${getCountColor(count)}`}>{count}</span>
      </div>
    </div>
  );
}

export default RemainingAffirmationsText;
