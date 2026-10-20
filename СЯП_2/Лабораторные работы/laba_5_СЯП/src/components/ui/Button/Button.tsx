import React from 'react';

interface TitleProps {
  children?: React.ReactNode;
}

const Title: React.FC<TitleProps> = ({
  children,
}) => {

  return (
    <h1>
      {children}
    </h1>
  );
};

export default Title;