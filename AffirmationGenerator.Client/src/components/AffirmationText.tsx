type DisplayedTextProps = {
  text: string;
};

function AffirmationText({text}: DisplayedTextProps) {
  function getTextSizeClass() {
    let textSizeClass = "text-4xl md:text-8xl";

    if (text.length > 100) {
      textSizeClass = "text-lg md:text-3xl";
    } else if (text.length > 75) {
      textSizeClass = "text-xl md:text-4xl";
    } else if (text.length > 50) {
      textSizeClass = "text-2xl md:text-5xl";
    } else if (text.length > 25) {
      textSizeClass = "text-3xl md:text-6xl";
    }

    return textSizeClass;
  }

  return (
    <h1 className={`${getTextSizeClass()} font-bold typing-cursor text-center wrap-break-word transition-all duration-300 px-5`}>
      {text}
    </h1>
  );
}


export default AffirmationText;
