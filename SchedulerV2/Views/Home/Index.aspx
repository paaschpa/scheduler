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
                        item.Text = employee.FirstName;               
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
    <span id="selected_location"></span> <%=Html.ActionLink("new schedule", "create", "schedule", new {id = "create_schedule"}) %>
</p>

<%= Html.Telerik().Grid(new List<SchedulerV2.Models.Schedule>())
                .Name("Grid")
                .HtmlAttributes(new { style="width:300px" })
                .Columns(columns =>
                    {
                        columns.Bound(x => x.ScheduleID).Width(50);
                        columns.Bound(x => x.StartDate).Width(125);
                        columns.Bound(x => x.EndDate);
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

<script>
    function treeView_OnSelect(e) {
        if (e.item.childElementCount == 1) {
            //alert(this.getItemValue(e.item));
            $("#selected_location").html(this.getItemText(e.item));
            $("#create_schedule").attr("href", "/Schedule/Create?locationID=" + this.getItemValue(e.item));         
        }              
    }
</script>
</asp:Content>
