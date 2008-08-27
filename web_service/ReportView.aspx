<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="ReportView.aspx.cs" Inherits="ReportView" Title="MoMA Studio - View Report" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" runat="Server">
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
            This view is only available to logged-in users.
        </AnonymousTemplate>
        <LoggedInTemplate>
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False">
                <Columns>
                    <asp:BoundField DataField="ID" HeaderText="ID" />
                    <asp:BoundField DataField="ReportFilename" HeaderText="Filename" />
                    <asp:BoundField DataField="ReporterName" HeaderText="Name" />
                </Columns>
            </asp:GridView>
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Content>
