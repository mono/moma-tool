<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="Submissions.aspx.cs" Inherits="Submissions" Title="MoMA Studio - View Submissions" %>

<asp:Content ID="ContentHeaderContent" ContentPlaceHolderID="ContentHeaderPlaceholder" runat="server">
    A list of all reports
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" Runat="Server">
    <%-- Need something in the filter here so it will actually filter at all --%>
    <asp:SqlDataSource ID="SubmissionsSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
        ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT rep.id, rep.report_date, meta.importance, meta.application_name, meta.application_type, rep.reporter_name, rep.reporter_organization, def.display_name, miss.miss, niex.niex, pinv.pinv, todo.todo, total.total FROM moma_definition def, report rep LEFT JOIN report_metadata meta ON rep.id = meta.report_id LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS miss FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'MISS' AND issue_report.issue_id = issue.id GROUP BY report_id) AS miss ON rep.id = miss.report_id LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS niex FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'NIEX' AND issue_report.issue_id = issue.id GROUP BY report_id) AS niex ON rep.id = niex.report_id LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS pinv FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'PINV' AND issue_report.issue_id = issue.id GROUP BY report_id) AS pinv ON rep.id = pinv.report_id LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS todo FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'TODO' AND issue_report.issue_id = issue.id GROUP BY report_id) AS todo ON rep.id = todo.report_id LEFT JOIN (SELECT report_id, COUNT(report_id) AS total FROM issue_report GROUP BY report_id) AS total ON rep.id = total.report_id WHERE rep.moma_definition_id = def.id ORDER BY rep.report_date DESC;"
        EnableCaching="True" CacheDuration="300" FilterExpression="importance = 'Important'"
        OnFiltering="SubmissionsSqlDataSource_Filtering"></asp:SqlDataSource>
    <asp:SqlDataSource ID="ProfileSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
        ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT display_name FROM moma_definition ORDER BY create_date DESC;"
        EnableCaching="True" CacheDuration="300"></asp:SqlDataSource>
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
            This view is only available to logged-in users.
        </AnonymousTemplate>
        <RoleGroups>
            <asp:RoleGroup Roles="Novell">
                <ContentTemplate>
                    <asp:UpdatePanel ID="Novell_ReportsGridUpdatePanel" runat="server">
                        <ContentTemplate>
                            <%-- I tried making the Submissions GridView editable (to streamline the metadata
                                 editing workflow) but encountered two problems: First, it seems I need a
                                 read-write ID field so I can use it in an UpdateParameters section (or it
                                 doesn't bind), and second, the show-stopper, on submit the GridView does a
                                 full database reread, which makes it unusable.
                            --%>
                            <div id="sidebar">
                                <h2>Filter</h2>
                                <h3>Importance:</h3>
                                <asp:CheckBoxList ID="Novell_ImportanceCheckBoxList" runat="server">
                                    <asp:ListItem>Important</asp:ListItem>
                                    <asp:ListItem>Useful</asp:ListItem>
                                    <asp:ListItem>Not useful</asp:ListItem>
                                </asp:CheckBoxList>
                                <h3>Application Name:</h3>
                                <asp:TextBox ID="Novell_AppNameFilterTextBox" runat="server"></asp:TextBox>
                                <h3>Application Type:</h3>
                                <asp:TextBox ID="Novell_AppTypeFilterTextBox" runat="server"></asp:TextBox>
                                <h3>Profile:</h3>
                                <asp:DropDownList ID="Novell_ProfileFilterDropDownList" runat="server" DataSourceID="ProfileSqlDataSource"
                                    DataTextField="display_name" DataValueField="display_name" OnDataBound="ProfileFilterDropDownList_DataBound">
                                </asp:DropDownList>
                                <asp:Button ID="Novell_FilterButton" runat="server" Text="Update" OnClick="FilterButton_Click" />
                            </div>
                            <asp:GridView ID="Novell_ReportsGridView" runat="server" DataSourceID="SubmissionsSqlDataSource"
                                AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" OnRowDataBound="ReportsGridView_RowDataBound">
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
                                    <asp:BoundField DataField="application_name" HeaderText="App Name" SortExpression="application_name" />
                                    <asp:BoundField DataField="application_type" HeaderText="App Type" SortExpression="application_type" />
                                    <asp:BoundField DataField="reporter_organization" HeaderText="Organization" SortExpression="reporter_organization" />
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
                                        OnSelectedIndexChanged="PagerPageSizeDropDownList_SelectedIndexChanged">
                                        <asp:ListItem Value="10"></asp:ListItem>
                                        <asp:ListItem Value="20"></asp:ListItem>
                                        <asp:ListItem Value="30"></asp:ListItem>
                                    </asp:DropDownList>
                                    Page
                                    <asp:TextBox ID="PagerGotoTextBox" runat="server" AutoPostBack="true" OnTextChanged="PagerGotoTextBox_TextChanged"
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
                    <%-- I tried making the Submissions GridView editable (to streamline the metadata
                         editing workflow) but encountered two problems: First, it seems I need a
                         read-write ID field so I can use it in an UpdateParameters section (or it
                         doesn't bind), and second, the show-stopper, on submit the GridView does a
                         full database reread, which makes it unusable.
                    --%>
                    <div id="sidebar">
                        <h2>Filter</h2>
                        <h3>Application Type:</h3>
                        <asp:TextBox ID="LoggedIn_AppTypeFilterTextBox" runat="server"></asp:TextBox>
                        <h3>Profile:</h3>
                        <asp:DropDownList ID="LoggedIn_ProfileFilterDropDownList" runat="server" DataSourceID="ProfileSqlDataSource"
                            DataTextField="display_name" DataValueField="display_name" OnDataBound="ProfileFilterDropDownList_DataBound">
                        </asp:DropDownList>
                        <asp:Button ID="LoggedIn_FilterButton" runat="server" Text="Update" OnClick="FilterButton_Click" />
                    </div>
                    <asp:GridView ID="LoggedIn_ReportsGridView" runat="server" DataSourceID="SubmissionsSqlDataSource"
                        AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                        OnRowDataBound="ReportsGridView_RowDataBound">
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
                            <asp:BoundField DataField="display_name" HeaderText="Profile" 
                                SortExpression="display_name" />
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

