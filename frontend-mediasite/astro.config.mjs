// @ts-check
import { defineConfig } from 'astro/config';
import node from '@astrojs/node';
import { webcore } from 'webcoreui/integration'

// https://astro.build/config
export default defineConfig({
	output: 'server',
	adapter: node({ mode: 'standalone' }),
	integrations: [webcore()],vite: {
		css: {
			preprocessorOptions: {
				scss: {
					loadPaths: ['./src/styles']
				}
			}
		} 
	}
});