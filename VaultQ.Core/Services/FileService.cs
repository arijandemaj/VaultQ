using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VaultQ.Core.Models;

namespace VaultQ.Core.Services
{
    internal class FileService
    {
        private string AppDataPath 
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }
        }

        private string VaultDirectoryPath
        {
            get
            {
                return Path.Combine(AppDataPath, "VaultQ");
            }
        }

        private string VaultConfigPath
        {
            get
            {
                return Path.Combine(VaultDirectoryPath, "config.json");
            }
        }

        public byte[] SerializeVault(Vault vault)
        {
            try
            {
                var bytes = MessagePackSerializer.Serialize(vault);
                return bytes;
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException("Failed to serialize vault", ex);
            }
    
        }

        public Vault LoadDefaultVault()
        {
            if (!File.Exists(VaultConfigPath))
                throw new FileNotFoundException("Config file not found");


            string? defaultVaultName;

            try
            {
                string configJson = File.ReadAllText(VaultConfigPath);

                using (var config = JsonDocument.Parse(configJson))
                {
                    var defaultVaultProperty = config.RootElement.GetProperty("DefaultVault");

                    if(defaultVaultProperty.ValueKind == JsonValueKind.Null || defaultVaultProperty.ValueKind == JsonValueKind.String)
                    {
                        throw new InvalidOperationException("Invalid or missing DefaultVault in config.json");
                    }

                    defaultVaultName = defaultVaultProperty.GetString();

                }

            }
            catch(Exception ex)
            {
                throw new InvalidOperationException("Failed to read config file", ex);
            }

            if (string.IsNullOrEmpty(defaultVaultName))
                throw new InvalidOperationException("Default vault name is empty");

            string defaultVaultPath = Path.Combine(VaultDirectoryPath, defaultVaultName);

            if (!File.Exists(defaultVaultPath))
                throw new FileNotFoundException($"Vault file not found at {defaultVaultPath}");

            try
            {
                var vaultBytes = File.ReadAllBytes(defaultVaultPath);
                var vault = MessagePackSerializer.Deserialize<Vault>(vaultBytes);

                if (vault == null)
                    throw new InvalidOperationException("Failed to deserialize vault");

                return vault;
            }
            catch (Exception ex) 
            {
                throw new InvalidOperationException("Failed to load vault file", ex);
            }
         
        }

        public bool SaveToFile(byte[] serializedFile, string fileName)
        {
            // TODO: Catch Errors
            if (!Directory.Exists(VaultDirectoryPath))
            {
                Directory.CreateDirectory(VaultDirectoryPath);
            }

            var configObject = new
            {
                DefaultVaultName = fileName
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string configJson = JsonSerializer.Serialize(configObject, options);
            File.WriteAllText(VaultConfigPath, configJson);

            string fullPath = Path.Combine(VaultDirectoryPath, fileName);
            File.WriteAllBytes(fullPath, serializedFile);

            return true;
        }

    }
}
