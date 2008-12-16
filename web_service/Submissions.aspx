<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="Submissions.aspx.cs" Inherits="Submissions" Title="MoMA Studio - View Submissions" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="ContentHeaderContent" ContentPlaceHolderID="ContentHeaderPlaceholder" runat="server">
    A list of all reports
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" Runat="Server">
    <%-- Need something in the filter here so it will actually filter at all --%>
    <asp:SqlDataSource ID="SubmissionsSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
        ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT rep.id, rep.report_date, meta.importance, meta.application_name, rep.application_type, rep.reporter_name, rep.reporter_organization, def.display_name, rep.miss, rep.niex, rep.pinv, rep.todo, rep.total FROM moma_definition def, report rep, report_metadata meta WHERE rep.moma_definition_id = def.id AND rep.id = meta.report_id ORDER BY rep.report_date DESC;"
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
                                <h3>Date From:</h3>
                                <asp:TextBox ID="Novell_DateFromTextBox" runat="server"></asp:TextBox>
                                <cc1:CalendarExtender ID="Novell_DateFromCalendarExtender" TargetControlID="Novell_DateFromTextBox" runat="server">
                                </cc1:CalendarExtender>
                                <h3>Date To:</h3>
                                <asp:TextBox ID="Novell_DateToTextBox" runat="server"></asp:TextBox>
                                <cc1:CalendarExtender ID="Novell_DateToCalendarExtender" TargetControlID="Novell_DateToTextBox" runat="server">
                                </cc1:CalendarExtender>
                                <h3>Importance:</h3>
                                <asp:CheckBoxList ID="Novell_ImportanceCheckBoxList" runat="server">
                                    <asp:ListItem>Important</asp:ListItem>
                                    <asp:ListItem>Useful</asp:ListItem>
                                    <asp:ListItem>Not useful</asp:ListItem>
                                </asp:CheckBoxList>
                                <h3>Application Name:</h3>
                                <asp:TextBox ID="Novell_AppNameFilterTextBox" runat="server"></asp:TextBox>
                                <h3>Application Type:</h3>
                                <%-- abbreviate the items so they fit cleanly in the sidebar --%>
                                <asp:CheckBoxList ID="Novell_AppTypeCheckBoxList" runat="server">
                                    <asp:ListItem>Corporate Website</asp:ListItem>
                                    <asp:ListItem>Public Website</asp:ListItem>
                                    <asp:ListItem Value="Corporate Desktop Application">Corporate Desktop App</asp:ListItem>
                                    <asp:ListItem Value="Desktop Application for Resale">Desktop App for Resale</asp:ListItem>
                                    <asp:ListItem Value="Web Application for Resale">Web App for Resale</asp:ListItem>
                                    <asp:ListItem>Open Source Project</asp:ListItem>
                                    <asp:ListItem>Other</asp:ListItem>
                                </asp:CheckBoxList>
                                <h3>Profile:</h3>
                                <asp:DropDownList ID="Novell_ProfileFilterDropDownList" runat="server" DataSourceID="ProfileSqlDataSource"
                                    DataTextField="display_name" DataValueField="display_name" OnDataBound="ProfileFilterDropDownList_DataBound">
                                </asp:DropDownList>
                                <asp:Button ID="Novell_FilterButton" runat="server" Text="Update" OnClick="FilterButton_Click" />
                            </div>
                            <asp:GridView ID="Novell_ReportsGridView" runat="server" DataSourceID="SubmissionsSqlDataSource"
                                AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" 
                                OnRowDataBound="ReportsGridView_RowDataBound" PageSize="30" 
                                onprerender="ReportsGridView_PreRender">
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
                                        OnSelectedIndexChanged="PagerPageSizeDropDownList_SelectedIndexChanged">
                                        <asp:ListItem Value="20"></asp:ListItem>
                                        <asp:ListItem Value="30"></asp:ListItem>
                                        <asp:ListItem Value="50"></asp:ListItem>
                                        <asp:ListItem Value="100"></asp:ListItem>
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
                        <h3>Date From:</h3>
                        <asp:TextBox ID="LoggedIn_DateFromTextBox" runat="server"></asp:TextBox>
                        <cc1:CalendarExtender ID="LoggedIn_DateFromCalendarExtender" TargetControlID="LoggedIn_DateFromTextBox" runat="server">
                        </cc1:CalendarExtender>
                        <h3>Date To:</h3>
                        <asp:TextBox ID="LoggedIn_DateToTextBox" runat="server"></asp:TextBox>
                        <cc1:CalendarExtender ID="LoggedIn_DateToCalendarExtender" TargetControlID="LoggedIn_DateToTextBox" runat="server">
                        </cc1:CalendarExtender>
                        <h3>Application Type:</h3>
                        <%-- abbreviate the items so they fit cleanly in the sidebar --%>
                        <asp:CheckBoxList ID="LoggedIn_AppTypeCheckBoxList" runat="server">
                            <asp:ListItem>Corporate Website</asp:ListItem>
                            <asp:ListItem>Public Website</asp:ListItem>
                            <asp:ListItem Value="Corporate Desktop Application">Corporate Desktop App</asp:ListItem>
                            <asp:ListItem Value="Desktop Application for Resale">Desktop App for Resale</asp:ListItem>
                            <asp:ListItem Value="Web Application for Resale">Web App for Resale</asp:ListItem>
                            <asp:ListItem>Open Source Project</asp:ListItem>
                            <asp:ListItem>Other</asp:ListItem>
                        </asp:CheckBoxList>
                        <h3>Profile:</h3>
                        <asp:DropDownList ID="LoggedIn_ProfileFilterDropDownList" runat="server" DataSourceID="ProfileSqlDataSource"
                            DataTextField="display_name" DataValueField="display_name" OnDataBound="ProfileFilterDropDownList_DataBound">
                        </asp:DropDownList>
                        <asp:Button ID="LoggedIn_FilterButton" runat="server" Text="Update" OnClick="FilterButton_Click" />
                    </div>
                    <asp:GridView ID="LoggedIn_ReportsGridView" runat="server" DataSourceID="SubmissionsSqlDataSource"
                        AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                        OnRowDataBound="ReportsGridView_RowDataBound" PageSize="20" 
                        onprerender="ReportsGridView_PreRender">
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

