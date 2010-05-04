<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FSNEP.Core.Domain.Record>>" %>
<%@ Import Namespace="Telerik.Web.Mvc.UI"%>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Cost Share List
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Cost Share List</h2>

    <% 
        ViewData["ReviewAction"] = "CostShareReview";
        
        Html.RenderPartial("RecordReviewTable");
    %>
    
</asp:Content>