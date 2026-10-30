using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace helpdesk_ti.Models
{
    [Table("Tickets")]
    public class Ticket
    {
        [Key]
        public int IdTicket { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int IdUsuario { get; set; }
        public int? IdTecnico { get; set; }
        public int IdEstado { get; set; }
        public int IdPrioridad { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string Solucion { get; set; }
    }
}