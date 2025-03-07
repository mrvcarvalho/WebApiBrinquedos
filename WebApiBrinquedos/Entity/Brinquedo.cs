namespace WebApiBrinquedos.Entity
{
    public class Brinquedo
    {
        public int Id { get; set; }                 // Identificador único
        public required string Nome { get; set; }   // Nome do brinquedo
        public required string Cor { get; set; }    // Cor do brinquedo
    }

    public class BrinquedoRequest
    {
        public int? Id { get; set; }        // Identificador único (Nullable int)
        public string? Nome { get; set; }   // Nome do brinquedo (Nullable string)
        public string? Cor { get; set; }    // Cor do brinquedo (Nullable string)
    }
}
