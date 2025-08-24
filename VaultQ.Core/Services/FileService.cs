using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VaultQ.Core.Interfaces;
using VaultQ.Core.Models;

namespace VaultQ.Core.Services
{
    internal class FileService : IFileService
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

        public Vault DeserializeVault(byte[] vaultBytes)
        {
            try
            {
                var vault = MessagePackSerializer.Deserialize<Vault>(vaultBytes);

                if (vault == null)
                    throw new InvalidOperationException();

                return vault;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to deserialize vault file", ex);
            }
        }

        public async Task<byte[]> GetVaultBytes(string vaultName)
        {
            if (!File.Exists(VaultConfigPath))
                throw new FileNotFoundException("Config file not found");

            var json = await File.ReadAllTextAsync(VaultConfigPath);
            var configFile = JsonSerializer.Deserialize<VaultConfigFile>(json) ?? throw new InvalidOperationException("Config file is null");
            var vault = configFile.Vaults?.FirstOrDefault(x => x.VaultName == vaultName) ?? throw new InvalidOperationException("Vault is null");


            if (string.IsNullOrWhiteSpace(vault.VaultName))
                throw new InvalidOperationException("Vault name is empty");

            string vaultPath = Path.Combine(VaultDirectoryPath, vault.VaultName);

            if (!File.Exists(vaultPath))
                throw new FileNotFoundException("Vault file does not exist");

            try
            {
                var vaultBytes = await File.ReadAllBytesAsync(vaultPath);
                return vaultBytes;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to load vault file", ex);
            }

        }

        public async Task<string?> GetDefaultVaultName()
        {
            if (!File.Exists(VaultConfigPath))
                throw new FileNotFoundException("Config file not found");

            try
            {
                var json = await File.ReadAllTextAsync(VaultConfigPath);
                var configFile = JsonSerializer.Deserialize<VaultConfigFile>(json);

                if (configFile == null)
                    return null;

                var defaultVault = configFile.Vaults.FirstOrDefault(x => x.DefaultVault == true);

                if (defaultVault == null || string.IsNullOrWhiteSpace(defaultVault.VaultName))
                    return null;

                return defaultVault.VaultName;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new InvalidOperationException("Access denied while getting config file.", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Failed to deserialize vault config.", ex);
            }
        }
     
        public async Task SaveSetup(byte[] serializedFile, string fileName)
        {
            try
            {
                if (!Directory.Exists(VaultDirectoryPath))
                {
                    Directory.CreateDirectory(VaultDirectoryPath);
                }

                // Saving Vault 
                string fullPath = Path.Combine(VaultDirectoryPath, fileName);
                await File.WriteAllBytesAsync(fullPath, serializedFile);

                // Saving Vault Config
                var vaultConfigFile = new VaultConfigFile();
                vaultConfigFile.Vaults.Add(new VaultConfig
                {
                    VaultName = fileName,
                    DefaultVault = true
                });

                string configJson = JsonSerializer.Serialize(vaultConfigFile, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await File.WriteAllTextAsync(VaultConfigPath, configJson);

            }
            catch (UnauthorizedAccessException ex)
            {
                throw new InvalidOperationException("Access denied while saving vault files.", ex);
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException("I/O error while saving vault files.", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Failed to serialize vault config.", ex);
            }



        }

    }
}
