<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ReviewViewModel<CostShare, CostShareEntry>>" %>
<%@ Import Namespace="FSNEP.Core.Domain"%>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	CostShareReview
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= Html.Encode(string.Format("Reviewing {0} for {1:MMMM yyyy}", Model.Record.User.FullName, Model.Record.Date)) %></h2>

    <% Html.RenderPartial("CostShareEntryList", Model.Entries); %>
    
    <% Html.RenderPartial("ApproveOrDeny", Model.Record); %>
    
    <div class="backnav">
        <%= Html.ActionLink<SupervisorController>(x=>x.CostShareList(), "Back to Local Support Review List") %>
    </div>

</asp:Content>