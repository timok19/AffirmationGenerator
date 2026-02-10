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
      <p className="text-xl opacity-90">
        Available affirmations: <span
        className={`font-bold text-2xl ${getCountColor(count)}`}>{count}</span>
      </p>
    </div>
  );
}

export default RemainingAffirmationsText;
