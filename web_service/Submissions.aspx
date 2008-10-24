<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="Submissions.aspx.cs" Inherits="Submissions" Title="MoMA Studio - View Submissions" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" Runat="Server">
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
            This view is only available to logged-in users.
        </AnonymousTemplate>
        <LoggedInTemplate>
            <asp:UpdatePanel ID="ReportsGridUpdatePanel" runat="server">
                <ContentTemplate>
                    <asp:SqlDataSource ID="SubmissionsSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
                        ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT a.id, a.report_date, a.application_name, a.application_type, a.reporter_name, a.reporter_organization, a.display_name, miss.miss, niex.niex, pinv.pinv, todo.todo, total.total FROM (SELECT rep.id, rep.report_date, meta.application_name, meta.application_type, rep.reporter_name, rep.reporter_organization, def.display_name FROM moma_definition def, report rep LEFT JOIN report_metadata meta ON rep.id = meta.report_id WHERE rep.moma_definition_id = def.id) AS a, (SELECT report_id, COUNT(report_id) AS miss FROM issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'MISS' GROUP BY report_id) AS miss, (SELECT report_id, COUNT(report_id) as niex FROM issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'NIEX' GROUP BY report_id) AS niex, (SELECT report_id, COUNT(report_id) as pinv FROM issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'PINV' GROUP BY report_id) AS pinv, (SELECT report_id, COUNT(report_id) AS todo FROM issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'TODO' GROUP BY report_id) AS todo, (SELECT report_id, COUNT(report_id) AS total FROM issue GROUP BY report_id) AS total WHERE (a.id = miss.report_id) AND (a.id = niex.report_id) AND (a.id = pinv.report_id) AND (a.id = todo.report_id) AND (a.id = total.report_id);">
                    </asp:SqlDataSource>
                    <asp:GridView ID="ReportsGridView" runat="server" DataSourceID="SubmissionsSqlDataSource" 
                        AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False">
                        <Columns>
                            <asp:HyperLinkField DataNavigateUrlFields="id" DataNavigateUrlFormatString="~/ReportView.aspx?ReportID={0}"
                            HeaderText="ID" Text="View" SortExpression="id" />
                            <asp:BoundField DataField="report_date" HeaderText="Date" 
                                SortExpression="report_date" />
                            <asp:BoundField DataField="application_name" HeaderText="Application Name" 
                                SortExpression="application_name" />
                            <asp:BoundField DataField="application_type" HeaderText="Application Type" 
                                SortExpression="application_type" />
                            <asp:BoundField DataField="reporter_name" HeaderText="Reporter" 
                                SortExpression="reporter_name" />
                            <asp:BoundField DataField="reporter_organization" HeaderText="Organization" 
                                SortExpression="reporter_organization" />
                            <asp:BoundField DataField="display_name" HeaderText="Profile" 
                                SortExpression="display_name" />
                            <asp:BoundField DataField="miss" HeaderText="MISS" 
                                SortExpression="miss" />
                            <asp:BoundField DataField="niex" HeaderText="NIEX" 
                                SortExpression="niex" />
                            <asp:BoundField DataField="pinv" HeaderText="PINV" 
                                SortExpression="pinv" />
                            <asp:BoundField DataField="todo" HeaderText="TODO" 
                                SortExpression="todo" />
                            <asp:BoundField DataField="total" HeaderText="Total" 
                                SortExpression="total" />
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Content>

