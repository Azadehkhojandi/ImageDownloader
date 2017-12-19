using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ImageDownloader.Models;
using Newtonsoft.Json;

namespace ImageDownloader.Services
{
    public interface IGoogleImageSearchService : IDisposable
    {
        Task<GoogleImageSearchModel> ImageSearch(string searchQuery);
    }

    public class GoogleImageSearchService : IGoogleImageSearchService
    {
       

        public void Dispose()
        {

        }

        /// <summary>
        ///     Performs a Bing Image search and return the results as a SearchResult.
        /// </summary>
        public async Task<GoogleImageSearchModel> ImageSearch(string searchQuery)
        {
            var subscriptionKey = Program.Configuration["googlecustomsearch:key"];
            var uriBase = Program.Configuration["googlecustomsearch:url"];
            var cx = Program.Configuration["googlecustomsearch:cx"];

            if (string.IsNullOrEmpty(subscriptionKey) || string.IsNullOrEmpty(cx))
                throw new Exception("Invalid access keys");

            var items = new List<Item>();
            GoogleImageSearchModel result = null;
            for (var i = 0; i < 3; i++)
            {
                var requestParameters = "q=\"" + Uri.EscapeDataString(searchQuery) + "\"" +
                                        "&searchType=image&key=" + subscriptionKey + "&cx=" + cx + "&start=" +
                                        (i <= 0 ? 1 : 10 * i);


                var client = new HttpClient();


                // Assemble the URI for the REST API Call.
                var uri = uriBase + "?" + requestParameters;


                var response = await client.GetAsync(uri);

                var json = await response.Content.ReadAsStringAsync();

                result = JsonConvert.DeserializeObject<GoogleImageSearchModel>(json);

                items.AddRange(result.items);
            }

            if (result != null)
                result.items = items;


            return result;
        }
    }
}