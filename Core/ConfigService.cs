using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LangGuard.Core
{
    public sealed class ConfigService
    {
        private const string FolderName = "LangGuard";
        private const string FileName = "config.json";

        public string ConfigPath { get; }

        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public ConfigService()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, FolderName);
            Directory.CreateDirectory(folder);
            ConfigPath = Path.Combine(folder, FileName);
        }

        public AppConfig Load()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    var cfg = new AppConfig();
                    Save(cfg);
                    return cfg;
                }

                string json = File.ReadAllText(ConfigPath);
                return JsonSerializer.Deserialize<AppConfig>(json, Options) ?? new AppConfig();
            }
            catch
            {
                return new AppConfig();
            }
        }

        public void Save(AppConfig cfg)
        {
            File.WriteAllText(ConfigPath, JsonSerializer.Serialize(cfg, Options));
        }
    }
}
