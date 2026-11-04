using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using helpdesk_ti.Models;

namespace helpdesk_ti
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Si ya inició sesión, no tiene que volver a ver el login
            if (!IsPostBack && Session["IdUsuario"] != null)
            {
                string rol = Autorizacion.RefrescarRolSesion(Session);
                if (rol == null)
                    return;

                string ruta = rol == "Usuario"
                    ? "~/NuevoTicket.aspx"
                    : "~/Tickets.aspx";
                Response.Redirect(ruta);
            }
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            string correo = txtUsuario.Text.Trim();
            string hash = Hash(txtContrasena.Text);

            using (var db = new HelpDeskTIEntities())
            {
                var encontrado = db.Usuarios
                    .Where(u => u.Correo == correo
                                && u.ContrasenaHash == hash
                                && u.Activo)
                    .Select(u => new { u.IdUsuario, u.Nombre })
                    .FirstOrDefault();

                if (encontrado == null)
                {
                    lblError.Text = "Correo o contraseña incorrectos, o usuario inactivo.";
                    lblError.Visible = true;
                    return;
                }

                string rol = Autorizacion.ObtenerRol(db, encontrado.IdUsuario);
                if (rol == null)
                {
                    lblError.Text = "El perfil está inactivo o tiene una asignación de rol inválida.";
                    lblError.Visible = true;
                    return;
                }

                Session["IdUsuario"] = encontrado.IdUsuario;
                Session["NombreUsuario"] = encontrado.Nombre;
                Session["Rol"] = rol;
            }

            string rutaDestino = Convert.ToString(Session["Rol"]) == "Usuario"
                ? "~/NuevoTicket.aspx"
                : "~/Tickets.aspx";
            Response.Redirect(rutaDestino);
        }

        // Mismo algoritmo que usarás para guardar las contraseñas en la BD
        public static string Hash(string texto)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.Unicode.GetBytes(texto));
                return BitConverter.ToString(bytes).Replace("-", "");
            }
        }
    }
}
