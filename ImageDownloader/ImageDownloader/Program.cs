using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ImageDownloader.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace ImageDownloader
{
    internal class Program
    {

        public static IConfigurationRoot Configuration { get; set; }
        public static ServiceProvider ServiceProvider { get; private set; }

        private static async Task Extractdataanddownloadimages()
        {


            Console.OutputEncoding = Encoding.UTF8;
            var csvFilePath = Configuration["csvfilepath"] ;

            var dataExtracterService = new DataExtracterService();
            var distinctNames = dataExtracterService.Getinfo(csvFilePath);


            foreach (var searchTerm in distinctNames.Take(1))
            {
                Console.WriteLine("Searching images for: " + searchTerm);


                await DownloadImagesViaBing(searchTerm);

                await DownloadImagesViaGoogle(searchTerm);
            }
        }

        private static async Task DownloadImagesViaGoogle(string searchTerm)
        {
            using (var googleImageSearchService = ServiceProvider.GetService<IGoogleImageSearchService>())
            {
                var googleresult = await googleImageSearchService.ImageSearch(searchTerm);


                if (googleresult.items != null)
                {
                    var pathString = Configuration["photospath"] + "\\google\\" + searchTerm;
                    Directory.CreateDirectory(pathString);
                    foreach (var item in googleresult.items)
                        try
                        {
                            var imageurl = item.link;
                            var webClient = new WebClient();
                            var uri = new Uri(imageurl);
                            var filename = uri.Segments.Last();
                            webClient.DownloadFile(imageurl, pathString + "\\" + filename);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            // throw;
                        }
                }
            }


        }

        private static async Task DownloadImagesViaBing(string searchTerm)
        {
            using (var bingImageSearchService = ServiceProvider.GetService<IBingImageSearchService>())
            {
                var bingresult = await bingImageSearchService.ImageSearch(searchTerm);

                if (bingresult.value != null)
                {
                    var pathString = Configuration["photospath"] + "\\bing\\" + searchTerm;
                    Directory.CreateDirectory(pathString);
                    foreach (var item in bingresult.value)
                        try
                        {
                            var imageurl = item.contentUrl;
                            var webClient = new WebClient();
                            var uri = new Uri(imageurl);
                            var filename = uri.Segments.Last();
                            webClient.DownloadFile(imageurl, pathString + "\\" + filename);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            // throw;
                        }
                }
            }
        }


        private static void Main()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();


            //setup our DI
            ServiceProvider = new ServiceCollection()

               .AddTransient<IGoogleImageSearchService, GoogleImageSearchService>()
               .AddTransient<IBingImageSearchService, BingImageSearchService>()
               .BuildServiceProvider();


            try
            {
                Extractdataanddownloadimages().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an exception: {ex}");
            }


            Console.Write("\nPress Enter to exit ");
            Console.ReadLine();
        }
    }
}