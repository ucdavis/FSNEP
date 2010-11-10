<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Change Question/Answer
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Change Question/Answer</h2>

     <%= Html.ValidationSummary("Question/Answer change was unsuccessful. Please correct the errors and try again.")%>

    <% using (Html.BeginForm()) { %>
        <div>
            <fieldset>
                <legend>Account Information</legend>
                <p>
                    <label for="username">UserName:</label>
                    <%= Html.TextBox("username")%>
                    <%= Html.ValidationMessage("username")%>
                </p>
                <p>
                    <label for="password">Current password:</label>
                    <%= Html.Password("password")%>
                    <%= Html.ValidationMessage("password")%>
                </p>
                <p>
                    <label for="newQuestion">New Question:</label>
                    <%= Html.TextBox("newQuestion")%>
                    <%= Html.ValidationMessage("newQuestion")%>
                </p>
                <p>
                    <label for="newAnswer">New Answer:</label>
                    <%= Html.TextBox("newAnswer")%>
                    <%= Html.ValidationMessage("newAnswer")%>
                </p>
                <p>
                    <input type="submit" value="Change Question/Answer" />
                </p>
            </fieldset>
        </div>
    <% } %>
</asp:Content>
