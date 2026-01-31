import { useState, useEffect } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'

function App() {
  const [count, setCount] = useState(0)
  const [weather, setWeather] = useState([]) // store API result
  const [error, setError] = useState(null)

  useEffect(() => {
    // Fetch data from API
    fetch('http://localhost:7073/WeatherForecast')
      .then((res) => {
        if (!res.ok) throw new Error('Network response was not ok')
        return res.json()
      })
      .then((data) => setWeather(data))
      .catch((err) => setError(err.message))
  }, [])

  return (
    <>
      <div>
        <a href="https://vite.dev" target="_blank">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
      <h1>Vite + React</h1>
      <div className="card">
        <button onClick={() => setCount((count) => count + 1)}>
          count is {count}
        </button>
        <p>
          Edit <code>src/App.jsx</code> and save to test HMR
        </p>
      </div>

      <div className="api-result">
        <h2>Weather Forecast API</h2>
        {error && <p style={{ color: 'red' }}>Error: {error}</p>}
        {!error && weather.length === 0 && <p>Loading...</p>}
        {!error && weather.length > 0 && (
          <pre>{JSON.stringify(weather, null, 2)}</pre>
        )}
      </div>

      <p className="read-the-docs">
        Click on the Vite and React logos to learn more
      </p>
    </>
  )
}

export default App
