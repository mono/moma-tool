<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="IssueView.aspx.cs" Inherits="NamespaceView" Title="MoMA Studio - View Issue" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Namespace="MoMATool" TagPrefix="disqus" %>
<%-- Have to disable event validation for this page, due to the dynamically populated cascading drop down lists --%>

<asp:Content ID="ContentHeaderContent" ContentPlaceHolderID="ContentHeaderPlaceholder" Runat="Server">
    See issue details
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" Runat="Server">
    <asp:SqlDataSource ID="IssueByQueryIDSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
        ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT issue.id, issue.method_name, issue.method_namespace, issue.method_class, issue.is_latest_definition, issue_type.lookup_name FROM issue, issue_type WHERE issue.id = @id AND issue.issue_type_id = issue_type.id;">
        <SelectParameters>
            <asp:Parameter DefaultValue="1" Name="id" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="IssueReportsSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
        ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT DISTINCT rep.id, rep.report_date, meta.importance, meta.application_name, rep.application_type, rep.reporter_name, rep.reporter_organization, def.display_name, rep.miss, rep.niex, rep.pinv, rep.todo, rep.total FROM report rep, report_metadata meta, moma_definition def, issue_report, issue WHERE issue.id = @id AND issue_report.issue_id = issue.id AND issue_report.report_id = rep.id AND rep.moma_definition_id = def.id AND rep.id = meta.report_id ORDER by rep.report_date ASC;">
        <SelectParameters>
            <asp:Parameter DefaultValue="1" Name="id" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>    
    <%-- This isn't in the sidebar, as the drop down values are much too wide --%>
    <asp:DropDownList ID="NamespaceDropDownList" runat="server" Width="900px">
    </asp:DropDownList>
    <cc1:CascadingDropDown ID="NamespaceDropDownList_CascadingDropDown" runat="server"
        Enabled="True" ServiceMethod="GetNamespaceDropDownContents" TargetControlID="NamespaceDropDownList"
        UseContextKey="True" Category="namespace" EmptyText="[No namespaces]" LoadingText="[Loading namespaces...]"
        PromptText="[Please choose a namespace...]">
    </cc1:CascadingDropDown>
    <br />
    <asp:DropDownList ID="ClassDropDownList" runat="server" Width="900px">
    </asp:DropDownList>
    <cc1:CascadingDropDown ID="ClassDropDownList_CascadingDropDown" runat="server" Enabled="True"
        ServiceMethod="GetClassDropDownContents" TargetControlID="ClassDropDownList"
        UseContextKey="True" Category="class" EmptyText="[No classes]" LoadingText="[Loading classes...]"
        ParentControlID="NamespaceDropDownList" PromptText="[Please choose a class...]">
    </cc1:CascadingDropDown>
    <br />
    <asp:DropDownList ID="IssueDropDownList" runat="server" AutoPostBack="True" Width="900px"
        OnSelectedIndexChanged="IssueDropDownList_SelectedIndexChanged">
    </asp:DropDownList>
    <cc1:CascadingDropDown ID="IssueDropDownList_CascadingDropDown" runat="server" Enabled="True"
        ServiceMethod="GetIssueDropDownContents" TargetControlID="IssueDropDownList"
        UseContextKey="True" Category="issue" EmptyText="[No issues]" LoadingText="[Loading issues...]"
        ParentControlID="ClassDropDownList" PromptText="[Please choose an issue...]">
    </cc1:CascadingDropDown>
    <br />
    <asp:UpdatePanel ID="IssueDetailsUpdatePanel" runat="server" UpdateMode="Conditional" RenderMode="Inline">
        <ContentTemplate>
            <br />
            <asp:Label ID="DetailsLabel" runat="server" Text="<h2>Issue Details:</h2>" Visible="false"></asp:Label>            
            <asp:DetailsView ID="IssueDetailsView" runat="server" Height="50px" 
                Width="900px" AutoGenerateRows="False" 
                DataSourceID="IssueByQueryIDSqlDataSource" Visible="false">
                <AlternatingRowStyle CssClass="dv_row_alternating" />
                <FieldHeaderStyle CssClass="dv_field_header" Font-Bold="true" />
                <Fields>
                    <asp:HyperLinkField DataNavigateUrlFields="id" 
                        DataNavigateUrlFormatString="IssueView.aspx?IssueID={0}" DataTextField="id" 
                        DataTextFormatString="{0} (Click to go directly to this issue)" 
                        HeaderText="ID" />
                    <asp:HyperLinkField DataNavigateUrlFields="method_namespace"
                        DataNavigateUrlFormatString="NamespaceView.aspx?Namespace={0}" DataTextField="method_namespace"
                        DataTextFormatString="{0}" HeaderText="Namespace" />
                    <asp:BoundField DataField="method_class" HeaderText="Class" />
                    <asp:BoundField DataField="method_name" HeaderText="Method" />
                    <asp:BoundField DataField="lookup_name" HeaderText="Type" />
                </Fields>
            </asp:DetailsView>
            <br />
            <asp:Label ID="FixedLabel" runat="server" Text="<span style='color: red;'>This issue has been fixed in current versions of Mono.</span>" Visible="false"></asp:Label>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="IssueDropDownList" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
        </AnonymousTemplate>
        <RoleGroups>
            <asp:RoleGroup Roles="Novell">
                <ContentTemplate>
                    <asp:UpdatePanel ID="Novell_ReportsGridUpdatePanel" runat="server">
                        <ContentTemplate>
                            <asp:Label ID="Novell_ReportedByLabel" runat="server" Text="<br /><br /><h2>Reported By:</h2>" Visible="false"></asp:Label>
                            <asp:GridView ID="Novell_ReportsGridView" runat="server" DataSourceID="IssueReportsSqlDataSource"
                                AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" OnRowDataBound="ReportsGridView_RowDataBound"
                                PageSize="30" OnPreRender="ReportsGridView_PreRender" Visible="false">
                                <AlternatingRowStyle CssClass="gv_col_alternating" />
                                <HeaderStyle CssClass="gv_header" />
                                <PagerStyle CssClass="gv_pager" />
                                <Columns>
                                    <asp:HyperLinkField DataNavigateUrlFields="id" DataNavigateUrlFormatString="~/ReportView.aspx?ReportID={0}"
                                        HeaderText="ID" Text=">>" SortExpression="id" />
                                    <asp:TemplateField HeaderText="Date" SortExpression="report_date">
                                        <ItemTemplate>
                                            <%-- Shorten the date, otherwise it's too wide --%>
                                            <asp:Label ID="Label1" runat="server" Text='<%# ((DateTime)Eval("report_date")).ToShortDateString () %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%-- The &nbsp; header text means it can still be clicked on for sorting --%>
                                    <asp:TemplateField HeaderText="&nbsp;" SortExpression="importance">
                                        <ItemTemplate>
                                            <asp:Image ID="ImportanceImage" runat="server" ImageUrl='<%# "~/" + Eval("importance").ToString().ToLower() + ".png" %>'
                                                Visible='<%# Eval("importance").ToString() != "" %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%-- <asp:BoundField DataField="application_name" HeaderText="App Name" SortExpression="application_name" /> --%>
                                    <asp:BoundField DataField="application_type" HeaderText="App Type" SortExpression="application_type" />
                                    <asp:BoundField DataField="reporter_organization" HeaderText="Org" SortExpression="reporter_organization" />
                                    <asp:BoundField DataField="display_name" HeaderText="Profile" SortExpression="display_name" />
                                    <asp:TemplateField HeaderText="MISS" SortExpression="miss">
                                        <ItemTemplate>
                                            <asp:Label ID="Label2" runat="server" Text='<%# FormatIssueCount (Eval("miss").ToString()) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="NIEX" SortExpression="niex">
                                        <ItemTemplate>
                                            <asp:Label ID="Label3" runat="server" Text='<%# FormatIssueCount (Eval("niex").ToString()) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="PINV" SortExpression="pinv">
                                        <ItemTemplate>
                                            <asp:Label ID="Label4" runat="server" Text='<%# FormatIssueCount (Eval("pinv").ToString()) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="TODO" SortExpression="todo">
                                        <ItemTemplate>
                                            <asp:Label ID="Label5" runat="server" Text='<%# FormatIssueCount (Eval("todo").ToString()) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Total" SortExpression="total">
                                        <ItemTemplate>
                                            <asp:Label ID="Label6" runat="server" Text='<%# FormatIssueCount (Eval("total").ToString()) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerTemplate>
                                    <asp:Label ID="PagerRowsLabel" runat="server" Text="Show rows:" />
                                    <asp:DropDownList ID="PagerPageSizeDropDownList" runat="server" AutoPostBack="true"
                                        OnSelectedIndexChanged="ReportsPagerPageSizeDropDownList_SelectedIndexChanged">
                                        <asp:ListItem Value="20"></asp:ListItem>
                                        <asp:ListItem Value="30"></asp:ListItem>
                                        <asp:ListItem Value="50"></asp:ListItem>
                                        <asp:ListItem Value="100"></asp:ListItem>
                                    </asp:DropDownList>
                                    Page
                                    <asp:TextBox ID="PagerGotoTextBox" runat="server" AutoPostBack="true" OnTextChanged="ReportsPagerGotoTextBox_TextChanged"
                                        MaxLength="5" Columns="5"></asp:TextBox>
                                    of
                                    <asp:Label ID="PagerCountLabel" runat="server"></asp:Label>
                                    <asp:Button ID="Novell_PagerPrevButton" runat="server" CommandName="Page" CommandArgument="Prev"
                                        Text="Prev" />
                                    <asp:Button ID="Novell_PagerNextButton" runat="server" CommandName="Page" CommandArgument="Next"
                                        Text="Next" />
                                </PagerTemplate>
                                <EmptyDataTemplate>
                                    No rows to show
                                </EmptyDataTemplate>
                            </asp:GridView>
                            <asp:UpdateProgress ID="Novell_UpdateProgress1" runat="server">
                                <ProgressTemplate>
                                    Thinking...
                                </ProgressTemplate>
                            </asp:UpdateProgress>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </asp:RoleGroup>
        </RoleGroups>
        <LoggedInTemplate>
            <asp:UpdatePanel ID="LoggedIn_ReportsGridUpdatePanel" runat="server">
                <ContentTemplate>
                    <asp:Label ID="LoggedIn_ReportedByLabel" runat="server" Text="<br /><br /><h2>Reported By:</h2>" Visible="false"></asp:Label>
                    <asp:GridView ID="LoggedIn_ReportsGridView" runat="server" DataSourceID="IssueReportsSqlDataSource"
                        AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" OnRowDataBound="ReportsGridView_RowDataBound"
                        PageSize="20" OnPreRender="ReportsGridView_PreRender" Visible="false">
                        <AlternatingRowStyle CssClass="gv_col_alternating" />
                        <HeaderStyle CssClass="gv_header" />
                        <PagerStyle CssClass="gv_pager" />
                        <Columns>
                            <asp:HyperLinkField DataNavigateUrlFields="id" DataNavigateUrlFormatString="~/ReportView.aspx?ReportID={0}"
                                HeaderText="ID" Text=">>" SortExpression="id" />
                            <asp:TemplateField HeaderText="Date" SortExpression="report_date">
                                <ItemTemplate>
                                    <%-- Shorten the date, otherwise it's too wide --%>
                                    <asp:Label ID="Label1" runat="server" Text='<%# ((DateTime)Eval("report_date")).ToShortDateString () %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="application_type" HeaderText="App Type" SortExpression="application_type" />
                            <asp:BoundField DataField="display_name" HeaderText="Profile" SortExpression="display_name" />
                            <asp:TemplateField HeaderText="MISS" SortExpression="miss">
                                <ItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# FormatIssueCount (Eval("miss").ToString()) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="NIEX" SortExpression="niex">
                                <ItemTemplate>
                                    <asp:Label ID="Label3" runat="server" Text='<%# FormatIssueCount (Eval("niex").ToString()) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="PINV" SortExpression="pinv">
                                <ItemTemplate>
                                    <asp:Label ID="Label4" runat="server" Text='<%# FormatIssueCount (Eval("pinv").ToString()) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="TODO" SortExpression="todo">
                                <ItemTemplate>
                                    <asp:Label ID="Label5" runat="server" Text='<%# FormatIssueCount (Eval("todo").ToString()) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Total" SortExpression="total">
                                <ItemTemplate>
                                    <asp:Label ID="Label6" runat="server" Text='<%# FormatIssueCount (Eval("total").ToString()) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerTemplate>
                            <asp:Label ID="PagerRowsLabel" runat="server" Text="Show rows:" />
                            <asp:DropDownList ID="PagerPageSizeDropDownList" runat="server" AutoPostBack="true"
                                OnSelectedIndexChanged="ReportsPagerPageSizeDropDownList_SelectedIndexChanged">
                                <asp:ListItem Value="10"></asp:ListItem>
                                <asp:ListItem Value="20"></asp:ListItem>
                                <asp:ListItem Value="30"></asp:ListItem>
                            </asp:DropDownList>
                            Page
                            <asp:TextBox ID="PagerGotoTextBox" runat="server" AutoPostBack="true" OnTextChanged="ReportsPagerGotoTextBox_TextChanged"
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
                    <asp:UpdateProgress ID="LoggedIn_UpdateProgress1" runat="server">
                        <ProgressTemplate>
                            Thinking...
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                </ContentTemplate>
            </asp:UpdatePanel>
        </LoggedInTemplate>
    </asp:LoginView>
    <asp:UpdatePanel ID="IssuesCommentsUpdatePanel" runat="server">
        <ContentTemplate>
            <disqus:DisqusControl ID="DisqusComments" Width="900px" DisqusForum="mono-momastudio-issues" DisqusDeveloper="false" runat="server"></disqus:DisqusControl>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

