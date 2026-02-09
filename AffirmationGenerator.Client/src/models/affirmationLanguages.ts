export interface AffirmationLanguageOption {
    code: string;
    label: string;
}

export const affirmationLanguages: AffirmationLanguageOption[] = [
    { code: 'en', label: 'English' },
    { code: 'cs', label: 'Czech' },
    { code: 'de', label: 'German' },
    { code: 'fr', label: 'French' }
];