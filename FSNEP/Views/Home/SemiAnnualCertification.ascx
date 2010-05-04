<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SemiAnnualCertificationViewModel>" %>

<% if (Model.ShouldShowCertificationLink)
   { %>

    Certification Link and text goes here

<% } %>
