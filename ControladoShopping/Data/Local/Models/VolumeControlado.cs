using SQLite;

namespace ControladoShopping.Data.Local.Models
{
    public class VolumeControlado
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; }
        public string? Sigla { get; set; }
        public long Volume { get; set; }
        public DateTime? Created { get; set; } = DateTime.Now;
        public bool Enviado { get; set; }
    }
}
