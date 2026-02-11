type DisplayedTextProps = {
  text: string;
  totalLength?: number;
};

function AffirmationText({text, totalLength}: DisplayedTextProps) {
  const length = totalLength ?? text.length;
  let textSizeClass = "text-4xl md:text-8xl"; // Default

  if (length > 100) {
    textSizeClass = "text-lg md:text-3xl";
  } else if (length > 75) {
    textSizeClass = "text-xl md:text-4xl";
  } else if (length > 50) {
    textSizeClass = "text-2xl md:text-5xl";
  } else if (length > 25) {
    textSizeClass = "text-3xl md:text-6xl";
  }

  return (
    <h1 className={`${textSizeClass} font-bold typing-cursor text-center break-words transition-all duration-300 px-5`}>
      {text}
    </h1>
  );
}

export default AffirmationText;
