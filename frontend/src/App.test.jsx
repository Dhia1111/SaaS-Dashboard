import { describe, it, expect } from 'vitest'
import { render, screen, fireEvent, within } from '@testing-library/react'
import App from './App'

describe('App Component', () => {
  it('renders the main heading', () => {
    render(<App />)
    expect(screen.getByRole('heading', { level: 1 }))
      .toHaveTextContent('Vite + React')
  })

  it('renders count button with initial value 0', () => {
    render(<App />)
    const button = screen.getByRole('button', { name: /count is 0/i })
    expect(button).toBeInTheDocument()
  })

  it('increments count when button is clicked', () => {
    render(<App />)
    const button = screen.getByRole('button', { name: /count is 0/i })
    
    fireEvent.click(button)
    expect(button).toHaveTextContent('count is 1')
    
    fireEvent.click(button)
    expect(button).toHaveTextContent('count is 2')
  })

  it('renders edit instruction text', () => {
    render(<App />)
    
    // ✅ Narrow search: find the card first, then the text inside it
    const card = screen.getByRole('button').closest('div.card') // get the card container
    const instruction = within(card).getByText((content, element) =>
      element.tagName.toLowerCase() === 'p' &&
      element.textContent.includes('Edit src/App.jsx and save to test HMR')
    )
    
    expect(instruction).toBeInTheDocument()
  })
})
