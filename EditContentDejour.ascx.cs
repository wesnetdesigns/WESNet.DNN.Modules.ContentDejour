//Copyright (c) 2008-2016, William Severance, Jr., WESNet Designs
//All rights reserved.

//Redistribution and use in source and binary forms, with or without modification, are permitted
//provided that the following conditions are met:

//Redistributions of source code must retain the above copyright notice, this list of conditions
//and the following disclaimer.

//Redistributions in binary form must reproduce the above copyright notice, this list of conditions
//and the following disclaimer in the documentation and/or other materials provided with the distribution.

//Neither the name of William Severance, Jr. or WESNet Designs may be used to endorse or promote
//products derived from this software without specific prior written permission.

//Disclaimer: THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS
//            OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
//            AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER BE LIABLE
//            FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
//            LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//            INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
//            OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
//            IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

//Although I will try to answer questions regarding the installation and use of this software when
//such questions are submitted via e-mail to the below address, no promise of further
//support or enhancement is made nor should be assumed.

//Developer Contact Information:
//     William Severance, Jr.
//     WESNet Designs
//     679 Upper Ridge Road
//     Bridgton, ME 04009
//     Phone: 207-647-9375
//     E-Mail: bill@wesnetdesigns.com
//     Website: www.wesnetdesigns.com


using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Security.Roles;
using DotNetNuke.Security.Roles.Internal;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Client.ClientResourceManagement;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace WESNet.DNN.Modules.ContentDejour
{
    public partial class EditContentDejour : ContentDejourModuleBase
    {
        private const string DefaultSortExpression = "Months DESC, Days DESC, DaysofWeek DESC, StartTime ASC";
        private SortColumnList _SortColumns = null;
        private DotNetNuke.UI.WebControls.EditControl _ProfilePropertyEditControl = null;
        private string _LocalizedProfilePropertyName;

        public MonthArray Months
        {
            get
            {
                return new MonthArray((short)Utilities.GetSelectedCBLItems(cblMonths));
            }
            set
            {
                Utilities.SelectCBLItems(cblMonths, value.Value);
            }
        }

        public DayArray Days
        {
            get
            {
                switch (rblDayMode.SelectedValue)
                {
                    case "0":
                        return new DayArray(Utilities.GetSelectedCBLItems(cblDaysofMonth));
                    case "1":
                        return new DayArray(Convert.ToInt32(ddlDayofYear.SelectedValue) | DayArray.ModeFlag);
                    case "2":
                        return DayArray.Null;
                    default:
                        return new DayArray();
                }
            }
            set
            {
                if (value.IsNull)
                {
                    SetDayMode(Enums.DayMode.DateSpan);
                }
                else if (value.IsDayOfYear)
                {
                    SetDayMode(Enums.DayMode.DayOfYear);
                    ddlDayofYear.SelectedIndex = value.Value & ~DayArray.ModeFlag;
                }
                else
                {
                    SetDayMode(Enums.DayMode.DaysOfMonth);
                    Utilities.SelectCBLItems(cblDaysofMonth, value.Value);
                }
            }
        }

        public DayofWeekArray DaysofWeek
        {
            get
            {
                return new DayofWeekArray((byte)Utilities.GetSelectedCBLItems(cblDaysofWeek));
            }
            set
            {
                Utilities.SelectCBLItems(cblDaysofWeek, value.Value);
            }
        }

        public int CategoryID
        {
            get
            {
                return Convert.ToInt32(ddlCategory.SelectedValue);
            }
            set
            {
                ListItem li = null;
                li = ddlCategory.Items.FindByValue(value.ToString());
                if (li == null)
                {
                    ddlCategory.SelectedIndex = 0;
                }
                else
                {
                    ddlCategory.SelectedIndex = ddlCategory.Items.IndexOf(li);
                }
            }
        }

        public int GroupID
        {
            get
            {
                return Convert.ToInt32(ddlGroup.SelectedValue);
            }
            set
            {
                ListItem li = null;
                li = ddlGroup.Items.FindByValue(value.ToString());
                if (li == null)
                {
                    ddlGroup.SelectedIndex = 0;
                }
                else
                {
                    ddlGroup.SelectedIndex = ddlGroup.Items.IndexOf(li);
                }
            }
        }

        public int ProfilePropertyUse
        {
            get
            {
                if (!(divProfilePropertyValue.Visible && phProfilePropertyValue.Controls.Count > 1))
                {
                    return -1;
                }
                int i = 0;
                foreach (ListItem li in cblUnauthenticatedOrDefaultValue.Items)
                {
                    if (li.Selected)
                    {
                        i += Convert.ToInt32(li.Value);
                    }
                }
                return i;
            }
            set
            {
                if (divProfilePropertyValue.Visible && phProfilePropertyValue.Controls.Count > 1)
                {
                    if (value == -1)
                    {
                        divProfilePropertyValue.Visible = false;
                    }
                    else
                    {
                        divProfilePropertyValue.Visible = true;
                        foreach (ListItem li in cblUnauthenticatedOrDefaultValue.Items)
                        {
                            li.Selected = ((Convert.ToInt32(li.Value) & value) != 0);
                        }
                        phProfilePropertyValue.Visible = (value == 0); 
                    }
                }
            }
        }

        public string ProfilePropertyValue
        {
            get
            {
                switch (ProfilePropertyUse)
                {
                    case 0:
                        var editor = (DotNetNuke.UI.WebControls.EditControl)(phProfilePropertyValue.Controls[0]);
                        return editor.Value.ToString();
                    case 1:
                        return "<Unauthenticated>";
                    case 2:
                        return "<Default>";
                    default:
                        return string.Empty;
                }
            }
            set
            {
                if (value == "<Unauthenticated>")
                {
                    ProfilePropertyUse = 1;
                }
                else if (value == "<Default>")
                {
                    ProfilePropertyUse = 2;
                }
                else if (value == string.Empty)
                {
                    ProfilePropertyUse = 3;
                }
                else
                {
                    if (phProfilePropertyValue.Controls.Count > 1)
                    {
                        ProfilePropertyUse = 0;
                        var editor = (DotNetNuke.UI.WebControls.EditControl)(phProfilePropertyValue.Controls[0]);
                        editor.Value = value;
                    }
                    else
                    {
                        ProfilePropertyUse = 1;
                    }
                }
            }
        }

        public bool Disabled
        {
            get
            {
                return cbDisabled.Checked;
            }
            set
            {
                cbDisabled.Checked = value;
            }
        }

        public DateInteger StartDate
        {
            get
            {
                if (rblDayMode.SelectedValue == "2" && pickerStartDate.Text != string.Empty)
                {
                    var d = DateTime.MinValue;
                    if (DateTime.TryParse(pickerStartDate.Text, out d))
                    {
                        return new DateInteger(d, cbIgnoreYear.Checked);
                    }
                    else return DateInteger.Null;
                }
                else
                {
                    return DateInteger.Null;
                }
            }
            set
            {
                if (value.IsNull)
                {
                    pickerStartDate.Text = string.Empty;
                }
                else
                {
                    cbIgnoreYear.Checked = !(value.HasYear && (EndDate.IsNull || EndDate.HasYear));
                    pickerStartDate.Text = value.ToDateTime().ToShortDateString();
                }
            }
        }

        public DateInteger EndDate
        {
            get
            {
                if (rblDayMode.SelectedValue == "2" && pickerEndDate.Text != string.Empty)
                {
                    var d = DateTime.MinValue;
                    if (DateTime.TryParse(pickerEndDate.Text, out d))
                    {
                        return new DateInteger(d, cbIgnoreYear.Checked);
                    }
                    else return DateInteger.Null;
                }
                else
                {
                    return DateInteger.Null;
                }
            }
            set
            {
                if (value.IsNull)
                {
                    pickerEndDate.Text = string.Empty;
                }
                else
                {
                    cbIgnoreYear.Checked = !(value.HasYear && (StartDate.IsNull || StartDate.HasYear));
                    pickerEndDate.Text = value.ToDateTime().ToShortDateString();
                }
            }
        }

        public int StartTime
        {
            get
            {
                return Convert.ToInt32(ddlStartTime.SelectedValue);
            }
            set
            {
                if (value > Consts.MaxTime)
                {
                    throw new ArgumentOutOfRangeException("StartTime cannot be greater than " + Consts.MaxTime);
                }
                SetTimeControl(ddlStartTime, value);
            }
        }

        public int EndTime
        {
            get
            {
                return Convert.ToInt32(ddlEndTime.SelectedValue);
            }
            set
            {
                if (value > Consts.MaxTime)
                {
                    throw new ArgumentOutOfRangeException("EndTime cannot be greater than " + Consts.MaxTime);
                }
                SetTimeControl(ddlEndTime, value);
            }
        }

        public string Mode
        {
            get
            {
                if (ViewState["Mode"] == null)
                {
                    return "Add";
                }
                else
                {
                    return Convert.ToString(ViewState["Mode"]);
                }
            }
            set
            {
                switch (value)
                {
                    case "Add":
                        divSelect.Visible = false;
                        cmdFind.Visible = false;
                        cmdAdd.Visible = false;
                        divEdit.Visible = true;
                        cmdDelete.Visible = false;
                        break;
                    case "Edit":
                        divSelect.Visible = false;
                        cmdFind.Visible = false;
                        cmdAdd.Visible = false;
                        divEdit.Visible = true;
                        cmdDelete.Visible = true;
                        break;
                    case "Select":
                        divSelect.Visible = true;
                        cmdFind.Visible = true;
                        divEdit.Visible = false;
                        cmdAdd.Visible = true;
                        cmdDelete.Visible = false;
                        break;
                }
                ViewState["Mode"] = value;
            }
        }

        public SortColumnList SortColumns
        {
            get
            {
                if (_SortColumns == null)
                {
                    if (ViewState["SortColumns"] == null)
                    {
                        _SortColumns = new SortColumnList();
                        ViewState["SortColumns"] = _SortColumns;
                    }
                    else
                    {
                        _SortColumns = (SortColumnList)(ViewState["SortColumns"]);
                    }
                }
                return _SortColumns;
            }
        }

        protected void Page_Init(object sender, System.EventArgs e)
        {
            JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            ClientResourceManager.RegisterStyleSheet(Page, Consts.ModuleFolderPath + "Themes/smoothness/jquery-ui-1.10.0.custom.css");


            if (MyConfiguration.ProfilePropertyName != string.Empty)
            {
                ProfilePropertyDefinition ppd = ProfileController.GetPropertyDefinitionByName(PortalId, MyConfiguration.ProfilePropertyName);
                if (ppd != null)
                {
                    string strEditor = DotNetNuke.UI.WebControls.EditorInfo.GetEditor(ppd.DataType);
                    if (!(string.IsNullOrEmpty(strEditor)))
                    {
                        System.Type editType = System.Type.GetType(strEditor, true, true);
                        _ProfilePropertyEditControl = (DotNetNuke.UI.WebControls.EditControl)Activator.CreateInstance(editType);
                        if (_ProfilePropertyEditControl != null)
                        {
                            _ProfilePropertyEditControl.ID = ppd.PropertyName;
                            _ProfilePropertyEditControl.Name = ppd.PropertyName;
                            _ProfilePropertyEditControl.Required = false;
                            _ProfilePropertyEditControl.EditMode = DotNetNuke.UI.WebControls.PropertyEditorMode.Edit;
                            _LocalizedProfilePropertyName = Localization.GetString("ProfileProperties_" + ppd.PropertyName + ".Text", MyConfiguration.ProfileResourceFile, PortalSettings, UserInfo.Profile.PreferredLocale);
                            if (_LocalizedProfilePropertyName == null)
                            {
                                _LocalizedProfilePropertyName = ppd.PropertyName;
                            }
                            else
                            {
                                _LocalizedProfilePropertyName = _LocalizedProfilePropertyName.TrimEnd(':');
                            }
                            lblProfilePropertyValue.Text = _LocalizedProfilePropertyName + ":";
                            string lblHelp = Localization.GetString("ProfileProperties_" + ppd.PropertyName + ".Help", MyConfiguration.ProfileResourceFile, PortalSettings, UserInfo.Profile.PreferredLocale);
                            if (lblHelp == null)
                            {
                                lblHelp = LocalizeString("plProfilePropertyValue.Help");
                            }
                            lblProfilePropertyValue.HelpText = lblHelp;
                            phProfilePropertyValue.Controls.AddAt(0, _ProfilePropertyEditControl);

                            if (_ProfilePropertyEditControl is DotNetNuke.UI.WebControls.DNNListEditControl)
                            {
                                var ddlProfilePropertyValue = (DotNetNuke.UI.WebControls.DNNListEditControl)_ProfilePropertyEditControl;
                                var attributes = new object[1];
                                attributes[0] = new DotNetNuke.UI.WebControls.ListAttribute(ppd.PropertyName, "", DotNetNuke.UI.WebControls.ListBoundField.Text, DotNetNuke.UI.WebControls.ListBoundField.Text);
                                ddlProfilePropertyValue.CustomAttributes = attributes;
                            }
                            _ProfilePropertyEditControl.Value = ppd.DefaultValue;
                        }
                    }
                }
            }

            divProfilePropertyValue.Visible = _ProfilePropertyEditControl != null;

            cblUnauthenticatedOrDefaultValue.SelectedIndexChanged += cblUnauthenticatedOrDefaultValue_SelectedIndexChanged;
            dgSelectContent.SortCommand += dgSelectContent_SortCommand;
            dgSelectContent.PageIndexChanged += dgSelectContent_PageIndexChanged;
            dgSelectContent.ItemCreated += dgSelectContent_ItemCreated;
            cmdDelete.Click += cmdDelete_Click;
            cmdReturn.Click += cmdReturn_Click;
            cmdAdd.Click += cmdAdd_Click;
            cmdFind.Click += cmdFind_Click;
            rblDayMode.SelectedIndexChanged += rblDayMode_SelectedIndexChanged;
            dgSelectContent.EditCommand += dgSelectContent_EditCommand;
            dgSelectContent.DeleteCommand += dgSelectContent_DeleteCommand;
            cmdUpdate.Click += cmdUpdate_Click;
            cmdPreview.Click += cmdPreview_Click;
            cmdCancel.Click += cmdCancel_Click;
            this.Load += Page_Load;

        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                if (!IsEditable)
                {
                    Response.Redirect(Globals.AccessDeniedURL(LocalizeSharedResource("No_Edit_Permission")), true);
                }

                DotNetNuke.Framework.AJAX.RegisterScriptManager();
                DotNetNuke.Framework.AJAX.RegisterPostBackControl(cmdFind);
                DotNetNuke.Framework.AJAX.RegisterPostBackControl(cmdAdd);
                DotNetNuke.Framework.AJAX.RegisterPostBackControl(cmdReturn);

                string validationGroup = "WESNet_ContentDejour_Update" + ModuleId.ToString();
                valStartDate.ValidationGroup = validationGroup;
                valEndDate.ValidationGroup = validationGroup;
                cmdUpdate.ValidationGroup = validationGroup;

                DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(cmdDelete, LocalizeString("CONFIRM_DELETE"));

                if (!IsPostBack)
                {
                    Localization.LocalizeDataGrid(ref dgSelectContent, LocalResourceFile);
                    LocalizeCheckBoxList(cblUnauthenticatedOrDefaultValue);
                    if (_ProfilePropertyEditControl == null)
                    {
                        dgSelectContent.Columns[5].Visible = false;
                    }
                    else
                    {
                        dgSelectContent.Columns[5].HeaderText = _LocalizedProfilePropertyName;
                    }
                    PopulateListControls();
                    if (Request.QueryString["KeyID"] != null)
                    {
                        KeyID = int.Parse(Request.QueryString["KeyID"]);
                    }
                    if (KeyID > 0)
                    {
                        Mode = "Edit";
                        BindData(true);
                    }
                    else
                    {
                        //Clear form field values
                        ClearFields();
                        if (KeyID == Null.NullInteger)
                        {
                            Mode = "Add";
                        }
                        else if (KeyID == 0)
                        {
                            Mode = "Select";
                            SortColumns.SetInitialSort(DefaultSortExpression);
                            BindGrid(false);
                        }
                        else
                        {
                            Response.Redirect(Globals.AccessDeniedURL(string.Format(LocalizeSharedResource("No_Access_Invalid_KeyID"), KeyID)), true);
                        }
                    }
                }
                if (cblMonths.Items[12].Text == string.Empty)
                {
                    cblMonths.Items[12].Attributes.Add("style", "display:none");
                }

            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void LocalizeCheckBoxList(CheckBoxList cbl)
        {
            foreach (ListItem li in cbl.Items)
            {
                if (li.Attributes["ResourceKey"] != null)
                {
                    li.Text = LocalizeString(li.Attributes["ResourceKey"]);
                }
            }
        }

        private void ClearFields()
        {
            Utilities.SelectAllCBLItems(cblMonths);
            SetDayMode(Enums.DayMode.DaysOfMonth);
            Utilities.SelectAllCBLItems(cblDaysofMonth);
            Utilities.SelectAllCBLItems(cblDaysofWeek);
            ddlCategory.SelectedIndex = 0;
            ddlGroup.SelectedIndex = 0;
            ProfilePropertyUse = 0;
            pickerStartDate.Text = "";
            pickerEndDate.Text = "";
            cbIgnoreYear.Checked = false;
            ddlStartTime.SelectedIndex = 0;
            ddlEndTime.SelectedIndex = ddlEndTime.Items.Count - 1;
            cbDisabled.Checked = false;
            tbTitle.Text = string.Empty;
            teContent.RichText.Text = string.Empty;
            tbDesktopSummary.Text = string.Empty;
        }

        private void BindData(bool Reload)
        {
            if (Reload)
            {
                RefreshCurrentItem();
            }
            Months = CurrentItem.Months;
            PopulateDayControls();
            Days = CurrentItem.Days;
            DaysofWeek = CurrentItem.DaysofWeek;
            CategoryID = CurrentItem.CategoryID;
            GroupID = CurrentItem.GroupID;
            ProfilePropertyValue = CurrentItem.ProfilePropertyValue;
            StartDate = CurrentItem.StartDate;
            EndDate = CurrentItem.EndDate;
            StartTime = CurrentItem.StartTime;
            EndTime = CurrentItem.EndTime;
            cbDisabled.Checked = CurrentItem.Disabled;
            tbTitle.Text = HttpUtility.HtmlDecode(CurrentItem.Title);
            teContent.Text = CurrentItem.DesktopHTML;
            tbDesktopSummary.Text = HttpUtility.HtmlDecode(CurrentItem.DesktopSummary);
            //Create new ContentItem if one does not already exist (i.e. after upgrade of module from earlier version)
            SaveContentItem(CurrentItem, false);
        }

        private void PopulateListControls()
        {
            var monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            cblMonths.Items.Clear();
            for (int i = 0; i < monthNames.Length; i++)
            {
                cblMonths.Items.Add(new ListItem(monthNames[i], i.ToString()));
            }
            
            var weekdayNames = CultureInfo.CurrentCulture.DateTimeFormat.DayNames;
            cblDaysofWeek.Items.Clear();
            for (int i = 0; i <= 6; i++)
            {
                cblDaysofWeek.Items.Add(new ListItem(weekdayNames[i], i.ToString()));
            }
            
            PopulateTimeControl(ddlStartTime, MyConfiguration.Interval);
            PopulateTimeControl(ddlEndTime, MyConfiguration.Interval);

            ContentDejourController cdc = new ContentDejourController();
            ddlCategory.DataSource = cdc.GetCategories(ModuleId);
            ddlCategory.DataBind();
            ddlCategory.Items.Insert(0, new ListItem(LocalizeSharedResource("ANY"), "-1"));

            Func<RoleInfo, bool> groupPredicate = r => r.SecurityMode != SecurityMode.SecurityRole && r.Status == RoleStatus.Approved && (r.IsPublic || UserInfo.IsInRole(r.RoleName));
            var groups = TestableRoleController.Instance.GetRoles(PortalId, groupPredicate).Select(r => new { GroupName = r.RoleName, GroupID = r.RoleID });
            ddlGroup.DataSource = groups;
            ddlGroup.DataBind();
            ddlGroup.Items.Insert(0, new ListItem(LocalizeSharedResource("ANY"), "-1"));         
        }

        private void PopulateDayControls()
        {
            switch (rblDayMode.SelectedValue)
            {
                case "0":
                    divDaysofMonth.Visible = true;
                    divDayofYear.Visible = false;
                    divDateSpan.Visible = false;
                    cblDaysofMonth.Items.Clear();
                    for (int i = 1; i <= 31; i++)
                    {
                        cblDaysofMonth.Items.Add(new ListItem(i.ToString(), (i - 1).ToString()));
                    }
                    break;
                case "1":
                    divDaysofMonth.Visible = false;
                    divDayofYear.Visible = true;
                    divDateSpan.Visible = false;
                    ddlDayofYear.Items.Clear();
                    for (int i = 1; i <= 366; i++)
                    {
                        ddlDayofYear.Items.Add(new ListItem(i.ToString()));
                    }
                    ddlDayofYear.Items.Insert(0, new ListItem(LocalizeSharedResource("ANY"), "-1"));
                    break;
                case "2":
                    divDaysofMonth.Visible = false;
                    divDayofYear.Visible = false;
                    divDateSpan.Visible = true;
                    break;
            }
        }

        private void PopulateTimeControl(DropDownList ddl, int Interval)
        {
            ddl.Items.Clear();
            DateTime ts = DateTime.MinValue;
            DateTime tsMax = ts.Add(TimeSpan.FromMinutes(Consts.MaxTime));
            int i = 0;
            while (i < Consts.MaxTime)
            {
                ddl.Items.Add(new ListItem(string.Format("{0:t}", ts.AddMinutes(Convert.ToDouble(i))), i.ToString()));
                i += Interval;
            }
            ddl.Items.Add(new ListItem(string.Format("{0:t}", tsMax), Consts.MaxTime.ToString()));
        }

        private void SetTimeControl(DropDownList ddl, int Minutes)
        {
            int i = 0;
            while (i < ddl.Items.Count)
            {
                switch (Convert.ToInt32(ddl.Items[i].Value).CompareTo(Minutes))
                {
                    case 0:
                        ddl.SelectedIndex = i;
                        return;
                    case 1:
                        ddl.Items.Insert(i, new ListItem(string.Format("{0:t}", TimeSpan.FromMinutes(Minutes)), Minutes.ToString()));
                        ddl.SelectedIndex = i;
                        return;
                }
                i++;
            }
        }

        public string GetDaysofWeekString(DayofWeekArray daysOfWeek)
        {
            return daysOfWeek.ToString(CultureInfo.CurrentCulture);
        }

        public string GetMonthsString(MonthArray months)
        {
            return months.ToString(CultureInfo.CurrentCulture);
        }

        public string GetDaysString(DayArray days)
        {
            return days.ToString(CultureInfo.CurrentCulture);
        }

        public string GetTimePeriodString(int StartTime, int EndTime)
        {
            if (StartTime == Consts.MinTime && EndTime == Consts.MaxTime)
            {
                return string.Empty;
            }
            else
            {
                return string.Format("{0:t} - {1:t}", TimeSpan.FromMinutes(StartTime), TimeSpan.FromMinutes(EndTime));
            }
        }

        public string GetDateSpanString(DateInteger StartDate, DateInteger EndDate)
        {
            if (StartDate.IsNull || EndDate.IsNull)
            {
                return string.Empty;
            }
            else
            {
                string startDateString = StartDate.ToString("MMM dd");
                string endDateString = EndDate.ToString("MMM dd");
                return startDateString + "-" + endDateString;
            }
        }

        private void SetDayMode(Enums.DayMode mode)
        {
            switch (mode)
            {
                case Enums.DayMode.DaysOfMonth:
                    rblDayMode.SelectedValue = "0";
                    break;
                case Enums.DayMode.DayOfYear:
                    rblDayMode.SelectedValue = "1";
                    break;
                case Enums.DayMode.DateSpan:
                    rblDayMode.SelectedValue = "2";
                    break;
            }
            PopulateDayControls();
        }

        private void BindGrid()
        {
            BindGrid(false);
        }

        private void BindGrid(bool PerformSorting)
        {
            var cdc = new ContentDejourController();
            List<ContentDejourInfo> objContents = null;
            objContents = cdc.FindContents(ModuleId, Months, Days, DaysofWeek, StartDate, EndDate, StartTime, EndTime, CategoryID, GroupID, ProfilePropertyValue, !Disabled);

            if (PerformSorting)
            {
                objContents.Sort(new ContentDejourInfoComparer(SortColumns));
            }

            dgSelectContent.DataSource = objContents;
            dgSelectContent.DataBind();
        }

        private void DeleteCurrentItem()
        {
            if (KeyID > 0)
            {
                if (CurrentItem.ContentItemId != Null.NullInteger)
                {
                    ContentController cc = new ContentController();
                    cc.DeleteContentItem(CurrentItem);
                }
                ContentDejourController cdc = new ContentDejourController();
                cdc.DeleteContent(KeyID);
                KeyID = 0;
                Mode = "Select";
                BindGrid(SortColumns.Count > 0);
                InvalidateCurrentItem();
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            try
            {
                switch (Mode)
                {
                    case "Add":
                        KeyID = 0;
                        Mode = "Select";
                        break;
                    case "Edit":
                        KeyID = 0;
                        Mode = "Select";
                        break;
                    case "Select":
                        Response.Redirect(Globals.NavigateURL(), true);
                        break;
                }
                Response.Redirect(Globals.NavigateURL(), true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cmdPreview_Click(object sender, System.EventArgs e)
        {
            try
            {
                LoadData(); // load form fields into CurrentItem
                var content = CurrentItem.Content;
                if (MyConfiguration.ReplaceTokens)
                {
                    TokenReplace tr = new TokenReplace(CurrentItem);
                    tr.ModuleId = ModuleId;
                    tr.AccessingUser = UserInfo;
                    tr.DebugMessages = !(PortalSettings.UserMode == PortalSettings.Mode.View);
                    content = tr.ReplaceEnvironmentTokens(content);
                }
                lblPreview.Text = Globals.ManageUploadDirectory(HttpUtility.HtmlDecode(content), PortalSettings.HomeDirectory);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var cdc = new ContentDejourController();
                try
                {
                    LoadData(); // load form fields into CurrentItem

                    // save the content
                    if (Mode == "Add")
                    {
                        KeyID = cdc.AddContent(CurrentItem, UserId);
                        SaveContentItem(CurrentItem, false);
                    }
                    else if (Mode == "Edit")
                    {
                        cdc.UpdateContent(CurrentItem, UserId);
                        SaveContentItem(CurrentItem, true);
                    }
                    // refresh cache
                    ModuleController.SynchronizeModule(ModuleId);

                    if (!divSelect.Visible && KeyID > 0)
                    {
                        Response.Redirect(Globals.NavigateURL("", "keyid=" + KeyID.ToString()), true);
                    }
                    else
                    {
                        KeyID = 0;
                        Mode = "Select";
                        BindGrid(SortColumns.Count > 0);
                    }
                }
                catch (Exception exc)
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                }
            }
        }

        protected void dgSelectContent_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            KeyID = Convert.ToInt32(e.CommandArgument);
            DeleteCurrentItem();
        }

        protected void dgSelectContent_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            KeyID = Convert.ToInt32(e.CommandArgument);
            if (KeyID > 0)
            {
                Mode = "Edit";
                BindData(true);
            }
        }

        private void LoadData()
        {

            CurrentItem.Months = Months;
            CurrentItem.Days = Days;
            CurrentItem.DaysofWeek = DaysofWeek;
            CurrentItem.CategoryID = CategoryID;
            CurrentItem.GroupID = GroupID;
            CurrentItem.ProfilePropertyValue = ProfilePropertyValue;
            CurrentItem.StartDate = StartDate;
            CurrentItem.EndDate = EndDate;
            CurrentItem.StartTime = StartTime;
            CurrentItem.EndTime = EndTime;
            CurrentItem.Disabled = Disabled;
            CurrentItem.Title = HttpUtility.HtmlEncode(tbTitle.Text);
            CurrentItem.DesktopHTML = teContent.Text;
            CurrentItem.DesktopSummary = HttpUtility.HtmlEncode(tbDesktopSummary.Text);

        }

        public string FormatProfilePropertyValue(string value)
        {
            if (value == "<Unauthenticated>" || value == "<Default>")
            {
                return Localization.GetString(value + ".Text", MyConfiguration.LocalSharedResourceFile);
            }
            else
            {
                return value;
            }
        }

        protected void rblDayMode_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            PopulateDayControls();
        }

        protected void cmdFind_Click(object sender, System.EventArgs e)
        {
            Mode = "Select";
            SortColumns.SetInitialSort(DefaultSortExpression);
            BindGrid(false);
        }

        protected void cmdAdd_Click(object sender, System.EventArgs e)
        {
            KeyID = Null.NullInteger;
            Mode = "Add";
            tbTitle.Text = string.Empty;
            teContent.RichText.Text = string.Empty;
            tbDesktopSummary.Text = string.Empty;
        }

        protected void cmdReturn_Click(object sender, System.EventArgs e)
        {
            try
            {
                Response.Redirect(Globals.NavigateURL(), true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdDelete_Click(object sender, System.EventArgs e)
        {
            DeleteCurrentItem();
        }

        protected void dgSelectContent_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                for (int i = 0; i < dgSelectContent.Columns.Count; i++)
                {
                    DataGridColumn dgColumn = dgSelectContent.Columns[i];
                    if (!(string.IsNullOrEmpty(dgColumn.SortExpression)))
                    {
                        LinkButton lbHeader = (LinkButton)(e.Item.Cells[i].Controls[0]);
                        int pt = lbHeader.Text.IndexOf("<img");
                        string headerText = null;
                        if (pt == -1)
                        {
                            headerText = lbHeader.Text;
                        }
                        else
                        {
                            headerText = lbHeader.Text.Substring(0, pt);
                        }
                        int SortIdx = SortColumns.FindColumnIndex(dgColumn.SortExpression);
                        if (SortIdx == -1 || SortColumns[SortIdx].Direction == SortColumnInfo.SortDirection.NotSorted)
                        {
                            lbHeader.Text = headerText;
                        }
                        else
                        {
                            lbHeader.Text = headerText + " " + SortColumns[SortIdx].DirectionGlyph + "<span class='sortIndex'>" + (SortIdx + 1).ToString() + "</span>";
                        }
                    }
                }
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Control btnDelete = e.Item.FindControl("btnDelete");
                if (btnDelete != null)
                {
                    DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm((WebControl)btnDelete, LocalizeString("CONFIRM_DELETE.Text"));
                }
            }
        }

        protected void dgSelectContent_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            dgSelectContent.CurrentPageIndex = e.NewPageIndex;
            BindGrid(SortColumns.Count > 0);
        }

        protected void dgSelectContent_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
        {
            SortColumns.SortOn(e.SortExpression);
            BindGrid(SortColumns.Count > 0);
        }

        protected void cblUnauthenticatedOrDefaultValue_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (divProfilePropertyValue.Visible && phProfilePropertyValue.Controls.Count > 1)
            {
                phProfilePropertyValue.Visible = (ProfilePropertyUse == 0);
            }
        }

        public EditContentDejour()
        {
           this.Init += Page_Init;
        }
    }
}
