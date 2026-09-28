import type { APIRoute } from 'astro';
import { API_BASE_URL } from '../../../lib/api';
import { json } from '../../../lib/auth';

interface ProblemDetails {
	errors?: Record<string, string[]>;
	title?: string;
	type?: string;
}

export const POST: APIRoute = async ({ request }) => {
	const form = await request.formData();
	const email = String(form.get('email') ?? '').trim();

	if (!email) {
		return json({ ok: false, message: 'Anna sähköpostiosoitteesi.' }, 400);
	}

	let response: Response;
	try {
		response = await fetch(`${API_BASE_URL}/api/newslettersubscribers`, {
			method: 'POST',
			headers: { 'content-type': 'application/json' },
			body: JSON.stringify({ email }),
		});
	} catch {
		return json({ ok: false, message: 'Yhteyden muodostaminen palvelimeen epäonnistui.' }, 500);
	}

	if (!response.ok) {
		let message = 'Tilaus epäonnistui.';
		try {
			const problem = (await response.json()) as ProblemDetails;
			const firstError = Object.values(problem.errors ?? {})
				.flat()
				.find(Boolean);
			if (firstError) {
				message = firstError;
			} else if (response.status === 400) {
				message = 'Tämä sähköpostiosoite on jo tilattu.';
			} else if (problem.title) {
				message = problem.title;
			}
		} catch {
			if (response.status === 400) {
				message = 'Tämä sähköpostiosoite on jo tilattu.';
			}
		}
		return json({ ok: false, message }, response.status);
	}

	return json({ ok: true });
};
