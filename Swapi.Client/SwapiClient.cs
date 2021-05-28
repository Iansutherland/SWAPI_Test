using Swapi.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Swapi.Client
{
    public class SwapiClient : IDisposable
    {
        private readonly string baseUrl = @"http://swapi.dev/api/";

        private HttpClient client;

        private readonly int totalNumFilms = 6;

        private readonly List<int> evenNumbers2To6 = new List<int>{2, 4, 6};

        public SwapiClient()
        {
            //the Swapi server issues a 301 redirect and adds the trailing forward slash if you forget it
            //httpClient won't follow the redirect 'location' header, even with a handler AllowAutoRedirect = true
            HttpClientHandler httpHandler = new HttpClientHandler();
            httpHandler.AllowAutoRedirect = true;
            this.client = new HttpClient(httpHandler);
            this.client.BaseAddress = new Uri(baseUrl);
        }

        /// <summary>
        /// Gets http reponse content after ensuring success status code
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        private async Task<string> GetEndpointJsonString(string endpoint)
        {
            var response = await client.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            return content;
        }

        /// <summary>
        /// Get all Star Wars Films
        /// </summary>
        /// <returns></returns>
        public async Task<AllSwFilms> GetAllSwFilms()
        {
            var allFilmsjson = await GetEndpointJsonString("films/");
            AllSwFilms films = JsonSerializer.Deserialize<AllSwFilms>(allFilmsjson);
            return films;
        }

        /// <summary>
        /// Get Star Wars Film by Id
        /// </summary>
        /// <param name="id">Database Id of movie to fetch, NOT episode number</param>
        /// <returns></returns>
        public async Task<SwFilm> GetSwFilm(int id)
        {

            if(id < 1 || id > this.totalNumFilms)
            {
                throw new Exception($"Provided film id: ({id}) outside available range.");
            }

            SwFilm foundFilm = null;
            var endpointUrl = $"films/{id}/";
            var content = await this.GetEndpointJsonString(endpointUrl);

            if (content != null && content != "")
            {
                foundFilm = JsonSerializer.Deserialize<SwFilm>(content);
            }
            return foundFilm;
        }

        /// <summary>
        /// Return all the even number films, by ID
        /// </summary>
        /// <returns></returns>
        public async Task<List<SwFilm>> GetEvenSwFilms()
        {
            var tasks = this.evenNumbers2To6.Select(id => this.GetSwFilm(id));
            var filmArray = await Task.WhenAll(tasks);
            return filmArray.ToList();
        }

        /// <summary>
        /// Gets all Star Wars films and returns only even episode numbers
        /// </summary>
        /// <returns></returns>
        public async Task<List<SwFilm>> GetAllFilmsReturnEvenEpisodes()
        {
            var allFilms = await this.GetAllSwFilms();
            var evenFilms = allFilms.results.Where(film => this.evenNumbers2To6.Contains(film.episode_id)).ToList();
            return evenFilms;
        }

        /// <summary>
        /// Get a Star Wars Person by ID
        /// </summary>
        /// <param name="id">Swapi Person Identity</param>
        /// <returns>Person matching ID</returns>
        public async Task<SwPerson> GetSwPerson(int id)
        {
            
            var endpointUrl = $"people/{id}/";
            var content = await this.GetEndpointJsonString(endpointUrl);

            if(content != null && content != "")
            {
                SwPerson foundPerson = JsonSerializer.Deserialize<SwPerson>(content);
                return foundPerson;
            }
            else
            {
                throw new Exception($"No information Provided by {endpointUrl}");
            }
        }

        /// <summary>
        /// Get a Star Wars Person using the URL provided by other Swapi objects
        /// </summary>
        /// <param name="endpoint">full URL provided by Swapi objects</param>
        /// <returns>SwPerson from provided URL</returns>
        public async Task<SwPerson> GetSwPerson(string endpoint)
        {

            string endpointUrl;
            if(endpoint != null && endpoint != "")
            {
                endpointUrl = endpoint.Replace(baseUrl, "");
            }
            else
            {
                throw new Exception($"Url is null");
            }
            var content = await this.GetEndpointJsonString(endpointUrl);

            if (content != null && content != "")
            {
                SwPerson foundPerson = JsonSerializer.Deserialize<SwPerson>(content);
                return foundPerson;
            }
            else
            {
                throw new Exception($"No information Provided by {endpointUrl}");
            }
        }

        public void Dispose()
        {
            this.client.Dispose();
        }
    }
}
