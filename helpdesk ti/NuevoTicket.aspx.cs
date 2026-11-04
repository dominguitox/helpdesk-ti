using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using helpdesk_ti.Models;
using TicketEntity = helpdesk_ti.Models.Tickets;

namespace helpdesk_ti
{
    public partial class NuevoTicket : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["IdUsuario"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!string.Equals(
                Convert.ToString(Session["Rol"]),
                "Usuario",
                StringComparison.Ordinal))
            {
                Response.Redirect("~/Tickets.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarListas();
            }
        }

        private void CargarListas()
        {
            using (var db = new HelpDeskTIEntities())
            {
                ddlPrioridad.DataSource = db.Prioridades
                    .OrderBy(p => p.Nivel)
                    .ToList();
                ddlPrioridad.DataTextField = "Nombre";
                ddlPrioridad.DataValueField = "IdPrioridad";
                ddlPrioridad.DataBind();
                ddlPrioridad.Items.Insert(
                    0, new ListItem("Selecciona una prioridad", ""));
            }
        }

        protected void btnCrear_Click(object sender, EventArgs e)
        {
            Page.Validate("CrearTicket");
            if (!Page.IsValid)
                return;

            int idPrioridad;
            if (!int.TryParse(ddlPrioridad.SelectedValue, out idPrioridad))
            {
                MostrarMensaje("Selecciona una prioridad.", false);
                return;
            }

            int idUsuario = Convert.ToInt32(Session["IdUsuario"]);

            using (var db = new HelpDeskTIEntities())
            {
                var usuario = db.Usuarios.Find(idUsuario);
                var prioridad = db.Prioridades.Find(idPrioridad);
                var estado = db.Estados
                    .FirstOrDefault(estadoRow => estadoRow.Nombre == "Nuevo");

                if (usuario == null || !usuario.Activo ||
                    prioridad == null || estado == null)
                {
                    MostrarMensaje(
                        "No se pudo validar el usuario, la prioridad o el estado.",
                        false);
                    return;
                }

                var ticket = new TicketEntity
                {
                    Titulo = txtTitulo.Text.Trim(),
                    Descripcion = txtDescripcion.Text.Trim(),
                    IdUsuario = usuario.IdUsuario,
                    IdTecnico = null,
                    IdEstado = estado.IdEstado,
                    IdPrioridad = prioridad.IdPrioridad,
                    FechaCreacion = DateTime.Now
                };

                db.Tickets.Add(ticket);
                db.SaveChanges();

                MostrarMensaje(
                    "Ticket #" + ticket.IdTicket +
                    " creado. El equipo de soporte lo asignará a un técnico.",
                    true);

                txtTitulo.Text = "";
                txtDescripcion.Text = "";
                ddlPrioridad.SelectedIndex = 0;
            }
        }

        private void MostrarMensaje(string texto, bool exito)
        {
            lblMensaje.Text = texto;
            lblMensaje.CssClass = exito
                ? "alert alert-success d-block"
                : "alert alert-danger d-block";
        }
    }
}
