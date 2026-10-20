import React from 'react';
import styles from './LayoutCard.module.css';

interface LayoutCardProps {
  title: React.ReactNode;
  children: React.ReactNode;
  footer?: React.ReactNode;
}

const LayoutCard: React.FC<LayoutCardProps> = ({ title, children, footer }) => {
  return (
    <div className={styles.card}>
      <div className={styles.header}>
        <h3 className={styles.title}>{title}</h3>
      </div>
      <div className={styles.body}>{children}</div>
      {footer && <div className={styles.footer}>{footer}</div>}
    </div>
  );
};

export default LayoutCard;