<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FSNEP.Controllers.CostShareEntryViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Entry
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= Html.Encode(string.Format("Cost Share for {0}", Model.CostShare.Date.ToString("MMMM yyyy"))) %></h2>
    
    <%= Html.ValidationSummary("Cost Share Entry was unsuccessful. Please correct the errors and try again.") %>
    <%= Html.ClientSideValidation<FSNEP.Core.Domain.CostShareEntry>("Entry") %>
    
    <% using(Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        
        <%= Html.Hidden("id", Model.CostShare.Id) %>
        
        <fieldset>
            <legend>New Cost Share Entry</legend>
            <p>
                <%= this.TextBox("Entry.Amount").Label("Amount ($):") %>
            </p>
            <p>
                <%= this.Select("Entry.ExpenseType")
                    .Label("Expense Type:")
                    .Options(Model.ExpenseTypes, x=>x.Id, x=>x.Name)
                    .FirstOption("Select An Expense Type")
                %>
            </p>
            <p>
                <%= this.TextBox("Entry.Description").Label("Expense Description:")%>
            </p>
            <p>
                <%= this.Select("Entry.FundType")
                    .Label("Fund Type:")
                    .Options(Model.FundTypes, x=>x.Id, x=>x.Name)
                    .FirstOption("Select A Fund Type")
                %>
            </p>
            <p>
                <%= this.Select("Entry.Project")
                    .Label("Project:")
                    .Options(Model.Projects, x=>x.Id, x=>x.Name)
                    .FirstOption("Select A Project")
                %>
            </p>
            <p>Account</p>
            <p>
                <%= this.TextArea("Entry.Comment").Label("Comment:")%>
            </p>
            <p>
                <input type="submit" value="Add Expense" />
            </p>
        </fieldset>
    
    <%
       }%>

    <%--//TODO--%> List of existing cost shares 
    
</asp:Content>