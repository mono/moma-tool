<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="APIReport.aspx.cs" Inherits="APIReport" Title="MoMA Studio - API Report" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" runat="Server">
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
            This view is only available to logged-in users.
        </AnonymousTemplate>
        <LoggedInTemplate>
            <asp:UpdatePanel ID="ReportsGridUpdatePanel" runat="server">
                <ContentTemplate>
                    <%-- Need something in the filter here so it will actually filter at all --%>
                    <asp:SqlDataSource ID="IssuesSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
                        ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>"
                        SelectCommand="SELECT c.apps, i.method_namespace, i.method_class, i.method_name, i.display_name, i.lookup_name FROM (SELECT COUNT(DISTINCT(report_id)) AS Apps, issue_id FROM issue_report GROUP BY issue_id) as c, (SELECT issue.id, issue.method_namespace, issue.method_class, issue.method_name, issue_type.display_name, issue_type.lookup_name FROM issue, issue_type WHERE issue_type.id = issue.issue_type_id) AS i WHERE c.issue_id = i.id;"
                        CacheDuration="300" EnableCaching="True" FilterExpression="lookup_name = 'TODO'"
                        OnFiltering="IssuesSqlDataSource_Filtering">
                    </asp:SqlDataSource>
                    <asp:Label ID="IssueTypeFilterLabel" runat="server" Text="Show only:"></asp:Label>
                    <asp:ListBox ID="IssueTypeFilterListBox" runat="server"
                        SelectionMode="Multiple" Rows="4">
                        <asp:ListItem>MISS</asp:ListItem>
                        <asp:ListItem>NIEX</asp:ListItem>
                        <asp:ListItem>PINV</asp:ListItem>
                        <asp:ListItem>TODO</asp:ListItem>
                    </asp:ListBox>
                    <asp:Button ID="IssueTypeFilterButton" runat="server" Text="Update" OnClick="IssueTypeFilterButton_Click" />
                    <asp:GridView ID="IssuesGridView" runat="server" AutoGenerateColumns="False" DataSourceID="IssuesSqlDataSource"
                        AllowPaging="True" AllowSorting="True" PageSize="20">
                        <Columns>
                            <asp:BoundField DataField="Apps" HeaderText="Apps" SortExpression="apps" />
                            <asp:BoundField DataField="method_namespace" HeaderText="Namespace" SortExpression="method_namespace" />
                            <asp:BoundField DataField="method_class" HeaderText="Class" SortExpression="method_class" />
                            <asp:BoundField DataField="method_name" HeaderText="Method" SortExpression="method_name" />
                            <asp:BoundField DataField="display_name" HeaderText="Type" SortExpression="display_name" />
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Content>
