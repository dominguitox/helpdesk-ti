using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.UI;
using helpdesk_ti.Models;

namespace helpdesk_ti
{
    public partial class AdminTecnicos : Page
    {
        private const string IdEdicionKey = "TecnicoEnEdicion";

        protected void Page_Load(object sender, EventArgs e)
        {
            string rol = Autorizacion.RefrescarRolSesion(Session);
            if (rol == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!string.Equals(rol, "Administrador", StringComparison.Ordinal))
            {
                Response.Redirect("~/Tickets.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarTecnicos();
            }
        }

        private void CargarTecnicos()
        {
            using (var db = new HelpDeskTIEntities())
            {
                var tecnicos = (
                    from tecnico in db.Tecnicos
                    join usuario in db.Usuarios
                        on tecnico.IdUsuario equals usuario.IdUsuario
                    orderby usuario.Nombre
                    select new
                    {
                        tecnico.IdTecnico,
                        usuario.Nombre,
                        usuario.Correo,
                        Activo = tecnico.Activo && usuario.Activo,
                        tecnico.FechaIngreso
                    }
                ).ToList();

                gvTecnicos.DataSource = tecnicos;
                gvTecnicos.DataBind();
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            Page.Validate("GuardarTecnico");
            if (!Page.IsValid)
            {
                return;
            }

            string nombre = txtNombre.Text.Trim();
            string correo = txtCorreo.Text.Trim();
            string contrasena = txtContrasena.Text;
            object idEdicion = ViewState[IdEdicionKey];

            try
            {
                using (var db = new HelpDeskTIEntities())
                {
                    if (idEdicion != null)
                    {
                        var tecnico = db.Tecnicos.Find(Convert.ToInt32(idEdicion));
                        if (tecnico == null)
                        {
                            MostrarMensaje("No se encontró el técnico.", false);
                            return;
                        }

                        var usuario = db.Usuarios.Find(tecnico.IdUsuario);
                        if (usuario == null)
                        {
                            MostrarMensaje("No se encontró la cuenta del técnico.", false);
                            return;
                        }

                        usuario.Nombre = nombre;
                        usuario.Correo = correo;
                        if (!string.IsNullOrWhiteSpace(contrasena))
                        {
                            usuario.ContrasenaHash = Login.Hash(contrasena);
                        }

                        db.SaveChanges();
                        MostrarMensaje("Datos del técnico actualizados.", true);
                    }
                    else
                    {
                        using (var transaccion = db.Database.BeginTransaction())
                        {
                            var usuario = new Usuarios
                            {
                                Nombre = nombre,
                                Correo = correo,
                                ContrasenaHash = Login.Hash(contrasena),
                                Activo = true,
                                FechaCreacion = DateTime.Now
                            };

                            db.Usuarios.Add(usuario);
                            db.SaveChanges();

                            var tecnico = new Tecnicos
                            {
                                IdUsuario = usuario.IdUsuario,
                                Activo = true,
                                FechaIngreso = DateTime.Now
                            };

                            db.Tecnicos.Add(tecnico);
                            db.SaveChanges();
                            transaccion.Commit();
                        }

                        MostrarMensaje("Técnico creado.", true);
                    }
                }

                LimpiarFormulario();
                CargarTecnicos();
            }
            catch (DbUpdateException)
            {
                MostrarMensaje(
                    "No se pudo guardar. Revisa que el correo no esté registrado.",
                    false);
            }
        }

        protected void gvTecnicos_RowCommand(object sender,
            System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int idTecnico;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out idTecnico))
            {
                return;
            }

            if (e.CommandName == "Editar")
            {
                EditarTecnico(idTecnico);
            }
            else if (e.CommandName == "CambiarEstado")
            {
                CambiarEstado(idTecnico);
            }
        }

        private void EditarTecnico(int idTecnico)
        {
            using (var db = new HelpDeskTIEntities())
            {
                var tecnico = db.Tecnicos.Find(idTecnico);
                if (tecnico == null)
                {
                    MostrarMensaje("No se encontró el técnico.", false);
                    return;
                }

                var usuario = db.Usuarios.Find(tecnico.IdUsuario);
                if (usuario == null)
                {
                    MostrarMensaje("No se encontró la cuenta del técnico.", false);
                    return;
                }

                ViewState[IdEdicionKey] = tecnico.IdTecnico;
                txtNombre.Text = usuario.Nombre;
                txtCorreo.Text = usuario.Correo;
                txtContrasena.Text = "";
                rfvContrasena.Enabled = false;
                btnGuardar.Text = "Guardar cambios";
                btnCancelar.Visible = true;
                MostrarMensaje("Editando los datos de " + usuario.Nombre + ".", true);
            }
        }

        private void CambiarEstado(int idTecnico)
        {
            using (var db = new HelpDeskTIEntities())
            {
                var tecnico = db.Tecnicos.Find(idTecnico);
                if (tecnico == null)
                {
                    MostrarMensaje("No se encontró el técnico.", false);
                    return;
                }

                var usuario = db.Usuarios.Find(tecnico.IdUsuario);
                if (usuario == null)
                {
                    MostrarMensaje("No se encontró la cuenta del técnico.", false);
                    return;
                }

                bool activar = !(tecnico.Activo && usuario.Activo);
                tecnico.Activo = activar;
                usuario.Activo = activar;
                db.SaveChanges();

                MostrarMensaje(
                    activar ? "Técnico reactivado." : "Técnico desactivado.",
                    true);
            }

            CargarTecnicos();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            MostrarMensaje("Edición cancelada.", true);
        }

        private void LimpiarFormulario()
        {
            ViewState.Remove(IdEdicionKey);
            txtNombre.Text = "";
            txtCorreo.Text = "";
            txtContrasena.Text = "";
            rfvContrasena.Enabled = true;
            btnGuardar.Text = "Crear técnico";
            btnCancelar.Visible = false;
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
