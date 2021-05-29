using Swapi.Client;
using Swapi.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwapiCsv.ConsoleUI
{
    public class Program
    {
        static void Main(string[] args)
        {
            //synchronous Main calls an async function, that async function can use the await keyword for the async SwapiClient
            StartAsync(args).GetAwaiter().GetResult();
        }

        private static async Task StartAsync(string[] args)
        {
            using(var swClient = new SwapiClient())
            {
                try
                {
                    var evenFilms = await swClient.GetAllFilmsReturnEvenEpisodes();
                    //ToList to expose index for later looping
                    var peopleUrlArrays = evenFilms.Select(film => film.characters).ToList();
                    
                    //build dictionary to keep film connected with people from that film for sorting
                    Dictionary<SwFilm, List<SwPerson>> filmToPeopleDict = new Dictionary<SwFilm, List<SwPerson>>();
                    for(int i = 0; i < evenFilms.Count; i++)
                    {
                        string[] currentPeopleUrls = peopleUrlArrays[i];
                        var peopleTasks = currentPeopleUrls.Select(url => swClient.GetSwPerson(url));
                        SwPerson[] peopleFromFilm = await Task.WhenAll(peopleTasks);

                        //sort by homeworld (planet), age (birth year)
                        var peopleFromFilmList = peopleFromFilm
                            .OrderBy(person => person.homeworld)
                            .ThenBy(person => person.birth_year)
                            .ToList();

                        filmToPeopleDict.Add(evenFilms[i], peopleFromFilmList);
                    }

                    //sort by film ID
                    var OrderedDict = filmToPeopleDict
                        .OrderBy(pair => pair.Key.episode_id)
                        .ToDictionary(keyValPair => keyValPair.Key, keyValPair => keyValPair.Value);

                    var csvWriter = new CsvWriter();
                    Console.WriteLine("Create Headers and Data...");
                    //CsvData contains headers at index 0
                    var CsvData = csvWriter.GetCSVData(OrderedDict);

                    //var headersToRemove = new List<string> { "films", "species", "starships", "vehicles", "url" };
                    //var csvRemovedData = csvWriter.RemoveColumnsIfPresent(headersToRemove, CsvData);

                    Console.WriteLine("Write CSV file...");
                    string csvLocation  = await csvWriter.WriteCSVFile(CsvData);

                    Console.WriteLine("Finished Writing People to CSV");
                }
                catch(Exception exception)
                {
                    //use logger
                    Console.WriteLine(ExceptionTools.BundleExceptionMessages(exception));
                }

#if DEBUG
                Console.ReadLine();
#endif
            }
        }
    }
}
