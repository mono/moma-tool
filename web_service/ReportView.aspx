﻿<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="ReportView.aspx.cs" Inherits="ReportView" Title="MoMA Studio - View Report" %>

<asp:Content ID="ContentHeaderContent" ContentPlaceHolderID="ContentHeaderPlaceholder" runat="server">
    See report details
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" runat="Server">
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
            This view is only available to logged-in users.
        </AnonymousTemplate>
        <LoggedInTemplate>
            <div class="reportview_metadata">
                <asp:UpdatePanel ID="ReportMetaDataUpdatePanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:SqlDataSource ID="MetadataSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
                            ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT id FROM report_metadata WHERE report_id = @id;">
                            <SelectParameters>
                                <asp:QueryStringParameter DefaultValue="1" Name="id" QueryStringField="ReportID"
                                    Type="Int32" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                        <asp:SqlDataSource ID="ReportWithMetadataSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
                            ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT rep.id, meta.importance, meta.application_name, rep.reporter_name, rep.reporter_email, rep.reporter_homepage, def.display_name, rep.application_type FROM report rep, report_metadata meta, moma_definition def WHERE meta.report_id = rep.id AND rep.moma_definition_id = def.id AND rep.id = @id;"
                            UpdateCommand="UPDATE report_metadata SET application_name = @application_name, importance = @importance, application_type = @application_type WHERE report_id = @id; UPDATE report SET application_type = @application_type WHERE id = @id;">
                            <SelectParameters>
                                <asp:QueryStringParameter DefaultValue="1" Name="id" QueryStringField="ReportID"
                                    Type="Int32" />
                            </SelectParameters>
                            <UpdateParameters>
                                <asp:QueryStringParameter DefaultValue="1" Name="id" QueryStringField="ReportID"
                                    Type="Int32" />
                                <asp:Parameter Name="application_name" Type="String" />
                                <asp:Parameter Name="importance" Type="String" />
                                <asp:Parameter Name="application_type" Type="String" />
                            </UpdateParameters>
                        </asp:SqlDataSource>
                        <asp:Label ID="ReportDetailsLabel" runat="server" Text="<h2>Report Details:</h2>"></asp:Label><br />
                        <asp:DetailsView ID="ReportDetailsView" runat="server" AutoGenerateRows="False" 
                            DataSourceID="ReportWithMetadataSqlDataSource" 
                            ondatabound="ReportDetailsView_DataBound">
                            <AlternatingRowStyle CssClass="dv_row_alternating" />
                            <FieldHeaderStyle CssClass="dv_field_header" Font-Bold="true" />
                            <Fields>
                                <asp:BoundField DataField="id" HeaderText="Report ID" ReadOnly="True" />
                                <asp:BoundField DataField="application_name" HeaderText="Application" />
                                <asp:TemplateField HeaderText="Importance">
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="ImportanceDropDownList" runat="server" SelectedValue='<%# Bind("importance") %>'>
                                            <asp:ListItem>Important</asp:ListItem>
                                            <asp:ListItem>Useful</asp:ListItem>
                                            <asp:ListItem>Not useful</asp:ListItem>
                                            <asp:ListItem Value="">[Not set]</asp:ListItem>
                                        </asp:DropDownList>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="ImportanceLabel" runat="server" Text='<%# Bind("importance") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="reporter_name" HeaderText="Author" ReadOnly="True" />
                                <asp:TemplateField HeaderText="Email">
                                    <ItemTemplate>
                                        <asp:Label ID="EmailContent" runat="server" Text='<%# Bind("reporter_email") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="reporter_homepage" HeaderText="Website" 
                                    ReadOnly="True" />
                                <asp:BoundField DataField="display_name" HeaderText="Framework Version" 
                                    ReadOnly="True" />
                                <asp:TemplateField HeaderText="Application Type">
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="ApplicationTypeDropDownList" runat="server" SelectedValue='<%# Bind("application_type") %>'>
                                            <asp:ListItem>Corporate Website</asp:ListItem>
                                            <asp:ListItem>Public Website</asp:ListItem>
                                            <asp:ListItem>Corporate Desktop Application</asp:ListItem>
                                            <asp:ListItem>Desktop Application for Resale</asp:ListItem>
                                            <asp:ListItem>Web Application for Resale</asp:ListItem>
                                            <asp:ListItem>Open Source Project</asp:ListItem>
                                            <asp:ListItem>Other</asp:ListItem>
                                            <asp:ListItem Value="">[Not set]</asp:ListItem>
                                        </asp:DropDownList>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="ApplicationTypeLabel" runat="server" Text='<%# Bind("application_type") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:CommandField ShowEditButton="True" />
                            </Fields>
                        </asp:DetailsView>
                        <asp:Label ID="NoSuchReportLabel" runat="server" Text="That report does not exist.  Please try again." Visible="False"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div class="reportview_comments">
                <asp:UpdatePanel ID="CommentUpdatePanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:SqlDataSource ID="CommentsSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
                            ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT * FROM report_comment WHERE report_id = @id ORDER BY comment_date ASC;"
                            InsertCommand="INSERT INTO report_comment (report_id, comment, commenter, comment_date, emailed) VALUES (@id, @comment, @commenter, @comment_date, @emailed);">
                            <SelectParameters>
                                <asp:QueryStringParameter DefaultValue="1" Name="id" QueryStringField="ReportID"
                                    Type="Int32" />
                            </SelectParameters>
                            <InsertParameters>
                                <asp:QueryStringParameter DefaultValue="1" Name="id" QueryStringField="ReportID" />
                                <asp:FormParameter Name="comment" FormField="NewComment" />
                                <%-- commenter and comment_date set in CommentButton_Click () --%>
                                <asp:Parameter Name="commenter" DefaultValue="" />
                                <asp:Parameter Name="comment_date" DefaultValue="" />
                                <asp:FormParameter Name="emailed" FormField="SendCommentCheckBox" Type="Boolean" />
                            </InsertParameters>
                        </asp:SqlDataSource>
                        <asp:Label ID="CommentsLabel" runat="server" Text="<h2>Comments:</h2>"></asp:Label><br />
                        <asp:TextBox ID="Comments" runat="server" ReadOnly="True" TextMode="MultiLine" Columns="40"
                            Rows="12"></asp:TextBox><br />
                        <asp:TextBox ID="NewComment" runat="server" Rows="6" TextMode="MultiLine" Columns="40"></asp:TextBox><br />
                        <asp:CheckBox ID="SendCommentCheckBox" runat="server" Checked="True" Text="Send comment to submitter" /><br />
                        <asp:Button ID="CommentButton" runat="server" Text="Add Comment" OnClick="CommentButton_Click" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div class="reportview_issues">
                <asp:UpdatePanel ID="IssuesUpdatePanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:SqlDataSource ID="IssuesSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
                            ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT DISTINCT type.lookup_name, iss.id, iss.method_namespace, iss.method_class, iss.method_name FROM issue_type type, issue iss, issue_report rep WHERE rep.report_id = @id AND rep.issue_id = iss.id AND iss.issue_type_id = type.id;"
                            EnableCaching="True" CacheDuration="300">
                            <SelectParameters>
                                <asp:QueryStringParameter DefaultValue="1" Name="id" QueryStringField="ReportID"
                                    Type="Int32" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                        <asp:Label ID="ReportIssuesLabel" runat="server" Text="<h2>Reported Issues:</h2>"></asp:Label><br />
                        <asp:GridView ID="IssuesGridView" runat="server" AllowPaging="True" AllowSorting="True"
                            AutoGenerateColumns="False" DataSourceID="IssuesSqlDataSource" 
                            OnRowDataBound="IssuesGridView_RowDataBound" 
                            onprerender="IssuesGridView_PreRender">
                            <RowStyle CssClass="gv_col" />
                            <AlternatingRowStyle CssClass="gv_col_alternating" />
                            <HeaderStyle CssClass="gv_header" />
                            <PagerStyle CssClass="gv_pager" />
                            <Columns>
                                <asp:HyperLinkField DataNavigateUrlFields="id" DataNavigateUrlFormatString="~/IssueView.aspx?IssueID={0}"
                                    HeaderText="ID" Text=">>" SortExpression="id" />
                                <asp:BoundField DataField="lookup_name" HeaderText="Type" SortExpression="lookup_name" />
                                <asp:HyperLinkField DataNavigateUrlFormatString="~/NamespaceView.aspx?Namespace={0}" 
                                    DataNavigateUrlFields="method_namespace" HeaderText="Namespace" 
                                    SortExpression="method_namespace" DataTextField="method_namespace" />
                                <asp:BoundField DataField="method_class" HeaderText="Classname" SortExpression="method_class" />
                                <asp:BoundField DataField="method_name" HeaderText="Method" SortExpression="method_name" />
                            </Columns>
                            <PagerTemplate>
                                <asp:Label ID="PagerRowsLabel" runat="server" Text="Show rows:" />
                                <asp:DropDownList ID="PagerPageSizeDropDownList" runat="server" AutoPostBack="true"
                                    OnSelectedIndexChanged="ReportedIssuesPagerPageSizeDropDownList_SelectedIndexChanged">
                                    <asp:ListItem Value="10"></asp:ListItem>
                                    <asp:ListItem Value="20"></asp:ListItem>
                                    <asp:ListItem Value="30"></asp:ListItem>
                                </asp:DropDownList>
                                Page
                                <asp:TextBox ID="PagerGotoTextBox" runat="server" AutoPostBack="true" OnTextChanged="ReportedIssuesPagerGotoTextBox_TextChanged"
                                    MaxLength="5" Columns="5"></asp:TextBox>
                                of
                                <asp:Label ID="PagerCountLabel" runat="server"></asp:Label>
                                <asp:Button ID="PagerPrevButton" runat="server" CommandName="Page" CommandArgument="Prev"
                                    Text="Prev" />
                                <asp:Button ID="PagerNextButton" runat="server" CommandName="Page" CommandArgument="Next"
                                    Text="Next" />
                            </PagerTemplate>
                            <EmptyDataTemplate>
                                No rows to show
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <br /><br />
                <asp:UpdatePanel ID="CurrentIssuesUpdatePanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:SqlDataSource ID="CurrentIssuesSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
                            ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT DISTINCT type.lookup_name, iss.id, iss.method_namespace, iss.method_class, iss.method_name FROM issue_type type, issue iss, issue_report rep WHERE rep.report_id = @id AND rep.issue_id = iss.id AND iss.issue_type_id = type.id AND iss.is_latest_definition = true;"
                            EnableCaching="True" CacheDuration="300">
                            <SelectParameters>
                                <asp:QueryStringParameter DefaultValue="1" Name="id" QueryStringField="ReportID"
                                    Type="Int32" />
                            </SelectParameters>
                        </asp:SqlDataSource>
                        <asp:Label ID="ReportCurrentIssuesLabel" runat="server" Text="<h2>Current Issues:</h2>"></asp:Label><br />
                        <asp:GridView ID="CurrentIssuesGridView" runat="server" AllowPaging="True" AllowSorting="True"
                            AutoGenerateColumns="False" DataSourceID="CurrentIssuesSqlDataSource" 
                            OnRowDataBound="IssuesGridView_RowDataBound" 
                            onprerender="IssuesGridView_PreRender">
                            <RowStyle CssClass="gv_col" />
                            <AlternatingRowStyle CssClass="gv_col_alternating" />
                            <HeaderStyle CssClass="gv_header" />
                            <PagerStyle CssClass="gv_pager" />
                            <Columns>
                                <asp:HyperLinkField DataNavigateUrlFields="id" DataNavigateUrlFormatString="~/IssueView.aspx?IssueID={0}"
                                    HeaderText="ID" Text=">>" SortExpression="id" />
                                <asp:BoundField DataField="lookup_name" HeaderText="Type" SortExpression="lookup_name" />
                                <asp:HyperLinkField DataNavigateUrlFormatString="~/NamespaceView.aspx?Namespace={0}" 
                                    DataNavigateUrlFields="method_namespace" HeaderText="Namespace" 
                                    SortExpression="method_namespace" DataTextField="method_namespace" />
                                <asp:BoundField DataField="method_class" HeaderText="Classname" SortExpression="method_class" />
                                <asp:BoundField DataField="method_name" HeaderText="Method" SortExpression="method_name" />
                            </Columns>
                            <PagerTemplate>
                                <asp:Label ID="PagerRowsLabel" runat="server" Text="Show rows:" />
                                <asp:DropDownList ID="PagerPageSizeDropDownList" runat="server" AutoPostBack="true"
                                    OnSelectedIndexChanged="CurrentIssuesPagerPageSizeDropDownList_SelectedIndexChanged">
                                    <asp:ListItem Value="10"></asp:ListItem>
                                    <asp:ListItem Value="20"></asp:ListItem>
                                    <asp:ListItem Value="30"></asp:ListItem>
                                </asp:DropDownList>
                                Page
                                <asp:TextBox ID="PagerGotoTextBox" runat="server" AutoPostBack="true" OnTextChanged="CurrentIssuesPagerGotoTextBox_TextChanged"
                                    MaxLength="5" Columns="5"></asp:TextBox>
                                of
                                <asp:Label ID="PagerCountLabel" runat="server"></asp:Label>
                                <asp:Button ID="PagerPrevButton" runat="server" CommandName="Page" CommandArgument="Prev"
                                    Text="Prev" />
                                <asp:Button ID="PagerNextButton" runat="server" CommandName="Page" CommandArgument="Next"
                                    Text="Next" />
                            </PagerTemplate>
                            <EmptyDataTemplate>
                                No rows to show
                            </EmptyDataTemplate>
                        </asp:GridView>
                        <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                            <ProgressTemplate>
                                Thinking...
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <br style="clear: both" /><br /><br />
            <asp:Label ID="Label1" runat="server" Text="Jump to Report:"></asp:Label>
            <asp:TextBox ID="ReportIDTextBox" runat="server" AutoPostBack="True" 
                MaxLength="7" ontextchanged="ReportIDTextBox_TextChanged"></asp:TextBox>
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Content>
