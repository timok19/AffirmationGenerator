import './App.css';
import AffirmationLanguagesDropdown, {AffirmationLanguageOption} from "./components/AffirmationLanguagesDropdown.tsx";
import AffirmationErrorMessage from "./components/AffirmationErrorMessage.tsx";
import AffirmationText from "./components/AffirmationText.tsx";
import RemainingAffirmationsText from "./components/RemainingAffirmationsText.tsx";
import MainCard from "./components/MainCard.tsx";
import Footer from "./components/Footer.tsx";
import {useEffect, useState} from 'react';
import axios from 'axios';
import AffirmationResponse from './models/affirmationResponse.ts';
import RemainingAffirmationsResponse from "./models/remainingAffirmationsResponse.ts";
import AffirmationLanguagesResponse from "./models/affirmationLanguagesResponse.ts";

function App() {
  const [remainingAffirmations, setRemainingAffirmations] = useState(0);
  const [affirmationText, setAffirmationText] = useState('Select a language for the affirmation');
  const [displayedText, setDisplayedText] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const [selectedLanguageCode, setSelectedLanguageCode] = useState('');
  const [isShaking, setIsShaking] = useState(false);
  const [isFetching, setIsFetching] = useState(false);
  const [languages, setLanguages] = useState<AffirmationLanguageOption[]>([]);

  useEffect(() => {
    axios
      .get<RemainingAffirmationsResponse>('/affirmations/remaining')
      .then(response => response.data.remainingAffirmations)
      .then(setRemainingAffirmations)
      .catch(() => console.error('Failed to fetch remaining affirmations'));

    axios
      .get<AffirmationLanguagesResponse>('/affirmations/languages')
      .then(response => response.data.languages)
      .then(languages => Object.entries(languages).map(([code, label]) => ({code, label})))
      .then(setLanguages)
      .catch(() => console.error('Failed to fetch languages'));
  }, []);

  useEffect(() => {
    if (displayedText.length < affirmationText.length) {
      const text = affirmationText.slice(0, displayedText.length + 1);
      const timeout = setTimeout(() => setDisplayedText(text), 50);
      return () => clearTimeout(timeout);
    }
  }, [displayedText, affirmationText]);

  const isTyping = displayedText.length < affirmationText.length;
  const isInteractionDisabled = isFetching || isTyping;

  function handleLanguageChange(code: string) {
    if (isInteractionDisabled)
      return;
    setSelectedLanguageCode(code);
    getAffirmation(code);
  }

  function getAffirmation(languageCode: string) {
    setErrorMessage('');
    setIsFetching(true);
    axios
      .get<AffirmationResponse>('/affirmations', {params: {affirmationLanguageCode: languageCode}})
      .then(response => response.data)
      .then(data => {
        setAffirmationText(data.affirmation);
        setDisplayedText('');
        setRemainingAffirmations(data.remaining);
        setIsShaking(true);
        setTimeout(() => setIsShaking(false), 500);
      })
      .catch(error => {
        if (axios.isAxiosError(error) && error.response?.status === 429)
          setErrorMessage('Achieved maximum amount of affirmations per day. Come back tomorrow for more affirmations! 😁');
        else
          setErrorMessage('Unable to generate affirmation right now ☹️');
      })
      .finally(() => setIsFetching(false));
  }

  return (
    <div className="animated-bg min-h-screen flex flex-col items-center justify-between p-4 font-sans text-gray-800">
      <MainCard isShaking={isShaking} error={<AffirmationErrorMessage message={errorMessage}/>}>
        <AffirmationText text={displayedText} isLoading={isFetching}/>
        <RemainingAffirmationsText count={remainingAffirmations}/>
        <AffirmationLanguagesDropdown
          value={selectedLanguageCode}
          onChange={handleLanguageChange}
          disabled={isInteractionDisabled}
          languages={languages}/>
      </MainCard>
      <Footer/>
    </div>
  );
}

export default App;