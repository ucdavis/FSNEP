<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FSNEP.Controllers.TimeRecordEntryViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    Entry
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        <%= Html.Encode(string.Format("Time Record for {0}", Model.TimeRecord.Date.ToString("MMMM yyyy"))) %></h2>
            
    <% Html.RenderPartial("Calendar"); %>
    
    
</asp:Content>
