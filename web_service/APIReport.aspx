<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="APIReport.aspx.cs" Inherits="APIReport" Title="MoMA Studio - API Report" %>


<asp:Content ID="ContentHeaderContent" ContentPlaceHolderID="ContentHeaderPlaceholder" runat="server">
    A list of all reported outstanding issues
</asp:Content>
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
                        SelectCommand="SELECT c.total, c.apps, c.total/c.apps AS totalperapp, i.method_namespace, i.method_class, i.method_name, i.display_name, i.lookup_name FROM (SELECT COUNT(report_id) AS total, COUNT(DISTINCT(report_id)) AS apps, issue_id FROM issue_report GROUP BY issue_id) as c, (SELECT issue.id, issue.method_namespace, issue.method_class, issue.method_name, issue_type.display_name, issue_type.lookup_name FROM issue, issue_type WHERE issue_type.id = issue.issue_type_id AND (issue.is_latest_definition = true OR issue_type.lookup_name = 'PINV')) AS i WHERE c.issue_id = i.id ORDER BY total DESC;"
                        CacheDuration="300" EnableCaching="True" FilterExpression="lookup_name = 'TODO'"
                        OnFiltering="IssuesSqlDataSource_Filtering">
                    </asp:SqlDataSource>
                    <div id="sidebar">
                        <h2>Filter</h2>
                        <h3>Issue Type:</h3>
                        <asp:CheckBoxList ID="IssueTypeFilterCheckBoxList" runat="server">
                            <asp:ListItem>MISS</asp:ListItem>
                            <asp:ListItem>NIEX</asp:ListItem>
                            <asp:ListItem>PINV</asp:ListItem>
                            <asp:ListItem>TODO</asp:ListItem>
                        </asp:CheckBoxList>
                        <h3>Namespace:</h3>
                        <asp:TextBox ID="IssueNamespaceFilterTextBox" runat="server"></asp:TextBox>
                        <h3>Class:</h3>
                        <asp:TextBox ID="IssueClassFilterTextBox" runat="server"></asp:TextBox>
                        <asp:Button ID="IssueFilterButton" runat="server" Text="Update" OnClick="IssueFilterButton_Click" />
                    </div>
                    <asp:GridView ID="IssuesGridView" runat="server" AutoGenerateColumns="False" DataSourceID="IssuesSqlDataSource"
                        AllowPaging="True" AllowSorting="True" PageSize="30" 
                        onrowdatabound="IssuesGridView_RowDataBound" 
                        onprerender="IssuesGridView_PreRender">
                        <AlternatingRowStyle CssClass="gv_col_alternating" />
                        <HeaderStyle CssClass="gv_header" />
                        <PagerStyle CssClass="gv_pager" />
                        <Columns>
                            <asp:BoundField DataField="total" HeaderText="Count" SortExpression="total" />
                            <asp:BoundField DataField="apps" HeaderText="Apps" SortExpression="apps" />
                            <asp:BoundField DataField="totalperapp" HeaderText="Ratio" SortExpression="totalperapp" />
                            <asp:BoundField DataField="method_namespace" HeaderText="Namespace" SortExpression="method_namespace" />
                            <asp:BoundField DataField="method_class" HeaderText="Class" SortExpression="method_class" />
                            <asp:BoundField DataField="method_name" HeaderText="Method" SortExpression="method_name" />
                            <asp:BoundField DataField="lookup_name" HeaderText="Type" SortExpression="lookup_name" />
                        </Columns>
                        <PagerTemplate>
                            <asp:Label ID="PagerRowsLabel" runat="server" Text="Show rows:" />
                            <asp:DropDownList ID="PagerPageSizeDropDownList" runat="server" AutoPostBack="true"
                                OnSelectedIndexChanged="PagerPageSizeDropDownList_SelectedIndexChanged">
                                <asp:ListItem Value="30"></asp:ListItem>
                                <asp:ListItem Value="50"></asp:ListItem>
                                <asp:ListItem Value="100"></asp:ListItem>
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
