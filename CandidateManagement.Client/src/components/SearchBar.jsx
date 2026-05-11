import { useState } from "react";

function SearchBar({
  label = "Search",
  placeholder = "Type to search...",
  buttonText = "Search",
  initialValue = "",
  onSearch,
  onClear,
}) {
  const [value, setValue] = useState(initialValue);

  function handleSubmit(event) {
    event.preventDefault();
    onSearch?.(value.trim());
  }

  function handleClear() {
    setValue("");
    onClear?.();
  }

  return (
    <form className="search-bar" onSubmit={handleSubmit}>
      <label className="search-bar__label">{label}</label>
      <div className="search-bar__row">
        <input
          type="text"
          value={value}
          placeholder={placeholder}
          onChange={(event) => setValue(event.target.value)}
        />
        <button type="submit">{buttonText}</button>
        <button type="button" className="button-secondary" onClick={handleClear}>
          Clear
        </button>
      </div>
    </form>
  );
}

export default SearchBar;
