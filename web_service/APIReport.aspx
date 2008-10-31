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
                        SelectCommand="SELECT c.apps, i.method_namespace, i.method_class, i.method_name, i.display_name, i.lookup_name FROM (SELECT COUNT(DISTINCT(report_id)) AS Apps, issue_id FROM issue_report GROUP BY issue_id) as c, (SELECT issue.id, issue.method_namespace, issue.method_class, issue.method_name, issue_type.display_name, issue_type.lookup_name FROM issue, issue_type WHERE issue_type.id = issue.issue_type_id) AS i WHERE c.issue_id = i.id ORDER BY apps DESC;"
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
                    <asp:Label ID="IssueNamespaceFilterLabel" runat="server" Text="in namespace: "></asp:Label>
                    <asp:TextBox ID="IssueNamespaceFilterTextBox" runat="server"></asp:TextBox>
                    <asp:Label ID="IssueClassFilterLabel" runat="server" Text="with class: "></asp:Label>
                    <asp:TextBox ID="IssueClassFilterTextBox" runat="server"></asp:TextBox>
                    <asp:Button ID="IssueFilterButton" runat="server" Text="Update" OnClick="IssueFilterButton_Click" />
                    <asp:GridView ID="IssuesGridView" runat="server" AutoGenerateColumns="False" DataSourceID="IssuesSqlDataSource"
                        AllowPaging="True" AllowSorting="True" PageSize="20" 
                        onrowdatabound="IssuesGridView_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="Apps" HeaderText="Apps" SortExpression="apps" />
                            <asp:BoundField DataField="method_namespace" HeaderText="Namespace" SortExpression="method_namespace" />
                            <asp:BoundField DataField="method_class" HeaderText="Class" SortExpression="method_class" />
                            <asp:BoundField DataField="method_name" HeaderText="Method" SortExpression="method_name" />
                            <asp:BoundField DataField="display_name" HeaderText="Type" SortExpression="display_name" />
                        </Columns>
                        <PagerTemplate>
                            <asp:Label ID="PagerRowsLabel" runat="server" Text="Show rows:" />
                            <asp:DropDownList ID="PagerPageSizeDropDownList" runat="server" AutoPostBack="true"
                                OnSelectedIndexChanged="PagerPageSizeDropDownList_SelectedIndexChanged">
                                <asp:ListItem Value="10"></asp:ListItem>
                                <asp:ListItem Value="20"></asp:ListItem>
                                <asp:ListItem Value="30"></asp:ListItem>
                            </asp:DropDownList>
                            Page
                            <asp:TextBox ID="PagerGotoTextBox" runat="server" AutoPostBack="true"
                                OnTextChanged="PagerGotoTextBox_TextChanged" MaxLength="5" Columns="5"></asp:TextBox>
                            of
                            <asp:Label ID="PagerCountLabel" runat="server"></asp:Label>
                            <asp:Button ID="PagerPrevButton" runat="server" CommandName="Page" CommandArgument="Prev" Text="Prev" />
                            <asp:Button ID="PagerNextButton" runat="server" CommandName="Page" CommandArgument="Next" Text="Next" />
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
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Content>
