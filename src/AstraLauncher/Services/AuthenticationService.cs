using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AstraLauncher.Models;

namespace AstraLauncher.Services;

public class AuthenticationService
{
    private readonly HttpClient _httpClient = new();
    private readonly AccountService _accountService;

    public AuthenticationService(AccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<Account> SignInWithMicrosoftAsync()
    {
        // Placeholder for Microsoft OAuth device code flow
        LoggingService.Log("Démarrage de l'authentification Microsoft");
        await Task.Delay(500);

        var fakeRefreshToken = Guid.NewGuid().ToString();
        var account = new Account
        {
            DisplayName = "Player",
            XboxGamertag = "Player123",
            EncryptedRefreshToken = AccountService.Protect(fakeRefreshToken),
            LastLogin = DateTime.UtcNow
        };

        _accountService.AddOrUpdate(account);
        return account;
    }

    public void SignOut(Account account)
    {
        LoggingService.Log($"Déconnexion de {account.DisplayName}");
    }
}
