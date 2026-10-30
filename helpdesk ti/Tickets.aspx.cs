using System;
using System.Linq;
using helpdesk_ti.Models;

namespace helpdesk_ti
{
    public partial class Tickets : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarTickets();
            }
        }

        private void CargarTickets()
        {
            using (var db = new HelpDeskContexto())
            {
                var tickets = from t in db.Tickets
                              join e in db.Estados on t.IdEstado equals e.IdEstado
                              join p in db.Prioridades on t.IdPrioridad equals p.IdPrioridad
                              orderby t.FechaCreacion descending
                              select new
                              {
                                  t.IdTicket,
                                  t.Titulo,
                                  Estado = e.Nombre,
                                  Prioridad = p.Nombre,
                                  PrioridadNivel = p.Nivel,
                                  t.FechaCreacion
                              };

                gvTickets.DataSource = tickets.ToList();
                gvTickets.DataBind();
            }
        }

        protected void gvTickets_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = (int)gvTickets.SelectedDataKey.Value;
            CargarDetalle(id);
        }

        private void CargarDetalle(int idTicket)
        {
            using (var db = new HelpDeskContexto())
            {
                var ticket = db.Tickets.Find(idTicket);
                if (ticket == null) return;

                var estado = db.Estados.Find(ticket.IdEstado);
                var prioridad = db.Prioridades.Find(ticket.IdPrioridad);
                var solicitante = db.Usuarios.Find(ticket.IdUsuario);

                string tecnico = "Sin asignar";
                if (ticket.IdTecnico.HasValue)
                {
                    var registroTecnico = db.Tecnicos.Find(ticket.IdTecnico.Value);
                    if (registroTecnico != null)
                    {
                        var usuarioTecnico = db.Usuarios.Find(registroTecnico.IdUsuario);
                        if (usuarioTecnico != null)
                        {
                            tecnico = usuarioTecnico.Nombre;
                        }
                    }
                }

                // Banner de prioridad: color según el nivel, texto desde la base de datos
                pnlPrioridad.CssClass = "prioridad-banner " + ClasePrioridad(prioridad.Nivel);
                lblPrioridadNombre.Text = Texto(prioridad.Nombre);
                lblPrioridadDescripcion.Text = Texto(prioridad.Descripcion);

                lblNumero.Text = ticket.IdTicket.ToString();
                lblTitulo.Text = Texto(ticket.Titulo);
                lblSolicitante.Text = Texto(solicitante != null ? solicitante.Nombre : null);
                lblTecnico.Text = Texto(tecnico);
                lblEstado.Text = Texto(estado.Nombre);
                lblFechaCreacion.Text = FormatearFecha(ticket.FechaCreacion);
                lblFechaActualizacion.Text = FormatearFecha(ticket.FechaActualizacion);
                lblFechaCierre.Text = FormatearFecha(ticket.FechaCierre);
                lblDescripcion.Text = Texto(ticket.Descripcion);
                lblSolucion.Text = Texto(ticket.Solucion);

                pnlDetalle.Visible = true;
            }
        }

        private static string ClasePrioridad(byte nivel)
        {
            switch (nivel)
            {
                case 1: return "prioridad-baja";
                case 2: return "prioridad-media";
                case 3: return "prioridad-alta";
                default: return "prioridad-critica";
            }
        }
        protected string ClaseCirculo(object nivel)
        {
            return ClasePrioridad(Convert.ToByte(nivel));
        }

        // Codifica el texto para que no se interprete como HTML.
        private string Texto(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? "—" : Server.HtmlEncode(valor);
        }

        private static string FormatearFecha(DateTime? fecha)
        {
            return fecha.HasValue ? fecha.Value.ToString("dd-MM-yyyy HH:mm") : "—";
        }

    }
}