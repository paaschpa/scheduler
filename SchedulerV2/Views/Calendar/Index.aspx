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
        newAppointment.set_start(start);
        newAppointment.set_end(end);
        newAppointment.set_subject("Test");

        scheduler.insertAppointment(newAppointment);
        scheduler.editAppointment(newAppointment);
    }

    function cancelEvent(sender, eventArgs)
    {
        eventArgs.set_cancel(true);
    }

    function parseDateToYearMonthDate(full_date) {
        var mnth = full_date.getMonth() + 1; //months are 0 based
        return full_date.getFullYear() + "/" + mnth + "/" + full_date.getDate();
    }
                
            
//            args.set_cancel(true);
//            Sys.Debug.traceDump({
//                draggedItemsCount: args.get_draggedItems().length,
//                targetItemId: args.get_targetItemId(),
//                targetItemIndex: args.get_targetItemIndexHierarchical(),
//                dropPosition: args.get_dropPosition(),
//                targetHtmlElement: args.get_destinationHtmlElement().tagName
//            });
    </script>

<%=Html.ActionLink("create new", "Create", "Shift", new { locationId = Model.Location.LocationID }, null)%>
<p>&nbsp;</p>

<telerik:RadScheduler runat="server" ID="RadScheduler1" Height="400px" OnClientAppointmentWebServiceInserting="cancelEvent"
    OnClientAppointmentsPopulating="cancelEvent" OnClientResourcesPopulating="cancelEvent" StartInsertingInAdvancedForm="true" >
	<WebServiceSettings Path="~/DataAccess/Calendar/SchedulerWebService.asmx" />
    <AdvancedEditTemplate>     
        <asp:TextBox ID="TitleTextBox" Rows="5" Columns="20" runat="server" Text='<%# Bind("Subject") %>' Width="97%" TextMode="MultiLine">
        </asp:TextBox><br />
        
        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Description") %>' Width="97%">
        </asp:TextBox><br />

        <telerik:RadDateInput ID="StartInput" SelectedDate='<%# Bind("Start") %>' runat="server">
        </telerik:RadDateInput><br />

        <telerik:RadDateInput ID="RadDateInput1" SelectedDate='<%# Bind("End") %>' runat="server">
        </telerik:RadDateInput><br />
    </AdvancedEditTemplate>
    <AdvancedInsertTemplate>
        <asp:TextBox ID="TitleTextBox" Rows="5" Columns="20" runat="server" Text='<%# Bind("Subject") %>' Width="97%" TextMode="MultiLine">
        </asp:TextBox><br />
        
        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Description") %>' Width="97%">
        </asp:TextBox><br />

        <telerik:RadDateInput ID="StartInput" SelectedDate='<%# Bind("Start") %>' runat="server">
        </telerik:RadDateInput><br />

        <telerik:RadDateInput ID="RadDateInput1" SelectedDate='<%# Bind("End") %>' runat="server">
        </telerik:RadDateInput><br />    </AdvancedInsertTemplate>
</telerik:RadScheduler>
   
<telerik:RadScriptManager runat="server" ID="RadScriptManager1"></telerik:RadScriptManager>
</asp:Content>
