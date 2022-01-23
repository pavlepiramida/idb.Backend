using System;

namespace idb.Backend.Providers
{
    public class EnvironmentProvider : IJwtEnvironmentProvider, IDatabaseEnvironmentProvider, ISentryEnvironmentProvider
    {
        public string JwtKey => Environment.GetEnvironmentVariable(nameof(JwtKey))
            ?? throw new Exception($"Missing {nameof(JwtKey)} environment variable");

        public string JwtIssuer => Environment.GetEnvironmentVariable(nameof(JwtIssuer))
            ?? throw new Exception($"Missing {nameof(JwtIssuer)} environment variable");

        public string JwtAudience => Environment.GetEnvironmentVariable(nameof(JwtAudience))
            ?? throw new Exception($"Missing {nameof(JwtAudience)} environment variable");

        public int JwtLifeTimeMinutes => int.Parse(Environment.GetEnvironmentVariable(nameof(JwtLifeTimeMinutes))
                ?? throw new Exception($"Missing {nameof(JwtLifeTimeMinutes)} environment variable"));

        public bool JwtValidateLifeTime => bool.Parse(Environment.GetEnvironmentVariable(nameof(JwtValidateLifeTime))
            ?? throw new Exception($"Missing {nameof(JwtValidateLifeTime)} environment variable"));

        public string DatabaseConnection => Environment.GetEnvironmentVariable(nameof(DatabaseConnection))
            ?? throw new Exception($"Missing {nameof(DatabaseConnection)} environment variable");

        public string Database => Environment.GetEnvironmentVariable(nameof(Database))
            ?? throw new Exception($"Missing {nameof(Database)} environment variable");

        public string SentryDns => Environment.GetEnvironmentVariable(nameof(SentryDns)) ?? string.Empty;

        public double SentryTraceSampleRate => double.Parse(Environment.GetEnvironmentVariable(nameof(SentryTraceSampleRate)) ?? "1.0");

        public bool SentryIntegration => !string.IsNullOrEmpty(SentryDns);
    }


    public interface IJwtEnvironmentProvider
    {
        string JwtKey { get; }
        string JwtIssuer { get; }
        string JwtAudience { get; }
        int JwtLifeTimeMinutes { get; }
        bool JwtValidateLifeTime { get; }
    }
    public interface IDatabaseEnvironmentProvider
    {
        string DatabaseConnection { get; }
        string Database { get; }
    }

    public interface ISentryEnvironmentProvider
    {
        string SentryDns { get; }
        double SentryTraceSampleRate { get; }
        bool SentryIntegration { get; }
    }
}
