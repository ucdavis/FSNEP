<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ReviewViewModel<TimeRecord, TimeRecordEntry>>" %>
<%@ Import Namespace="FSNEP.Core.Domain"%>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	TimeRecordReview
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= Html.Encode(string.Format("Reviewing {0} for {1:MMMM yyyy}", Model.Record.User.FullName, Model.Record.Date)) %></h2>

    <% Html.RenderPartial("TimeRecordEntryList", Model.Entries); %>

    <% Html.RenderPartial("ApproveOrDeny", Model.Record); %>
    
    <div>
        <%= Html.ActionLink<SupervisorController>(x=>x.TimeRecordList(), "Back to Time Record Review List") %>
    </div>

</asp:Content>