using System;
using System.Linq;
using System.Web.UI;
using helpdesk_ti.Models;

namespace helpdesk_ti
{
    public partial class MisTickets : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string rol = Autorizacion.RefrescarRolSesion(Session);
            if (rol == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!string.Equals(rol, "Usuario", StringComparison.Ordinal))
            {
                Response.Redirect("~/Tickets.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarMisTickets();
            }
        }

        private void CargarMisTickets()
        {
            int idUsuario = Convert.ToInt32(Session["IdUsuario"]);

            using (var db = new HelpDeskTIEntities())
            {
                var misTickets = db.Tickets
                    .Where(ticket => ticket.IdUsuario == idUsuario)
                    .OrderByDescending(ticket => ticket.FechaCreacion)
                    .Select(ticket => new
                    {
                        ticket.IdTicket,
                        ticket.Titulo,
                        ticket.Descripcion,
                        Estado = ticket.Estados.Nombre,
                        Prioridad = ticket.Prioridades.Nombre,
                        ticket.FechaCreacion
                    })
                    .ToList();

                var resumen = db.Tickets
                    .Where(ticket => ticket.IdUsuario == idUsuario)
                    .GroupBy(ticket => ticket.Estados.Nombre)
                    .Select(grupo => new
                    {
                        Estado = grupo.Key,
                        Cantidad = grupo.Count()
                    })
                    .OrderBy(item => item.Estado)
                    .ToList();

                gvMisTickets.DataSource = misTickets;
                gvMisTickets.DataBind();

                gvResumen.DataSource = resumen;
                gvResumen.DataBind();

                pnlSinTickets.Visible = misTickets.Count == 0;
            }
        }
    }
}
