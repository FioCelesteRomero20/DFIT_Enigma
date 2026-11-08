using Microsoft.AspNetCore.Identity;

namespace Dfit.Services
{
    /// <summary>
    /// Implementación del servicio de hasheo de contraseñas usando el PasswordHasher&lt;T&gt; de ASP.NET Core Identity.
    /// Algoritmo: PBKDF2 con HMAC-SHA256/512, sal aleatoria de 128 bits, 100,000+ iteraciones.
    /// 
    /// Soporta migración transparente: si el valor almacenado no tiene formato de hash válido,
    /// lo trata como texto plano legado y realiza comparación directa, permitiendo rehashear en el login.
    /// </summary>
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly PasswordHasher<object> _hasher = new();

        // Los hashes generados por ASP.NET Core PasswordHasher siempre inician con "AQ" en Base64
        // porque el primer byte codifica la versión del formato (0x00 = V2, 0x01 = V3).
        private const string HashPrefix = "AQ";

        /// <inheritdoc />
        public string HashPassword(string password)
        {
            return _hasher.HashPassword(null!, password);
        }

        /// <inheritdoc />
        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            // Migración transparente: si el valor almacenado es texto plano legado,
            // comparar directamente (el flujo de login se encargará de rehashear).
            if (!IsHashed(hashedPassword))
            {
                return hashedPassword == providedPassword;
            }

            var result = _hasher.VerifyHashedPassword(null!, hashedPassword, providedPassword);
            return result != PasswordVerificationResult.Failed;
        }

        /// <inheritdoc />
        public bool IsHashed(string storedValue)
        {
            // Un hash válido de ASP.NET Core Identity tiene al menos 40 caracteres en Base64
            // y comienza con el prefijo de versión del algoritmo.
            if (string.IsNullOrEmpty(storedValue) || storedValue.Length < 40)
            {
                return false;
            }

            return storedValue.StartsWith(HashPrefix, StringComparison.Ordinal);
        }
    }
}
