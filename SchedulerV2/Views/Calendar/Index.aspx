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
<script type="text/javascript">

var shouldCancelInsert = false;
function gridRowDropping(sender, args) {
    var scheduler = $find('<%= RadScheduler1.ClientID %>');
    var htmlElement = args.get_destinationHtmlElement();
    var dropTimeSlot = scheduler._activeModel.getTimeSlotFromDomElement(htmlElement).get_startTime();

    var scheduler_date = parseDateToYearMonthDate(dropTimeSlot);
    var grid_startTime = args.get_draggedItems()[0].get_element().cells[1].innerHTML;
    var grid_endTime = args.get_draggedItems()[0].get_element().cells[2].innerHTML;

    var start = new Date(scheduler_date + " " + grid_startTime);
    var end = new Date(scheduler_date + " " + grid_endTime);

    var newAppointment = new Telerik.Web.UI.SchedulerAppointment();
    newAppointment.get_attributes().setAttribute('ScheduleID', <%= Model.ScheduleID %>)
    newAppointment.set_start(start);
    newAppointment.set_end(end);
    newAppointment.set_subject("Test");

    shouldCancelInsert = true;
    scheduler.insertAppointment(newAppointment);
    shouldCancelInsert = false;
    scheduler.editAppointment(newAppointment);
}

//function onClientFormCreated(sender, eventArgs) {
//    var mode = eventArgs.get_mode();
//	if (mode == Telerik.Web.UI.SchedulerFormMode.AdvancedInsert || mode == Telerik.Web.UI.SchedulerFormMode.AdvancedEdit) {
//		var inputCurrentValue = $telerik.$("[id*='Form_ResType_Input']").attr("Value");
//		if (inputCurrentValue == "-" || inputCurrentValue == "None") {
//			$telerik.$("[id*='Form_ResType_Input']").attr("Value", "Please select a type");
//			$telerik.$("[id*='Form_ResType_DropDown'] li:eq(0)").html("None");
//		} else {
//			$telerik.$("[id*='Form_ResType_DropDown'] li:eq(0)").html("None");
//		}
//	}
//}

function cancelEvent(sender, eventArgs)
{
    eventArgs.set_cancel(shouldCancelInsert);
}

function parseDateToYearMonthDate(full_date) {
    var mnth = full_date.getMonth() + 1; //months are 0 based
    return full_date.getFullYear() + "/" + mnth + "/" + full_date.getDate();
}        
           
</script>

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

<%=Html.ActionLink("create new", "Create", "Shift", new { locationId = Model.Location.LocationID }, null)%>
<p>&nbsp;</p>


<telerik:RadScheduler runat="server" ID="RadScheduler1" Height="400px"
    OnClientAppointmentWebServiceInserting="cancelEvent"
    StartInsertingInAdvancedForm="true"
    CustomAttributeNames="ScheduleID">
	<WebServiceSettings Path="~/DataAccess/Calendar/SchedulerWebService.asmx" ResourcePopulationMode="ServerSide" />
    <ResourceTypes>
        <telerik:ResourceType KeyField="ID" Name="Employee" TextField="Employee" ForeignKeyField="UserID" />
        <telerik:ResourceType KeyField="ID" Name="Type" TextField="Type" ForeignKeyField="TypeID" />
    </ResourceTypes>

</telerik:RadScheduler>
   
<telerik:RadScriptManager runat="server" ID="RadScriptManager1"></telerik:RadScriptManager>
</asp:Content>
