<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="Verify.aspx.cs" Inherits="Verify" Title="MoMA Studio - Verify" %>

<asp:Content ID="ContentHeaderContent" ContentPlaceHolderID="ContentHeaderPlaceholder" Runat="Server">
Verify your account
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" runat="Server">
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
            <asp:Label ID="InformationLabel" runat="server"></asp:Label>
        </AnonymousTemplate>
        <LoggedInTemplate>
            You already appear to be logged in!
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Content>

