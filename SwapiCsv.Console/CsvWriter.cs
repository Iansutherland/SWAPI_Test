using Microsoft.Extensions.Configuration;
using Swapi.Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwapiCsv.ConsoleUI
{
    internal class CsvWriter
    {
        public readonly string CsvFileName;
        public CsvWriter()
        {
            this.CsvFileName = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true) 
            .Build()
            .GetValue<string>("CsvFileName");

            CreateCsvFileOrClearContents();
        }

        /// <summary>
        /// Check for file and Create csv file if it doesn't exist or remove contents
        /// </summary>
        /// <returns>FileStream</returns>
        private void CreateCsvFileOrClearContents()
        {
            if (File.Exists(this.CsvFileName))
            {
                File.Delete(this.CsvFileName);
            }

            File.Create(this.CsvFileName).DisposeAsync();
        }

        public async Task<string> WriteFile(Dictionary<SwFilm, List<SwPersonCsvPropertiesOnly>> filmToPeopleDict)
        {
            Console.WriteLine("Write CSV file");
            string headerLine = CreateHeaderLine(filmToPeopleDict.Values.FirstOrDefault().FirstOrDefault().GetType());
            List<string> dataLines = CreateDataLines(headerLine, filmToPeopleDict.Values);
            dataLines = dataLines.Prepend(headerLine).ToList();
            File.WriteAllLines(this.CsvFileName, dataLines);

            return this.CsvFileName;
        }

        /// <summary>
        /// Use Reflection to create header from object property names
        /// </summary>
        /// <param name="swPersonType"></param>
        /// <returns>String of comma separated headers</returns>
        private string CreateHeaderLine(Type swPersonType)
        {
            var propNames = swPersonType.GetProperties().Select(prop => prop.Name).ToList();
            //reflection above fetches all property names, but not the formattedName which is a field
            propNames.Add(swPersonType.GetField("formattedName").Name);
            return String.Join(", ", propNames);
        }
        /// <summary>
        /// Create a data line for Csv file
        /// </summary>
        /// <param name="values"></param>
        /// <returns>CSV data line</returns>
        private List<string> CreateDataLines(string headerLine, Dictionary<SwFilm, List<SwPersonCsvPropertiesOnly>>.ValueCollection values)
        {
            //flatten to a single list and keep sorted order
            List<SwPersonCsvPropertiesOnly> peopleList = values.ToList().FlattenListOfLists<SwPersonCsvPropertiesOnly>();
            List<string> DataLines = new List<string>(); 

            foreach(SwPersonCsvPropertiesOnly person in peopleList) 
            {
                //this holds the values of the headers from the SwPerson object
                var currentHeaderValueList = new List<object>();

                //foreach property name
                foreach (string header in headerLine.Split(", "))
                {
                    //formattedName is a field, not a propterty
                    if(header != "formattedName")
                    {
                        var valueOfHeader = person.GetType().GetProperty(header).GetValue(person);
                        currentHeaderValueList.Add(valueOfHeader);
                    }
                    else
                    {
                        var valueOfHeader = person.GetType().GetField("formattedName").GetValue(person);
                        currentHeaderValueList.Add(valueOfHeader);
                    }
                }
                string currentCsvLine = String.Join(", ", currentHeaderValueList);
                DataLines.Add(currentCsvLine);
            }

            return DataLines;
        }
    }
}
