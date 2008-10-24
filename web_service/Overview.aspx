<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="Overview.aspx.cs" Inherits="Overview" Title="MoMA Studio - Overview" %>
<%@ Register TagPrefix="zgw" Namespace="ZedGraph.Web" Assembly="ZedGraph.Web" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" runat="server">
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
            This view is only available to logged-in users.
        </AnonymousTemplate>
        <LoggedInTemplate>
            <asp:Label ID="Latest20Label" runat="server" Text="Latest 20 Reports:"></asp:Label>
            <br />
            <asp:SqlDataSource ID="Latest20SqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
                ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>"
                SelectCommand="SELECT a.id, a.create_date, a.reporter_name, a.display_name, miss.miss, niex.niex, pinv.pinv, todo.todo, total.total FROM (SELECT rep.id, rep.create_date, rep.reporter_name, prof.display_name FROM report rep, moma_definition prof WHERE rep.moma_definition_id = prof.id) AS a, (SELECT report_id, COUNT(report_id) AS miss FROM issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'MISS' GROUP BY report_id) AS miss, (SELECT report_id, COUNT(report_id) AS niex FROM issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'NIEX' GROUP BY report_id) AS niex, (SELECT report_id, COUNT(report_id) AS pinv FROM issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'PINV' GROUP BY report_id) AS pinv, (SELECT report_id, COUNT(report_id) AS todo FROM issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'TODO' GROUP BY report_id) AS todo, (SELECT report_id, COUNT(report_id) AS total FROM issue GROUP BY report_id) AS total WHERE (a.id = miss.report_id) AND (a.id = niex.report_id) AND (a.id = pinv.report_id) AND (a.id = todo.report_id) AND (a.id = total.report_id) ORDER BY a.create_date DESC LIMIT 20;">
            </asp:SqlDataSource>
            <asp:GridView ID="Latest20GridView" runat="server" AutoGenerateColumns="False" 
                DataSourceID="Latest20SqlDataSource">
                <Columns>
                    <asp:HyperLinkField DataNavigateUrlFields="id" DataNavigateUrlFormatString="~/ReportView.aspx?ReportID={0}"
                        HeaderText="Details" Text="View" />
                    <asp:BoundField DataField="create_date" HeaderText="Date" />
                    <asp:BoundField DataField="id" HeaderText="ID" />
                    <asp:BoundField DataField="reporter_name" HeaderText="Name" />
                    <asp:BoundField DataField="display_name" HeaderText="Profile" />
                    <asp:BoundField DataField="miss" HeaderText="MISS" />
                    <asp:BoundField DataField="niex" HeaderText="NIEX" />
                    <asp:BoundField DataField="pinv" HeaderText="PINV" />
                    <asp:BoundField DataField="todo" HeaderText="TODO" />
                    <asp:BoundField DataField="total" HeaderText="Total" />
                </Columns>
            </asp:GridView>
            <asp:Label ID="MostNeededLabel" runat="server" Text="Most needed API:"></asp:Label>
            <br />
<%--
            <asp:SqlDataSource ID="MostNeededSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
                ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT COUNT(DISTINCT(issue.report_id)) AS Apps, issue.method_namespace, issue.method_class, issue.method_name, issue_type.display_name FROM issue, issue_type WHERE issue_type.id = issue.issue_type_id GROUP BY method_namespace, method_class, method_name, display_name ORDER BY Apps DESC LIMIT 20;">
            </asp:SqlDataSource>
            <asp:GridView ID="MostNeededGridView" runat="server" AutoGenerateColumns="False" DataSourceID="MostNeededSqlDataSource">
                <Columns>
                    <asp:BoundField DataField="method_namespace" HeaderText="Namespace" />
                    <asp:BoundField DataField="method_class" HeaderText="Class" />
                    <asp:BoundField DataField="method_name" HeaderText="Method" />
                    <asp:BoundField DataField="display_name" HeaderText="Type" />
                    <asp:BoundField DataField="Apps" HeaderText="Apps" />
                </Columns>
            </asp:GridView>
--%>
            <asp:SqlDataSource ID="IssuesPerAppSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
                ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT COUNT(issue.report_id) AS Count FROM issue GROUP BY issue.report_id;">
            </asp:SqlDataSource>
            <asp:Label ID="IssuesPerAppLabel" runat="server" Text="Issues per Application"></asp:Label>
            <br />
            <zgw:zedgraphweb ID="IssuesPerAppGraph" runat="server"></zgw:zedgraphweb>
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Content>
