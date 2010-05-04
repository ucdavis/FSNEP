<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ReviewViewModel<CostShare, CostShareEntry>>" %>
<%@ Import Namespace="FSNEP.Core.Domain"%>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	CostShareReview
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Cost Share Review</h2>

    <% Html.RenderPartial("CostShareEntryList", Model.Entries); %>
    
</asp:Content>