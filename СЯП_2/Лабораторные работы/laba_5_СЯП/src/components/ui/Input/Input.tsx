import React, { useState } from 'react';
import classNames from 'classnames';
import styles from './Input.module.css';

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label: string;
  error?: string;
  isFullWidth?: boolean;
}

const Input: React.FC<InputProps> = ({
  label,
  error,
  isFullWidth = false,
  id,
  value,
  onChange,
  ...rest
}) => {
  const inputId = id || `input-${label.replace(/\s/g, '-')}`;
  const [innerValue, setInnerValue] = useState(value || '');

  const currentValue = value !== undefined ? value : innerValue;

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (onChange) {
      onChange(e);
    } else {
      setInnerValue(e.target.value);
    }
  };

  const containerClass = classNames(styles.container, {
    [styles.fullWidth]: isFullWidth,
  });

  const inputClass = classNames(styles.input, {
    [styles.inputError]: error,
  });

  return (
    <div className={containerClass}>
      <label htmlFor={inputId} className={styles.label}>
        {label}
      </label>
      <input
        id={inputId}
        className={inputClass}
        value={currentValue}
        onChange={handleChange}
        {...rest}
      />
      {error && <span className={styles.error}>{error}</span>}
    </div>
  );
};

export default Input;