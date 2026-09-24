import type { APIRoute } from 'astro';
import { json, logout } from '../../../lib/auth';

export const POST: APIRoute = async ({ cookies }) => {
	await logout(cookies);
	return json({ ok: true });
};