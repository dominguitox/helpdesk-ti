<%@ Page Title="Administrar técnicos" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Tecnicos.aspx.cs"
    Inherits="helpdesk_ti.AdminTecnicos" %>

<asp:Content ID="ContentTecnicos" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mt-4">Administrar técnicos</h2>
    <p class="text-muted">Crea y edita cuentas. Desactivar conserva el historial de sus tickets.</p>

    <asp:Label ID="lblMensaje" runat="server" CssClass="d-block mb-3" />
    <asp:ValidationSummary ID="vsTecnico" runat="server"
        ValidationGroup="GuardarTecnico" CssClass="alert alert-danger" />

    <div class="card mb-4">
        <div class="card-body">
            <h4 class="card-title">Datos del técnico</h4>

            <div class="mb-3">
                <asp:Label runat="server" AssociatedControlID="txtNombre"
                    CssClass="form-label" Text="Nombre" />
                <asp:TextBox ID="txtNombre" runat="server" MaxLength="100"
                    CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvNombre" runat="server"
                    ControlToValidate="txtNombre" ValidationGroup="GuardarTecnico"
                    ErrorMessage="Escribe el nombre." CssClass="text-danger" />
            </div>

            <div class="mb-3">
                <asp:Label runat="server" AssociatedControlID="txtCorreo"
                    CssClass="form-label" Text="Correo" />
                <asp:TextBox ID="txtCorreo" runat="server" MaxLength="150"
                    CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvCorreo" runat="server"
                    ControlToValidate="txtCorreo" ValidationGroup="GuardarTecnico"
                    ErrorMessage="Escribe el correo." CssClass="text-danger" />
                <asp:RegularExpressionValidator ID="revCorreo" runat="server"
                    ControlToValidate="txtCorreo" ValidationGroup="GuardarTecnico"
                    ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                    ErrorMessage="Escribe un correo válido." CssClass="text-danger" />
            </div>

            <div class="mb-3">
                <asp:Label runat="server" AssociatedControlID="txtContrasena"
                    CssClass="form-label" Text="Contraseña" />
                <asp:TextBox ID="txtContrasena" runat="server" TextMode="Password"
                    CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvContrasena" runat="server"
                    ControlToValidate="txtContrasena" ValidationGroup="GuardarTecnico"
                    ErrorMessage="Escribe una contraseña para el técnico nuevo."
                    CssClass="text-danger" />
                <small class="form-text text-muted">
                    Al editar, déjala vacía para conservar la contraseña actual.
                </small>
            </div>

            <asp:Button ID="btnGuardar" runat="server" Text="Crear técnico"
                CssClass="btn btn-primary" ValidationGroup="GuardarTecnico"
                OnClick="btnGuardar_Click" />
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar edición"
                CssClass="btn btn-outline-secondary" CausesValidation="false"
                Visible="false" OnClick="btnCancelar_Click" />
        </div>
    </div>

    <h4>Técnicos registrados</h4>
    <div class="table-responsive">
        <asp:GridView ID="gvTecnicos" runat="server" AutoGenerateColumns="false"
            CssClass="table table-hover align-middle" GridLines="Horizontal"
            OnRowCommand="gvTecnicos_RowCommand">
            <Columns>
                <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                <asp:BoundField DataField="Correo" HeaderText="Correo" />
                <asp:TemplateField HeaderText="Estado">
                    <ItemTemplate>
                        <%# Convert.ToBoolean(Eval("Activo")) ? "Activo" : "Desactivado" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="FechaIngreso" HeaderText="Ingreso"
                    DataFormatString="{0:dd-MM-yyyy}" />
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnEditar" runat="server" Text="Editar"
                            CssClass="btn btn-sm btn-outline-primary me-1"
                            CommandName="Editar" CommandArgument='<%# Eval("IdTecnico") %>'
                            CausesValidation="false" />
                        <asp:LinkButton ID="btnCambiarEstado" runat="server"
                            Text='<%# Convert.ToBoolean(Eval("Activo")) ? "Desactivar" : "Reactivar" %>'
                            CssClass="btn btn-sm btn-outline-secondary"
                            CommandName="CambiarEstado" CommandArgument='<%# Eval("IdTecnico") %>'
                            CausesValidation="false"
                            OnClientClick="return confirm('¿Cambiar el estado de este técnico?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
