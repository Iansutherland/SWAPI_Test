﻿using Swapi.Client;
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
                    Dictionary<SwFilm, List<SwPersonCsvPropertiesOnly>> filmToPeopleDict = new Dictionary<SwFilm, List<SwPersonCsvPropertiesOnly>>();
                    for(int i = 0; i < evenFilms.Count; i++)
                    {
                        string[] currentPeopleUrls = peopleUrlArrays[i];
                        var peopleTasks = currentPeopleUrls.Select(url => swClient.GetSwPerson(url));
                        SwPerson[] peopleFromFilm = await Task.WhenAll(peopleTasks);

                        //sort by homeworld (planet), age (birth year)
                        var peopleFromFilmList = peopleFromFilm
                            .Select(person => person.SwPersonCsvProps)
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
                     string csvLocation = await csvWriter.WriteFile(OrderedDict);

                    Console.WriteLine("Finished all the stuff");
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
