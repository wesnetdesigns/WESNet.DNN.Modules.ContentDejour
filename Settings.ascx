<%@ Control Language="C#" AutoEventWireup="false" Inherits="WESNet.DNN.Modules.ContentDejour.Settings" Codebehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register Src="CategoryEditor.ascx" TagName="CategoryEditor" TagPrefix="wes" %>
<div id="WESNet_CD_Settings" class="dnnForm WESNet_ContentDejour dnnClear">
    <div class="dnnFormExpandContent"><a href=""><%=Localization.GetString("ExpandAll", Localization.SharedResourceFile)%></a></div>
	<h2 id="H1" class="dnnFormSectionHead"><a href=""><%=LocalizeString("GeneralSettings")%></a></h2>
	<fieldset>
		<div class="dnnFormItem">
			<dnn:Label ID="plSelectBy" runat="server" ControlName="rblSelectBy" />
			<asp:RadioButtonList ID="rblSelectBy" runat="server" RepeatColumns="2" RepeatDirection="Vertical" class="dnnFormRadioButtons">
				<asp:ListItem ResourceKey="Random" Value="0" Selected="True"></asp:ListItem>
				<asp:ListItem ResourceKey="Month" Value="1"></asp:ListItem>
				<asp:ListItem ResourceKey="DayofMonth" Value="2"></asp:ListItem>
				<asp:ListItem ResourceKey="DayofYear" Value="3"></asp:ListItem>
				<asp:ListItem ResourceKey="MonthAndDayofMonth" Value="4"></asp:ListItem>
				<asp:ListItem ResourceKey="DaysofWeek" Value="5"></asp:ListItem>
				<asp:ListItem ResourceKey="MonthAndDaysofWeek" Value="6"></asp:ListItem>
				<asp:ListItem ResourceKey="TimeSpan" Value="7"></asp:ListItem>
				<asp:ListItem ResourceKey="DateSpan" Value="8"></asp:ListItem>
				<asp:ListItem ResourceKey="DateAndTimeSpan" Value="9"></asp:ListItem>
			</asp:RadioButtonList>
		</div>
		<div class="dnnFormItem">
			<dnn:Label ID="plMultipleHandling" runat="server" ControlName="rblMultipleHandling" />
			<asp:RadioButtonList ID="rblMultipleHandling" runat="server" RepeatColumns="2" RepeatDirection="Vertical" class="dnnFormRadioButtons">
				<asp:ListItem ResourceKey="Random" Value="0" Selected="True"></asp:ListItem>
				<asp:ListItem ResourceKey="First" Value="1"></asp:ListItem>
				<asp:ListItem ResourceKey="Sequential" Value="2"></asp:ListItem>
				<asp:ListItem ResourceKey="Last" Value="3"></asp:ListItem>
				<asp:ListItem ResourceKey="LeastViewed" Value="4"></asp:ListItem>
				<asp:ListItem ResourceKey="TimeSpan" Value="5"></asp:ListItem>          
			</asp:RadioButtonList>
		</div>
		<div class="dnnFormItem">
			<dnn:Label ID="plEnableUserTimeConversion" runat="server" ControlName="cbEnableUserTimeConversion" />
			<asp:CheckBox ID="cbEnableUserTimeConversion" runat="server" />
		</div>
		<div class="dnnFormItem">
			<dnn:Label ID="plCategory" runat="server" ControlName="ddlCategory" />
			<asp:DropDownList ID="ddlCategory" runat="server" DataTextField="Category" DataValueField="CategoryID" />
		</div>
		<div class="dnnFormItem">
			<dnn:Label ID="plProfilePropertyName" runat="server" ControlName="ddlProfilePropertyName" />
			<asp:DropDownList ID="ddlProfilePropertyName" runat="server" />
		</div>
		<div class="dnnFormItem">
			<dnn:Label ID="plIncludeDisabled" runat="server" ControlName="cbIncludeDisabled" />
			<asp:CheckBox ID="cbIncludeDisabled" runat="server" />
		</div>
		<div class="dnnFormItem">
			<dnn:Label ID="plHideWhenNoContent" runat="server" ControlName="cbHideWhenNoContent" />
			<asp:CheckBox ID="cbHideWhenNoContent" runat="server" />
		</div>
		<div class="dnnFormItem">
			<dnn:Label ID="plReplaceTitle" runat="server" ControlName="cbReplaceTitle" />
			<asp:CheckBox ID="cbReplaceTitle" runat="server" />
		</div>
		<div class="dnnFormItem">
			<dnn:Label ID="plReplaceTokens" runat="server" ControlName="cbReplaceTokens" />
			<asp:CheckBox ID="cbReplaceTokens" runat="server" />
		</div>
		<div class="dnnFormItem">
			<dnn:Label ID="plInterval" runat="server" ControlName="tbInterval" />
            <asp:TextBox ID="tbNumericInterval" runat="server" CssClass="jQSpinner"></asp:TextBox>
            <asp:RangeValidator ID="valNumericInterval" runat="server" MinimumValue="1" MaximumValue="60" ControlToValidate="tbNumericInterval"
                      Type="Integer" Display="Dynamic" ResourceKey="valNumericInterval.ErrorMessage" CssClass="dnnFormMessage dnnFormError"></asp:RangeValidator>
		</div>
	</fieldset>
	<h2 id="dnnPanel-CategoryEditor" class="dnnFormSectionHead"><a href=""><%=LocalizeString("CategoryEditor")%></a></h2>
	<fieldset>
		<wes:CategoryEditor ID="ctlCategoryEditor" runat="server" />
	</fieldset>
</div>
<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        function setUpWESNet_CD_Settings() {
            $('#WESNet_CD_Settings').dnnPanels();
            $('#WESNet_CD_Settings .dnnFormExpandContent a').dnnExpandAll({
                expandText: '<%= Localization.GetSafeJSString("ExpandAll", Localization.SharedResourceFile) %>',
                collapseText: '<%= Localization.GetSafeJSString("CollapseAll", Localization.SharedResourceFile) %>',
                targetArea: '#WESNet_CD_Settings'
            });

            $('#<%= tbNumericInterval.ClientID %>').spinner({ min: 1, max: 60, step: 1, numberFormat: 'n0' });
        }

        $(document).ready(function () {
            setUpWESNet_CD_Settings();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                setUpWESNet_CD_Settings();
            });
        });
    } (jQuery, window.Sys));
</script>