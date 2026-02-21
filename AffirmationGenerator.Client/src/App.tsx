import './App.css';
import AffirmationLanguagesDropdown from "./components/AffirmationLanguagesDropdown.tsx";
import AffirmationErrorMessage from "./components/AffirmationErrorMessage.tsx";
import AffirmationText from "./components/AffirmationText.tsx";
import RemainingAffirmationsText from "./components/RemainingAffirmationsText.tsx";
import MainCard from "./components/MainCard.tsx";
import Footer from "./components/Footer.tsx";
import {useEffect, useState} from 'react';
import axios from 'axios';
import {useQuery, useQueryClient} from "@tanstack/react-query";
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
  
  const queryClient = useQueryClient();

  const {data: languages = []} = useQuery({
    queryKey: ['languages'],
    queryFn: () => axios
      .get<AffirmationLanguagesResponse>('/affirmations/languages')
      .then(response => response.data.languages)
      .then(languages => Object.entries(languages).map(([code, label]) => ({code, label})))
  });

  const {data: remainingCount} = useQuery({
    queryKey: ['remainingAffirmations'],
    queryFn: () => axios
      .get<RemainingAffirmationsResponse>('/affirmations/remaining')
      .then(response => response.data.remainingAffirmations)
  });

  useEffect(() => {
    if (remainingCount === undefined) return;
    setRemainingAffirmations(remainingCount);
    if (remainingCount !== 0) return;
    setMaxAmountOfAffirmationsErrorMessage();
  }, [remainingCount]);

  useEffect(() => {
    if (displayedText.length < affirmationText.length) {
      const text = affirmationText.slice(0, displayedText.length + 1);
      const timeout = setTimeout(() => setDisplayedText(text), 50);
      return () => clearTimeout(timeout);
    }
  }, [displayedText, affirmationText]);

  const isTyping = displayedText.length < affirmationText.length;
  const isInteractionDisabled = isFetching || isTyping || remainingAffirmations === 0;
  
  function setMaxAmountOfAffirmationsErrorMessage() {
    setErrorMessage('Achieved maximum amount of affirmations per day. Come back tomorrow for more affirmations! 😁');
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
        queryClient.setQueryData(['remainingAffirmations'], data.remaining);
        if (data.remaining === 0) 
          setMaxAmountOfAffirmationsErrorMessage();
        setIsShaking(true);
        setTimeout(() => setIsShaking(false), 500);
      })
      .catch(error => {
        if (axios.isAxiosError(error) && error.response?.status === 429)
          setMaxAmountOfAffirmationsErrorMessage();
        else
          setErrorMessage('Unable to generate affirmation right now ☹️');
      })
      .finally(() => setIsFetching(false));
  }

  function handleLanguageChange(languageCode: string) {
    if (isInteractionDisabled)
      return;
    setSelectedLanguageCode(languageCode);
    getAffirmation(languageCode);
  }

  return (
    <div className="animated-bg min-h-screen flex flex-col items-center justify-between p-4 font-sans text-gray-800">
      <MainCard isShaking={isShaking} error={<AffirmationErrorMessage message={errorMessage}/>}>
        <AffirmationText text={displayedText} isLoading={isFetching}/>
        <RemainingAffirmationsText count={remainingAffirmations}/>
        <AffirmationLanguagesDropdown
          value={selectedLanguageCode}
          onChange={handleLanguageChange}
          languages={languages}
          disabled={isInteractionDisabled}/>
      </MainCard>
      <Footer/>
    </div>
  );
}

export default App;