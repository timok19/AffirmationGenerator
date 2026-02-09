import './App.css';

import AffirmationResponse from './models/affirmationResponse.ts';
import { affirmationLanguages } from './models/affirmationLanguages.ts';

import { useState } from 'react';
import axios from 'axios';

function App() {
  const [affirmationText, setAffirmationText] = useState('Affirmation text here');
  const [selectedLanguageCode, setSelectedLanguageCode] = useState(affirmationLanguages[0].code);
  const [errorMessage, setErrorMessage] = useState('');
  const totalAffirmations = 5;

  const [remainingAffirmations, setRemainingAffirmations] = useState(totalAffirmations);

    return (
      <div>
        <p className="text-sm">Affirmations per day: {remainingAffirmations}</p>
      
        <p className={"text-accent"}>{affirmationText}</p>
      
        {errorMessage ? <p className="text-error">{errorMessage}</p> : null}
      
        <select
          className={"select select-bordered"}
          value={selectedLanguageCode}
          onChange={event => setSelectedLanguageCode(event.target.value)}
        >
          {affirmationLanguages.map(language => (
            <option key={language.code} value={language.code}>
              {language.label}
            </option>
          ))}
        </select>
      
        <button className={"btn"} onClick={generateAffirmation}>Generate</button>
      </div>
    );

  async function generateAffirmation() {
    try {
      const response = await axios.postForm<AffirmationResponse>(
        '/affirmations/generate', 
        {
          affirmationLanguageCode: selectedLanguageCode
        });

      setAffirmationText(response.data.affirmation);
      setRemainingAffirmations(response.data.remaining);
      
      setErrorMessage('');
    } catch (error) {
      if (axios.isAxiosError(error) && error.response?.status === 429) {
          setErrorMessage('You used the maximum amount of affirmations per day.');
          return;
      }
      
      setErrorMessage('Unable to generate affirmation right now.');
    }
  }
}

export default App;