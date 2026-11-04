<%@ Page Title="Iniciar sesión" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="helpdesk_ti.Login" %>

<asp:Content ID="ContentLogin" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row justify-content-center">
        <div class="col-md-4">
            <h2 class="mt-4">Iniciar sesión</h2>

            <asp:Label ID="lblError" runat="server" CssClass="d-block mb-3 text-danger" Visible="false" />

            <div class="mb-3">
                <label class="form-label" for="<%= txtUsuario.ClientID %>">Correo electrónico</label>
                <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" />
            </div>

            <div class="mb-3">
                <label class="form-label" for="<%= txtContrasena.ClientID %>">Contraseña</label>
                <asp:TextBox ID="txtContrasena" runat="server" TextMode="Password" CssClass="form-control" />
            </div>

            <asp:Button ID="btnIngresar" runat="server" Text="Ingresar"
                CssClass="btn btn-primary w-100" OnClick="btnIngresar_Click" />
        </div>
    </div>
</asp:Content>
