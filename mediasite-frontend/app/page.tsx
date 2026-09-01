"use client";

import Image from "next/image";
import { useState, useEffect } from "react";

export default function Home() {
  const [articles, setArticles] = useState([]);

async function fetchArticles() {
  const url = "https://localhost:7135/api/articles";
  try {
    const response = await fetch(url);
    if (!response.ok) {
      throw new Error(`Response status: ${response.status}`);
    }

    const result = await response.json();
    setArticles(result)
    console.log(articles);
  } catch (error) {
    console.error(error);
  }
}

  return (
    <div>
      <button onClick={fetchArticles}>Press me</button>

      <div>
        {articles.map((article) => (
          <div key={article.id}>
            <h2>{article.title}</h2>
            <p>{article.content}</p>
            <Image
              src={`https://localhost:7135${article.heroImage}`}
              width={300}
              height={300}
              alt="Picture of the author"
              unoptimized
            />
          </div>
        ))}
      </div>
    </div>
  );
}
