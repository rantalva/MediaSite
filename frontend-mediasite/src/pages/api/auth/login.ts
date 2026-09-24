import type { APIRoute } from 'astro';
import { json, login, setSession } from '../../../lib/auth';

export const POST: APIRoute = async ({ request, cookies }) => {
	const form = await request.formData();
	const email = String(form.get('email') ?? '');
	const password = String(form.get('password') ?? '');

	if (!email || !password) {
		return json({ ok: false, message: 'Täytä sähköposti ja salasana.' }, 400);
	}

	const tokens = await login(email, password);

	if (!tokens) {
		return json({ ok: false, message: 'Väärä sähköposti tai salasana.' }, 401);
	}

	await setSession(cookies, tokens);
	return json({ ok: true });
};