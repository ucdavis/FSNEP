<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AuditHistoryViewModel<TimeRecord>>" %>
<%@ Import Namespace="FSNEP.Core.Domain"%>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Time Record History
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(function() {
            $("#project").change(function() {

                var listUrlBase = '<%= Url.Action("TimeRecordHistory") %>';

                var projectId = $(this).val();

                if (projectId == "") window.location = listUrlBase;
                else window.location = listUrlBase + '/?projectId=' + projectId;
            });
        });
    </script>

    <h2>Time Record History</h2>

    Project: <%= this.Select("project")
            .Options(Model.Projects, x=>x.Id, x=>x.Name)
            .Selected(Model.Project != null ? Model.Project.Id : 0)
            .FirstOption("--Select A Project--")
        %>

    <% if (Model.Project != null)
       { %>

        <br /><br />
        <div>

            <table class="t-widget t-grid">
                <thead>
                    <tr>
                        <th class="t-header">
                        </th>
                        <th class="t-header">
                            Name
                        </th>
                        <th class="t-header t-last-header">
                            Status
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <% int lastMonth = 0; %>
                    <% foreach (var record in Model.Records.OrderByDescending(a => a.Year).ThenByDescending(a => a.Month).ThenBy(a => a.User.FullNameLastFirst))
                       { %>
                    <% int currentMonth = record.Month; %>
                    <% if (currentMonth != lastMonth)
                       { %>
                    <tr class="ui-widget-header">
                        <td colspan="4">
                            <h3>
                                <%= Html.Encode(record.MonthName)%>
                                <%= Html.Encode(record.Year)%>
                            </h3>
                        </td>
                    </tr>
                    <% lastMonth = currentMonth; %>
                    <% } %>
                    <tr>
                        <td>
                            <%= Html.ActionLink<AuditController>(x=>x.TimeRecordReview(record.Id), "Review") %>
                        </td>
                        <td>
                            <%= Html.Encode(record.User.FullName)%>
                        </td>
                        <td>
                            <%= Html.Encode(record.Status.Name)%>
                        </td>
                    </tr>
                    <% } %>
                </tbody>
            </table>

        </div>

    <% } %>

</asp:Content>
