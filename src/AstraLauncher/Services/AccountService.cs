using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using AstraLauncher.Models;

namespace AstraLauncher.Services;

public class AccountService
{
    private readonly string _accountsPath;
    private readonly List<Account> _accounts = new();

    public IReadOnlyList<Account> Accounts => _accounts;

    public AccountService(JsonDocument configuration)
    {
        var path = configuration.RootElement.GetProperty("Launcher").GetProperty("AccountsPath").GetString();
        _accountsPath = Environment.ExpandEnvironmentVariables(path ?? "%APPDATA%/AstraLauncher/accounts.dat");
    }

    public void Load()
    {
        try
        {
            if (!File.Exists(_accountsPath))
            {
                return;
            }

            var json = File.ReadAllText(_accountsPath);
            var accounts = JsonSerializer.Deserialize<List<Account>>(json);
            if (accounts != null)
            {
                _accounts.Clear();
                _accounts.AddRange(accounts);
            }
        }
        catch (Exception ex)
        {
            LoggingService.LogException(ex);
        }
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_accountsPath)!);
            var json = JsonSerializer.Serialize(_accounts, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_accountsPath, json);
        }
        catch (Exception ex)
        {
            LoggingService.LogException(ex);
        }
    }

    public void AddOrUpdate(Account account)
    {
        var existing = _accounts.FirstOrDefault(a => a.Id == account.Id);
        if (existing == null)
        {
            _accounts.Add(account);
        }
        else
        {
            existing.DisplayName = account.DisplayName;
            existing.XboxGamertag = account.XboxGamertag;
            existing.EncryptedRefreshToken = account.EncryptedRefreshToken;
            existing.LastLogin = account.LastLogin;
        }
        Save();
    }

    public static string Protect(string data)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(data);
        var protectedBytes = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(protectedBytes);
    }

    public static string Unprotect(string encrypted)
    {
        var bytes = Convert.FromBase64String(encrypted);
        var unprotected = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
        return System.Text.Encoding.UTF8.GetString(unprotected);
    }
}
