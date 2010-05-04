<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CostShareEntryViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Entry
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        var Services = {
            GetAccountsForProject: '<%= Url.Action("GetAccountsForProject", "Association") %>'
        }

        $(function() {
            //PopulateAccounts($("#Entry_Project"));

            $("#Entry_Project").change(function() { PopulateAccounts(this); });
        });
        
        function PopulateAccounts(el) {
            var projectId = $(el).val();

            //Get the new accounts for this project
            $.getJSON(
                Services.GetAccountsForProject + "/" + projectId,
                null,
                OnPopulateAccountsComplete);
        }

        function OnPopulateAccountsComplete(result) {
            var accountSelect = $("#Entry_Account");
            accountSelect.empty(); //remove the current options
            accountSelect.removeAttr("disabled");

            $(result).each(function() { //append in the new elements
                var acct = this;
                var newOption = $("<option>" + acct.Name + "</option>").val(acct.Id);

                accountSelect.append(newOption);
            });
        }
    </script>

    <h2><%= Html.Encode(string.Format("Cost Share for {0}", Model.CostShare.Date.ToString("MMMM yyyy"))) %></h2>
    
    <%= Html.ValidationSummary("Cost Share Entry was unsuccessful. Please correct the errors and try again.") %>
    <%= Html.ClientSideValidation<FSNEP.Core.Domain.CostShareEntry>("Entry") %>
    <%= Html.ClientSideValidation<FSNEP.Core.Domain.CostShareEntry>()
            .AddRule("PostedFile", new xVal.Rules.RegularExpressionRule(@"^.+\.(pdf)$", RegexOptions.IgnoreCase) { ErrorMessage = "File Must be a PDF" })
    %>
    
    <% using(Html.BeginForm("Entry", "CostShare", FormMethod.Post, new { id = "entryForm", enctype = "multipart/form-data" })) {%>
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
                <%= this.FileUpload("PostedFile").Label("Expense Recepit (PDF):") %>
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
            <p>
                <label for="Entry_Account" id="Account_Label">
                    Account:</label>
                <select id="Entry_Account" name="Entry.Account" disabled="disabled">
                    <option value="">Select An Account</option>
                </select>
            </p>
            <p>
                <%= this.TextArea("Entry.Comment").Label("Comment:")%>
            </p>
            <p>
                <input type="submit" value="Add Expense" />
            </p>
        </fieldset>
    
    <%
       }%>

    <div id="ExistingEntries">
        <% Html.RenderPartial("ExistingCostShareEntries", Model.CostShareEntries); %>
    </div>
    
    <div id="SubmitRecord">
        <% using (Html.BeginForm("Submit", "CostShare", new {id = Model.CostShare.Id})) { %>
            <%= Html.AntiForgeryToken() %>
        
            <input type="submit" value="Submit Final" />
        
        <% } %>
    </div>
    
</asp:Content>