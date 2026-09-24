import type { Article, Category } from '../types/api';

export const API_BASE_URL: string = process.env.API_BASE_URL ?? 'http://localhost:5248';

export class ApiError extends Error {
	status: number;

	constructor(status: number, path: string) {
		super(`API request failed (${status}): ${path}`);
		this.name = 'ApiError';
		this.status = status;
	}
}

async function getJson<T>(path: string, token?: string | null): Promise<T> {
	const response = await fetch(`${API_BASE_URL}${path}`, {
		headers: token ? { authorization: `Bearer ${token}` } : undefined,
	});

	if (!response.ok) {
		throw new ApiError(response.status, path);
	}

	return (await response.json()) as T;
}

export function getArticles(token?: string | null): Promise<Article[]> {
	return getJson<Article[]>('/api/articles', token);
}

export function getArticleBySlug(slug: string, token?: string | null): Promise<Article> {
	return getJson<Article>(`/api/articles/${slug}`, token);
}

export function getCategories(token?: string | null): Promise<Category[]> {
	return getJson<Category[]>('/api/categories', token);
}

export function imageUrl(path: string): string {
	if (/^https?:\/\//.test(path)) {
		return path;
	}

	return `${API_BASE_URL}${path}`;
}