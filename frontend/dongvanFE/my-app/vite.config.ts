import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import mkcert from 'vite-plugin-mkcert';
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [react(), mkcert(),tailwindcss(),],
  resolve: {
    extensions: ['.tsx', '.ts', '.jsx', '.js']
  },
  server: {
    https: true,
    port: 5173,
  }
});
