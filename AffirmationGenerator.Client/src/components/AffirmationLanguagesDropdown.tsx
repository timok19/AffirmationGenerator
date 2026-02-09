import {affirmationLanguages} from "../models/affirmationLanguages.ts";

type LanguageDropdownProps = {
  value: string;
  onChange: (languageCode: string) => void;
};

function AffirmationLanguagesDropdown({value, onChange}: LanguageDropdownProps) {
  return (
    <select
      className={"select select-bordered"}
      value={value}
      onChange={event => onChange(event.target.value)}
    >
      {affirmationLanguages.map(affirmationLanguage => (
        <option key={affirmationLanguage.code} value={affirmationLanguage.code}>{affirmationLanguage.label}</option>
      ))}
    </select>
  );
}

export default AffirmationLanguagesDropdown;