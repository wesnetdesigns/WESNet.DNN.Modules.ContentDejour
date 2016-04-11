<%@ Control Language="C#" AutoEventWireup="false" Inherits="WESNet.DNN.Modules.ContentDejour.EditContentDejour" Codebehind="EditContentDejour.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Import Namespace="WESNet.DNN.Modules.ContentDejour" %>

<div class="dnnForm WESNet_ContentDejour dnnClear">
	<asp:UpdatePanel ID="UpdateCriteria" runat="server">
		<ContentTemplate>
				<div class="dnnFormItem">
					<dnn:Label ID="plMonths" runat="server" ControlName="cblMonths" />
					<div class="Months">
						<asp:CheckBoxList ID="cblMonths" runat="server" RepeatDirection="Horizontal" RepeatColumns="6" CssClass="dnnCheckBoxList" />
						<div class="CommandButtons">
                            <a href="#" id="btnSelectAllMonths" class="dnnSecondaryAction"><%=LocalizeString("btnSelectAllMonths")%></a>
                            <a href="#" id="btnClearAllMonths" class="dnnSecondaryAction"><%=LocalizeString("btnClearAllMonths")%></a>
						</div>
					</div>     
				</div>
				<div class="dnnFormItem">
					<dnn:Label ID="plDay" runat="server" ControlName="rblDayMode" />
					<asp:RadioButtonList ID="rblDayMode" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" CssClass="dnnFormRadioButtons" >
						<asp:ListItem ResourceKey="plDaysofMonth" Value="0"></asp:ListItem>
						<asp:ListItem ResourceKey="plDayofYear" Value = "1"></asp:ListItem>
						<asp:ListItem ResourceKey="plDateSpan" Value = "2"></asp:ListItem>
					</asp:RadioButtonList>
					<div id="divDaysofMonth" runat="server" visible="false" class="DaysOfMonth">
						<asp:CheckBoxList ID="cblDaysofMonth" runat="server" RepeatColumns = "11" RepeatDirection="Horizontal" CssClass="dnnCheckBoxList" />
						<div class="CommandButtons">
							<a href="#" id="btnSelectAllDays" class="dnnSecondaryAction"><%=LocalizeString("btnSelectAllDays")%></a>
                            <a href="#" id="btnClearAllDays" class="dnnSecondaryAction"><%=LocalizeString("btnClearAllDays")%></a>
						</div>
					</div>
					<div id="divDayofYear" runat="server" visible="false" class="DaysOfYear">
						<asp:DropDownList ID="ddlDayofYear" runat="server" Width="100px"/>
					</div>
					<div id="divDateSpan" runat="server" visible="false" class="DateSpan">
						<asp:Label ID="plStartDate" runat="server" ResourceKey="plStartDate" CssClass="NormalBold"></asp:Label>&nbsp;
						<asp:TextBox id="pickerStartDate" runat="server" CssClass="jQDatePicker" />
						<asp:CompareValidator ID="valStartDate" ControlToValidate="pickerStartDate" Operator="DataTypeCheck"
							Type="Date" runat="server" Display="Dynamic" resourcekey="valStartDate.ErrorMessage" CssClass="dnnFormMessage dnnFormError" />
						<asp:Label ID="plEndDate" runat="server" ResourceKey="plEndDate" CssClass="NormalBold"></asp:Label>&nbsp;
						<asp:TextBox id="pickerEndDate" runat="server" CssClass="jQDatePicker"  />
						<asp:CompareValidator ID="valEndDate" ControlToValidate="pickerEndDate" Operator="DataTypeCheck"
							Type="Date" runat="server" Display="Dynamic" resourcekey="valEndDate.ErrorMessage" CssClass="dnnFormMessage dnnFormError" /><br />
						<asp:Checkbox ID="cbIgnoreYear" runat="server" Checked="true" ResourceKey="plIgnoreYear" />
					</div>
				 </div>
				 <div class="dnnFormItem">
					<dnn:Label ID="plDaysofWeek" runat="server" ControlName="cblDaysofWeek" />
					<div class="DaysOfWeek">
						<asp:CheckBoxList ID="cblDaysofWeek" runat="server" RepeatDirection="Horizontal" RepeatColumns="7" CssClass="dnnCheckBoxList" ></asp:CheckBoxList>
						<div class="CommandButtons">
							<a href="#" id="btnSelectAllDaysofWeek" class="dnnSecondaryAction"><%=LocalizeString("btnSelectAllDaysofWeek")%></a>
                            <a href="#" id="btnClearAllDaysofWeek" class="dnnSecondaryAction"><%=LocalizeString("btnClearAllDaysofWeek")%></a>
						</div>
					</div>
				 </div>
				 <div class="dnnFormItem">
						<dnn:Label ID="plCategory" runat="server" ControlName="ddlCategory" />
						<asp:DropDownList ID="ddlCategory" runat="server" DataTextField="Category" DataValueField="CategoryID" />
				 </div>
				 <div class="dnnFormItem">
					 <dnn:Label id="plGroup" runat="server" ControlName="ddlGroup"></dnn:Label>
					 <asp:DropDownList id="ddlGroup" runat="server" DataTextField="GroupName" DataValueField="GroupID"></asp:DropDownList>
				 </div>
				 <div class="dnnFormItem" id="divProfilePropertyValue" runat="server" >
					<dnn:Label ID="lblProfilePropertyValue" runat="server" ControlName="ctlProfilePropertyValue" />
					<asp:PlaceHolder ID="phProfilePropertyValue" runat="server"><br /></asp:PlaceHolder>
					<asp:CheckBoxList ID="cblUnauthenticatedOrDefaultValue" runat="server" AutoPostBack="True" RepeatDirection="Vertical" RepeatLayout="Flow">
							<asp:ListItem ResourceKey = "plProfilePropertyValueUnauthenticated" Value = "1"></asp:ListItem>
							<asp:ListItem ResourceKey = "plProfilePropertyValueDefault" Value = "2"></asp:ListItem>
					</asp:CheckBoxList>
				  </div>
				<div class="dnnFormItem">
					<dnn:Label ID="plTimePeriod" runat="server" ControlName="ddlStartTime" />
					<asp:Label ID="plStartTime" runat="server" CssClass="NormalBold" resourcekey="plStartTime" ></asp:Label>&nbsp;
					<asp:DropDownList ID="ddlStartTime" runat="server" CssClass="narrowField" ></asp:DropDownList>&nbsp;
					<asp:Label id="plEndTime" runat="Server" CssClass="NormalBold" resourcekey="plEndTime"></asp:Label>&nbsp;
					<asp:DropDownList ID="ddlEndTime" runat="server" CssClass="narrowField" ></asp:DropDownList>
				</div>
				<div class="dnnFormItem">
					<dnn:Label ID="plDisabled" runat="server" ControlName="cbDisabled" />
					<asp:CheckBox ID="cbDisabled" runat="server" />
				</div>
				<ul class="dnnActions dnnClear">
					<li><asp:LinkButton ID="cmdFind" runat="server" CssClass="dnnSecondaryAction" ResourceKey="cmdFind" Text="Find" CausesValidation="false" Visible="false" /></li>
					<li><asp:LinkButton ID="cmdAdd" runat="server" CssClass="dnnSecondaryAction" ResourceKey="cmdAdd" Text="Add" CausesValidation="false" Visible="false" /></li>
					<li><asp:LinkButton ID="cmdReturn" runat="server" CssClass="dnnSecondaryAction" ResourceKey="cmdReturn" Text="Return" CausesValidation="false" /></li>
				</ul> 
		</ContentTemplate>
	</asp:UpdatePanel>

	<div id="divSelect" runat="server" Visible="false">
		<asp:DataGrid ID="dgSelectContent" runat="server" AutoGenerateColumns="false" AllowSorting ="true" DataKeyField="KeyID" CssClass="dnnGrid"
			AllowPaging="true" PageSize="10" PagerStyle-Mode="NumericPages">
			<HeaderStyle CssClass="dnnGridHeader" />
			<ItemStyle CssClass="dnnGridItem" horizontalalign="Left" />
			<AlternatingItemStyle CssClass="dnnGridAltItem" />
			<PagerStyle CssClass="dnnGridPager" />
			<Columns>
				<asp:TemplateColumn HeaderText="Months" SortExpression="Months">
					<ItemTemplate>
						<asp:Label ID="lblMonths" runat="server" Text='<%#GetMonthsString((MonthArray)Eval("Months"))%>'></asp:Label>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn HeaderText="Days" SortExpression="Days">
					<ItemTemplate>
						<asp:Label ID="lblDays" runat="server" Text='<%#GetDaysString((DayArray)Eval("Days"))%>'></asp:Label>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn HeaderText="DaysofWeek" SortExpression="DaysofWeek">
					<ItemTemplate>
						<asp:Label ID="lblDayofWeek" runat="server" Text='<%#GetDaysofWeekString((DayofWeekArray)Eval("DaysofWeek"))%>'></asp:Label>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn HeaderText="TimePeriod" SortExpression="StartTime">
					<ItemTemplate>
						<asp:Label ID="lblTimePeriod" runat="server" Text='<%#GetDateSpanString((DateInteger)Eval("StartDate"), (DateInteger)Eval("EndDate")) + " " + GetTimePeriodString((int)Eval("StartTime"), (int)Eval("EndTime"))%>'></asp:Label>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:BoundColumn DataField="Category" HeaderText="Category" SortExpression="Category" />
				<asp:TemplateColumn HeaderText = "ProfilePropertyValue" SortExpression="ProfileProperty">
					<ItemTemplate>
						<asp:Label ID="lblProfileProperty" runat="server" Text='<%#FormatProfilePropertyValue((string)Eval("ProfilePropertyValue"))%>'></asp:Label>
					</ItemTemplate>
				</asp:TemplateColumn>
                <asp:BoundColumn DataField="GroupName" HeaderText="GroupName" SortExpression="GroupName" />
 				<asp:BoundColumn DataField="Title" HeaderText="Title" SortExpression="Title" ItemStyle-CssClass="titleColumn"></asp:BoundColumn>
				<asp:TemplateColumn HeaderText="Summary" ItemStyle-CssClass="summaryColumn">
					<ItemTemplate>
						<asp:Label ID="lblSummary" runat="server" Text='<%#WESNet.DNN.Modules.ContentDejour.Utilities.GetSummary((string)Eval("DesktopSummary"), (string)Eval("DesktopHTML"))%>'></asp:Label>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<ItemTemplate>
						<dnn:DnnImageButton Runat="server" ID="btnDelete" resourcekey="cmdDelete" CommandName="Delete" IconKey="Delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "KeyID")%>'  />
						<dnn:DnnImageButton Runat="server" ID="btnEdit" resourcekey="cmdEdit" IconKey="Edit" CommandName="Edit" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "KeyID")%>' /> 
					</ItemTemplate>
				</asp:TemplateColumn>
			</Columns>            
		</asp:DataGrid>
	</div>
	<div id="divEdit" runat="server" class="WESNet_Edit">
		<fieldset>
			<div class="dnnFormItem">
				<dnn:Label ID="plTitle" runat="server" ControlName="tbTitle" />
				<asp:TextBox id="tbTitle" runat="server"></asp:TextBox>
			</div>
			<div class="dnnFormItem">
				<dnn:texteditor id="teContent" runat="server" height="400" width="100%"></dnn:texteditor>
			</div>
			<div class="dnnFormItem">
				<dnn:label id="plSummary" runat="server" ControlName="tbDesktopSummary" />
				<asp:textbox id="tbDesktopSummary" runat="server" textmode="multiline" rows="12"></asp:textbox>
			</div>
		</fieldset>
		<ul class="dnnActions dnnClear">
			<li><asp:linkbutton CssClass="dnnPrimaryAction" id="cmdUpdate" resourcekey="cmdUpdate" runat="server" text="Update"></asp:linkbutton></li>
			<li><asp:LinkButton CssClass="dnnSecondaryAction" ID="cmdDelete" ResourceKey="cmdDelete" runat="server" Text="Delete" CausesValidation="false" Visible="false"></asp:LinkButton></li>
			<li><asp:linkbutton CssClass="dnnSecondaryAction" id="cmdCancel" resourcekey="cmdCancel" runat="server" text="Cancel" causesvalidation="false"></asp:linkbutton></li>
			<li><asp:linkbutton CssClass="dnnSecondaryAction" id="cmdPreview" resourcekey="cmdPreview" runat="server" text="Preview" causesvalidation="false"></asp:linkbutton></li>
		</ul>
		<div class="WESNet_Preview">
			 <asp:label id="lblPreview" cssclass="Normal" runat="server" />
		</div>
	</div>
</div>
<script type="text/javascript">
    (function ($, Sys) {
        function setUpWESNet_ContentDejour() {
            $('#btnSelectAllMonths').click(function (e) {
                e.preventDefault();
                $('#<%= cblMonths.ClientID %> input:checkbox').attr('checked', true);
            });

            $('#btnClearAllMonths').click(function (e) {
                e.preventDefault();
                $('#<%= cblMonths.ClientID %> input:checkbox').attr('checked', false);
            });

            $('#btnSelectAllDays').click(function (e) {
                e.preventDefault();
                $('#<%= cblDaysofMonth.ClientID %> input:checkbox').attr('checked', true);
            });

            $('#btnClearAllDays').click(function (e) {
                e.preventDefault();
                $('#<%= cblDaysofMonth.ClientID %> input:checkbox').attr('checked', false);
            });

            $('#btnSelectAllDaysofWeek').click(function (e) {
                e.preventDefault();
                $('#<%= cblDaysofWeek.ClientID %> input:checkbox').attr('checked', true);
            });

            $('#btnClearAllDaysofWeek').click(function (e) {
                e.preventDefault();
                $('#<%= cblDaysofWeek.ClientID %> input:checkbox').attr('checked', false);
            });

            $('div.WESNet_ContentDejour input.jQDatePicker').datepicker({
                dateFormat: 'mm/dd/yy',
                changeMonth: true,
                changeYear: true,
                yearRange: '-100 : +1',
                minDate: '-100Y',
                maxDate: '+1Y'
            }).attr("autocomplete", "off");
        }

         $(document).ready(function () {
             setUpWESNet_ContentDejour();
             Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                 setUpWESNet_ContentDejour();
             });
         });

     }(jQuery, window.Sys));

</script>