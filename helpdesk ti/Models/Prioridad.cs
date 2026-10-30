using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace helpdesk_ti.Models
{
    [Table("Prioridades")]
    public class Prioridad
    {
        [Key]
        public int IdPrioridad { get; set; }

        public string Nombre { get; set; }
        public byte Nivel { get; set; }
        public string Descripcion { get; set; }
    }
}