import type { AstroCookies } from 'astro';
import { API_BASE_URL } from './api';

export const ACCESS_COOKIE = 'mediasite_access';
export const REFRESH_COOKIE = 'mediasite_refresh';

interface TokenResponse {
	tokenType: string;
	accessToken: string;
	expiresIn: number;
	refreshToken: string;
}

export interface SessionUser {
	email: string;
	isEmailConfirmed: boolean;
}

interface CookieOptions {
	httpOnly: true;
	sameSite: 'lax';
	path: '/';
	secure: boolean;
	maxAge: number;
}

function cookieOptions(): CookieOptions {
	return {
		httpOnly: true,
		sameSite: 'lax',
		path: '/',
		secure: process.env.NODE_ENV === 'production',
		maxAge: 60 * 60 * 24 * 30,
	};
}

async function postTokens(path: string, body: Record<string, unknown>): Promise<TokenResponse | null> {
	try {
		const response = await fetch(`${API_BASE_URL}${path}`, {
			method: 'POST',
			headers: { 'content-type': 'application/json' },
			body: JSON.stringify(body),
		});

		if (!response.ok) {
			return null;
		}

		return (await response.json()) as TokenResponse;
	} catch {
		return null;
	}
}

export function login(email: string, password: string): Promise<TokenResponse | null> {
	return postTokens('/login', { email, password });
}

function refreshTokens(refreshToken: string): Promise<TokenResponse | null> {
	return postTokens('/refresh', { refreshToken });
}

async function getUserInfo(accessToken: string): Promise<SessionUser | null> {
	try {
		const response = await fetch(`${API_BASE_URL}/manage/info`, {
			headers: { authorization: `Bearer ${accessToken}` },
		});

		if (!response.ok) {
			return null;
		}

		const info = (await response.json()) as { email: string; isEmailConfirmed: boolean };
		return { email: info.email, isEmailConfirmed: info.isEmailConfirmed };
	} catch {
		return null;
	}
}

export async function setSession(cookies: AstroCookies, tokens: TokenResponse): Promise<void> {
	const options = cookieOptions();
	cookies.set(ACCESS_COOKIE, tokens.accessToken, {
		...options,
		maxAge: tokens.expiresIn,
	});
	cookies.set(REFRESH_COOKIE, tokens.refreshToken, options);
}

export function clearSession(cookies: AstroCookies): void {
	const options = cookieOptions();
	cookies.delete(ACCESS_COOKIE, options);
	cookies.delete(REFRESH_COOKIE, options);
}

export async function getAccessToken(cookies: AstroCookies): Promise<string | null> {
	const refreshToken = cookies.get(REFRESH_COOKIE)?.value;

	if (!refreshToken) {
		return null;
	}

	const existing = cookies.get(ACCESS_COOKIE)?.value;

	if (existing) {
		return existing;
	}

	const tokens = await refreshTokens(refreshToken);
	if (!tokens) {
		return null;
	}
	await setSession(cookies, tokens);
	return tokens.accessToken;
}

export async function refreshAccessToken(cookies: AstroCookies): Promise<string | null> {
	const refreshToken = cookies.get(REFRESH_COOKIE)?.value;

	if (!refreshToken) {
		return null;
	}

	const tokens = await refreshTokens(refreshToken);
	if (!tokens) {
		return null;
	}
	await setSession(cookies, tokens);
	return tokens.accessToken;
}

export async function getSession(cookies: AstroCookies): Promise<SessionUser | null> {
	let accessToken = await getAccessToken(cookies);

	if (!accessToken) {
		return null;
	}

	let user = await getUserInfo(accessToken);

	if (!user) {
		accessToken = await refreshAccessToken(cookies);
		if (!accessToken) {
			return null;
		}
		user = await getUserInfo(accessToken);
	}

	return user;
}

export async function logout(cookies: AstroCookies): Promise<void> {
	const accessToken = cookies.get(ACCESS_COOKIE)?.value;

	if (accessToken) {
		try {
			await fetch(`${API_BASE_URL}/logout`, {
				method: 'POST',
				headers: { authorization: `Bearer ${accessToken}` },
			});
		} catch {
			// best effort
		}
	}

	clearSession(cookies);
}

export function json(data: unknown, status = 200): Response {
	return new Response(JSON.stringify(data), {
		status,
		headers: { 'content-type': 'application/json; charset=utf-8' },
	});
}