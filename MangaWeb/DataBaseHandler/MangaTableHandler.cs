﻿using MangaWeb.APIClient;
using MangaWeb.Models;

namespace MangaWeb.DataBaseHandler
{
    public static class MangaTableHandler
    {

        public static async Task<Manga> UpdateMangaImagesUrls(this Manga manga)
        {
            var animeAndHentaiClient = new AnimeAndHentaiImageClient();
            var animeImagesUrls = await animeAndHentaiClient.GetImagesUrlsByTitle(manga.Title);
            var hentaiImagesUrls = await animeAndHentaiClient.GetImagesUrlsByTitle(manga.Title, AnimeType.Hentai);
            manga.HentaiImagesUrls = new string[hentaiImagesUrls.Count];
            manga.AnimeImagesUrls = new string[animeImagesUrls.Count];

            for (int i = 0; i < animeImagesUrls.Count; ++i)
            {
                manga.AnimeImagesUrls[i] = animeImagesUrls[i];
                manga.HentaiImagesUrls[i] = hentaiImagesUrls[i];
            }

            return manga;
        }
    }
}
