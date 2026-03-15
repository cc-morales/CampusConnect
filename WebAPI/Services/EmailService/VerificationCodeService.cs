using Microsoft.Extensions.Caching.Memory;

namespace WebAPI.Services.EmailService
{
    /// <summary>
    /// Manages verification codes using IMemoryCache with a 1-hour expiry.
    /// </summary>
    public class VerificationCodeService
    {
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan CodeExpiry = TimeSpan.FromHours(1);

        public VerificationCodeService(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Generates a 6-digit code, caches it keyed by email, and returns it.
        /// </summary>
        public string GenerateAndStore(string email)
        {
            var code = Random.Shared.Next(100000, 999999).ToString();
            var key = GetCacheKey(email);

            _cache.Set(key, code, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CodeExpiry
            });

            return code;
        }

        /// <summary>
        /// Validates the code for the given email. Removes the code on success.
        /// </summary>
        public bool Validate(string email, string code)
        {
            var key = GetCacheKey(email);

            if (_cache.TryGetValue<string>(key, out var cachedCode) && cachedCode == code)
            {
                _cache.Remove(key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a pending verification code exists for the email.
        /// </summary>
        public bool HasPendingCode(string email)
        {
            return _cache.TryGetValue(GetCacheKey(email), out _);
        }

        private static string GetCacheKey(string email)
            => $"verification_code:{email.ToLowerInvariant()}";
    }
}

