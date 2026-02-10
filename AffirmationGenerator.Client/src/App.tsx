import './App.css';
import AffirmationLanguagesDropdown from "./components/AffirmationLanguagesDropdown.tsx";
import AffirmationErrorMessage from "./components/AffirmationErrorMessage.tsx";
import AffirmationText from "./components/AffirmationText.tsx";
import RemainingAffirmationsText from "./components/RemainingAffirmationsText.tsx";
import AffirmationResponse from './models/affirmationResponse.ts';
import {useEffect, useState} from 'react';
import axios from 'axios';

function App() {
  const totalAffirmations = 5;

  const [remainingAffirmations, setRemainingAffirmations] = useState(totalAffirmations);
  const [affirmationText, setAffirmationText] = useState('Select a language for the affirmation');
  const [displayedText, setDisplayedText] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const [selectedLanguageCode, setSelectedLanguageCode] = useState('');
  const [isAnimating, setIsAnimating] = useState(false);

  async function handleLanguageChange(code: string) {
    setSelectedLanguageCode(code);
    await generateAffirmation(code);
  }

  async function generateAffirmation(languageCode: string) {
    try {
      setErrorMessage('');
      const response = await axios.get<AffirmationResponse>(
        '/affirmations',
        {
          params: {
            affirmationLanguageCode: languageCode
          }
        });

      setAffirmationText(response.data.affirmation);
      setDisplayedText('');
      setRemainingAffirmations(response.data.remaining);
      setIsAnimating(true);
      setTimeout(() => setIsAnimating(false), 500);
    } catch (error) {
      if (axios.isAxiosError(error) && error.response?.status === 429) {
        setErrorMessage('Achieved maximum amount of affirmations per day. Come back tomorrow for more affirmations! 😁');
        return;
      }

      setErrorMessage('Unable to generate affirmation right now ☹️');
    }
  }

  useEffect(() => {
    if (displayedText.length < affirmationText.length) {
      const timeout = setTimeout(() => {
        setDisplayedText(affirmationText.slice(0, displayedText.length + 1));
      }, 50);
      return () => clearTimeout(timeout);
    }
  }, [displayedText, affirmationText]);

  return (
    <div className="animated-bg min-h-screen flex flex-col items-center justify-center p-4 font-sans text-gray-800">
      <div className="relative max-w-4xl w-full flex flex-col items-center">
        <div
          className={`glass w-full min-h-150 p-12 rounded-3xl shadow-2xl flex flex-col items-center justify-center relative z-10 ${isAnimating ? 'animate-pop-shake' : ''}`}>
          <AffirmationText text={displayedText}/>

          <RemainingAffirmationsText count={remainingAffirmations}/>

          <div className="absolute bottom-8 right-8">
            <AffirmationLanguagesDropdown value={selectedLanguageCode} onChange={handleLanguageChange}/>
          </div>
        </div>

        <AffirmationErrorMessage message={errorMessage}/>
      </div>
    </div>
  );
}

export default App;