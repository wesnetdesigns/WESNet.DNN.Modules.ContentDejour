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
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;

using System;
using System.Web;

namespace WESNet.DNN.Modules.ContentDejour
{
    public partial class ViewContentDejour : ContentDejourModuleBase, IActionable
    {
        private MonthArray _Months = MonthArray.Null;
        private DayArray _Days = DayArray.Null;
        private DayofWeekArray _DaysofWeek = DayofWeekArray.Null;
        private int _Time = Null.NullInteger;
        private DateInteger _Today = DateInteger.Null;
        private string _ProfilePropertyValue = string.Empty;
        private int _GroupID = Null.NullInteger;

        public MonthArray Months
        {
            get
            {
                return _Months;
            }
            set
            {
                _Months = value;
            }
        }

        public DayArray Days
        {
            get
            {
                return _Days;
            }
            set
            {
                _Days = value;
            }
        }

        public DayofWeekArray DaysofWeek
        {
            get
            {
                return _DaysofWeek;
            }
            set
            {
                _DaysofWeek = value;
            }
        }

        public string ProfilePropertyValue
        {
            get
            {
                return _ProfilePropertyValue;
            }
            set
            {
                _ProfilePropertyValue = value;
            }
        }

        public int Time
        {
            get
            {
                return _Time;
            }
            set
            {
                _Time = value;
            }
        }

        public DateInteger Today
        {
            get
            {
                return _Today;
            }
            set
            {
                _Today = value;
            }
        }

        public int GroupID
        {
            get
            {
                return _GroupID;
            }
            set
            {
                _GroupID = value;
            }
        }

        private void Page_Load(object sender, System.EventArgs e)
        {
            int i = -1;
            string content = string.Empty;
            string title = string.Empty;

            DateTime todayAdjusted = Utilities.GetCurrentPortalTime();
            if (MyConfiguration.EnableUserTimeConversion)
            {
                todayAdjusted = Utilities.ConvertPortalToUserTime(todayAdjusted);
            }

            try
            {
                ContentDejourController cdc = new ContentDejourController();
                if (!(string.IsNullOrEmpty(Request.QueryString["KeyID"])))
                {
                    int keyID = 0;
                    if (int.TryParse(Request.QueryString["KeyID"], out keyID))
                    {
                        CurrentItem = cdc.GetContent(keyID, ModuleId);
                    }
                }

                if (CurrentItem.KeyID == -1)
                {
                    switch (MyConfiguration.SelectBy)
                    {
                        case Enums.SelectBy.Month:
                            Months = (new MonthArray()).AddMonth(todayAdjusted.Month);
                            break;
                        case Enums.SelectBy.DayofMonth:
                            Days = (new DayArray()).AddDay(todayAdjusted.Day);
                            break;
                        case Enums.SelectBy.DayofYear:
                            Days = new DayArray(todayAdjusted.DayOfYear | DayArray.ModeFlag);
                            break;
                        case Enums.SelectBy.MonthAndDayofMonth:
                            Months = (new MonthArray()).AddMonth(todayAdjusted.Month);
                            Days = (new DayArray()).AddDay(todayAdjusted.Day);
                            break;
                        case Enums.SelectBy.DayofWeek:
                            DaysofWeek = (new DayofWeekArray()).AddDay(Convert.ToInt32(todayAdjusted.DayOfWeek));
                            break;
                        case Enums.SelectBy.MonthAndDayofWeek:
                            Months = (new MonthArray()).AddMonth(todayAdjusted.Month);
                            DaysofWeek = (new DayofWeekArray()).AddDay(Convert.ToInt32(todayAdjusted.DayOfWeek));
                            break;
                        case Enums.SelectBy.TimeSpan:
                            Time = Convert.ToInt32(todayAdjusted.TimeOfDay.TotalMinutes);
                            break;
                        case Enums.SelectBy.DateSpan:
                            Today = new DateInteger(todayAdjusted.Date, false);
                            break;
                        case Enums.SelectBy.DateAndTimeSpan:
                            Today = new DateInteger(todayAdjusted.Date, false);
                            Time = Convert.ToInt32(todayAdjusted.TimeOfDay.TotalMinutes);
                            break;
                        case Enums.SelectBy.Random:
                            //Do nothing as they will be handled below
                            break;
                    }

                    if (!(string.IsNullOrEmpty(MyConfiguration.ProfilePropertyName)))
                    {
                        string tmp = null;
                        if (UserId == -1)
                        {
                            tmp = "<Unauthenticated>";
                        }
                        else
                        {
                            ProfilePropertyDefinition ppd = UserInfo.Profile.GetProperty(MyConfiguration.ProfilePropertyName);
                            if (ppd == null)
                            {
                                tmp = string.Empty;
                            }
                            else
                            {
                                tmp = ppd.PropertyValue;
                                if (tmp == null)
                                {
                                    if (string.IsNullOrEmpty(ppd.DefaultValue))
                                    {
                                        tmp = "<Default>";
                                    }
                                    else
                                    {
                                        tmp = ppd.DefaultValue;
                                    }
                                }
                            }
                        }
                        ProfilePropertyValue = tmp;
                    }

                    if (!(string.IsNullOrEmpty(Request.QueryString["GroupId"])))
                    {
                        int.TryParse(Request.QueryString["GroupId"], out _GroupID);
                    }

                    if (MyConfiguration.MultipleHandling == Enums.MultipleHandling.TimeSpan)
                    {
                        Time = Convert.ToInt32(todayAdjusted.TimeOfDay.TotalMinutes);
                    }

                    // get ContentDejourInfo object(s)
                    ContentsDejour = cdc.GetContents(ModuleId, Months, Days, DaysofWeek, Today, Time, MyConfiguration.CategoryID, GroupID, ProfilePropertyValue, MyConfiguration.IncludeDisabled);

                    if (ContentsDejour.Count > 1)
                    {
                        int minViews = int.MaxValue;
                        int leastViewed = 0;
                        switch (MyConfiguration.MultipleHandling)
                        {
                            case Enums.MultipleHandling.Random:
                                i = (new Random()).Next(0, ContentsDejour.Count);
                                break;
                            case Enums.MultipleHandling.First:
                                i = 0;
                                break;
                            case Enums.MultipleHandling.Sequential:
                            case Enums.MultipleHandling.LeastViewed:
                                System.Web.HttpCookie cookie = Request.Cookies["ContentDejour" + TabModuleId.ToString()];
                                int[] views = new int[ContentsDejour.Count + 1];
                                if (cookie != null)
                                {
                                    i = int.Parse(cookie.Values["LastViewed"]);
                                    for (int j = 0; j < ContentsDejour.Count; j++)
                                    {
                                        string v = cookie.Values[j.ToString()];
                                        if (string.IsNullOrEmpty(v))
                                        {
                                            views[j] = 0;
                                        }
                                        else
                                        {
                                            views[j] = int.Parse(v);
                                        }
                                        if (views[j] < minViews)
                                        {
                                            leastViewed = j;
                                            minViews = views[j];
                                        }
                                    }
                                }
                                if (MyConfiguration.MultipleHandling == Enums.MultipleHandling.Sequential)
                                {
                                    i = (i + 1) % ContentsDejour.Count;
                                }
                                else
                                {
                                    i = leastViewed;
                                }
                                views[i]++;
                                cookie = new HttpCookie("ContentDejour" + TabModuleId.ToString());
                                cookie.Values["LastViewed"] = i.ToString();
                                for (int j = 0; j < views.Length; j++)
                                {
                                    cookie.Values[j.ToString()] = views[j].ToString();
                                }
                                cookie.Expires = todayAdjusted.Date.AddMinutes(Convert.ToDouble(ContentsDejour[i].EndTime));
                                Response.Cookies.Add(cookie);
                                break;
                            case Enums.MultipleHandling.Last:
                                i = ContentsDejour.Count - 1;
                                break;
                            case Enums.MultipleHandling.TimeSpan:
                                ContentsDejour.Sort(new ContentDejourInfoComparer("TimeDuration ASC"));
                                i = 0;
                                break;
                        }
                    }
                    else if ((ContentsDejour.Count == 1) && ContentsDejour[0].IsWithinTimeSpan(todayAdjusted.TimeOfDay))
                    {
                        i = 0;
                    }
                }
                else
                {
                    ContentsDejour.Add(CurrentItem);
                    if (ContentsDejour.Count == 1)
                    {
                        i = 0;
                    }
                }

                if (i == -1)
                {
                    if (MyConfiguration.HideWhenNoContent && !IsEditable)
                    {
                        ContainerControl.Visible = false;
                    }
                    else
                    {
                        lnkEdit.Visible = false;
                        divContent.InnerHtml = LocalizeSharedResource("NO_CONTENT");
                    }
                }
                else
                {
                    KeyID = ContentsDejour[i].KeyID;
                    title = Server.HtmlDecode(ContentsDejour[i].Title);
                    content = HttpUtility.HtmlDecode(ContentsDejour[i].DesktopHTML);
                    TokenReplace tr = null;
                    if (MyConfiguration.ReplaceTokens)
                    {
                        tr = new TokenReplace(ContentsDejour[i]);
                        tr.ModuleId = ModuleId;
                        tr.AccessingUser = UserInfo;
                        tr.DebugMessages = !(PortalSettings.UserMode == PortalSettings.Mode.View);
                        content = tr.ReplaceEnvironmentTokens(content);
                        title = tr.ReplaceEnvironmentTokens(title);
                    }
                    // set edit link
                    if (IsEditable)
                    {
                        lnkEdit.NavigateUrl = EditUrl("KeyID", KeyID.ToString());
                        lnkEdit.Visible = true;
                    }
                    else
                    {
                        lnkEdit.Visible = false;
                    }
                    // add content to module
                    divContent.InnerHtml = Globals.ManageUploadDirectory(content, PortalSettings.HomeDirectory);
                    // replace module title if so specified in settings
                    if (MyConfiguration.ReplaceTitle && !(string.IsNullOrEmpty(title)))
                    {
                        ModuleConfiguration.ModuleTitle = title;
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        public DotNetNuke.Entities.Modules.Actions.ModuleActionCollection ModuleActions
        {
            get
            {
                var Actions = new DotNetNuke.Entities.Modules.Actions.ModuleActionCollection();
                Actions.Add(GetNextActionID(), LocalizeSharedResource("Add_New_Content"), ModuleActionType.AddContent, "", "add.gif", EditUrl("KeyID", "-1"), false, SecurityAccessLevel.Edit, true, false);
                Actions.Add(GetNextActionID(), LocalizeSharedResource("Edit_Contents"), ModuleActionType.AddContent, "", "edit.gif", EditUrl("KeyID", "0"), false, SecurityAccessLevel.Edit, true, false);
                return Actions;
            }
        }

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.Load += Page_Load;
        }
    }
}
