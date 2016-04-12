using System;
using System.Collections.Generic;
using System.IO;

namespace BDShared.Util
{
    public static class Config
    {

        private static Dictionary<string, object> configurations;
        private static string configurationFile;

        static Config()
        {
            configurations = new Dictionary<string, object>();
        }

        public static int Count
        {
            get
            {
                return configurations.Count;
            }
        }

        public static void LoadConfiguration(string filename)
        {
            if(!File.Exists(filename))
                return;

            configurationFile = filename;

            string[] lines = File.ReadAllLines(filename);
            foreach(var line in lines)
            {
                if(string.IsNullOrWhiteSpace(line))
                    continue;

                if(!line.Contains("="))
                    throw new InvalidDataException();

                string key = line.Split('=')[0];
                string value = null;
                for(int i = 1; i < line.Split('=').Length; i++)
                    value += line.Split('=')[i];

                configurations.Add(key, value);
            }
        }

        public static void SaveConfiguration(string filename = null)
        {
            if(filename == null)
                filename = configurationFile != null ? configurationFile : "config.cfg";

            using(var writer = new StreamWriter(filename))
            {
                foreach(var line in configurations)
                {
                    writer.WriteLine(string.Format("{0}={1}", line.Key, line.Value));
                }
            }
        }

        public static T GetValue<T>(string key)
        {
            if(!ContainsKey(key))
                throw new KeyNotFoundException();

            return (T)Convert.ChangeType(configurations[key], typeof(T));
        }

        public static void AddValue<T>(string key, T value)
        {
            if(ContainsKey(key))
                throw new ArgumentException("Key already exists in this configuration.");

            configurations.Add(key, value);
        }

        public static void SetValue<T>(string key, T value)
        {
            if(!ContainsKey(key))
                throw new KeyNotFoundException();

            configurations[key] = value;
        }

        public static bool ContainsKey(string key)
        {
            return configurations.ContainsKey(key);
        }

    }
}