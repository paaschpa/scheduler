<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<SchedulerV2.Models.Employee>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<p>Selection Your Location</p>
<%= Html.Telerik().TreeView()
        .Name("TreeView")
        .BindTo(Model, mappings =>
            {
                mappings.For<SchedulerV2.Models.Employee>(binding => binding
                    .ItemDataBound((item, employee) =>
                    {                                                
                        item.Text = employee.DisplayName;               
                    })
                    .Children(employee => employee.Locations));

                mappings.For<SchedulerV2.Models.Location>(binding => binding                       
                         .ItemDataBound((item, location) =>
                        {
                            item.Text = location.Name;
                            item.Value = location.LocationID.ToString();                            
                        })
                        );

            })
            .ClientEvents(events => 
                events.OnSelect("treeView_OnSelect")                
            )
%>

<p>
    Select a Schedule or create a schedule for location 
    <span id="selected_location"></span> 
</p>

<%= Html.Telerik().Grid(new List<SchedulerV2.Models.Schedule>())
                .Name("Grid")
                .HtmlAttributes(new { style="width:300px" })
                .Columns(columns =>
                    {
                        columns.Bound(x => x.ScheduleID).Width(50);
                        columns.Bound(x => x.StartDate).Width(125).Format("{0:MM/dd/yyyy}");
                        columns.Bound(x => x.EndDate).Format("{0:MM/dd/yyyy}");
                        columns.Bound(x => x.ScheduleID)
                            .Format("<a href='/Calendar/Index?scheduleId={0}'>Edit Calendar</a>")
                            .Encoded(false)
                            .Title("Edit");
                    }
                )
                .DataBinding(dataBinding => dataBinding
                    .Ajax().Select("GetSchedules", "Home", new { locationId = 1 })));
            //.Columns(columns =>
            //{
            //    columns.Bound(x => x.ScheduleID).Width(100);
            //});
            //.DataBinding(dataBinding => dataBinding.Ajax().Select("GetSchedules", "Home"))
            ////.DataBinding(d => d.Ajax().Select("Home","GetSchedules"));
%>
<%=Html.ActionLink("create new", "Create", "Schedule", new {id = "create_schedule"}) %>

<script type="text/javascript">
    function treeView_OnSelect(e) {
        if (e.item.childElementCount == 1 || e.item.childNodes.length == 1) {
            //alert(this.getItemValue(e.item));
            var locationId = this.getItemValue(e.item);
            $("#selected_location").html(this.getItemText(e.item));
            $("#create_schedule").attr("href", "/Schedule/Create?locationID=" + locationId);
            var grid = $("#Grid").data("tGrid");
            grid.ajax.selectUrl = "/Home/GetSchedules?locationId=" + locationId;
            grid.ajaxRequest();
        }              
    }
</script>
</asp:Content>
