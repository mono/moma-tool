<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="Overview.aspx.cs" Inherits="Overview" Title="MoMA Studio - Overview" %>
<%@ Register TagPrefix="zgw" Namespace="ZedGraph.Web" Assembly="ZedGraph.Web" %>

<asp:Content ID="ContentHeaderContent" ContentPlaceHolderID="ContentHeaderPlaceholder" runat="server">
Overview
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" runat="server">
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
            This view is only available to logged-in users.
        </AnonymousTemplate>
        <LoggedInTemplate>
            <asp:Label ID="Latest20Label" runat="server" Text="<h2>Latest 20 Reports:</h2>"></asp:Label>
            <br />
            <asp:SqlDataSource ID="Latest20SqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
                ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>"
                SelectCommand="SELECT rep.id, rep.report_date, rep.reporter_name, def.display_name, meta.importance, miss.miss, niex.niex, pinv.pinv, todo.todo, total.total FROM moma_definition def, report rep LEFT JOIN report_metadata meta ON rep.id = meta.report_id LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS miss FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'MISS' AND issue_report.issue_id = issue.id GROUP BY report_id) AS miss ON rep.id = miss.report_id LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS niex FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'NIEX' AND issue_report.issue_id = issue.id GROUP BY report_id) AS niex ON rep.id = niex.report_id LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS pinv FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'PINV' AND issue_report.issue_id = issue.id GROUP BY report_id) AS pinv ON rep.id = pinv.report_id LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS todo FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'TODO' AND issue_report.issue_id = issue.id GROUP BY report_id) AS todo ON rep.id = todo.report_id LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS total FROM issue_report GROUP BY report_id) AS total ON rep.id = total.report_id WHERE rep.moma_definition_id = def.id ORDER BY rep.report_date DESC LIMIT 20;" EnableCaching="True" CacheDuration="300">
            </asp:SqlDataSource>
            <asp:GridView ID="Latest20GridView" runat="server" AutoGenerateColumns="False" 
                DataSourceID="Latest20SqlDataSource">
                <Columns>
                    <asp:HyperLinkField DataNavigateUrlFields="id" DataNavigateUrlFormatString="~/ReportView.aspx?ReportID={0}"
                        HeaderText="Details" Text="View" />
                    <asp:BoundField DataField="report_date" HeaderText="Date" />
                    <asp:BoundField DataField="id" HeaderText="ID" />
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
            <br />
            <asp:Label ID="MostNeededLabel" runat="server" Text="<h2>Most needed API:</h2>"></asp:Label>
            <br />
            <asp:SqlDataSource ID="MostNeededSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
                ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT c.apps, i.method_namespace, i.method_class, i.method_name, i.display_name FROM (SELECT COUNT(DISTINCT(report_id)) AS Apps, issue_id FROM issue_report GROUP BY issue_id) as c, (SELECT issue.id, issue.method_namespace, issue.method_class, issue.method_name, issue_type.display_name FROM issue, issue_type WHERE issue_type.id = issue.issue_type_id) AS i WHERE c.issue_id = i.id ORDER BY Apps DESC LIMIT 20;" EnableCaching="True" CacheDuration="300">
            </asp:SqlDataSource>
            <asp:GridView ID="MostNeededGridView" runat="server" 
                AutoGenerateColumns="False" DataSourceID="MostNeededSqlDataSource" 
                onrowdatabound="MostNeededGridView_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="method_namespace" HeaderText="Namespace" />
                    <asp:BoundField DataField="method_class" HeaderText="Class" />
                    <asp:BoundField DataField="method_name" HeaderText="Method" />
                    <asp:BoundField DataField="display_name" HeaderText="Type" />
                    <asp:BoundField DataField="Apps" HeaderText="Apps" />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="IssuesPerAppSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
                ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT COUNT(report_id) AS Count FROM issue_report GROUP BY report_id;" EnableCaching="True" CacheDuration="300">
            </asp:SqlDataSource>
            <br />
            <asp:Label ID="IssuesPerAppLabel" runat="server" Text="<h2>Issues per Application:</h2>"></asp:Label>
            <br />
            <zgw:zedgraphweb ID="IssuesPerAppGraph" runat="server"></zgw:zedgraphweb>
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Content>
