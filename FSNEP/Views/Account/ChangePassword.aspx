﻿<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="titleContent" runat="server">
    Change Password
</asp:Content>

<asp:Content ID="changePasswordContent" ContentPlaceHolderID="mainContent" runat="server">
    <h2>Change Password</h2>
    <p>
        Use the form below to change your password. 
    </p>
    <p>
        New passwords are required to be a minimum of <%= Html.Encode(ViewData["PasswordLength"])%> characters in length
        and contain at least one non-alphanumeric character (ex: !@#$%^&*()_+[]\';/.,).
    </p>
    <%= Html.ValidationSummary("Password change was unsuccessful. Please correct the errors and try again.")%>

    <% using (Html.BeginForm()) { %>
        <div>
            <fieldset>
                <legend>Account Information</legend>
                <p>
                    <label for="currentPassword">Current password:</label>
                    <%= Html.Password("currentPassword") %>
                    <%= Html.ValidationMessage("currentPassword") %>
                </p>
                <p>
                    <label for="newPassword">New password:</label>
                    <%= Html.Password("newPassword") %>
                    <%= Html.ValidationMessage("newPassword") %>
                </p>
                <p>
                    <label for="confirmPassword">Confirm new password:</label>
                    <%= Html.Password("confirmPassword") %>
                    <%= Html.ValidationMessage("confirmPassword") %>
                </p>
                <p>
                    <input type="submit" value="Change Password" />
                </p>
            </fieldset>
        </div>
    <% } %>
</asp:Content>
