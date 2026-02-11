type DisplayedTextProps = {
  text: string;
};

function AffirmationText({text}: DisplayedTextProps) {
  const length = text.length;
  let textSizeClass = "text-5xl md:text-8xl"; // Default

  if (length > 60) {
    textSizeClass = "text-3xl md:text-5xl";
  } else if (length > 30) {
    textSizeClass = "text-4xl md:text-6xl";
  }

  return (
    <h1 className={`${textSizeClass} font-bold typing-cursor text-center transition-all duration-300`}>
      {text}
    </h1>
  );
}

export default AffirmationText;
