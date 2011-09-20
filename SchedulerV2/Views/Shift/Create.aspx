<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SchedulerV2.Models.Shift>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<style type="text/css">

    #TimePicker_wrapper
    {
        width: 125px;
    }
     
</style>



New Shift

<% using (Html.BeginForm("Create", "Shift"))
   { %>
   <fieldset>        
        <p>
            <%= Html.HiddenFor(x => x.LocationID) %>            
        </p>
        
        <p>
            <%= Html.LabelFor(x => x.Name) %>
            <%= Html.TextBoxFor(x => x.Name) %>
        </p>

        <div>
        <p>
            <%= Html.Telerik().TimePicker()
                    .Name("Start")
                    .HtmlAttributes(new { id = "TimePicker_wrapper" })
                    .Min(new DateTime(1900, 1, 1, 0, 0, 0))
                    .Max(new DateTime(1900, 1, 1, 23, 45, 0))                    
                    .ShowButton(true)
                    .Interval(15)
            %>
        </p>

        <p>  
            <%= Html.Telerik().TimePicker()
                    .Name("End")
                    .HtmlAttributes(new { id = "TimePicker_wrapper" })
                    .Min(new DateTime(1900, 1, 1, 0, 0, 0))
                    .Max(new DateTime(1900, 1, 1, 23, 45, 0))    
                    .ShowButton(true)
                    .Interval(15)                                      
            %>
        </p>
        </div>
        <input type="submit" value="Create" />
    </fieldset>
<% } %>

</asp:Content>
