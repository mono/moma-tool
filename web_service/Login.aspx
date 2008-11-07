<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" Title="MoMA Studio - Login" %>

<asp:Content ID="ContentHeaderContent" ContentPlaceHolderID="ContentHeaderPlaceholder" runat="server">
    Login
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" runat="Server">
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
            <asp:Login ID="Login1" runat="server" CreateUserText="Sign up for your new account"
                CreateUserUrl="~/Register.aspx" TitleText="">
            </asp:Login>
            <asp:PasswordRecovery ID="PasswordRecovery1" runat="server">
            </asp:PasswordRecovery>
        </AnonymousTemplate>
        <LoggedInTemplate>
            <asp:ChangePassword ID="ChangePassword1" runat="server">
            </asp:ChangePassword>
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Content>
