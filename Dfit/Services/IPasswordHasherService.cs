namespace Dfit.Services
{
    /// <summary>
    /// Abstracción para el hasheo y verificación de contraseñas.
    /// Cumple con OWASP A02:2021 – Fallos Criptográficos, evitando el almacenamiento de contraseñas en texto plano.
    /// Utiliza PBKDF2 con sal aleatoria a través de ASP.NET Core Identity PasswordHasher.
    /// </summary>
    public interface IPasswordHasherService
    {
        /// <summary>
        /// Genera un hash seguro (PBKDF2 + sal aleatoria) a partir de una contraseña en texto plano.
        /// </summary>
        string HashPassword(string password);

        /// <summary>
        /// Verifica si una contraseña en texto plano coincide con un hash almacenado.
        /// También soporta migración transparente desde contraseñas almacenadas en texto plano.
        /// </summary>
        /// <param name="hashedPassword">El hash almacenado en la base de datos.</param>
        /// <param name="providedPassword">La contraseña proporcionada por el usuario.</param>
        /// <returns>True si la contraseña es válida, false en caso contrario.</returns>
        bool VerifyPassword(string hashedPassword, string providedPassword);

        /// <summary>
        /// Determina si un valor almacenado en el campo PasswordHash es un hash real o texto plano legado.
        /// Permite la migración progresiva de contraseñas antiguas al formato hasheado.
        /// </summary>
        bool IsHashed(string storedValue);
    }
}
