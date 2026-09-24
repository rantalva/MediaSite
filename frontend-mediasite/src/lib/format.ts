export function formatDate(date?: string | null): string {
	if (!date) {
		return '';
	}

	return new Intl.DateTimeFormat('fi-FI', { dateStyle: 'long' }).format(new Date(date));
}