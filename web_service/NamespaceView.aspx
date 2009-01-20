<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="NamespaceView.aspx.cs" Inherits="NamespaceView" Title="MoMA Studio - Namespace Stats" %>

<asp:Content ID="ContentHeaderContent" ContentPlaceHolderID="ContentHeaderPlaceholder" Runat="Server">
    See namespace statistics
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" Runat="Server">
    <asp:SqlDataSource ID="IssueNamespacesSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
        ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT DISTINCT method_namespace FROM issue, issue_type WHERE method_namespace != '' AND issue.issue_type_id = issue_type.id AND issue_type.lookup_name != 'PINV' AND issue.is_latest_definition = true ORDER BY method_namespace;">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="NamespaceStatsSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
        ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT DISTINCT miss.act_miss, niex.act_niex, todo.act_todo, miss.app_miss, niex.app_niex, todo.app_todo, miss.rep_miss, niex.rep_niex, todo.rep_todo FROM issue LEFT JOIN (SELECT COUNT(DISTINCT(issue.id)) AS act_miss, COUNT(DISTINCT(issue_report.report_id)) AS app_miss, COUNT(issue_report.report_id) AS rep_miss, issue.method_namespace FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'MISS' AND issue_report.issue_id = issue.id AND issue.is_latest_definition = true AND issue.method_namespace = @ns GROUP BY method_namespace) AS miss ON miss.method_namespace = issue.method_namespace LEFT JOIN (SELECT COUNT(DISTINCT(issue.id)) AS act_niex, COUNT(DISTINCT(issue_report.report_id)) AS app_niex, COUNT(issue_report.report_id) AS rep_niex, issue.method_namespace FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'NIEX' AND issue_report.issue_id = issue.id AND issue.is_latest_definition = true AND issue.method_namespace = @ns GROUP BY method_namespace) AS niex ON niex.method_namespace = issue.method_namespace LEFT JOIN (SELECT COUNT(DISTINCT(issue.id)) AS act_todo, COUNT(DISTINCT(issue_report.report_id)) AS app_todo, COUNT(issue_report.report_id) AS rep_todo, issue.method_namespace FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'TODO' AND issue_report.issue_id = issue.id AND issue.is_latest_definition = true AND issue.method_namespace = @ns GROUP BY method_namespace) AS todo ON todo.method_namespace = issue.method_namespace WHERE issue.method_namespace = @ns;">
        <SelectParameters>
            <asp:QueryStringParameter DefaultValue="System" Name="ns" QueryStringField="Namespace" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="NamespaceIssuesSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
        ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT DISTINCT issue.id, issue.method_namespace, issue.method_class, issue.method_name, issue_type.lookup_name FROM issue_type, issue, issue_report WHERE issue.is_latest_definition = true AND issue.issue_type_id = issue_type.id AND issue.method_namespace = @ns AND issue_report.issue_id = issue.id ORDER BY issue.method_class, issue.method_name;">
        <SelectParameters>
            <asp:QueryStringParameter DefaultValue="System" Name="ns" QueryStringField="Namespace" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="NamespaceReportsSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
        ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT DISTINCT rep.id, rep.report_date, meta.importance, meta.application_name, rep.application_type, rep.reporter_name, rep.reporter_organization, def.display_name, rep.miss, rep.niex, rep.pinv, rep.todo, rep.total FROM report rep, report_metadata meta, moma_definition def, issue_report, issue WHERE issue.is_latest_definition = true AND issue.method_namespace = @ns AND issue_report.issue_id = issue.id AND issue_report.report_id = rep.id AND rep.moma_definition_id = def.id AND rep.id = meta.report_id ORDER by rep.report_date ASC;">
        <SelectParameters>
            <asp:QueryStringParameter DefaultValue="System" Name="ns" QueryStringField="Namespace" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
    <div id="sidebar">
        <h3>Namespaces</h3>
        <asp:UpdatePanel ID="TreeViewUpdatePanel" runat="server">
            <ContentTemplate>
                <asp:TreeView ID="NamespaceTreeView" runat="server" OnSelectedNodeChanged="NamespaceTreeView_SelectedNodeChanged"
                    ShowLines="True">
                </asp:TreeView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="StatsUpdatePanel" runat="server">
        <ContentTemplate>
            <asp:Label ID="StatsLabel" runat="server" Text="<h3>Statistics</h3>"></asp:Label>
            <asp:DetailsView ID="StatsDetailsView" runat="server" DataSourceID="NamespaceStatsSqlDataSource" AutoGenerateRows="False">
                <AlternatingRowStyle CssClass="dv_row_alternating" />
                <FieldHeaderStyle CssClass="dv_field_header" Font-Bold="true" />
                <Fields>
                    <asp:TemplateField HeaderText="Issues That Have Been Reported: MISS">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# FormatIssueCount (Eval("act_miss").ToString()) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Issues That Have Been Reported: NIEX">
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# FormatIssueCount (Eval("act_niex").ToString()) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Issues That Have Been Reported: TODO">
                        <ItemTemplate>
                            <asp:Label ID="Label3" runat="server" Text='<%# FormatIssueCount (Eval("act_todo").ToString()) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Reports Containing: MISS">
                        <ItemTemplate>
                            <asp:Label ID="Label4" runat="server" Text='<%# FormatIssueCount (Eval("app_miss").ToString()) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Reports Containing: NIEX">
                        <ItemTemplate>
                            <asp:Label ID="Label5" runat="server" Text='<%# FormatIssueCount (Eval("app_niex").ToString()) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Reports Containing: TODO">
                        <ItemTemplate>
                            <asp:Label ID="Label6" runat="server" Text='<%# FormatIssueCount (Eval("app_todo").ToString()) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Total Reports: MISS">
                        <ItemTemplate>
                            <asp:Label ID="Label7" runat="server" Text='<%# FormatIssueCount (Eval("rep_miss").ToString()) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Total Reports: NIEX">
                        <ItemTemplate>
                            <asp:Label ID="Label8" runat="server" Text='<%# FormatIssueCount (Eval("rep_niex").ToString()) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Total Reports: TODO">
                        <ItemTemplate>
                            <asp:Label ID="Label9" runat="server" Text='<%# FormatIssueCount (Eval("rep_todo").ToString()) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="IssuesGridUpdatePanel" runat="server">
        <ContentTemplate>
            <br /><br /><h3>Issues That Have Been Reported</h3>
            <asp:GridView ID="IssuesGridView" runat="server" DataSourceID="NamespaceIssuesSqlDataSource"
                AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" OnRowDataBound="IssuesGridView_RowDataBound"
                PageSize="20" OnPreRender="IssuesGridView_PreRender">
                <AlternatingRowStyle CssClass="gv_col_alternating" />
                <HeaderStyle CssClass="gv_header" />
                <PagerStyle CssClass="gv_pager" />
                <Columns>
                    <asp:HyperLinkField DataNavigateUrlFields="id" DataNavigateUrlFormatString="~/IssueView.aspx?ReportID={0}"
                        HeaderText="ID" Text=">>" SortExpression="id" />
                    <asp:BoundField DataField="method_namespace" HeaderText="Namespace" SortExpression="method_namespace" />
                    <asp:BoundField DataField="method_class" HeaderText="Class" SortExpression="method_class" />
                    <asp:BoundField DataField="method_name" HeaderText="Method" SortExpression="method_name" />
                    <asp:BoundField DataField="lookup_name" HeaderText="Type" SortExpression="lookup_name" />
                </Columns>
                <PagerTemplate>
                    <asp:Label ID="PagerRowsLabel" runat="server" Text="Show rows:" />
                    <asp:DropDownList ID="PagerPageSizeDropDownList" runat="server" AutoPostBack="true"
                        OnSelectedIndexChanged="IssuesPagerPageSizeDropDownList_SelectedIndexChanged">
                        <asp:ListItem Value="10"></asp:ListItem>
                        <asp:ListItem Value="20"></asp:ListItem>
                        <asp:ListItem Value="30"></asp:ListItem>
                    </asp:DropDownList>
                    Page
                    <asp:TextBox ID="PagerGotoTextBox" runat="server" AutoPostBack="true" OnTextChanged="IssuesPagerGotoTextBox_TextChanged"
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
            <asp:UpdateProgress ID="IssuesUpdateProgress1" runat="server">
                <ProgressTemplate>
                    Thinking...
                </ProgressTemplate>
            </asp:UpdateProgress>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
        </AnonymousTemplate>
        <RoleGroups>
            <asp:RoleGroup Roles="Novell">
                <ContentTemplate>
                    <asp:UpdatePanel ID="Novell_ReportsGridUpdatePanel" runat="server">
                        <ContentTemplate>
                            <br /><br /><h3>Reports Including These Issues</h3>
                            <asp:GridView ID="Novell_ReportsGridView" runat="server" DataSourceID="NamespaceReportsSqlDataSource"
                                AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" OnRowDataBound="ReportsGridView_RowDataBound"
                                PageSize="30" OnPreRender="ReportsGridView_PreRender">
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
                    <br /><br /><h3>Reports Including These Issues</h3>
                    <asp:GridView ID="LoggedIn_ReportsGridView" runat="server" DataSourceID="NamespaceReportsSqlDataSource"
                        AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" OnRowDataBound="ReportsGridView_RowDataBound"
                        PageSize="20" OnPreRender="ReportsGridView_PreRender">
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
</asp:Content>

