<%@ Page Language="C#" MasterPageFile="~/MoMA.master" AutoEventWireup="true" CodeFile="MyAccount.aspx.cs" Inherits="MyAccount" Title="MoMA Studio - My Account" %>

<asp:Content ID="ContentHeaderContent" ContentPlaceHolderID="ContentHeaderPlaceholder" Runat="Server">
    View account details
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="BodyContentPlaceHolder" Runat="Server">
    <asp:LoginView ID="LoginView1" runat="server">
        <AnonymousTemplate>
            This view is only available to logged-in users.
        </AnonymousTemplate>
        <LoggedInTemplate>
            <asp:SqlDataSource ID="MyReportsSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:MomaDB %>"
                ProviderName="<%$ ConnectionStrings:MomaDB.ProviderName %>" SelectCommand="SELECT rep.id, rep.report_date, meta.importance, meta.application_name, rep.reporter_name, rep.reporter_email, rep.reporter_homepage, def.display_name, rep.application_type, rep.miss, rep.niex, rep.pinv, rep.todo, rep.total, curmiss.miss AS curmiss, curniex.niex AS curniex, curtodo.todo AS curtodo, curtotal.total AS curtotal FROM report_metadata meta, moma_definition def, report rep LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS miss FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'MISS' AND issue_report.issue_id = issue.id AND issue.is_latest_definition = true GROUP BY report_id) AS curmiss ON rep.id = curmiss.report_id LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS niex FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'NIEX' AND issue_report.issue_id = issue.id AND issue.is_latest_definition = true GROUP BY report_id) AS curniex ON rep.id = curniex.report_id LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS todo FROM issue_report, issue, issue_type WHERE issue.issue_type_id = issue_type.id AND issue_type.lookup_name = 'TODO' AND issue_report.issue_id = issue.id AND issue.is_latest_definition = true GROUP BY report_id) AS curtodo ON rep.id = curtodo.report_id LEFT JOIN (SELECT issue_report.report_id, COUNT(issue_report.report_id) AS total FROM issue_report, issue WHERE issue_report.issue_id = issue.id AND issue.is_latest_definition = true GROUP BY report_id) AS curtotal ON rep.id = curtotal.report_id WHERE meta.report_id = rep.id AND rep.moma_definition_id = def.id AND rep.reporter_email = @email;">
                <SelectParameters>
                    <asp:Parameter DefaultValue="" Name="email" Type="String" />
                </SelectParameters>
            </asp:SqlDataSource>
            <div id="sidebar">
                <asp:Label runat="server" Text="<h3>Logged in as:"></asp:Label>
                <asp:LoginName ID="LoginName1" runat="server" />
                <asp:Label runat="server" Text="</h3>"></asp:Label>
                <br />
                <asp:Label runat="server" Text="<h3>User Options:</h3>"></asp:Label>
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Password.aspx">Change Password</asp:HyperLink>
                <br />
                <asp:LoginStatus ID="LoginStatus1" runat="server" LogoutPageUrl="~/Overview.aspx" LogoutAction="Redirect" />
            </div>
            <asp:Label runat="server" Text="<h3>Submitted Reports:</h3>"></asp:Label>
            <asp:GridView ID="MyReportsGridView" runat="server" AllowPaging="True" AllowSorting="True"
                AutoGenerateColumns="False" OnRowDataBound="ReportsGridView_RowDataBound" 
                PageSize="10" onprerender="MyReportsGridView_PreRender">
                <RowStyle CssClass="gv_col" />
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
                    <asp:BoundField DataField="application_name" HeaderText="App Name" SortExpression="application_name" />
                    <asp:BoundField DataField="application_type" HeaderText="App Type" SortExpression="application_type" />
                    <asp:BoundField DataField="display_name" HeaderText="Profile" SortExpression="display_name" />
                    <%-- %><asp:BoundField DataField="curmiss" HeaderText="Current MISS" SortExpression="curmiss" />
                    <asp:BoundField DataField="curniex" HeaderText="Current NIEX" SortExpression="curniex" />
                    <asp:BoundField DataField="curtodo" HeaderText="Current TODO" SortExpression="curtodo" />
                    <asp:BoundField DataField="curtotal" HeaderText="Current Total" SortExpression="curtotal" />--%>
                    <asp:TemplateField HeaderText="MISS" SortExpression="miss">
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# FormatIssueCount (Eval("curmiss").ToString()) + " (" + Eval("miss") + ")" %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="NIEX" SortExpression="niex">
                        <ItemTemplate>
                            <asp:Label ID="Label3" runat="server" Text='<%# FormatIssueCount (Eval("curniex").ToString()) + " (" + Eval("niex") + ")" %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="PINV" SortExpression="pinv">
                        <ItemTemplate>
                            <asp:Label ID="Label4" runat="server" Text='<%# "(" + Eval("pinv") + ")" %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="TODO" SortExpression="todo">
                        <ItemTemplate>
                            <asp:Label ID="Label5" runat="server" Text='<%# FormatIssueCount (Eval("curtodo").ToString()) + " (" + Eval("todo") + ")" %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Total" SortExpression="total">
                        <ItemTemplate>
                            <asp:Label ID="Label6" runat="server" Text='<%# FormatIssueCount (Eval("curtotal").ToString()) + " (" + Eval("total") + ")" %>'></asp:Label>
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
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Content>

