<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<FSNEP.Core.Domain.CostShareEntry>>" %>

<% Html.RenderPartial("CostShareEntriesDisplay", Model.Where(x => x.Exclude == false)); %>
        
<% if (Model.Where(x => x.Exclude).Count() > 0) { %>

<div class="excludedEntries">

    <h3>Excluded Cost Share Entries</h3>

    <% Html.RenderPartial("CostShareEntriesDisplay", Model.Where(x => x.Exclude)); %>
    
</div>

<% } %>