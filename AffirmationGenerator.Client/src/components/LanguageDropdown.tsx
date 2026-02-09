import { affirmationLanguages } from "../models/affirmationLanguages.ts";

type LanguageDropdownProps = {
  value: string;
  onChange: (languageCode: string) => void;
};

function LanguageDropdown({ value, onChange }: LanguageDropdownProps) {
  return (
    <select
      className={"select select-bordered"}
      value={value}
      onChange={event => onChange(event.target.value)}
    >
      {affirmationLanguages.map(language => (
        <option key={language.code} value={language.code}>
          {language.label}
        </option>
      ))}
    </select>
  );
}

export default LanguageDropdown;