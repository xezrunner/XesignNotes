using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace XesignNotes.App.Configuration
{
    public class ConfigurationManager
    {
        /// <summary>
        /// AppDirectory\Configuration\...
        /// </summary>
        public string GetPathForConfigurationFile(string file)
        {
            // Configuration directory path
            string configDir = AppDomain.CurrentDomain.BaseDirectory + @"Configuration\";

            // If the configuration directory doesn't exist, create it.
            if (!Directory.Exists(configDir))
                Directory.CreateDirectory(configDir);

            return configDir + file;
        }

        public object User_GetValue(string Property)
        {
            foreach (Configuration config in GetUserConfiguration())
            {
                if (config.Property == Property)
                {
                    if (config.Value == null & config.DefaultValue != null)
                        config.Value = config.DefaultValue;

                    return config.Value;
                }
            }

            return null;
        }

        public object App_GetValue(string Property)
        {
            foreach (Configuration config in GetApplicationConfiguration())
            {
                if (config.Value == null & config.DefaultValue != null)
                    config.Value = config.DefaultValue;

                return config.Value;
            }

            return null;
        }

        public object Debug_GetValue(string Property)
        {
            foreach (Configuration config in GetDebugConfiguration())
            {
                if (config.Value == null & config.DefaultValue != null)
                    config.Value = config.DefaultValue;

                return config.Value;
            }

            return null;
        }

        /// <summary>
        /// Check whether the configuration files exist, and if not, creates them using default values.
        /// </summary>
        public void CheckConfiguration()
        {
            if (!File.Exists(GetPathForConfigurationFile("UserConfiguration.json")))
                CreateDefaultUserConfiguration();
            if (!File.Exists(GetPathForConfigurationFile("ApplicationConfiguration.json")))
                CreateDefaultApplicationConfiguration();
            if (!File.Exists(GetPathForConfigurationFile("DebugConfiguration.json")))
                CreateDefaultDebugConfiguration();
        }

        public List<Configuration> GetUserConfiguration()
        {
            string file = File.ReadAllText(GetPathForConfigurationFile("UserConfiguration.json"));
            return JsonConvert.DeserializeObject<List<Configuration>>(file);
        }

        public List<Configuration> GetApplicationConfiguration()
        {
            string file = File.ReadAllText(GetPathForConfigurationFile("ApplicationConfiguration.json"));
            return JsonConvert.DeserializeObject<List<Configuration>>(file);
        }

        public List<Configuration> GetDebugConfiguration()
        {
            string file = File.ReadAllText(GetPathForConfigurationFile("DebugConfiguration.json"));
            return JsonConvert.DeserializeObject<List<Configuration>>(file);
        }

        public void ChangeUserConfiguration(string Property, string Value)
        {
            // TODO: change JSON
        }

        public void CreateDefaultUserConfiguration()
        {
            using (StreamWriter file = File.CreateText(GetPathForConfigurationFile("UserConfiguration.json")))
            {
                JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };
                serializer.Serialize(file, new DefaultUserConfiguration().CreateDefaultUserConfigurationList());
            }
        }

        public void CreateDefaultApplicationConfiguration()
        {
            using (StreamWriter file = File.CreateText(GetPathForConfigurationFile("ApplicationConfiguration.json")))
            {
                JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };
                serializer.Serialize(file, new DefaultApplicationConfiguration().CreateDefaultApplicationConfigurationList());
            }
        }

        public void CreateDefaultDebugConfiguration()
        {
            using (StreamWriter file = File.CreateText(GetPathForConfigurationFile("DebugConfiguration.json")))
            {
                JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };
                serializer.Serialize(file, new DefaultDebugConfiguration().CreateDefaultDebugConfigurationList());
            }
        }

        public void ClearConfiguration()
        {
            File.Delete(GetPathForConfigurationFile("UserConfiguration.json"));
            File.Delete(GetPathForConfigurationFile("ApplicationConfiguration.json"));
            File.Delete(GetPathForConfigurationFile("DebugConfiguration.json"));

            CreateDefaultUserConfiguration();
            CreateDefaultApplicationConfiguration();
            CreateDefaultDebugConfiguration();
        }
    }
}
