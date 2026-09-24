import type { APIRoute } from 'astro';
import { API_BASE_URL } from '../../../lib/api';
import { json, login, setSession } from '../../../lib/auth';

interface ProblemDetails {
	errors?: Record<string, string[]>;
	title?: string;
	type?: string;
}

export const POST: APIRoute = async ({ request, cookies }) => {
	const form = await request.formData();
	const email = String(form.get('email') ?? '');
	const password = String(form.get('password') ?? '');
	const confirmPassword = String(form.get('confirmPassword') ?? '');

	if (!email || !password) {
		return json({ ok: false, message: 'Täytä sähköposti ja salasana.' }, 400);
	}

	if (password !== confirmPassword) {
		return json({ ok: false, message: 'Salasanat eivät täsmää.' }, 400);
	}

	let response: Response;
	try {
		response = await fetch(`${API_BASE_URL}/register`, {
			method: 'POST',
			headers: { 'content-type': 'application/json' },
			body: JSON.stringify({ email, password }),
		});
	} catch {
		return json({ ok: false, message: 'Yhteyden muodostaminen palvelimeen epäonnistui.' }, 500);
	}

	if (!response.ok) {
		let message = 'Rekisteröinti epäonnistui.';
		try {
			const problem = (await response.json()) as ProblemDetails;
			const firstError = Object.values(problem.errors ?? {})
				.flat()
				.find(Boolean);
			if (firstError) {
				message = firstError;
			} else if (problem.title) {
				message = problem.title;
			}
		} catch {
			// keep default message
		}
		return json({ ok: false, message }, response.status);
	}

	const tokens = await login(email, password);

	if (tokens) {
		await setSession(cookies, tokens);
	}

	return json({ ok: true });
};