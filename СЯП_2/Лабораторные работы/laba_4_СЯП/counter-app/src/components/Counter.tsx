import { useState } from 'react'
import Button from './Button'

const Counter = () => {
  const [count, setCount] = useState<number>(0)

  const handleIncrease = (): void => {
    if (count < 5) {
      setCount(count + 1)
    }
  }

  const handleReset = (): void => {
    setCount(0)
  }

  return (
    <div style={{
      textAlign: 'center',
      padding: '20px',
      fontFamily: 'Arial, sans-serif'
    }}>
      <h1>Счетчик</h1>
      <div style={{
        fontSize: '48px',
        margin: '20px',
        fontWeight: 'bold'
      }}>
        {count}
      </div>
      <div>
        <Button 
          title="increase" 
          onClick={handleIncrease}
          disabled={count >= 5}
        />
        <Button 
          title="reset" 
          onClick={handleReset}
          disabled={count === 0}
        />
      </div>
    </div>
  )
}

export default Counter