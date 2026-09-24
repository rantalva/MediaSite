export interface Article {
	id: string;
	title: string;
	slug: string;
	content: string;
	heroImage: string;
	createdDate: string | null;
	lastEditDate: string | null;
	categoryId: string;
	categoryName: string;
	authorId: string;
	authorName: string | null;
}

export interface CategoryArticle {
	id: string;
	title: string;
	slug: string;
	heroImage: string;
}

export interface Category {
	name: string | null;
	articles: CategoryArticle[];
}