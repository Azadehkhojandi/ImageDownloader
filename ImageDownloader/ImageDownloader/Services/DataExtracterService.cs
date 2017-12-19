using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using ImageDownloader.Models;

namespace ImageDownloader.Services
{
    public interface IDataExtracterService: IDisposable
    {
        List<string> Getinfo(string filePath);
    }

    public class DataExtracterService : IDataExtracterService
    {

        public List<string> Getinfo(string filePath)
        {
            var reader = File.OpenText(filePath);
            var csvFile = new CsvReader(reader);
            csvFile.Configuration.HasHeaderRecord = true;
            csvFile.Read();
            var records = csvFile.GetRecords<DataModel>().ToList();
            var distinctNames = records.Select(x => x.Name).Distinct();

            return distinctNames.ToList();
        }

        public void Dispose()
        {
            
        }
    }
}