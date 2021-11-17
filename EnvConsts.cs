using System;

namespace idb.Backend
{
    public static class EnvConsts
    {
        public static readonly string JWT_KEY = Environment.GetEnvironmentVariable(nameof(JWT_KEY)) ??
                                                throw new Exception($"Missing {nameof(JWT_KEY)} environment variable");

        public static readonly string JWT_ISSUER = Environment.GetEnvironmentVariable(nameof(JWT_ISSUER)) ??
                                                   throw new Exception(
                                                       $"Missing {nameof(JWT_ISSUER)} environment variable");

        public static readonly string JWT_AUDIENCE = Environment.GetEnvironmentVariable(nameof(JWT_AUDIENCE)) ??
                                                     throw new Exception(
                                                         $"Missing {nameof(JWT_AUDIENCE)} environment variable");

        public static readonly int JWT_LIFETIME_MINUTES =
            int.Parse(Environment.GetEnvironmentVariable(nameof(JWT_LIFETIME_MINUTES)) ??
                      throw new Exception($"Missing {nameof(JWT_LIFETIME_MINUTES)} environment variable"));

        public static readonly string DATABASE_CONNECTION =
            Environment.GetEnvironmentVariable(nameof(DATABASE_CONNECTION)) ??
            throw new Exception($"Missing {nameof(DATABASE_CONNECTION)} environment variable");

        public static readonly string DATABASE = Environment.GetEnvironmentVariable(nameof(DATABASE)) ??
                                                 throw new Exception(
                                                     $"Missing {nameof(DATABASE)} environment variable");
    }
}