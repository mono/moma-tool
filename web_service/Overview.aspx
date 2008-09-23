<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="Overview.aspx.cs" Inherits="Overview" Title="MoMA Studio - Overview" %>
<%@ Register TagPrefix="zgw" Namespace="ZedGraph.Web" Assembly="ZedGraph.Web" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" runat="server">
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
            This view is only available to logged-in users.
        </AnonymousTemplate>
        <LoggedInTemplate>
            <asp:Label ID="Label1" runat="server" Text="Latest 20 Reports:"></asp:Label>
            <br />
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False">
                <Columns>
                    <asp:HyperLinkField DataNavigateUrlFields="ID" DataNavigateUrlFormatString="~/ReportView.aspx?ReportID={0}"
                        HeaderText="Details" Text="View" />
                    <asp:BoundField DataField="CreateDate" HeaderText="Date" />
                    <asp:BoundField DataField="ID" HeaderText="ID" />
                    <asp:BoundField DataField="ReporterName" HeaderText="Name" />
                    <asp:BoundField DataField="Profile" HeaderText="Profile" />
                    <asp:BoundField DataField="Miss" HeaderText="MISS" />
                    <asp:BoundField DataField="Niex" HeaderText="NIEX" />
                    <asp:BoundField DataField="Pinv" HeaderText="PINV" />
                    <asp:BoundField DataField="Todo" HeaderText="TODO" />
                    <asp:BoundField DataField="Total" HeaderText="Total" />
                </Columns>
            </asp:GridView>
            <asp:Label ID="Label2" runat="server" Text="Most needed API:"></asp:Label>
            <br />
            <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False">
                <Columns>
                    <asp:BoundField DataField="Namespace" HeaderText="Namespace" />
                    <asp:BoundField DataField="Class" HeaderText="Class" />
                    <asp:BoundField DataField="Method" HeaderText="Method" />
                    <asp:BoundField DataField="Type" HeaderText="Type" />
                    <asp:BoundField DataField="Apps" HeaderText="Apps" />
                </Columns>
            </asp:GridView>
            <asp:Label ID="Label3" runat="server" Text="Issues per Application"></asp:Label>
            <br />
            <zgw:zedgraphweb ID="ZedGraph1" runat="server"></zgw:zedgraphweb>
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Content>
