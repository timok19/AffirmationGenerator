type RemainingAffirmationsTextProps = {
  count: number;
};

function RemainingAffirmationsText({count}: RemainingAffirmationsTextProps) {
  function getCountColor(count: number) {
    if (count >= 4) return 'text-green-600';
    if (count >= 2) return 'text-yellow-500';
    return 'text-red-600';
  }

  return (
    <div
      className="flex items-center justify-center w-52 h-12 rounded-lg glass border border-white/20 text-black font-medium absolute bottom-20 left-1/2 -translate-x-1/2 md:bottom-8 md:left-8 md:translate-x-0">
      Available affirmations: <span className={`ml-1 font-semibold ${getCountColor(count)}`}>{count}</span>
    </div>
  );
}

export default RemainingAffirmationsText;
