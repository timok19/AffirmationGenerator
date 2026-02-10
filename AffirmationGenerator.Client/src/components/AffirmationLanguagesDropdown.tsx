import {affirmationLanguages} from "../models/affirmationLanguages.ts";

type LanguageDropdownProps = {
  value: string;
  onChange: (languageCode: string) => void;
};

function AffirmationLanguagesDropdown({value, onChange}: LanguageDropdownProps) {
  return (
    <select
      className="select select-bordered text-xl text-white bg-neutral"
      value={value}
      onChange={event => onChange(event.target.value)}
    >
      <option value="" disabled>Choose language</option>
      {affirmationLanguages.map(affirmationLanguage => (
        <option key={affirmationLanguage.code} value={affirmationLanguage.code}>{affirmationLanguage.label}</option>
      ))}
    </select>
  );
}

export default AffirmationLanguagesDropdown;