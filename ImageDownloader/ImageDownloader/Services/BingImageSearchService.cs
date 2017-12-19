using System;
using System.Net.Http;
using System.Threading.Tasks;
using ImageDownloader.Models;
using Newtonsoft.Json;

namespace ImageDownloader.Services
{
    public interface IBingImageSearchService: IDisposable
    {
        Task<BingImageSearchModel> ImageSearch(string searchQuery);
    }

    public class BingImageSearchService : IBingImageSearchService
    {
       

   

       

        /// <summary>
        ///     Performs a Bing Image search and return the results as a SearchResult.
        /// </summary>
        public async Task<BingImageSearchModel> ImageSearch(string searchQuery)
        {
            var subscriptionKey = Program.Configuration["bingimagesearch:key"];
            var uriBase= Program.Configuration["bingimagesearch:url"];

            if (subscriptionKey.Length != 32)
                throw new Exception("Invalid access key");
            //\\domain: www.wikipedia.org - &license=Public
            // Construct the URI of the search request

            var requestParameters = "q=\"" + Uri.EscapeDataString(searchQuery) + "\"" +
                                    "&count=30&imageType=Photo";

            var client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);


            // Assemble the URI for the REST API Call.
            var uri = uriBase + "?" + requestParameters;


            var response = await client.GetAsync(uri);

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<BingImageSearchModel>(json);


            return result;
        }

        public void Dispose()
        {
           
        }
    }
}