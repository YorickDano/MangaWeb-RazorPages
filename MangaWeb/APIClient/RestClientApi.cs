﻿using HtmlAgilityPack;
using MangaWeb.APIClient.Services;
using MangaWeb.Enums;
using RestSharp;
using System.Net;
using System.Text.RegularExpressions;

namespace MangaWeb.APIClient
{
    public class RestClientApi
    {
        protected RestClient RestClient;
        private readonly Uri Url =  new Uri("https://www.google.by/");
        protected RequestBuilder requestBuilder = new RequestBuilder();

        protected WebClient WebClient;


        public RestClientApi()
        {
            RestClient = new RestClient(Url);
            WebClient = new WebClient();
        }

        public async Task<string> GetUrlOfMangaByTitle(string title)
        {
            var response = await SearchFor(title + " mangalib");
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response);
            var f = htmlDocument.DocumentNode.SelectNodes("//a[contains(@href,'https://mangalib.me/')]");
            var linkTag = f.FirstOrDefault();
            var link = linkTag.GetAttributeValue("href", "https://mangalib.me/");
            link = link.Remove(0, 7);
            link = link.Remove(link.IndexOf("&amp"));
            return link;
        }

        public async Task<string> GetMangaProfieImageUrlByTitle(string title, SearchType searchType = SearchType.DefaultImage)
        {
            var responseContent = await GetImageSearchRespounseContent(title + " mangalib", searchType);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(responseContent);
            var linkTag = htmlDocument.DocumentNode.SelectNodes("//img[contains(@src,'https://encrypted')]").FirstOrDefault();

            return linkTag.GetAttributeValue("src", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSb1kKGBpZv8tOMiEi2yTJzr9sPAlRbnQe2gY_F_C7qfn0jInMVbFFjuNesYkk&amp;s");
        }

        protected async Task<string> GetImageSearchRespounseContent(string title, SearchType searchType = SearchType.DefaultImage)
        {
            var searchString = searchType == SearchType.MangaImage ? title + " manga" : title;
            requestBuilder.CreateRequest()
                .SetRequestResource("search")
                .AddRequestParameter("q", searchString)
                .AddRequestParameter("tbm", "isch");
            var response = await RestClient.ExecuteAsync(requestBuilder.GetRequest());
            return response.Content;
        }

        protected async Task<string> GetHtmlOf(string url)
        {
            using var client = new WebClient();
            return client.DownloadString(url);
        }

        protected async Task<string> SearchFor(string title)
        {
            requestBuilder.CreateRequest()
               .SetRequestResource("search")
               .AddRequestParameter("q", title);
            var response = await RestClient.ExecuteAsync(requestBuilder.GetRequest());

            return response.Content;
        } 

    }
}