<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<TimeRecordReviewViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Review
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2><%= Html.Encode(string.Format("Time Record for {0:MMMM yyyy}", Model.TimeRecord.Date)) %></h2>
    
    <% Html.RenderPartial("TimeRecordEntryList", Model.TimeRecordEntries); %>
        
    <br />
    <div>
        <%= Html.ActionLink<TimeRecordController>(x=>x.History(), "Back to Time Record History") %>
    </div>    

</asp:Content>
