// src/test/setup.js
import '@testing-library/jest-dom/vitest'
import { cleanup } from '@testing-library/react'
import { afterEach } from 'vitest'
import { vi } from 'vitest'

// Automatically cleanup after each test
afterEach(() => {
  cleanup()
})
;

vi.mock('/vite.svg', () => ({ default: '/vite.svg', }));

vi.mock('/logo.svg', () => ({
  default: '/logo.svg',
}));