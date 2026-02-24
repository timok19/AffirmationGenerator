import {useRef} from 'react';
import ArrowDownIcon from "./ArrowDownIcon.tsx";

interface LanguageOption {
  code: string;
  label: string;
}

type LanguageDropdownProps = {
  value: string;
  onChange: (targetLanguage: string) => void;
  disabled?: boolean;
  languages: LanguageOption[];
};

function LanguagesDropdown({value, onChange, disabled, languages}: LanguageDropdownProps) {
  const detailsRef = useRef<HTMLDetailsElement>(null);

  function handleSelect(code: string) {
    if (disabled)
      return;
    
    onChange(code);

    if (detailsRef.current)
      detailsRef.current.removeAttribute('open');
  }

  const selectedLabel = languages.find(language => language.code === value)?.label || "Choose language";

  return (
    <div className="absolute bottom-6 left-1/2 -translate-x-1/2 md:bottom-8 md:right-8 md:left-auto md:translate-x-0">
      <details
        ref={detailsRef}
        className={`group dropdown dropdown-top dropdown-center ${disabled ? 'pointer-events-none opacity-50' : ''}`}
      >
        <summary className="btn w-56 h-12 rounded-lg flex justify-center border border-white/20 bg-neutral text-white text-lg hover:bg-neutral/80 flex-nowrap">
          {selectedLabel}
          <ArrowDownIcon/>
        </summary>

        <ul className="menu dropdown-content z-1 p-1 shadow bg-neutral text-white text-lg w-56 rounded-box">
          {languages.map(affirmationLanguage => (
            <li key={affirmationLanguage.code}>
              <button
                onClick={() => handleSelect(affirmationLanguage.code)}
                className={`${value === affirmationLanguage.code ? 'active' : ''} justify-center flex h-10 text-white m-1`}
              >
                {affirmationLanguage.label}
              </button>
            </li>
          ))}
        </ul>
      </details>
    </div>
  );
}

export default LanguagesDropdown;