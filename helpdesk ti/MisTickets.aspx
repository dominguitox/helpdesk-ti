<%@ Page Title="Mis tickets" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="MisTickets.aspx.cs"
    Inherits="helpdesk_ti.MisTickets" %>

<asp:Content ID="ContentMisTickets" ContentPlaceHolderID="MainContent" runat="server">
    <h2 class="mt-4">Mis tickets</h2>
    <p class="text-muted">Consulta el estado de las solicitudes que has creado.</p>

    <h4 class="mt-4">Resumen por estado</h4>
    <div class="table-responsive">
        <asp:GridView ID="gvResumen" runat="server" AutoGenerateColumns="false"
            CssClass="table table-sm table-striped" GridLines="Horizontal">
            <Columns>
                <asp:BoundField DataField="Estado" HeaderText="Estado" />
                <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
            </Columns>
        </asp:GridView>
    </div>

    <h4 class="mt-4">Solicitudes registradas</h4>
    <asp:Panel ID="pnlSinTickets" runat="server" Visible="false"
        CssClass="alert alert-info">
        Todavía no has creado tickets.
    </asp:Panel>

    <div class="table-responsive">
        <asp:GridView ID="gvMisTickets" runat="server" AutoGenerateColumns="false"
            CssClass="table table-hover align-middle" GridLines="Horizontal">
            <Columns>
                <asp:BoundField DataField="IdTicket" HeaderText="Nº" />
                <asp:BoundField DataField="Titulo" HeaderText="Título" />
                <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                <asp:BoundField DataField="Estado" HeaderText="Estado" />
                <asp:BoundField DataField="Prioridad" HeaderText="Prioridad" />
                <asp:BoundField DataField="FechaCreacion" HeaderText="Creado"
                    DataFormatString="{0:dd-MM-yyyy HH:mm}" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
