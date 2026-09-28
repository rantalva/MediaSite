const CATEGORY_LABELS: Record<string, string> = {
	Culture: 'Kulttuuri',
	Shopping: 'Ostokset',
	Sport: 'Urheilu',
	Style: 'Tyyli',
	Wellness: 'Hyvinvointi',
};

export function categoryLabel(name: string | null | undefined): string {
	if (!name) return '';
	return CATEGORY_LABELS[name] ?? name;
}
