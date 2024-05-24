﻿using System.IO;
using XProxy.Shared.Serialization;
using XProxy.Shared.Models;
using XProxy.Shared;

namespace XProxy.Services
{
    public class ConfigService
    {
        public static ConfigService Instance { get; private set; }

        string _configPath { get; } = "./config.yml";
        string _languagesDir { get; set; } = "./Languages";
        
        string _defaultLanguagePath { get; } = "./messages_en.yml";

        public ConfigService()
        {
            Instance = this;
            
            if (!Directory.Exists(_languagesDir))
                Directory.CreateDirectory(_languagesDir);

            Load();

            Logger.Info(Messages.ProxyVersion.Replace("%version%", ProxyBuildInfo.ReleaseInfo.Version).Replace("%gameVersion%", ProxyBuildInfo.ReleaseInfo.GameVersion), "XProxy");
        }

        public ConfigModel Value { get; private set; }
        public MessagesModel Messages { get; private set; }


        public MessagesModel GetMessagesForLanguage(string language)
        {
            string path = Path.Combine(this._languagesDir, $"messages_{language}.yml");

            if (!File.Exists(path))
            {
                // Get fallback.
                CreateIfMissing();
                return YamlParser.Deserializer.Deserialize<MessagesModel>(File.ReadAllText(_defaultLanguagePath));
            }

            return YamlParser.Deserializer.Deserialize<MessagesModel>(File.ReadAllText(path));
        }

        void CreateIfMissing()
        {
            if (!File.Exists(_configPath))
                File.WriteAllText(_configPath, YamlParser.Serializer.Serialize(new ConfigModel()));

            if (!File.Exists(Path.Combine(_languagesDir, _defaultLanguagePath)))
                File.WriteAllText(Path.Combine(_languagesDir, _defaultLanguagePath), YamlParser.Serializer.Serialize(new MessagesModel()));
        }

        public void Load()
        {
            ProxyService.Singleton?.RefreshServers();
            CreateIfMissing();
            Value = YamlParser.Deserializer.Deserialize<ConfigModel>(File.ReadAllText(_configPath));
            Logger.DebugMode = Value.Debug;
            Messages = GetMessagesForLanguage(Value.Langauge);
            Save();
            Logger.Debug(Messages.ConfigLoadedMessage, "ConfigService");
        }

        public void Save()
        {
            File.WriteAllText(_configPath, YamlParser.Serializer.Serialize(Value));

            string path = Path.Combine(this._languagesDir, $"messages_{Value.Langauge}.yml");
            if (File.Exists(path))
                File.WriteAllText(path, YamlParser.Serializer.Serialize(Messages));

            Logger.Debug(Messages.ConfigSavedMessage, "ConfigService");
        }
    }
}