using helpdesk_ti.Models;
using System;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

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
                                  Resuelto = e.Nombre == "Resuelto",
                                  t.FechaCreacion
                              };

                gvTickets.DataSource = tickets.ToList();
                gvTickets.DataBind();
            }
        }


        protected void gvTickets_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = (int)gvTickets.SelectedDataKey.Value;
            ViewState["IdTicket"] = id;
            CargarCombos();          // llena las listas desde la BD
            PreseleccionarCombos(id); // muestra los valores actuales del ticket
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
        private void CargarCombos()
        {
            using (var db = new HelpDeskContexto())
            {
                ddlEstado.DataSource = db.Estados.OrderBy(x => x.Nombre).ToList();
                ddlEstado.DataTextField = "Nombre";
                ddlEstado.DataValueField = "IdEstado";
                ddlEstado.DataBind();

                ddlPrioridad.DataSource = db.Prioridades.OrderBy(x => x.Nivel).ToList();
                ddlPrioridad.DataTextField = "Nombre";
                ddlPrioridad.DataValueField = "IdPrioridad";
                ddlPrioridad.DataBind();

                ddlTecnico.Items.Clear();
                ddlTecnico.Items.Add(new ListItem("Sin asignar", ""));

                var tecnicos = (from te in db.Tecnicos
                                join u in db.Usuarios on te.IdUsuario equals u.IdUsuario
                                select new { te.IdTecnico, u.Nombre }).ToList();

                foreach (var t in tecnicos)
                {
                    ddlTecnico.Items.Add(new ListItem(t.Nombre, t.IdTecnico.ToString()));
                }
            }
        }

        // Solo se llama al seleccionar un ticket, no en cada postback,
        // para no sobrescribir lo que el usuario elige antes de guardar.
        private void PreseleccionarCombos(int idTicket)
        {
            using (var db = new HelpDeskContexto())
            {
                var ticket = db.Tickets.Find(idTicket);
                ddlEstado.SelectedValue = ticket.IdEstado.ToString();
                ddlPrioridad.SelectedValue = ticket.IdPrioridad.ToString();
                ddlTecnico.SelectedValue = ticket.IdTecnico.HasValue ? ticket.IdTecnico.Value.ToString() : "";
                txtSolucion.Text = ticket.Solucion ?? "";
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            int id = (int)ViewState["IdTicket"];

            using (var db = new HelpDeskContexto())
            {
                var ticket = db.Tickets.Find(id);
                ticket.IdEstado = int.Parse(ddlEstado.SelectedValue);
                ticket.IdPrioridad = int.Parse(ddlPrioridad.SelectedValue);
                ticket.IdTecnico = string.IsNullOrEmpty(ddlTecnico.SelectedValue)
                    ? (int?)null
                    : int.Parse(ddlTecnico.SelectedValue);
                ticket.Solucion = string.IsNullOrWhiteSpace(txtSolucion.Text) ? null : txtSolucion.Text.Trim();
                ticket.FechaActualizacion = DateTime.Now;
                db.SaveChanges();
            }

            FinalizarCambio("Cambios guardados.");
        }

        protected void btnSolucionar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSolucion.Text))
            {
                MostrarMensaje("Escribe la solución antes de marcar el ticket como solucionado.", false);
                return;
            }
            CambiarEstado("Resuelto", false);
        }

        protected void btnCerrar_Click(object sender, EventArgs e)
        {
            CambiarEstado("Cerrado", true);
        }

        private void CambiarEstado(string nombreEstado, bool cerrar)
        {
            int id = (int)ViewState["IdTicket"];

            using (var db = new HelpDeskContexto())
            {
                var ticket = db.Tickets.Find(id);
                var estado = db.Estados.First(x => x.Nombre == nombreEstado);

                ticket.IdEstado = estado.IdEstado;
                ticket.Solucion = string.IsNullOrWhiteSpace(txtSolucion.Text) ? null : txtSolucion.Text.Trim();
                ticket.FechaActualizacion = DateTime.Now;
                if (cerrar) ticket.FechaCierre = DateTime.Now;

                db.SaveChanges();
            }

            FinalizarCambio("Ticket marcado como " + nombreEstado.ToLower() + ".");
        }

        private void FinalizarCambio(string mensaje)
        {
            CargarTickets();                          // refresca la lista
            CargarDetalle((int)ViewState["IdTicket"]); // refresca el detalle
            MostrarMensaje(mensaje, true);
        }

        private void MostrarMensaje(string texto, bool exito)
        {
            lblMensaje.Text = texto;
            lblMensaje.CssClass = exito ? "d-block mb-3 text-success" : "d-block mb-3 text-danger";
            lblMensaje.Visible = true;
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
        protected string ClaseBarras(object nivel, object resuelto)
        {
            string color = "color-" + ClasePrioridad(Convert.ToByte(nivel)).Replace("prioridad-", "");
            return color + (Convert.ToBoolean(resuelto) ? " resuelto" : "");
        }

        protected string BarrasHtml(object nivel)
        {
            int n = Convert.ToInt32(nivel);
            var sb = new StringBuilder();
            for (int i = 1; i <= 4; i++)
            {
                sb.Append(i <= n ? "<span class='barra activa'></span>" : "<span class='barra'></span>");
            }
            return sb.ToString();
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
        // NIGGER!  

    }
}