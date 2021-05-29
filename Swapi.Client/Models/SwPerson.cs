using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swapi.Client.Models
{
    /// <summary>
    /// Represents a character from Star Wars
    /// </summary>
    public class SwPerson
    {
        //used menu > edit > paste special > Paste JSON as Classes
        public string birth_year { get; set; }
        public string eye_color { get; set; }
        public string[] films { get; set; }
        public string gender { get; set; }
        public string hair_color { get; set; }
        public string height { get; set; }
        public string homeworld { get; set; }
        public string mass { get; set; }
        public string skin_color { get; set; }
        public DateTime created { get; set; }
        public DateTime edited { get; set; }
        public string[] species { get; set; }
        public string[] starships { get; set; }
        public string url { get; set; }
        public string[] vehicles { get; set; }
        private string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                this._name = value;
                this.formattedName = NameFormatter.ReorderSurnameFirst(value);
            }
        }

        //Holds an alternative name format, set by
        public string formattedName;

        private class NameFormatter
        {
            internal static string ReorderSurnameFirst(string name)
            {
                var nameSplit = name.Split(' ');
                int nameSplitLength = nameSplit.Length;

                //some characters will have only one name, looking at you Yoda 😃
                if (nameSplitLength == 1)
                {
                    return name;
                }
                //in case there are more than two names, make last first and the rest follow in original order
                var lastName = nameSplit.Last();
                var otherNames = String.Join("", nameSplit.Take(nameSplitLength - 1));
                return $"{lastName} {otherNames}";
            }
        }

        private SwPersonCsvPropertiesOnly swPersonCsvProps;
        public SwPersonCsvPropertiesOnly SwPersonCsvProps
        {
            get
            {
                return this.swPersonCsvProps;
            }
            set
            {
                this.swPersonCsvProps = new SwPersonCsvPropertiesOnly(this);
            }
        }
    }

    public class SwPersonCsvPropertiesOnly
    {
        public string name { get; set; }
        public string birth_year { get; set; }
        public string homeworld { get; set; }
        public string url { get; set; }

        public SwPersonCsvPropertiesOnly()
        {

        }

        public SwPersonCsvPropertiesOnly(SwPerson swPerson)
        {
            this.name = ReplaceCommeas(swPerson.formattedName);
            this.birth_year = ReplaceCommeas(swPerson.birth_year);
            this.homeworld = ReplaceCommeas(swPerson.homeworld);
            this.url = ReplaceCommeas(swPerson.url);
        }

        private string ReplaceCommeas(string inputString, char replacement = '-')
        {
            return inputString.Replace(',', replacement);
        }
    }
}
