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
            if (Session["IdUsuario"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            string rol = Convert.ToString(Session["Rol"]);
            if (rol == "Usuario")
            {
                Response.Redirect("~/NuevoTicket.aspx");
                return;
            }

            if (rol != "Tecnico" && rol != "Administrador")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            pnlPrioridadEdicion.Visible = EsAdministrador;
            pnlAsignacion.Visible = EsAdministrador;

            if (!IsPostBack)
                CargarTickets();
        }

        private bool EsAdministrador
        {
            get
            {
                return string.Equals(
                    Convert.ToString(Session["Rol"]),
                    "Administrador",
                    StringComparison.Ordinal);
            }
        }

        private void CargarTickets()
        {
            using (var db = new HelpDeskTIEntities())
            {
                var tickets = from t in db.Tickets
                              join e in db.Estados on t.IdEstado equals e.IdEstado
                              join p in db.Prioridades on t.IdPrioridad equals p.IdPrioridad
                              select new
                              {
                                  t.IdTecnico,
                                  t.IdTicket,
                                  t.Titulo,
                                  Estado = e.Nombre,
                                  Prioridad = p.Nombre,
                                  PrioridadNivel = p.Nivel,
                                  Resuelto = e.Nombre == "Resuelto" || e.Nombre == "Cerrado",
                                  t.FechaCreacion
                              };

                if (Convert.ToString(Session["Rol"]) == "Tecnico")
                {
                    int idTecnico = ObtenerIdTecnicoActual(db);
                    tickets = tickets.Where(t => t.IdTecnico == idTecnico);
                }

                gvTickets.DataSource = tickets
                    .OrderByDescending(t => t.FechaCreacion)
                    .ToList();
                gvTickets.DataBind();
            }
        }


        protected void gvTickets_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id;
            if (gvTickets.SelectedDataKey == null ||
                !int.TryParse(Convert.ToString(gvTickets.SelectedDataKey.Value), out id))
            {
                ViewState.Remove("IdTicket");
                pnlDetalle.Visible = false;
                MostrarMensaje("Selecciona un ticket válido.", false);
                return;
            }

            using (var db = new HelpDeskTIEntities())
            {
                var ticket = db.Tickets.Find(id);
                if (!PuedeGestionarTicket(db, ticket))
                {
                    ViewState.Remove("IdTicket");
                    pnlDetalle.Visible = false;
                    MostrarMensaje("No tienes permiso para ver este ticket.", false);
                    return;
                }
            }

            ViewState["IdTicket"] = id;
            CargarCombos(id);
            PreseleccionarCombos(id);
            CargarDetalle(id);
        }

        private void CargarDetalle(int idTicket)
        {
            using (var db = new HelpDeskTIEntities())
            {
                var ticket = db.Tickets.Find(idTicket);
                if (!PuedeGestionarTicket(db, ticket))
                {
                    ViewState.Remove("IdTicket");
                    pnlDetalle.Visible = false;
                    return;
                }

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
                pnlPrioridad.CssClass = "prioridad-banner " +
                    ClasePrioridad(prioridad == null ? (byte)0 : prioridad.Nivel);
                lblPrioridadNombre.Text = Texto(prioridad == null ? null : prioridad.Nombre);
                lblPrioridadDescripcion.Text = Texto(prioridad == null ? null : prioridad.Descripcion);

                lblNumero.Text = ticket.IdTicket.ToString();
                lblTitulo.Text = Texto(ticket.Titulo);
                lblSolicitante.Text = Texto(solicitante != null ? solicitante.Nombre : null);
                lblTecnico.Text = Texto(tecnico);
                lblEstado.Text = Texto(estado == null ? null : estado.Nombre);
                lblFechaCreacion.Text = FormatearFecha(ticket.FechaCreacion);
                lblFechaActualizacion.Text = FormatearFecha(ticket.FechaActualizacion);
                lblFechaCierre.Text = FormatearFecha(ticket.FechaCierre);
                lblDescripcion.Text = Texto(ticket.Descripcion);
                lblSolucion.Text = Texto(ticket.Solucion);

                pnlDetalle.Visible = true;
            }
        }
        private void CargarCombos(int idTicket)
        {
            using (var db = new HelpDeskTIEntities())
            {
                ddlEstado.DataSource = db.Estados.OrderBy(x => x.Nombre).ToList();
                ddlEstado.DataTextField = "Nombre";
                ddlEstado.DataValueField = "IdEstado";
                ddlEstado.DataBind();

                if (!EsAdministrador)
                    return;

                ddlPrioridad.DataSource = db.Prioridades.OrderBy(x => x.Nivel).ToList();
                ddlPrioridad.DataTextField = "Nombre";
                ddlPrioridad.DataValueField = "IdPrioridad";
                ddlPrioridad.DataBind();

                ddlTecnico.Items.Clear();
                ddlTecnico.Items.Add(new ListItem("Sin asignar", ""));

                var tecnicos = (from te in db.Tecnicos
                                join u in db.Usuarios on te.IdUsuario equals u.IdUsuario
                                where te.Activo && u.Activo && u.Rol == "Tecnico"
                                orderby u.Nombre
                                select new { te.IdTecnico, u.Nombre }).ToList();

                foreach (var t in tecnicos)
                {
                    ddlTecnico.Items.Add(new ListItem(t.Nombre, t.IdTecnico.ToString()));
                }

                var ticket = db.Tickets.Find(idTicket);
                if (ticket != null && ticket.IdTecnico.HasValue &&
                    !tecnicos.Any(t => t.IdTecnico == ticket.IdTecnico.Value))
                {
                    var tecnicoActual = (from te in db.Tecnicos
                                         join u in db.Usuarios on te.IdUsuario equals u.IdUsuario
                                         where te.IdTecnico == ticket.IdTecnico.Value
                                         select new { te.IdTecnico, u.Nombre }).FirstOrDefault();
                    if (tecnicoActual != null)
                    {
                        ddlTecnico.Items.Add(new ListItem(
                            tecnicoActual.Nombre + " (inactivo)",
                            tecnicoActual.IdTecnico.ToString()));
                    }
                }
            }
        }

        // Solo se llama al seleccionar un ticket, no en cada postback,
        // para no sobrescribir lo que el usuario elige antes de guardar.
        private void PreseleccionarCombos(int idTicket)
        {
            using (var db = new HelpDeskTIEntities())
            {
                var ticket = db.Tickets.Find(idTicket);
                if (!PuedeGestionarTicket(db, ticket))
                {
                    ViewState.Remove("IdTicket");
                    pnlDetalle.Visible = false;
                    return;
                }

                ddlEstado.SelectedValue = ticket.IdEstado.ToString();
                if (EsAdministrador)
                {
                    ddlPrioridad.SelectedValue = ticket.IdPrioridad.ToString();
                    ddlTecnico.SelectedValue = ticket.IdTecnico.HasValue
                        ? ticket.IdTecnico.Value.ToString()
                        : "";
                }
                txtSolucion.Text = ticket.Solucion ?? "";
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            int id;
            int idEstado;
            if (!int.TryParse(Convert.ToString(ViewState["IdTicket"]), out id) ||
                !int.TryParse(ddlEstado.SelectedValue, out idEstado))
            {
                MostrarMensaje("Selecciona un ticket y un estado válidos.", false);
                return;
            }

            using (var db = new HelpDeskTIEntities())
            {
                var ticket = db.Tickets.Find(id);
                if (!PuedeGestionarTicket(db, ticket))
                {
                    ViewState.Remove("IdTicket");
                    pnlDetalle.Visible = false;
                    MostrarMensaje("No tienes permiso para modificar este ticket.", false);
                    return;
                }

                var estado = db.Estados.Find(idEstado);
                if (estado == null)
                {
                    MostrarMensaje("El estado seleccionado no existe.", false);
                    return;
                }

                ticket.IdEstado = estado.IdEstado;

                if (EsAdministrador)
                {
                    int idPrioridad;
                    if (!int.TryParse(ddlPrioridad.SelectedValue, out idPrioridad) ||
                        !db.Prioridades.Any(p => p.IdPrioridad == idPrioridad))
                    {
                        MostrarMensaje("La prioridad seleccionada no es válida.", false);
                        return;
                    }

                    ticket.IdPrioridad = idPrioridad;

                    if (string.IsNullOrWhiteSpace(ddlTecnico.SelectedValue))
                    {
                        ticket.IdTecnico = null;
                    }
                    else
                    {
                        int idTecnico;
                        if (!int.TryParse(ddlTecnico.SelectedValue, out idTecnico))
                        {
                            MostrarMensaje("El técnico seleccionado no es válido.", false);
                            return;
                        }

                        bool tecnicoActivo = (from te in db.Tecnicos
                                              join u in db.Usuarios on te.IdUsuario equals u.IdUsuario
                                              where te.IdTecnico == idTecnico && te.Activo &&
                                                  u.Activo && u.Rol == "Tecnico"
                                              select te.IdTecnico).Any();

                        // Si el ticket ya estaba asignado a un técnico desactivado, se permite
                        // conservar esa asignación mientras se modifican otros datos.
                        if (!tecnicoActivo && ticket.IdTecnico != idTecnico)
                        {
                            MostrarMensaje("Solo puedes asignar tickets a técnicos activos.", false);
                            return;
                        }

                        ticket.IdTecnico = idTecnico;
                    }
                }

                ticket.Solucion = string.IsNullOrWhiteSpace(txtSolucion.Text) ? null : txtSolucion.Text.Trim();
                ticket.FechaActualizacion = DateTime.Now;
                ticket.FechaCierre = estado.Nombre == "Cerrado"
                    ? (ticket.FechaCierre ?? DateTime.Now)
                    : (DateTime?)null;
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
            int id;
            if (!int.TryParse(Convert.ToString(ViewState["IdTicket"]), out id))
            {
                MostrarMensaje("Selecciona primero un ticket.", false);
                return;
            }

            using (var db = new HelpDeskTIEntities())
            {
                var ticket = db.Tickets.Find(id);
                if (!PuedeGestionarTicket(db, ticket))
                {
                    ViewState.Remove("IdTicket");
                    pnlDetalle.Visible = false;
                    MostrarMensaje("No tienes permiso para modificar este ticket.", false);
                    return;
                }

                var estado = db.Estados.FirstOrDefault(x => x.Nombre == nombreEstado);
                if (estado == null)
                {
                    MostrarMensaje("El estado solicitado no existe en la base de datos.", false);
                    return;
                }

                ticket.IdEstado = estado.IdEstado;
                ticket.Solucion = string.IsNullOrWhiteSpace(txtSolucion.Text) ? null : txtSolucion.Text.Trim();
                ticket.FechaActualizacion = DateTime.Now;
                ticket.FechaCierre = cerrar
                    ? (ticket.FechaCierre ?? DateTime.Now)
                    : (DateTime?)null;

                db.SaveChanges();
            }

            FinalizarCambio("Ticket marcado como " + nombreEstado.ToLower() + ".");
        }

        private void FinalizarCambio(string mensaje)
        {
            int id = Convert.ToInt32(ViewState["IdTicket"]);
            CargarTickets();
            CargarDetalle(id);
            MostrarMensaje(mensaje, true);
        }

        private int ObtenerIdTecnicoActual(HelpDeskTIEntities db)
        {
            int idUsuario = Convert.ToInt32(Session["IdUsuario"]);
            return (from te in db.Tecnicos
                    join u in db.Usuarios on te.IdUsuario equals u.IdUsuario
                    where te.IdUsuario == idUsuario && te.Activo && u.Activo && u.Rol == "Tecnico"
                    select te.IdTecnico).FirstOrDefault();
        }

        private bool PuedeGestionarTicket(
            HelpDeskTIEntities db,
            helpdesk_ti.Models.Tickets ticket)
        {
            if (ticket == null)
                return false;

            if (EsAdministrador)
                return true;

            if (!string.Equals(
                Convert.ToString(Session["Rol"]),
                "Tecnico",
                StringComparison.Ordinal))
                return false;

            int idTecnico = ObtenerIdTecnicoActual(db);
            return idTecnico > 0 && ticket.IdTecnico.HasValue &&
                ticket.IdTecnico.Value == idTecnico;
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
    }
}
