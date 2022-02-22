using System.Text.Json;
using DataTypes;

namespace Utils
{
    public static class ConfigSerialization
    {
        public static void Serialize(string path){
            var json = JsonSerializer.Serialize(Config.v, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

        public static void Deserialize(string textContent) {
            var config = JsonSerializer.Deserialize<ConfigSettings>(textContent);
            Config.v = config!;
        }
    }
}