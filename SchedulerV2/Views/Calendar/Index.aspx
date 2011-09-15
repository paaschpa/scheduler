<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>


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

    <telerik:RadScheduler runat="server" ID="RadScheduler1" Height="400px">
		<WebServiceSettings Path="~/DataAccess/Calendar/SchedulerWebService.asmx" />
    </telerik:RadScheduler>
   
</asp:Content>
