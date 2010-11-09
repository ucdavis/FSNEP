<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<SemiAnnualCertificationViewModel>" %>

<% if (Model.ShouldShowCertificationLink || true)
   { %>

    Please download the required UC-FSNEP semi-annual certification. Employee signatures and certifying signatures are required. Please submit to your UC-FSNEP Analyst by either April 30th or October 31st depending on period certified. Thank you.
    <br /><br />
    
    <a href="http://fsnep.ucdavis.edu/administrative/time-records-info/time-records-information/document_view">http://fsnep.ucdavis.edu/administrative/time-records-info/time-records-information/document_view</a>

<% } %>
