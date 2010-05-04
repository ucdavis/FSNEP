<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<TimeRecordAuditReviewViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Time Record Review
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= Html.Encode(string.Format("Reviewing {0} for {1:MMMM yyyy}", Model.TimeRecord.User.FullName, Model.TimeRecord.Date)) %></h2>

    <% Html.RenderPartial("TimeRecordEntryList", Model.Entries); %>

    <%= Html.ActionLink("Back to Time Record List", "TimeRecordHistory", "Audit") %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalScripts" runat="server">
</asp:Content>
