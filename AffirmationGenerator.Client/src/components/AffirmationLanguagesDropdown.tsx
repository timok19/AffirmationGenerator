import {useRef} from 'react';

export interface AffirmationLanguageOption {
  code: string;
  label: string;
}


type LanguageDropdownProps = {
  value: string;
  onChange: (languageCode: string) => void;
  disabled?: boolean;
  languages: AffirmationLanguageOption[];
};

function AffirmationLanguagesDropdown({value, onChange, disabled, languages}: LanguageDropdownProps) {
  const detailsRef = useRef<HTMLDetailsElement>(null);

  const handleSelect = (code: string) => {
    if (disabled) return;
    onChange(code);
    if (detailsRef.current) {
      detailsRef.current.removeAttribute('open');
    }
  };

  const selectedLabel = languages.find(language => language.code === value)?.label || "Choose language";

  return (
    <details
      ref={detailsRef}
      className={`dropdown dropdown-top dropdown-end ${disabled ? 'pointer-events-none opacity-50' : ''}`}
    >
      <summary className="btn w-48 justify-center border border-white/20 bg-neutral text-white hover:bg-neutral/80 flex-nowrap">
        {selectedLabel}
        <svg className="fill-current h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
          <path fillRule="evenodd"
                d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z"
                clipRule="evenodd"/>
        </svg>
      </summary>

      <ul className="dropdown-content z-1 menu p-2 shadow bg-neutral text-white w-48 rounded-box">
        {languages.map(affirmationLanguage => (
          <li key={affirmationLanguage.code}>
            <a
              onClick={() => handleSelect(affirmationLanguage.code)}
              className={`${value === affirmationLanguage.code ? 'active' : ''} justify-center text-white`}
            >
              {affirmationLanguage.label}
            </a>
          </li>
        ))}
      </ul>

    </details>
  );
}

export default AffirmationLanguagesDropdown;