<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="Overview.aspx.cs" Inherits="Overview" Title="MoMA Studio - Overview" %>

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
                    <asp:HyperLinkField DataNavigateUrlFields="ID" 
                        DataNavigateUrlFormatString="~/ReportView.aspx?ReportID={0}" 
                        HeaderText="Details" Text="View" />
                    <asp:BoundField DataField="ID" HeaderText="ID" />
                    <asp:BoundField DataField="ReporterName" HeaderText="Name" />
                    <asp:BoundField DataField="CreateDate" HeaderText="Date" />
                </Columns>
            </asp:GridView>
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Content>
