using System;
using Google.Cloud.Firestore;
using Dfit.Services;

namespace Dfit.Models.Patterns.Builder
{
    [FirestoreData]
    public class UsuarioBuilder
    {
        private Usuario _usuario = new Usuario();
        private IPasswordHasherService? _passwordHasher;

        /// <summary>
        /// Inyecta el servicio de hasheo para que SetPassword genere un hash seguro.
        /// Si no se proporciona, se usa una instancia por defecto.
        /// </summary>
        public UsuarioBuilder WithPasswordHasher(IPasswordHasherService passwordHasher)
        {
            _passwordHasher = passwordHasher;
            return this;
        }

        public UsuarioBuilder SetDatosBasicos(string nombres, string apellidos, string correo)
        {
            _usuario.Nombres = nombres;
            _usuario.Apellidos = apellidos;
            _usuario.Correo = correo;
            return this;
        }

        public UsuarioBuilder SetDatosIdentidad(string dni, string celular)
        {
            _usuario.DNI = dni;
            _usuario.Celular = celular;
            return this;
        }

        public UsuarioBuilder SetPassword(string password)
        {
            var hasher = _passwordHasher ?? new PasswordHasherService();
            _usuario.PasswordHash = hasher.HashPassword(password);
            return this;
        }

        public UsuarioBuilder SetRol(string rol)
        {
            _usuario.Rol = rol;
            return this;
        }

        public UsuarioBuilder SetDefaults()
        {
            _usuario.FechaRegistro = DateTime.Now;
            _usuario.Activo = true;
            return this;
        }

        public Usuario Build()
        {
            var result = _usuario;
            _usuario = new Usuario(); // Reset para uso futuro
            return result;
        }
    }
}
