using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Pratik.DialogueSystem
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobTitle { get; set; }
    }

    public class People
    {
        [JsonProperty("People")]
        public List<Person> allPeople { get; set; }
    }

    public static class JSONFileParser
    {
        public static string filePath;
        

        public static void SetFilePath(string path)
        {
            filePath = path;
        }

        public static string ReadFile()
        {
            string text = File.ReadAllText(filePath);
            return text;
        }

        public static People ReadPeople(string text)
        {
            var people = JsonConvert.DeserializeObject<People>(text,Converter.Settings); ;
            return people;
        }


        internal static class Converter
        {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
                Converters =
                    {
                        new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
                    },
            };
        }
    }
}
