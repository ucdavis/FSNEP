<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FSNEP.Core.Domain.Record>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	>Time Record List
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Time Record List</h2>

    <% 
        ViewData["ReviewAction"] = "TimeRecordReview";
        
        Html.RenderPartial("RecordReviewTable"); 
    %>

</asp:Content>