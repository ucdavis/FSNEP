<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ OutputCache Duration="86400" VaryByParam="none" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    List Of Activities And Descriptions
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <a name="Top"></a>
    <h1 align="center">
        UC-FSNEP Cost Sharing and Time Records Database<br />
        List of Activities and Descriptions</h1>
    <p align="center">
        <a href="#DirectDelivery">[Direct Delivery]</a> <a href="#Administrative">[Administrative]</a>
        <a href="#Leave">[Leave]</a></p>
    <table cellspacing="0" cellpadding="0" class="activities">
        <col />
        <col />
        <col />
        <tr>
            <td colspan="3" width="10">
                <strong><a name="DirectDelivery">
                    <h2>Direct Delivery</h2>
                </a></strong>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td width="300px">
                <strong>Allowable SNAP Ed Activities</strong>
            </td>
            <td>
                Includes prep and delivery allowable nutrition education<br />
                <br />
                <ul>
                    <li>Direct program delivery: FSNEP educator and FSNEP collaborator/partner (include
                        travel to and from delivery sites, setup and cleanup).</li>
                    <li>Adapt, modify and change lessons to appropriately address diverse clientele.</li>
                    <li>Curriculum adaptation: Search, review , and select appropriate educational materials
                        to supplement FSNEP curriculum.</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <strong>Training</strong>
            </td>
            <td>
                UC-FSNEP interactions conveying/speaking training on Nutrition Education<br />
                <br />
                <ul>
                    <li>Creative Activity/Evaluation (i.e. time spent on evaluation projects, workgroups
                        and/or other FSNEP related projects).</li>
                    <li>Provider Training: teachers, partners and/or extenders of programs.</li>
                    <li>Programmatic Training: County, regional, and statewide leadership, personnel development,
                        and subject matter trainings related to program delivery.</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <strong>Program Development</strong>
            </td>
            <td>
                <ul>
                    <li>Data / Evaluation: data collection, entry, analysis and observation of delivery.</li>
                    <li>Program Delivery Communication: (e-mails, phone calls, conference calls, fax and
                        other forms of communication)</li>
                    <li>Staff meetings: State, County, small groups and individual (internal FSNEP)</li>
                    <li>Community Meetings: (collaborators/partners i.e. Network, re coordination, collaboration,
                        marketing, recruitment)</li>
                    <li>Time spent in advisory capacity to FSNEP in program assessment, planning, delivery,
                        evaluation, troubleshooting issues, etc.</li>
                    <li>Resource development: Develop, and adapt outreach materials i.e. newsletters, procure
                        educational materials.</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <a href="#Top">[Return to top]</a>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <strong><a name="Administrative">
                    <h2>
                        Administrative</h2>
                </a></strong>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <strong>Documentation and reporting</strong>
            </td>
            <td>
                <ul>
                    <li>Reporting: Generate computer data reports, analytical reports, and prepare accomplishment
                        reports.</li>
                    <li>Review and Consent: Review and approve all documentation/paperwork as required.</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <strong>Program management</strong>
            </td>
            <td>
                <ul>
                    <li>Communications: (e-mails, phone calls, conference calls, fax and other forms of
                        communication)</li>
                    <li>Personnel supervision (day-to-day interactions, staff trainings, performance reviews.
                        etc.)</li>
                    <li>Clerical Support, Budget support; message taking</li>
                    <li>Administrative Training: County, regional, and statewide trainings (administration,
                        leadership development, and professional development). </li>
                    <li>Professonal development (Advisors)</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <strong>Fiscal management</strong>
            </td>
            <td>
                <ul>
                    <li>State Share Documentation: Time overseeing county staff, documenting use of county
                        office space, maintenance, supplies, and related documentation.</li>
                    <li>Overall fiscal management: Budgeting, program planning, tracking financial/administrative
                        issues (timesheets, approving travel, hiring process, etc).</li>
                    <li>State Share Documentation: Time spent collecting Teacher Activity Reports.</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <a href="#Top">[Return to top]</a>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <strong><a name="Leave">
                    <h2>
                        Leave</h2>
                </a></strong>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
            </td>
            <td>
                <ul>
                    <li>Vacation, sick, jury duty, holiday, etc.</li></ul>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <a href="#Top">[Return to top]</a>
            </td>
        </tr>
    </table>
</asp:Content>
