interface ButtonProps {
  title: string;
  onClick: () => void;
  disabled?: boolean;
}

const Button = ({ title, onClick, disabled = false }: ButtonProps) => {
  return (
    <button 
      onClick={onClick} 
      disabled={disabled}
      style={{
        padding: '10px 20px',
        margin: '5px',
        fontSize: '16px',
        cursor: disabled ? 'not-allowed' : 'pointer',
        backgroundColor: disabled ? '#cccccc' : '#4CAF50',
        color: 'white',
        border: 'none',
        borderRadius: '4px'
      }}
    >
      {title}
    </button>
  )
}

export default Button