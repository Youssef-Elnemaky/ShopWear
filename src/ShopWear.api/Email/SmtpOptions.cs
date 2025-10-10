public sealed class SmtpOptions
{
    public string Host { get; init; } = "";
    public int Port { get; init; } = 587;
    public string User { get; init; } = "";
    public string Pass { get; init; } = "";
    public string FromName { get; init; } = "";
    public string FromEmail { get; init; } = "";
    public bool UseStartTls { get; init; } = true;
}