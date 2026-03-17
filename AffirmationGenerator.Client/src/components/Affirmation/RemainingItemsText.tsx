import { useEffect, useRef, useState } from "react";

type RemainingAffirmationsTextProps = {
  count: number;
  resetInSeconds: number;
};

function formatTime(totalSeconds: number) {
  const hours = Math.floor(totalSeconds / 3600);
  const minutes = Math.floor((totalSeconds % 3600) / 60);
  const seconds = totalSeconds % 60;

  return `${String(hours).padStart(2, "0")}:${String(minutes).padStart(2, "0")}:${String(seconds).padStart(2, "0")}`;
}

function getCountColor(count: number) {
  if (count >= 4) return "text-green-600";
  if (count >= 2) return "text-yellow-500";

  return "text-red-600";
}

function RemainingItemsText({ count, resetInSeconds }: RemainingAffirmationsTextProps) {
  const [secondsLeft, setSecondsLeft] = useState<number | null>(null);
  const intervalRef = useRef<ReturnType<typeof setInterval> | null>(null);

  useEffect(() => {
    if (intervalRef.current) {
      clearInterval(intervalRef.current);
      intervalRef.current = null;
    }

    if (count > 0 || resetInSeconds == 0) return;

    const targetTime = Date.now() + Math.max(0, Math.ceil(resetInSeconds)) * 1000;

    const updateTimer = () => {
      const remaining = Math.max(0, Math.ceil((targetTime - Date.now()) / 1000));

      setSecondsLeft(remaining);

      if (remaining <= 0 && intervalRef.current) {
        clearInterval(intervalRef.current);
        intervalRef.current = null;
      }
    };

    updateTimer();

    intervalRef.current = setInterval(updateTimer, 1000);

    return () => {
      if (intervalRef.current) {
        clearInterval(intervalRef.current);
        intervalRef.current = null;
      }
    };
  }, [count, resetInSeconds]);

  return (
    <div className="flex items-center justify-center md:w-60 w-68 h-12 rounded-lg glass border border-white/20 md:text-lg text-xl text-black font-medium absolute bottom-20 left-1/2 -translate-x-1/2 md:bottom-8 md:left-8 md:translate-x-0">
      {count === 0 && secondsLeft != null && secondsLeft > 0 ? (
        <>
          Resets in:{" "}
          <span className="ml-1 md:text-lg text-xl font-semibold text-red-600">
            {formatTime(secondsLeft)}
          </span>
        </>
      ) : (
        <>
          Remaining affirmations:{" "}
          <span className={`ml-1 md:text-lg text-xl font-semibold ${getCountColor(count)}`}>
            {count}
          </span>
        </>
      )}
    </div>
  );
}

export default RemainingItemsText;
