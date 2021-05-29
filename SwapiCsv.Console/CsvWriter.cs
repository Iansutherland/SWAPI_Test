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

        
        public List<string> GetCSVData(Dictionary<SwFilm, List<SwPerson>> filmToPeopleDict)
        {
            string headerLine = CreateHeaderLine(filmToPeopleDict.Values.FirstOrDefault().FirstOrDefault().GetType());
            List<string> dataLines = CreateDataLines(headerLine, filmToPeopleDict.Values);
            dataLines = dataLines.Prepend(headerLine).ToList();

            return dataLines;
        }

        /// <summary>
        /// Write stirng lines to file, headers should be zeroth index
        /// </summary>
        /// <param name="dataAndHeaders"></param>
        /// <returns>CSV file name</returns>
        public async Task<string> WriteCSVFile(List<string> dataAndHeaders)
        {
            await File.WriteAllLinesAsync(this.CsvFileName, dataAndHeaders);

            return this.CsvFileName;
        }

        /// <summary>
        /// Remove columns by header
        /// </summary>
        /// <param name="headersToRemove"></param>
        /// <returns>CSV file name</returns>
        public List<string> RemoveColumnsIfPresent(List<string> headersToRemove, List<string> dataLines)
        {
            var csvHeaders = dataLines[0].Split(", ").ToList();
            var newDataLines = new List<string> { dataLines[0] };
            foreach (string header in headersToRemove)
            {
                int index = csvHeaders.IndexOf(header);
                if (index == -1)
                {
                    continue;
                }
                if(index > csvHeaders.Count || index < 0)
                {
                    return dataLines;
                }
                //skip header at index 0
                foreach(var line in dataLines.Skip(1))
                {
                    var lineSplit = line.Split(',').ToList();
                    lineSplit.RemoveAt(index);
                    newDataLines.Add(String.Join(',', lineSplit));
                }
            }

            return newDataLines;
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
        private List<string> CreateDataLines(string headerLine, Dictionary<SwFilm, List<SwPerson>>.ValueCollection values)
        {
            //flatten to a single list and keep sorted order
            List<SwPerson> peopleList = values.ToList().FlattenListOfLists<SwPerson>();
            List<string> DataLines = new List<string>(); 

            foreach(SwPerson person in peopleList) 
            {
                //this holds the values of the headers from the SwPerson object
                var currentHeaderValueList = new List<object>();

                //foreach property name
                foreach (string header in headerLine.Split(","))
                {
                    var trimmedHeader = header.Trim();
                    //formattedName is a field, not a propterty
                    if(trimmedHeader != "formattedName")
                    {
                        var currentProp = person.GetType().GetProperty(trimmedHeader);
                        var valueOfHeader = currentProp.GetValue(person);
                        if(currentProp.PropertyType == typeof(string))
                        {
                            valueOfHeader = valueOfHeader.ToString().Replace(',', '-');
                        }
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
