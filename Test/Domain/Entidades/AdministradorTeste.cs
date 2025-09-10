using MinimalApi.Dominio.Entity;

namespace Test.Domain.Entidades;

[TestClass]
public sealed class AdministradorTeste
{
    [TestMethod]
    public void TestarGetSetPropriedade()
    {
        // Arrange
        var adm = new Administrador();

        // Act
        adm.Id = 1;
        adm.Email = "adm@teste.com";
        adm.Senha = "senha123";
        adm.Perfil = "Admin";

        // Assert
        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("adm@teste.com", adm.Email);
        Assert.AreEqual("senha123", adm.Senha);
        Assert.AreEqual("Admin", adm.Perfil);
    }
}
