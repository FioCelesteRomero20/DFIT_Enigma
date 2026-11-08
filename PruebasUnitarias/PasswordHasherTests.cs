using Dfit.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PruebasUnitarias;

[TestClass]
[TestCategory("SeguridadHasheo")]
public class PasswordHasherTests : PruebaDocumentada
{
    private readonly IPasswordHasherService _hasher = new PasswordHasherService();

    [TestMethod]
    [TestProperty("Caso", "CP-HASH-001")]
    public void HashPassword_GeneraHashDiferenteAlTextoPlano()
    {
        var password = "DfitPower#2026";

        var hash = _hasher.HashPassword(password);

        TestContext.WriteLine($"Password original: {password}");
        TestContext.WriteLine($"Hash generado: {hash}");
        Assert.AreNotEqual(password, hash, "El hash no debe ser igual a la contraseña en texto plano.");
        Assert.IsTrue(hash.Length > 40, "El hash debe tener una longitud significativa (PBKDF2 + sal).");
    }

    [TestMethod]
    [TestProperty("Caso", "CP-HASH-002")]
    public void HashPassword_GeneraHashesDiferentesParaMismaPassword()
    {
        var password = "DfitPower#2026";

        var hash1 = _hasher.HashPassword(password);
        var hash2 = _hasher.HashPassword(password);

        TestContext.WriteLine($"Hash 1: {hash1}");
        TestContext.WriteLine($"Hash 2: {hash2}");
        Assert.AreNotEqual(hash1, hash2, "Cada invocación debe generar un hash diferente (sal aleatoria).");
    }

    [TestMethod]
    [TestProperty("Caso", "CP-HASH-003")]
    public void VerifyPassword_ConHashCorrecto_RetornaTrue()
    {
        var password = "DfitPower#2026";
        var hash = _hasher.HashPassword(password);

        var resultado = _hasher.VerifyPassword(hash, password);

        Comprobar(true, resultado);
    }

    [TestMethod]
    [TestProperty("Caso", "CP-HASH-004")]
    public void VerifyPassword_ConPasswordIncorrecto_RetornaFalse()
    {
        var password = "DfitPower#2026";
        var hash = _hasher.HashPassword(password);

        var resultado = _hasher.VerifyPassword(hash, "PasswordIncorrecto#1");

        Comprobar(false, resultado);
    }

    [TestMethod]
    [TestProperty("Caso", "CP-HASH-005")]
    public void VerifyPassword_ConTextoPlanoLegado_SoportaMigracion()
    {
        // Simula una contraseña almacenada en texto plano (usuarios legados antes del hasheo)
        var passwordLegada = "MiPasswordViejo";

        var resultado = _hasher.VerifyPassword(passwordLegada, "MiPasswordViejo");

        Comprobar(true, resultado);
    }

    [TestMethod]
    [TestProperty("Caso", "CP-HASH-006")]
    public void VerifyPassword_ConTextoPlanoLegadoIncorrecto_RetornaFalse()
    {
        var passwordLegada = "MiPasswordViejo";

        var resultado = _hasher.VerifyPassword(passwordLegada, "OtroPassword");

        Comprobar(false, resultado);
    }

    [TestMethod]
    [TestProperty("Caso", "CP-HASH-007")]
    public void IsHashed_ConHashValido_RetornaTrue()
    {
        var hash = _hasher.HashPassword("CualquierPassword#1");

        var resultado = _hasher.IsHashed(hash);

        Comprobar(true, resultado);
    }

    [TestMethod]
    [TestProperty("Caso", "CP-HASH-008")]
    public void IsHashed_ConTextoPlano_RetornaFalse()
    {
        Comprobar(false, _hasher.IsHashed("TextoPlanoCorto"));
        Comprobar(false, _hasher.IsHashed("12345678"));
        Comprobar(false, _hasher.IsHashed(""));
    }
}
