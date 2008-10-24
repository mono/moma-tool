<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="ReportView.aspx.cs" Inherits="ReportView" Title="MoMA Studio - View Report" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" runat="Server">
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
            This view is only available to logged-in users.
        </AnonymousTemplate>
        <LoggedInTemplate>
            <div class="reportview_metadata">
                <asp:UpdatePanel ID="ReportMetaDataUpdatePanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Label ID="ReportIDLabel" runat="server" Text=""></asp:Label>
                        <br />
                        <asp:Label ID="ApplicationLabel" runat="server" Text="Application:"></asp:Label>
                        <asp:TextBox ID="ApplicationContent" runat="server" AutoPostBack="True" 
                            ontextchanged="ApplicationContent_TextChanged"></asp:TextBox><br />
                        <asp:Label ID="ImportanceLabel" runat="server" Text="Importance:"></asp:Label>
                        <asp:DropDownList ID="ImportanceDropDownList" runat="server" 
                            AutoPostBack="True" 
                            onselectedindexchanged="Importance_SelectedIndexChanged">
                            <asp:ListItem>Important</asp:ListItem>
                            <asp:ListItem>Useful</asp:ListItem>
                            <asp:ListItem>Not useful</asp:ListItem>
                        </asp:DropDownList>
                        <br />
                        <asp:Label ID="AuthorLabel" runat="server" Text="Author:"></asp:Label>
                        <asp:Label ID="AuthorContent" runat="server" Text=""></asp:Label><br />
                        <asp:Label ID="EmailLabel" runat="server" Text="Email:"></asp:Label>
                        <asp:Label ID="EmailContent" runat="server" Text=""></asp:Label><br />
                        <asp:Label ID="WebsiteLabel" runat="server" Text="Website:"></asp:Label>
                        <asp:Label ID="WebsiteContent" runat="server" Text=""></asp:Label><br />
                        <asp:Label ID="FrameworkVersionLabel" runat="server" Text="Framework Version:"></asp:Label>
                        <asp:Label ID="FrameworkVersionContent" runat="server"></asp:Label><br />
                        <asp:Label ID="ApplicationTypeLabel" runat="server" Text="Application Type:"></asp:Label>
                        <asp:TextBox ID="ApplicationTypeContent" runat="server" AutoPostBack="True" 
                            ontextchanged="ApplicationType_TextChanged"></asp:TextBox><br />
                        <asp:Button ID="UpdateButton" runat="server" Text="Update" OnClick="UpdateButton_Click" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div class="reportview_comments">
                <asp:UpdatePanel ID="CommentUpdatePanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Label ID="CommentsLabel" runat="server" Text="Comments:"></asp:Label><br />
                        <asp:TextBox ID="Comments" runat="server" ReadOnly="True" TextMode="MultiLine" 
                            Columns="40" Rows="12"></asp:TextBox><br />
                        <asp:TextBox ID="NewComment" runat="server" Rows="6" TextMode="MultiLine" 
                            Columns="40"></asp:TextBox><br />
                        <asp:CheckBox ID="SendCommentCheckBox" runat="server" Checked="True" Text="Send comment to submitter" /><br />
                        <asp:Button ID="CommentButton" runat="server" Text="Add Comment" 
                            onclick="CommentButton_Click" />
                    </ContentTemplate>
                    <%--<Triggers>
                        <asp:AsyncPostBackTrigger ControlID="CommentButton" EventName="Click" />
                    </Triggers>--%>
                </asp:UpdatePanel>
            </div>
            <div class="reportview_issues">
                <asp:GridView ID="IssuesGridView" runat="server">
                </asp:GridView>
            </div>
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Content>
