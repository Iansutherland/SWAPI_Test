using Microsoft.Extensions.Configuration;
using Swapi.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SwapiCsv.ConsoleUI
{
    internal class CsvWriter : IDisposable
    {
        public readonly string CsvFileName;
        private FileStream _fileStream;
        public CsvWriter()
        {
            this.CsvFileName = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true)
            .Build()
            .GetValue<string>("CsvFileName");

            this._fileStream = CreateCsvFile();
        }

        /// <summary>
        /// Create csv file if it doesn't exist
        /// </summary>
        /// <returns>FileStream</returns>
        private FileStream CreateCsvFile()
        {
            FileStream fileStream = null;
            if (!File.Exists(this.CsvFileName))
            {
                fileStream = File.Create(this.CsvFileName);
            }

            return fileStream;
        }

        public string WriteFile(Dictionary<SwFilm, List<SwPerson>> filmToPeopleDict)
        {
            string headerLine = CreateHeaderLine(filmToPeopleDict.Values.FirstOrDefault().FirstOrDefault().GetType());
            List<string> dataLines = CreateDataLines(filmToPeopleDict.Values);

            return this.CsvFileName;
        }

        /// <summary>
        /// Use Reflection to create header from object property names with newline
        /// </summary>
        /// <param name="swPersonType"></param>
        /// <returns>String of comma separated headers</returns>
        private string CreateHeaderLine(Type swPersonType)
        {
            var propNames = swPersonType.GetProperties().Select(prop => prop.Name);
            return String.Join(", ", propNames) + Environment.NewLine;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private List<string> CreateDataLines(Dictionary<SwFilm, List<SwPerson>>.ValueCollection values)
        {
            Console.WriteLine("wut");

            return new List<string>();
        }

        public void Dispose()
        {
            this._fileStream.DisposeAsync();
        }
    }
}
