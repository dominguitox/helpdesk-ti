using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace helpdesk_ti.Models
{
    [Table("Tecnicos")]
    public class Tecnico
    {
        [Key]
        public int IdTecnico { get; set; }

        public int IdUsuario { get; set; }
        public string Especialidad { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaIngreso { get; set; }
    }
}