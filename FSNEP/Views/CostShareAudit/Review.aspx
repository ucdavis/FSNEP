<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CostShareAuditReviewViewModel>" %>
<%@ Import Namespace="FSNEP.Core.Domain"%>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Review
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= Html.Encode(string.Format("Reviewing {0}'s Cost Share for {1:MMMM yyyy}", Model.CostShare.User.FullName, Model.CostShare.Date)) %></h2>
    <span><%= Html.Mailto(string.Format("[Send An Email To {0}]", Model.CostShare.User.FullName), Model.CostShare.User.Email) %></span>

    <% Html.RenderPartial("CostShareEntryList", Model.Entries); %>

    <div>
        <%= Html.ActionLink<CostShareAuditController>(x=>x.History(null), "Back to Cost Share History") %>
    </div>

</asp:Content>