import React from 'react';
import classNames from 'classnames';
import styles from './Badge.module.css';

interface BadgeProps {
  color: 'green' | 'red' | 'orange' | 'blue';
  text: string;
}

const Badge: React.FC<BadgeProps> = ({ color, text }) => {
  return (
    <span className={classNames(styles.badge, styles[color])}>
      {text}
    </span>
  );
};

export default Badge;