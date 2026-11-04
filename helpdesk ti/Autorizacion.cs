using System.Linq;
using System.Web.SessionState;
using helpdesk_ti.Models;

namespace helpdesk_ti
{
    public static class Autorizacion
    {
        public static string ObtenerRol(HelpDeskTIEntities db, int idUsuario)
        {
            bool usuarioActivo = db.Usuarios.Any(usuario =>
                usuario.IdUsuario == idUsuario && usuario.Activo);

            if (!usuarioActivo)
                return null;

            var administrador = db.Administradores
                .Where(item => item.IdUsuario == idUsuario)
                .Select(item => new { item.Activo })
                .FirstOrDefault();

            var tecnico = db.Tecnicos
                .Where(item => item.IdUsuario == idUsuario)
                .Select(item => new { item.Activo })
                .FirstOrDefault();

            // Una cuenta no puede tener simultaneamente ambos perfiles.
            if (administrador != null && tecnico != null)
                return null;

            if (administrador != null)
                return administrador.Activo ? "Administrador" : null;

            if (tecnico != null)
                return tecnico.Activo ? "Tecnico" : null;

            // Si no tiene perfil de personal, la cuenta corresponde a un cliente.
            return "Usuario";
        }

        public static string RefrescarRolSesion(HttpSessionState session)
        {
            int idUsuario;
            if (session["IdUsuario"] == null ||
                !int.TryParse(session["IdUsuario"].ToString(), out idUsuario))
            {
                session.Clear();
                session.Abandon();
                return null;
            }

            string rol;
            using (var db = new HelpDeskTIEntities())
            {
                rol = ObtenerRol(db, idUsuario);
            }

            if (rol == null)
            {
                session.Clear();
                session.Abandon();
                return null;
            }

            session["Rol"] = rol;
            return rol;
        }
    }
}
