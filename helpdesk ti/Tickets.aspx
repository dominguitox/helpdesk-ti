<%@ Page Title="Tickets" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Tickets.aspx.cs" Inherits="helpdesk_ti.Tickets" %>

<asp:Content ID="ContentTickets" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .barras {
            display: inline-flex;
            gap: 3px;
            vertical-align: middle;
        }

        .barra {
            width: 8px;
            height: 14px;
            border-radius: 2px;
            background-color: #dee2e6;
        }

        .color-baja .barra.activa {
            background-color: #28a745;
        }

        .color-media .barra.activa {
            background-color: #ffc107;
        }

        .color-alta .barra.activa {
            background-color: #fd7e14;
        }

        .color-critica .barra.activa {
            background-color: #dc3545;
        }

        .barras.resuelto .barra.activa {
            background-color: #adb5bd;
        }

        .prioridad-banner {
            padding: 12px 16px;
            border-radius: 6px;
            color: #fff;
        }

        .prioridad-baja {
            background-color: #28a745;
        }

        .prioridad-media {
            background-color: #ffc107;
            color: #212529;
        }

        .prioridad-alta {
            background-color: #fd7e14;
        }

        .prioridad-critica {
            background-color: #dc3545;
        }
    </style>

    <h2>Tickets</h2>
    <asp:GridView ID="gvTickets" runat="server"
        AutoGenerateColumns="false"
        DataKeyNames="IdTicket"
        OnSelectedIndexChanged="gvTickets_SelectedIndexChanged"
        GridLines="Horizontal"
        CssClass="table table-hover">
        <Columns>

            <asp:CommandField ShowSelectButton="true" />
            <asp:TemplateField HeaderText="Prioridad">
                <ItemTemplate>
                    <span class='barras <%# ClaseBarras(Eval("PrioridadNivel"), Eval("Resuelto")) %>'
                        title='<%# Eval("Prioridad") %>'><%# BarrasHtml(Eval("PrioridadNivel")) %></span>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="IdTicket" HeaderText="Nº" />
            <asp:BoundField DataField="Titulo" HeaderText="Título" />
            <asp:BoundField DataField="Estado" HeaderText="Estado" />
            <asp:BoundField DataField="FechaCreacion" HeaderText="Creado"
                DataFormatString="{0:dd-MM-yyyy HH:mm}" />
        </Columns>
    </asp:GridView>

    <asp:Panel ID="pnlDetalle" runat="server" Visible="false">
        <hr />

        <asp:Panel ID="pnlPrioridad" runat="server" CssClass="prioridad-banner">
            <strong>Prioridad:
                <asp:Label ID="lblPrioridadNombre" runat="server" /></strong>
            <div>
                <asp:Label ID="lblPrioridadDescripcion" runat="server" />
            </div>
        </asp:Panel>
        <div class="row">
            <div class=" col-md-6">
                <h3 class="mt-3">Ticket Nº
            <asp:Label ID="lblNumero" runat="server" />
                    <span><strong>
                        <asp:Label ID="lblTitulo" runat="server" /></strong></span>
                </h3>
                <p>
                    <strong>Solicitante:</strong>
                    <asp:Label ID="lblSolicitante" runat="server" />
                </p>
                <p>
                    <strong>Técnico:</strong>
                    <asp:Label ID="lblTecnico" runat="server" />
                </p>
                <p>
                    <strong>Estado:</strong>
                    <asp:Label ID="lblEstado" runat="server" />
                </p>
                <p>
                    <strong>Creado:</strong>
                    <asp:Label ID="lblFechaCreacion" runat="server" />
                </p>
                <p>
                    <strong>Actualizado:</strong>
                    <asp:Label ID="lblFechaActualizacion" runat="server" />
                </p>
                <p>
                    <strong>Cerrado:</strong>
                    <asp:Label ID="lblFechaCierre" runat="server" />
                </p>

                <h5>Descripción</h5>
                <p>
                    <asp:Label ID="lblDescripcion" runat="server" />
                </p>

                <h5>Solución</h5>
                <p>
                    <asp:Label ID="lblSolucion" runat="server" />
                </p>
            </div>
            <div class="col-md-6">
                <h3 class="mt-3">Gestionar ticket</h3>

                <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="d-block mb-3" />

                <div class="mb-3">
                    <label class="form-label" for="<%= ddlEstado.ClientID %>">Estado</label>
                    <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select" />
                </div>

                <asp:Panel ID="pnlPrioridadEdicion" runat="server" CssClass="mb-3">
                    <label class="form-label" for="<%= ddlPrioridad.ClientID %>">Prioridad</label>
                    <asp:DropDownList ID="ddlPrioridad" runat="server" CssClass="form-select" />
                </asp:Panel>

                <asp:Panel ID="pnlAsignacion" runat="server" CssClass="mb-3">
                    <label class="form-label" for="<%= ddlTecnico.ClientID %>">Asignar a técnico</label>
                    <asp:DropDownList ID="ddlTecnico" runat="server" CssClass="form-select" />
                </asp:Panel>

                <div class="mb-3">
                    <label class="form-label" for="<%= txtSolucion.ClientID %>">Solución</label>
                    <asp:TextBox ID="txtSolucion" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" />
                </div>

                <div class="d-flex flex-wrap gap-2">
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar cambios"
                        CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                    <asp:Button ID="btnSolucionar" runat="server" Text="Marcar como solucionado"
                        CssClass="btn btn-success" OnClick="btnSolucionar_Click" />
                    <asp:Button ID="btnCerrar" runat="server" Text="Cerrar ticket"
                        CssClass="btn btn-secondary" OnClick="btnCerrar_Click" />
                </div>
            </div>
        </div>

    </asp:Panel>
</asp:Content>
