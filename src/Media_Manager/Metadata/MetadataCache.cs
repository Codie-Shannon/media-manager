using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Media_Manager.Metadata
{
    internal sealed class MetadataCacheEnvelope
    {
        public string Provider { get; set; }
        public DateTime RetrievedAtUtc { get; set; }
        public JToken Payload { get; set; }
    }

    internal sealed class MetadataCache
    {
        private readonly string cacheDirectory;

        public MetadataCache(string dataDirectory)
        {
            cacheDirectory = Path.Combine(dataDirectory, "MetadataCache");
            Directory.CreateDirectory(cacheDirectory);
        }

        public bool TryRead<T>(
            string key,
            TimeSpan maximumAge,
            bool allowExpired,
            out T value)
        {
            value = default(T);
            string path = GetPath(key);
            if (!File.Exists(path))
            {
                return false;
            }

            try
            {
                MetadataCacheEnvelope envelope =
                    JsonConvert.DeserializeObject<MetadataCacheEnvelope>(
                        File.ReadAllText(path, Encoding.UTF8));
                if (envelope?.Payload == null)
                {
                    return false;
                }

                if (!allowExpired
                    && DateTime.UtcNow - envelope.RetrievedAtUtc > maximumAge)
                {
                    return false;
                }

                value = envelope.Payload.ToObject<T>();
                return value != null;
            }
            catch (IOException)
            {
                return false;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        public void Write<T>(string key, string provider, T value)
        {
            string path = GetPath(key);
            string temporaryPath = path + ".tmp";
            MetadataCacheEnvelope envelope = new MetadataCacheEnvelope
            {
                Provider = provider,
                RetrievedAtUtc = DateTime.UtcNow,
                Payload = JToken.FromObject(value)
            };

            try
            {
                File.WriteAllText(
                    temporaryPath,
                    JsonConvert.SerializeObject(envelope, Formatting.Indented),
                    Encoding.UTF8);
                if (File.Exists(path))
                {
                    File.Replace(temporaryPath, path, null);
                }
                else
                {
                    File.Move(temporaryPath, path);
                }
            }
            catch (IOException)
            {
                TryDelete(temporaryPath);
            }
        }

        private string GetPath(string key)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(key));
                string name = BitConverter.ToString(hash).Replace("-", string.Empty);
                return Path.Combine(cacheDirectory, name + ".json");
            }
        }

        private static void TryDelete(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (IOException)
            {
            }
        }
    }
}
