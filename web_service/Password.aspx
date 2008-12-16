<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="Password.aspx.cs" Inherits="Password" Title="MoMA Studio - Change Password" %>

<asp:Content ID="ContentHeaderContent" ContentPlaceHolderID="ContentHeaderPlaceholder" Runat="Server">
Change Password
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" Runat="Server">
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
            You must be logged in to change your password!
        </AnonymousTemplate>
        <LoggedInTemplate>
            <asp:ChangePassword ID="ChangePassword1" runat="server" CancelDestinationPageUrl="~/MyAccount.aspx" ContinueDestinationPageUrl="~/MyAccount.aspx">
            </asp:ChangePassword>
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Content>

