import React from 'react';

type MainCardProps = {
  children: React.ReactNode;
  isShaking: boolean;
  error?: React.ReactNode;
};

function MainCard({children, isShaking, error}: MainCardProps) {
  return (
    <div className="grow flex flex-col items-center justify-center w-full max-w-4xl relative">
      <div
        className={`glass w-full min-h-150 px-10 pt-6 pb-40 md:p-12 rounded-3xl shadow-2xl flex flex-col items-center justify-center relative z-10 ${isShaking ? 'animate-pop-shake' : ''}`}>
        {children}
      </div>
      {error}
    </div>
  );
}

export default MainCard;
