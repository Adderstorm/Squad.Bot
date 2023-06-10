using Microsoft.EntityFrameworkCore.Query;
using Squad.Bot.Constants;
using Squad.Bot.Logging;
using Squad.Bot.Models;
using System;
using System.Text.Json;

namespace Squad.Bot.Services
{
    public class ConfigService : Config
    {
        public static Config AddToken(string token)
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
            SerializeConfigToFile(config, TypeDataSerialize.Token);
            return config;
        }

        public static Config UpdateToken(string token)
        {
            
            if(!File.Exists(PathConstants.ConfigFile))
                throw new Exception("Config file doesn't exist");
            
            string configString = File.ReadAllText(PathConstants.ConfigFile);
            Config? config = JsonSerializer.Deserialize<Config>(configString);
            config.Token = token;
            SerializeConfigToFile(config, TypeDataSerialize.Token);
            return config;
        }
        public static Config AddConfig(Config config)
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
            SerializeConfigToFile(config, TypeDataSerialize.Config);
            return config;
        }
        public static Config UpdateConfig(Config config)
        {
            if(!File.Exists(PathConstants.ConfigFile))
                throw new Exception("Config file doesn't exist");

            string configString = File.ReadAllText(PathConstants.ConfigFile);
            Config? existedConfig = JsonSerializer.Deserialize<Config>(configString);
            config.Token ??= existedConfig.Token;
            config.TotalShards ??= existedConfig.TotalShards;
            config.DbOptions ??= existedConfig.DbOptions;
            SerializeConfigToFile(config, TypeDataSerialize.Config);
            return config;
        }

        public static Config? GetConfig()
        {
            if (!File.Exists(PathConstants.ConfigFile))
                throw new Exception("Config file doesn't exist");

            string ConfigString = File.ReadAllText(PathConstants.ConfigFile);
            return JsonSerializer.Deserialize<Config>(ConfigString);
        }

        private enum TypeDataSerialize
        {
            Config,
            Token
        }

        private static void SerializeConfigToFile(Config config, TypeDataSerialize type)
        {
            using FileStream fs = new(PathConstants.ConfigFile, FileMode.OpenOrCreate);
            JsonSerializer.Serialize(fs, config);

            switch (type)
            {
                case TypeDataSerialize.Config:
                    Logger.LogInfo("Config has been saved to file");
                    break;
                case TypeDataSerialize.Token:
                    Logger.LogInfo("Token has been saved to file");
                    break;
            }
            return;
        }
    }
}
