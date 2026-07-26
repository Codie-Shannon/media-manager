using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Media_Manager.Metadata
{
    internal sealed class ProviderSettings
    {
        public string TmdbAccessToken { get; set; }
        public string IgdbClientId { get; set; }
        public string IgdbClientSecret { get; set; }
        public string TmdbSource { get; set; }
        public string IgdbSource { get; set; }
    }

    internal sealed class ProviderSettingsDocument
    {
        public int Version { get; set; } = 1;
        public string TmdbAccessToken { get; set; }
        public string IgdbClientId { get; set; }
        public string IgdbClientSecret { get; set; }
    }

    internal sealed class ProviderSettingsStore
    {
        private const string TmdbTokenEnvironment = "MEDIA_MANAGER_TMDB_ACCESS_TOKEN";
        private const string IgdbClientIdEnvironment = "MEDIA_MANAGER_IGDB_CLIENT_ID";
        private const string IgdbClientSecretEnvironment = "MEDIA_MANAGER_IGDB_CLIENT_SECRET";
        private readonly string settingsPath;

        public ProviderSettingsStore(string dataDirectory)
        {
            settingsPath = Path.Combine(dataDirectory, "metadata-providers.json");
        }

        public ProviderSettings Load()
        {
            ProviderSettingsDocument document = ReadDocument();
            string tmdbEnvironment = Environment.GetEnvironmentVariable(TmdbTokenEnvironment);
            string igdbIdEnvironment = Environment.GetEnvironmentVariable(IgdbClientIdEnvironment);
            string igdbSecretEnvironment = Environment.GetEnvironmentVariable(IgdbClientSecretEnvironment);

            return new ProviderSettings
            {
                TmdbAccessToken = First(tmdbEnvironment, Unprotect(document.TmdbAccessToken)),
                IgdbClientId = First(igdbIdEnvironment, Unprotect(document.IgdbClientId)),
                IgdbClientSecret = First(igdbSecretEnvironment, Unprotect(document.IgdbClientSecret)),
                TmdbSource = !string.IsNullOrWhiteSpace(tmdbEnvironment) ? "environment" : "local settings",
                IgdbSource = !string.IsNullOrWhiteSpace(igdbIdEnvironment)
                    && !string.IsNullOrWhiteSpace(igdbSecretEnvironment)
                        ? "environment"
                        : "local settings"
            };
        }

        public void Save(string tmdbAccessToken, string igdbClientId, string igdbClientSecret)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
            ProviderSettingsDocument current = ReadDocument();
            if (!string.IsNullOrWhiteSpace(tmdbAccessToken))
            {
                current.TmdbAccessToken = Protect(tmdbAccessToken.Trim());
            }

            if (!string.IsNullOrWhiteSpace(igdbClientId))
            {
                current.IgdbClientId = Protect(igdbClientId.Trim());
            }

            if (!string.IsNullOrWhiteSpace(igdbClientSecret))
            {
                current.IgdbClientSecret = Protect(igdbClientSecret.Trim());
            }

            string temporaryPath = settingsPath + ".tmp";
            File.WriteAllText(
                temporaryPath,
                JsonConvert.SerializeObject(current, Formatting.Indented),
                Encoding.UTF8);
            if (File.Exists(settingsPath))
            {
                File.Replace(temporaryPath, settingsPath, null);
            }
            else
            {
                File.Move(temporaryPath, settingsPath);
            }
        }

        private ProviderSettingsDocument ReadDocument()
        {
            if (!File.Exists(settingsPath))
            {
                return new ProviderSettingsDocument();
            }

            try
            {
                return JsonConvert.DeserializeObject<ProviderSettingsDocument>(
                    File.ReadAllText(settingsPath, Encoding.UTF8))
                    ?? new ProviderSettingsDocument();
            }
            catch (JsonException)
            {
                return new ProviderSettingsDocument();
            }
            catch (CryptographicException)
            {
                return new ProviderSettingsDocument();
            }
        }

        private static string Protect(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(value);
            byte[] protectedBytes = ProtectedData.Protect(
                bytes,
                null,
                DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(protectedBytes);
        }

        private static string Unprotect(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            try
            {
                byte[] protectedBytes = Convert.FromBase64String(value);
                byte[] bytes = ProtectedData.Unprotect(
                    protectedBytes,
                    null,
                    DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (FormatException)
            {
                return string.Empty;
            }
            catch (CryptographicException)
            {
                return string.Empty;
            }
        }

        private static string First(string preferred, string fallback)
        {
            return !string.IsNullOrWhiteSpace(preferred) ? preferred.Trim() : fallback;
        }
    }
}
