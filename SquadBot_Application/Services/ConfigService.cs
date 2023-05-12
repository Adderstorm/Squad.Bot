using Microsoft.EntityFrameworkCore.Query;
using SquadBot_Application.Constants;
using SquadBot_Application.Logging;
using SquadBot_Application.Models;
using System;
using System.Text.Json;

namespace SquadBot_Application.Services
{
    public class ConfigService
    {
        public static void AddToken(string token)
        {
            Config? config;
            if (File.Exists(PathConstants.ConfigFile))
            {
                string configString = File.ReadAllText(PathConstants.ConfigFile);
                config = JsonSerializer.Deserialize<Config>(configString);
                if(config.Token != null)
                {
                    throw new Exception("Token already exist, please use another function for this");
                }
                config.Token = token;
            }
            else
            {
                config = new()
                {
                    Token = token
                };
            }
            SerializeConfigToFile(config);
            return;
        }

        public static void UpdateToken(string token)
        {
            
            if(!File.Exists(PathConstants.ConfigFile))
                throw new Exception("Config file doesn't exist");
            
            string configString = File.ReadAllText(PathConstants.ConfigFile);
            Config? config = JsonSerializer.Deserialize<Config>(configString);
            config.Token = token;
            SerializeConfigToFile(config);
            return;
        }
        public static void AddConfig(Config config)
        {
            if (File.Exists(PathConstants.ConfigFile))
            {
                string configString = File.ReadAllText(PathConstants.ConfigFile);
                Config? existedConfig = JsonSerializer.Deserialize<Config>(configString);
                if (existedConfig.TotalShards != null || existedConfig.DbOptions != null)
                        throw new Exception("Config file already exist, please use another function for this");
                else if (existedConfig.Token != null && config.Token != null)
                    throw new Exception("You're trying to replace existed token, please use another function for this");
                else
                    config.Token = existedConfig.Token;
            }
            SerializeConfigToFile(config);
            return;
        }
        public static void UpdateConfig(Config config)
        {
            if(!File.Exists(PathConstants.ConfigFile))
                throw new Exception("Config file doesn't exist");

            string configString = File.ReadAllText(PathConstants.ConfigFile);
            Config? existedConfig = JsonSerializer.Deserialize<Config>(configString);
            config.Token ??= existedConfig.Token;
            config.TotalShards ??= existedConfig.TotalShards;
            config.DbOptions ??= existedConfig.DbOptions;
            SerializeConfigToFile(config);
            return;
        }
        private static void SerializeConfigToFile(Config config)
        {
            using FileStream fs = new(PathConstants.ConfigFile, FileMode.OpenOrCreate);
            JsonSerializer.Serialize(fs, config);
            Logger.LogInfo("Data has been saved to file");
            return;
        }
    }
}
