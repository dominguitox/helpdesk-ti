using System.Data.Entity;

namespace helpdesk_ti.Models
{
    public class HelpDeskContexto : DbContext
    {
        static HelpDeskContexto()
        {
            Database.SetInitializer<HelpDeskContexto>(null);
        }

        public HelpDeskContexto() : base("name=HelpDeskContexto") { }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Prioridad> Prioridades { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Tecnico> Tecnicos { get; set; }
    }
}