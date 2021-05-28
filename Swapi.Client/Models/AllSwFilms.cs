using System;
using System.Collections.Generic;
using System.Text;

namespace Swapi.Client.Models
{
    /// <summary>
    /// An Object to hold list of SwFilms and metadata.
    /// </summary>
    public class AllSwFilms
    {
        //used menu > edit > paste special > Paste JSON as Classes
        public int count { get; set; }
        public object next { get; private set; }//not sure why this is here, it returns null from the server
        public object previous { get; private set; }// same ^
        public SwFilm[] results { get; set; }
    }
}
