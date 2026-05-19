import SharedSearchBar from '@/components/question-bank/SearchBar';

export type SearchBarProps = {
  placeholder?: string;
  value: string;
  onChange: (val: string) => void;
};

// Wrapper around the question-bank SearchBar to keep a single shared implementation
export const SearchBar = ({ placeholder = 'Search...', value, onChange }: SearchBarProps) => {
  return <SharedSearchBar value={value} onChange={onChange} placeholder={placeholder} />;
};
