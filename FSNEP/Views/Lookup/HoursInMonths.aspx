<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FSNEP.Core.Domain.HoursInMonth>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	HoursInMonths
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript">
    $(function() {
        $("#CreateHoursInMonthForm").validate();
    });
</script>

<% Html.RenderPartial("LookupsHeader"); %>

    <h3><%= Html.Encode(TempData["Message"]) %></h3>
    <h2>Hours In Months:</h2>

    <%using (Html.BeginForm("CreateHoursInMonth", "Lookup", FormMethod.Post, new { id = "CreateHoursInMonthForm" }))
      { %>
        <%= Html.AntiForgeryToken() %>
    <fieldset>
        <legend>New:</legend>
        <p>
            <label id="month_Label" for="month">Month: </label>
            <select id="month" name="month" class="required">
				<option value="1">
					January
				</option><option value="2">
					February
				</option><option value="3">
					March
				</option><option value="4">
					April
				</option><option value="5">
					May
				</option><option value="6">
					June
				</option><option value="7">
					July
				</option><option value="8">
					August
				</option><option value="9">
					September
				</option><option value="10">
					October
				</option><option value="11">
					November
				</option><option value="12">
					December
				</option>
			</select>
        </p>
        <p>
            <%= this.TextBox("year").Label("Year: ").MaxLength(4).Class("required digits") %>
        </p>
        <p>
            <%= this.TextBox("hours").Label("Hours: ").MaxLength(3).Class("required digits")%>
        </p>
        <p>
            <input type="submit" value="Add Hours In Month" />
        </p>
    </fieldset>
    
    <% } %>

    <br />

    <ul>
    <% foreach (var item in Model) { %>
    
        <li>
            <%= Html.Encode(item) %>
        </li>
        
    <% } %>

    </ul>

</asp:Content>

