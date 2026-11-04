<%@ Page Title="Nuevo ticket" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="NuevoTicket.aspx.cs"
    Inherits="helpdesk_ti.NuevoTicket" %>

<asp:Content ID="ContentNuevoTicket" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row justify-content-center">
        <div class="col-md-7">
            <h2 class="mt-4">Crear ticket</h2>

            <asp:Label ID="lblMensaje" runat="server"
                CssClass="d-block mb-3" />

            <asp:ValidationSummary ID="vsNuevoTicket" runat="server"
                ValidationGroup="CrearTicket"
                CssClass="alert alert-danger" />

            <div class="mb-3">
                <asp:Label runat="server" AssociatedControlID="txtTitulo"
                    CssClass="form-label" Text="Título" />
                <asp:TextBox ID="txtTitulo" runat="server" MaxLength="150"
                    CssClass="form-control" />
                <asp:RequiredFieldValidator runat="server"
                    ControlToValidate="txtTitulo" ValidationGroup="CrearTicket"
                    ErrorMessage="Escribe el título." CssClass="text-danger" />
            </div>

            <div class="mb-3">
                <asp:Label runat="server" AssociatedControlID="txtDescripcion"
                    CssClass="form-label" Text="Descripción del problema" />
                <asp:TextBox ID="txtDescripcion" runat="server"
                    TextMode="MultiLine" Rows="5" CssClass="form-control" />
                <asp:RequiredFieldValidator runat="server"
                    ControlToValidate="txtDescripcion" ValidationGroup="CrearTicket"
                    ErrorMessage="Describe el problema." CssClass="text-danger" />
            </div>

            <div class="mb-3">
                <asp:Label runat="server" AssociatedControlID="ddlPrioridad"
                    CssClass="form-label" Text="Prioridad" />
                <asp:DropDownList ID="ddlPrioridad" runat="server"
                    CssClass="form-select" />
                <asp:RequiredFieldValidator runat="server"
                    ControlToValidate="ddlPrioridad" InitialValue=""
                    ValidationGroup="CrearTicket"
                    ErrorMessage="Selecciona una prioridad." CssClass="text-danger" />
            </div>

            <asp:Button ID="btnCrear" runat="server" Text="Crear ticket"
                CssClass="btn btn-primary"
                ValidationGroup="CrearTicket"
                OnClick="btnCrear_Click" />
        </div>
    </div>
</asp:Content>
