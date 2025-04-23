import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd())

  return {
    plugins: [vue()],
    define: {
      __APP_ENV__: env.__APP_ENV__
    },
    server: {
      host: true  // needed so Docker can expose the dev server
    }
  }
})
