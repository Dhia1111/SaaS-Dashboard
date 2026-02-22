import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from 'tailwindcss'
import autoprefixer from 'autoprefixer'

// This is for your ACTUAL development server
export default defineConfig({
  plugins: [react()],
  css: {
    postcss: {
      plugins: [
        // Since your config is inside /src, we point to it here
        tailwindcss({ config: './tailwind.config.js' }), 
        autoprefixer(),
      ],
    },
  },
})