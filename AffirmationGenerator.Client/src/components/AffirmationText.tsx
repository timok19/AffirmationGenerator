type DisplayedTextProps = {
  text: string;
};

function AffirmationText({text}: DisplayedTextProps) {
  return (
    <h1 className="text-5xl md:text-8xl font-bold typing-cursor text-center">
      {text}
    </h1>
  );
}

export default AffirmationText;
