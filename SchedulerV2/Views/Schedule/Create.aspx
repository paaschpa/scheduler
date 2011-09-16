<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SchedulerV2.Models.Schedule>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
New Schedule For Location <%= Model.Location.Name %>

<% using (Html.BeginForm("Create", "Schedule"))
   { %>
   <fieldset>
        <%= Html.HiddenFor(x => x.CreatedByUserID) %>
        <p>
            <%= Html.HiddenFor(x => x.Location.LocationID) %>
            <%= Html.LabelFor(x => x.Location) %>
            <%= Model.Location.Name %>
        </p>
        
        <p>
            <%= Html.LabelFor(x => x.StartDate) %>
            <%= Html.Telerik().DatePicker()
                    .Name("StartDate")
                    .Value(DateTime.Now)
            %>
        </p>

        <p>
            <%= Html.LabelFor(x => x.EndDate) %>
            <%= Html.Telerik().DatePicker()
                    .Name("EndDate")
                    .Value(DateTime.Now)
            %>
        </p>

        <p>
            <%= Html.LabelFor(x => x.Approved) %>
            <%= Html.CheckBoxFor(x => x.Approved) %>
        </p>
        <input type="submit" value="Create" />
    </fieldset>
<% } %>
   
</asp:Content>
