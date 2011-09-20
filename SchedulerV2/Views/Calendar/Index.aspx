<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SchedulerV2.Models.Schedule>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>


<script runat="server">
	public override void VerifyRenderingInServerForm(Control control)
	{
	
	}
</script>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<telerik:RadScriptManager runat="server" ID="RadScriptManager1"></telerik:RadScriptManager>

<%= Html.Telerik().Grid(Model.Location.Shifts)
        .Name("Grid")
        .Columns(columns =>
        {
            columns.Bound(s => s.Name).Width(100).Title("Shift Name");
            columns.Bound(s => s.Start).Width(100).Title("Start Time").Format("{0:HH:mm}");
            columns.Bound(s => s.End).Width(100).Title("End Time").Format("{0:HH:mm}");
        })
%>
<%=Html.ActionLink("create new", "Create", "Shift", new { locationId = Model.Location.LocationID }, null)%>
<p>&nbsp;</p>

<telerik:RadScheduler runat="server" ID="RadScheduler1" Height="400px">
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
   
</asp:Content>
