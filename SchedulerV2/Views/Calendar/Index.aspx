<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SchedulerV2.Models.Schedule>" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>


<script runat="server">
	public override void VerifyRenderingInServerForm(Control control)
	{
	
	}
</script>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%
        RadGrid1.DataSource = Model.Location.Shifts;
        RadGrid1.DataBind();        
    %>

    <telerik:RadGrid ID="RadGrid1" runat="server" AutoGenerateColumns="false" AllowMultiRowSelection="true" >
        <MasterTableView>
            <Columns>
                <telerik:GridBoundColumn DataField="Name" HeaderText="Shfit Name" UniqueName="Name" />
                <telerik:GridBoundColumn DataField="Start" HeaderText="Start Time" UniqueName="Start" DataFormatString="{0:HH:mm}" />
                <telerik:GridDateTimeColumn DataField="End" HeaderText="End Time" UniqueName="End" DataFormatString="{0:HH:mm}" />
            </Columns>
        </MasterTableView>
        <ClientSettings AllowRowsDragDrop="true">
            <Selecting AllowRowSelect="true" />
            <ClientEvents OnRowDropping="gridRowDropping" />
        </ClientSettings>
    </telerik:RadGrid>

    <script type="text/javascript">
    function gridRowDropping(sender, args) {
        var scheduler = $find('<%= RadScheduler1.ClientID %>');

        var scheduler_date = parseDateToYearMonthDate(scheduler.get_selectedDate());
        var grid_startTime = args.get_draggedItems()[0].get_element().cells[1].innerHTML;
        var grid_endTime = args.get_draggedItems()[0].get_element().cells[2].innerHTML;

        var start = new Date(scheduler_date + " " + grid_startTime);
        var end = new Date(scheduler_date + " " + grid_endTime);

        var newAppointment = new Telerik.Web.UI.SchedulerAppointment();
        newAppointment.get_attributes().setAttribute('ScheduleID', <%= Model.ScheduleID %>)
        newAppointment.set_start(start);
        newAppointment.set_end(end);
        newAppointment.set_subject("Test");

        scheduler.insertAppointment(newAppointment);
        scheduler.editAppointment(newAppointment);
    }

    function saveOrUpdateAppointment(sender, eventArgs) {
        alert('hi');
    }

    function cancelEvent(sender, eventArgs)
    {
        eventArgs.set_cancel(true);
    }

    function parseDateToYearMonthDate(full_date) {
        var mnth = full_date.getMonth() + 1; //months are 0 based
        return full_date.getFullYear() + "/" + mnth + "/" + full_date.getDate();
    }                         
    </script>

<%=Html.ActionLink("create new", "Create", "Shift", new { locationId = Model.Location.LocationID }, null)%>
<p>&nbsp;</p>

<telerik:RadScheduler runat="server" ID="RadScheduler1" Height="400px" 
    OnClientAppointmentWebServiceInserting="cancelEvent"
    StartInsertingInAdvancedForm="true"
    CustomAttributeNames="ScheduleID">
	<WebServiceSettings Path="~/DataAccess/Calendar/SchedulerWebService.asmx" />
    <ResourceTypes>
    <telerik:ResourceType KeyField="ID" Name="User" TextField="UserName" ForeignKeyField="UserID"
        DataSourceID="UsersDataSource" />
    </ResourceTypes>
    <ResourceStyles>
        <telerik:ResourceStyleMapping Type="User" Text="Alex" ApplyCssClass="rsCategoryBlue" />
        <telerik:ResourceStyleMapping Type="User" Text="Bob" ApplyCssClass="rsCategoryOrange" />
        <telerik:ResourceStyleMapping Type="User" Text="Charlie" ApplyCssClass="rsCategoryGreen" />
    </ResourceStyles>
</telerik:RadScheduler>
   
<telerik:RadScriptManager runat="server" ID="RadScriptManager1"></telerik:RadScriptManager>
</asp:Content>
