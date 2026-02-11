import './App.css';
import AffirmationLanguagesDropdown from "./components/AffirmationLanguagesDropdown.tsx";
import AffirmationErrorMessage from "./components/AffirmationErrorMessage.tsx";
import AffirmationText from "./components/AffirmationText.tsx";
import RemainingAffirmationsText from "./components/RemainingAffirmationsText.tsx";
import Footer from "./components/Footer.tsx";
import {useEffect, useState} from 'react';
import axios from 'axios';
import AffirmationResponse from './models/affirmationResponse.ts';
import RemainingAffirmationsResponse from "./models/remainingAffirmationsResponse.ts";

function App() {
  const totalAffirmations = 5;

  const [remainingAffirmations, setRemainingAffirmations] = useState(totalAffirmations);
  const [affirmationText, setAffirmationText] = useState('Select a language for the affirmation');
  const [displayedText, setDisplayedText] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const [selectedLanguageCode, setSelectedLanguageCode] = useState('');
  const [isAnimating, setIsAnimating] = useState(false);
  const [isFetching, setIsFetching] = useState(false);

  const isTyping = displayedText.length < affirmationText.length;
  const isInteractionDisabled = isFetching || isTyping;

  async function handleLanguageChange(code: string) {
    if (isInteractionDisabled) return;
    
    setSelectedLanguageCode(code);
    
    setIsFetching(true);
    
    await getAffirmation(code);
    
    setIsFetching(false);
  }

  async function getAffirmation(languageCode: string) {
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
  
  async function getRemainingAffirmations() {
    try {
      const response = await axios.get<RemainingAffirmationsResponse>('/affirmations/remaining');
      
      setRemainingAffirmations(response.data.remainingAffirmations);
      
    } catch (error) {
      console.error('Failed to fetch remaining affirmations');
    }
  }
  
  useEffect(() => {
    getRemainingAffirmations();
  }, []);

  useEffect(() => {
    if (displayedText.length < affirmationText.length) {
      const timeout = setTimeout(() => {
        setDisplayedText(affirmationText.slice(0, displayedText.length + 1));
      }, 50);
      return () => clearTimeout(timeout);
    }
  }, [displayedText, affirmationText]);

  return (
    <div className="animated-bg min-h-screen flex flex-col items-center justify-between p-4 font-sans text-gray-800">
      <div className="flex-grow flex flex-col items-center justify-center w-full max-w-4xl relative">
        <div
          className={`glass w-full min-h-[600px] px-6 pt-6 pb-40 md:p-12 rounded-3xl shadow-2xl flex flex-col items-center justify-center relative z-10 ${isAnimating ? 'animate-pop-shake' : ''}`}>
          <AffirmationText text={displayedText} totalLength={affirmationText.length}/>

          <RemainingAffirmationsText
            count={remainingAffirmations}
            className="absolute bottom-20 left-1/2 -translate-x-1/2 md:bottom-8 md:left-8 md:translate-x-0"
          />

          <div className="absolute bottom-6 left-1/2 -translate-x-1/2 md:bottom-8 md:right-8 md:left-auto md:translate-x-0">
            <AffirmationLanguagesDropdown value={selectedLanguageCode} onChange={handleLanguageChange} disabled={isInteractionDisabled}/>
          </div>
        </div>

        <AffirmationErrorMessage message={errorMessage}/>
      </div>
      <Footer className="mt-8"/>
    </div>
  );
}

export default App;