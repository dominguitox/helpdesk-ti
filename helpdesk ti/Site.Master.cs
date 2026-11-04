using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace helpdesk_ti
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool autenticado = Session["IdUsuario"] != null;
            string rol = autenticado
                ? Autorizacion.RefrescarRolSesion(Session)
                : null;
            autenticado = rol != null;

            string rutaInicio = "~/Login.aspx";
            if (autenticado && rol == "Usuario")
                rutaInicio = "~/NuevoTicket.aspx";
            else if (autenticado && (rol == "Tecnico" || rol == "Administrador"))
                rutaInicio = "~/Tickets.aspx";

            lnkBrand.NavigateUrl = ResolveUrl(rutaInicio);
            phClienteNav.Visible = autenticado && rol == "Usuario";
            phStaffNav.Visible = autenticado &&
                (rol == "Tecnico" || rol == "Administrador");
            phAdminNav.Visible = autenticado && rol == "Administrador";
            phAnonymousNav.Visible = !autenticado;
            phSignedInNav.Visible = autenticado;

            lblNombreUsuario.Text = autenticado
                ? Server.HtmlEncode(Convert.ToString(Session["NombreUsuario"]))
                : "";
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
