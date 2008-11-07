<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="Submissions.aspx.cs" Inherits="Submissions" Title="MoMA Studio - View Submissions" %>

<asp:Content ID="ContentHeaderContent" ContentPlaceHolderID="ContentHeaderPlaceholder" runat="server">
    A list of all reports
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" Runat="Server">
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
            This view is only available to logged-in users.
        </AnonymousTemplate>
        <LoggedInTemplate>
            <asp:UpdatePanel ID="ReportsGridUpdatePanel" runat="server">
                <ContentTemplate>
                    <%-- Need something in the filter here so it will actually filter at all --%>
                    <asp:SqlDataSource ID="SubmissionsSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
                        ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" 
                        SelectCommand="SELECT rep.id, rep.report_date, meta.importance, meta.application_name, meta.application_type, rep.reporter_name, rep.reporter_organization, def.display_name, miss.miss, niex.niex, pinv.pinv, todo.todo, total.total FROM moma_definition def, report rep LEFT JOIN report_metadata meta ON rep.id = meta.report_id LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS miss FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'MISS' AND issue_report.issue_id = issue.id GROUP BY report_id) AS miss ON rep.id = miss.report_id LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS niex FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'NIEX' AND issue_report.issue_id = issue.id GROUP BY report_id) AS niex ON rep.id = niex.report_id LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS pinv FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'PINV' AND issue_report.issue_id = issue.id GROUP BY report_id) AS pinv ON rep.id = pinv.report_id LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS todo FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'TODO' AND issue_report.issue_id = issue.id GROUP BY report_id) AS todo ON rep.id = todo.report_id LEFT JOIN (SELECT report_id, COUNT(report_id) AS total FROM issue_report GROUP BY report_id) AS total ON rep.id = total.report_id WHERE rep.moma_definition_id = def.id ORDER BY rep.report_date DESC;" 
                        EnableCaching="True" CacheDuration="300" FilterExpression="importance = 'Important'"
                        onfiltering="SubmissionsSqlDataSource_Filtering">
                    </asp:SqlDataSource>
                    <asp:SqlDataSource ID="ProfileSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
                        ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" 
                        SelectCommand="SELECT display_name FROM moma_definition ORDER BY display_name DESC;"
                        EnableCaching="True" CacheDuration="300">
                    </asp:SqlDataSource>
                    <asp:Label ID="ImportanceFilterLabel" runat="server" Text="Show only:"></asp:Label>
                    <asp:ListBox ID="ImportanceFilterListBox" runat="server" 
                        SelectionMode="Multiple" Rows="3">
                        <asp:ListItem>Important</asp:ListItem>
                        <asp:ListItem>Useful</asp:ListItem>
                        <asp:ListItem>Not useful</asp:ListItem>
                    </asp:ListBox>
                    <asp:Label ID="AppTypeFilterLabel" runat="server" Text="application type:"></asp:Label>
                    <asp:TextBox ID="AppTypeFilterTextBox" runat="server"></asp:TextBox>
                    <asp:Label ID="ProfileFilterLabel" runat="server" Text="profile:"></asp:Label>
                    <asp:DropDownList ID="ProfileFilterDropDownList" runat="server" 
                        DataSourceID="ProfileSqlDataSource" DataTextField="display_name" 
                        DataValueField="display_name" 
                        ondatabound="ProfileFilterDropDownList_DataBound">
                    </asp:DropDownList>
                    <asp:Button ID="FilterButton" runat="server" Text="Update" onclick="FilterButton_Click" />
                    <asp:GridView ID="ReportsGridView" runat="server" DataSourceID="SubmissionsSqlDataSource" 
                        AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" 
                        onrowdatabound="ReportsGridView_RowDataBound">
                        <Columns>
                            <asp:HyperLinkField DataNavigateUrlFields="id" DataNavigateUrlFormatString="~/ReportView.aspx?ReportID={0}"
                            HeaderText="ID" Text="View" SortExpression="id" />
                            <asp:BoundField DataField="report_date" HeaderText="Date" 
                                SortExpression="report_date" />
                            <asp:BoundField DataField="importance" HeaderText="Importance" 
                                SortExpression="importance" />
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

