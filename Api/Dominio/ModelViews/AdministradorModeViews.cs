namespace minimal_api.Dominio.ModelViews;

public record AdministradorModeViews
{
    public int Id { get; set; } = default!;
    public string? Email { get; set; } = default!;
    public string? Perfil { get; set; } = default!;
}