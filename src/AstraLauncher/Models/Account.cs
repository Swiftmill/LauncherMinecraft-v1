using System;

namespace AstraLauncher.Models;

public class Account
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DisplayName { get; set; } = string.Empty;
    public string XboxGamertag { get; set; } = string.Empty;
    public string EncryptedRefreshToken { get; set; } = string.Empty;
    public DateTime LastLogin { get; set; } = DateTime.UtcNow;
}
