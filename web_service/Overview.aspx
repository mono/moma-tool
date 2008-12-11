<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="Overview.aspx.cs" Inherits="Overview" Title="MoMA Studio - Overview" %>

<asp:Content ID="ContentHeaderContent" ContentPlaceHolderID="ContentHeaderPlaceholder" runat="server">
    Overview
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" runat="server">
    <asp:SqlDataSource ID="Latest20SqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
        ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT rep.id, rep.report_date, rep.reporter_name, def.display_name, meta.importance, rep.miss, rep.niex, rep.pinv, rep.todo, rep.total FROM moma_definition def, report rep, report_metadata meta WHERE rep.moma_definition_id = def.id AND rep.id = meta.report_id ORDER BY rep.report_date DESC LIMIT 20;"
        EnableCaching="True" CacheDuration="300">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="MostNeededSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
        ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT c.reports, i.method_namespace, i.method_class, i.method_name, i.lookup_name FROM (SELECT COUNT(DISTINCT(report_id)) AS reports, issue_id FROM issue_report GROUP BY issue_id) as c, (SELECT issue.id, issue.method_namespace, issue.method_class, issue.method_name, issue_type.lookup_name FROM issue, issue_type WHERE issue_type.id = issue.issue_type_id AND issue.is_latest_definition = true) AS i WHERE c.issue_id = i.id ORDER BY reports DESC LIMIT 20;"
        EnableCaching="True" CacheDuration="300">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="IssuesPerAppSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
        ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT total AS count FROM report;"
        EnableCaching="True" CacheDuration="300">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="IssuesPerAppNowSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
        ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT COUNT(report_id) AS Count FROM issue_report, issue WHERE issue_report.issue_id = issue.id AND issue.is_latest_definition = true GROUP BY report_id;"
        EnableCaching="True" CacheDuration="300">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="StatsSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
        ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT COUNT(id) AS Count FROM report;"
        EnableCaching="True" CacheDuration="300">
    </asp:SqlDataSource>
    <div id="sidebar">
        <asp:Label runat="server" Text="<h3>Statistics:</h3>"></asp:Label>
        <asp:DetailsView ID="StatsDetailsView" runat="server" AutoGenerateRows="False" DataSourceID="StatsSqlDataSource">
            <AlternatingRowStyle CssClass="dv_row_alternating" />
            <FieldHeaderStyle CssClass="dv_field_header" Font-Bold="true" />
            <Fields>
                <asp:BoundField DataField="count" HeaderText="Report Count:" />
            </Fields>
        </asp:DetailsView>
        <br /><br />
        <asp:Label ID="IssuesPerAppLabel" runat="server" Text="<h3>Issues per Application (as reported):</h3>"></asp:Label>
        <asp:Image ID="IssuesPerAppGraphImage" runat="server" Height="150" Width="200" />
        <br /><br />
        <asp:Label ID="IssuesPerAppNowLabel" runat="server" Text="<h3>Issues per Application (latest mono version):</h3>"></asp:Label>
        <asp:Image ID="IssuesPerAppNowGraphImage" runat="server" Height="150" Width="200" />
    </div>
    <asp:Label ID="Latest20Label" runat="server" Text="<h3>Latest 20 Reports:</h3>"></asp:Label>
    <br />
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
            <asp:GridView ID="Anon_Latest20GridView" runat="server" AutoGenerateColumns="False"
                DataSourceID="Latest20SqlDataSource">
                    <AlternatingRowStyle CssClass="gv_col_alternating" />
                    <HeaderStyle CssClass="gv_header" />
                    <Columns>
                    <asp:TemplateField HeaderText="Date" SortExpression="report_date">
                        <ItemTemplate>
                            <%-- Shorten the date --%>
                            <asp:Label ID="Label1" runat="server" Text='<%# ((DateTime)Eval("report_date")).ToShortDateString () %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="display_name" HeaderText="Profile" />
                    <asp:BoundField DataField="miss" HeaderText="MISS" />
                    <asp:BoundField DataField="niex" HeaderText="NIEX" />
                    <asp:BoundField DataField="pinv" HeaderText="PINV" />
                    <asp:BoundField DataField="todo" HeaderText="TODO" />
                    <asp:BoundField DataField="total" HeaderText="Total" />
                </Columns>
            </asp:GridView>
        </AnonymousTemplate>
        <RoleGroups>
            <asp:RoleGroup Roles="Novell">
                <ContentTemplate>
                    <asp:GridView ID="Novell_Latest20GridView" runat="server" AutoGenerateColumns="False" DataSourceID="Latest20SqlDataSource">
                        <AlternatingRowStyle CssClass="gv_col_alternating" />
                        <HeaderStyle CssClass="gv_header" />
                        <Columns>
                            <asp:HyperLinkField DataNavigateUrlFields="id" DataNavigateUrlFormatString="~/ReportView.aspx?ReportID={0}"
                                HeaderText="ID" Text=">>" />
                            <asp:TemplateField HeaderText="Date" SortExpression="report_date">
                                <ItemTemplate>
                                    <%-- Shorten the date --%>
                                    <asp:Label ID="Label1" runat="server" Text='<%# ((DateTime)Eval("report_date")).ToShortDateString () %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="reporter_name" HeaderText="Name" />
                            <asp:BoundField DataField="display_name" HeaderText="Profile" />
                            <asp:BoundField DataField="miss" HeaderText="MISS" />
                            <asp:BoundField DataField="niex" HeaderText="NIEX" />
                            <asp:BoundField DataField="pinv" HeaderText="PINV" />
                            <asp:BoundField DataField="todo" HeaderText="TODO" />
                            <asp:BoundField DataField="total" HeaderText="Total" />
                            <asp:TemplateField ShowHeader="False">
                                <ItemTemplate>
                                    <asp:Image ID="ImportanceImage" runat="server" ImageUrl="~/important.png" Visible='<%# Eval("importance").ToString() == "Important" %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:RoleGroup>
        </RoleGroups>
        <LoggedInTemplate>
            <asp:GridView ID="LoggedIn_Latest20GridView" runat="server" AutoGenerateColumns="False" 
                DataSourceID="Latest20SqlDataSource">
                <AlternatingRowStyle CssClass="gv_col_alternating" />
                <HeaderStyle CssClass="gv_header" />
                <Columns>
                    <asp:TemplateField HeaderText="Date" SortExpression="report_date">
                        <ItemTemplate>
                            <%-- Shorten the date --%>
                            <asp:Label ID="Label1" runat="server" Text='<%# ((DateTime)Eval("report_date")).ToShortDateString () %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="display_name" HeaderText="Profile" />
                    <asp:BoundField DataField="miss" HeaderText="MISS" />
                    <asp:BoundField DataField="niex" HeaderText="NIEX" />
                    <asp:BoundField DataField="pinv" HeaderText="PINV" />
                    <asp:BoundField DataField="todo" HeaderText="TODO" />
                    <asp:BoundField DataField="total" HeaderText="Total" />
                </Columns>
            </asp:GridView>
        </LoggedInTemplate>
    </asp:LoginView>
    <br />
    <asp:Label ID="MostNeededLabel" runat="server" Text="<h3>Most needed API:</h3>"></asp:Label>
    <br />
    <asp:GridView ID="MostNeededGridView" runat="server" AutoGenerateColumns="False"
        DataSourceID="MostNeededSqlDataSource" OnRowDataBound="MostNeededGridView_RowDataBound">
        <AlternatingRowStyle CssClass="gv_col_alternating" />
        <HeaderStyle CssClass="gv_header" />
        <Columns>
            <asp:BoundField DataField="method_namespace" HeaderText="Namespace" />
            <asp:BoundField DataField="method_class" HeaderText="Class" />
            <asp:BoundField DataField="method_name" HeaderText="Method" />
            <asp:BoundField DataField="lookup_name" HeaderText="Type" />
            <asp:BoundField DataField="reports" HeaderText="Reports" />
        </Columns>
    </asp:GridView>
</asp:Content>
