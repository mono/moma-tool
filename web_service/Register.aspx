<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="Register.aspx.cs" Inherits="Register" Title="MoMA Studio - Register" %>

<asp:Content ID="ContentHeaderContent" ContentPlaceHolderID="ContentHeaderPlaceholder" runat="server">
    Register for an account
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" runat="Server">
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
            <asp:CreateUserWizard ID="CreateUserWizard1" runat="server" DisableCreatedUser="True"
                CompleteSuccessText="Your account has been successfully created, but before you can login you must first verify your email address.  A message has been sent to the email address you specified.  Please follow the instructions in that email to verify your account." 
                ContinueDestinationPageUrl="~/Overview.aspx" 
                CancelDestinationPageUrl="~/Overview.aspx" DisplayCancelButton="True" 
                FinishDestinationPageUrl="~/Overview.aspx" LoginCreatedUser="False" 
                onsendingmail="CreateUserWizard1_SendingMail">
                <WizardSteps>
                    <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
                    </asp:CreateUserWizardStep>
                    <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">
                    </asp:CompleteWizardStep>
                </WizardSteps>
                <MailDefinition BodyFileName="RegisterMail.html" IsBodyHtml="true" Subject="Activate your new account">
                </MailDefinition>
            </asp:CreateUserWizard>
        </AnonymousTemplate>
        <LoggedInTemplate>
            You already appear to be logged in!
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Content>
