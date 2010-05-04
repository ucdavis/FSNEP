<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FSNEP.Controllers.NewUserViewModel>" %>
<%@ Import Namespace="FSNEP.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	NewUser
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>NewUser</h2>

    <%= Html.ClientSideValidation<NewUserViewModel>() %>

    <%= Html.ValidationSummary("Edit was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <fieldset>
            <legend>Fields</legend>
            <p>
                <label for="Password">Password:</label>
                <%= Html.Password("Password", Model.Password) %>
                <%= Html.ValidationMessage("Password", "*") %>
            </p>
            <p>
                <label for="ConfirmPassword">Confirm Password:</label>
                <%= Html.Password("ConfirmPassword", Model.ConfirmPassword)%>
                <%= Html.ValidationMessage("ConfirmPassword", "*")%>
            </p>
            <p>
                <label for="Question">Question:</label>
                <%= Html.TextBox("Question", Model.Question) %>
                <%= Html.ValidationMessage("Question", "*") %>
            </p>
            <p>
                <label for="Answer">Answer:</label>
                <%= Html.TextBox("Answer", Model.Answer) %>
                <%= Html.ValidationMessage("Answer", "*") %>
            </p>
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

