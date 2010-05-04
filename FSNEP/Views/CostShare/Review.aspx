<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CostShareReviewViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Review
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= Html.Encode(string.Format("Cost Share for {0:MMMM yyyy}", Model.CostShare.Date)) %></h2>
    
    <% Html.RenderPartial("CostShareEntryList", Model.CostShareEntries); %>
        
    <br />
    <div>
        <%= Html.ActionLink<CostShareController>(x=>x.History(), "Back to Cost Share History") %>
    </div>    

</asp:Content>