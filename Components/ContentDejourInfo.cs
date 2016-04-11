//Copyright (c) 2008-2013, William Severance, Jr., WESNet Designs
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


using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.SystemDateTime;
using DotNetNuke.Services.Tokens;

using System;
using System.Xml.Serialization;

namespace WESNet.DNN.Modules.ContentDejour
{
    [Serializable(), XmlRoot("content")]
    public class ContentDejourInfo : DotNetNuke.Entities.Content.ContentItem, DotNetNuke.Services.Tokens.IPropertyAccess
    {
        #region Private Members

        private int _KeyID = Null.NullInteger;
        private int _PortalID;
        private MonthArray _Months = MonthArray.Null;
        private DayArray _Days = DayArray.Null;
        private DayofWeekArray _DaysofWeek = DayofWeekArray.Null;
        private DateInteger _StartDate = DateInteger.Null;
        private DateInteger _EndDate = DateInteger.Null;
        private int _StartTime = Consts.MinTime;
        private int _EndTime = Consts.MaxTime;
        private int _CategoryID = -1;
        private string _Category = string.Empty;
        private int _GroupID = -1;
        private string _GroupName = string.Empty;
        private string _ProfilePropertyValue = string.Empty;
        private bool _Disabled = false;
        private string _Title;
        private string _DesktopHTML;
        private string _DesktopSummary;
        #endregion

        #region Constructors
        public ContentDejourInfo() : this(Null.NullInteger, Null.NullInteger)
        {
        }

        public ContentDejourInfo(int PortalID, int ModuleID)
        {
            this.PortalID = PortalID;
            this.ModuleID = ModuleID;
        }

        #endregion

        #region IHydratable Implementation/Overrides

        [XmlIgnore()]
        public override int KeyID
        {
            get
            {
                return _KeyID;
            }
            set
            {
                _KeyID = value;
            }
        }

        public override void Fill(System.Data.IDataReader dr)
        {
            base.FillInternal(dr);

            _KeyID = Null.SetNullInteger(dr["KeyID"]);
            _PortalID = Null.SetNullInteger(dr["PortalID"]);
            if (!(Convert.IsDBNull(dr["Months"])))
            {
                _Months = new MonthArray(Convert.ToInt16(dr["Months"]));
            }
            else
            {
                _Months = MonthArray.Null;
            }
            if (!(Convert.IsDBNull(dr["Days"])))
            {
                _Days = new DayArray(Convert.ToInt32(dr["Days"]));
            }
            else
            {
                _Days = DayArray.Null;
            }
            if (!(Convert.IsDBNull(dr["DaysofWeek"])))
            {
                _DaysofWeek = new DayofWeekArray(Convert.ToByte(dr["DaysofWeek"]));
            }
            else
            {
                _DaysofWeek = DayofWeekArray.Null;
            }
            string currentYear = DateTime.Now.Year.ToString();
            if (!(Convert.IsDBNull(dr["StartDate"])))
            {
                _StartDate = new DateInteger(Convert.ToInt32(dr["StartDate"]));
            }
            if (!(Convert.IsDBNull(dr["EndDate"])))
            {
                _EndDate = new DateInteger(Convert.ToInt32(dr["EndDate"]));
            }
            if (!(Convert.IsDBNull(dr["StartTime"])))
            {
                _StartTime = Convert.ToInt32(dr["StartTime"]);
            }
            if (!(Convert.IsDBNull(dr["EndTime"])))
            {
                _EndTime = Convert.ToInt32(dr["EndTime"]);
            }
            _CategoryID = Null.SetNullInteger(dr["CategoryID"]);
            _Category = Null.SetNullString(dr["Category"]);
            _GroupID = Null.SetNullInteger(dr["GroupID"]);
            _GroupName = Null.SetNullString(dr["GroupName"]);
            _ProfilePropertyValue = Null.SetNullString(dr["ProfilePropertyValue"]);
            _Disabled = Null.SetNullBoolean(dr["Disabled"]);
            _Title = Null.SetNullString(dr["Title"]);
            _DesktopHTML = Null.SetNullString(dr["DesktopHTML"]);
            _DesktopSummary = Null.SetNullString(dr["DesktopSummary"]);
        }

        #endregion

        #region IPropertyAccess Implementation

        public string GetProperty(string strPropertyName, string strFormat, System.Globalization.CultureInfo formatProvider, DotNetNuke.Entities.Users.UserInfo AccessingUser, Scope CurrentScope, ref bool PropertyNotFound)
        {

            string OutputFormat = string.Empty;
            if (strFormat == string.Empty)
            {
                OutputFormat = "g";
            }

            PropertyNotFound = true;

            //Content locked for NoSettings
            if (CurrentScope == Scope.NoSettings)
            {
                return PropertyAccess.ContentLocked;
            }

            string result = string.Empty;
            bool PublicProperty = true;

            switch (strPropertyName.ToLower())
            {
                case "month":
                case "months":
                    {
                        PublicProperty = true;
                        PropertyNotFound = false;
                        result = Months.ToString(formatProvider);
                        break;
                    }
                case "day":
                case "days":
                    {
                        PublicProperty = true;
                        PropertyNotFound = false;
                        result = Days.ToString(formatProvider);
                        break;
                    }
                case "dayofweek":
                case "daysofweek":
                    {
                        PublicProperty = true;
                        PropertyNotFound = false;
                        result = DaysofWeek.ToString(formatProvider);
                        break;
                    }
                case "category":
                    {
                        PublicProperty = true;
                        PropertyNotFound = false;
                        result = Category;
                        break;
                    }
                case "groupname":
                    {
                        PublicProperty = true;
                        PropertyNotFound = false;
                        result = GroupName;
                        break;
                    }
                case "startdate":
                    {
                        PublicProperty = true;
                        PropertyNotFound = false;
                        result = StartDate.ToString(OutputFormat, formatProvider);
                        break;
                    }
                case "enddate":
                    {
                        PublicProperty = true;
                        PropertyNotFound = false;
                        result = EndDate.ToString(OutputFormat, formatProvider);
                        break;
                    }
                case "starttime":
                    {
                        PublicProperty = true;
                        PropertyNotFound = false;
                        result = string.Format(formatProvider, strFormat, TimeSpan.FromMinutes(Convert.ToDouble(StartTime)));
                        break;
                    }
                case "endtime":
                    {
                        PublicProperty = true;
                        PropertyNotFound = false;
                        result = string.Format(formatProvider, strFormat, TimeSpan.FromMinutes(Convert.ToDouble(EndTime)));
                        break;
                    }
                case "title":
                    {
                        PublicProperty = true;
                        PropertyNotFound = false;
                        result = Title;
                        break;
                    }
                case "createdbyuserid":
                    {
                        PublicProperty = true;
                        PropertyNotFound = false;
                        result = CreatedByUserID.ToString(formatProvider);
                        break;
                    }
                case "createdbyuser":
                    {
                        PublicProperty = true;
                        PropertyNotFound = false;
                        var mc = new ModuleController();
                        int PortalId = mc.GetModule(ModuleID, -1).PortalID;
                        DotNetNuke.Entities.Users.UserInfo usr = LastModifiedByUser(PortalId);
                        if (usr != null)
                        {
                            result = usr.DisplayName;
                        }
                        break;
                    }
                case "createddate":
                case "createdondate":
                    {
                        PublicProperty = true;
                        PropertyNotFound = false;
                        result = CreatedOnDate.ToString(OutputFormat, formatProvider);
                        break;
                    }
                case "lastmodifiedbyuserid":
                    {
                        PublicProperty = true;
                        PropertyNotFound = false;
                        result = LastModifiedByUserID.ToString(formatProvider);
                        break;
                    }
                case "lastmodifiedbyuser":
                    {
                        PublicProperty = true;
                        PropertyNotFound = false;
                        var mc = new ModuleController();
                        int PortalId = mc.GetModule(ModuleID, -1).PortalID;
                        DotNetNuke.Entities.Users.UserInfo usr = LastModifiedByUser(PortalId);
                        if (usr != null)
                        {
                            result = usr.DisplayName;
                        }
                        break;
                    }
                case "lastmodifiedondate":
                    {
                        PublicProperty = true;
                        PropertyNotFound = false;
                        result = LastModifiedOnDate.ToString(OutputFormat, formatProvider);
                        break;
                    }
                case "portaltime":
                    {
                        PublicProperty = true;
                        PropertyNotFound = false;
                        result = PortalTime.ToString(OutputFormat, formatProvider);
                        break;
                    }
                case "usertime":
                    {
                        PublicProperty = true;
                        PropertyNotFound = false;
                        result = UserTime.ToString(OutputFormat, formatProvider);
                        break;
                    }
                case "utctime":
                    {
                        PublicProperty = true;
                        PropertyNotFound = false;
                        result = UtcTime.ToString(OutputFormat, formatProvider);
                        break;
                    }
            }

            if (!PublicProperty && CurrentScope != Scope.Debug)
            {
                PropertyNotFound = true;
                result = PropertyAccess.ContentLocked;
            }

            return result;
        }

        public CacheLevel Cacheability
        {
            get
            {
                return CacheLevel.fullyCacheable;
            }
        }
        #endregion

        #region Public Properties

        [XmlIgnore()]
        public int PortalID
        {
            get
            {
                return _PortalID;
            }
            set
            {
                _PortalID = value;
            }
        }

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

        public DateInteger StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                _StartDate = value;
            }
        }

        public DateInteger EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                _EndDate = value;
            }
        }

        public int StartTime
        {
            get
            {
                return _StartTime;
            }
            set
            {
                _StartTime = value;
            }
        }

        public int EndTime
        {
            get
            {
                return _EndTime;
            }
            set
            {
                _EndTime = value;
            }
        }

        [XmlIgnore()]
        public int TimeDuration
        {
            get
            {
                return _EndTime - _StartTime;
            }
        }

        [XmlIgnore()]
        public int CategoryID
        {
            get
            {
                return _CategoryID;
            }
            set
            {
                _CategoryID = value;
            }
        }

        public string Category
        {
            get
            {
                return _Category;
            }
            set
            {
                _Category = value;
            }
        }

        [XmlIgnore()]
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

        public string GroupName
        {
            get
            {   
                return _GroupName;
            }
            set
            {
                _GroupName = value;
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

        public bool Disabled
        {
            get
            {
                return _Disabled;
            }
            set
            {
                _Disabled = value;
            }
        }

        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
            }
        }

        public string DesktopHTML
        {
            get
            {
                return _DesktopHTML;
            }
            set
            {
                _DesktopHTML = value;
            }
        }

        public string DesktopSummary
        {
            get
            {
                return _DesktopSummary;
            }
            set
            {
                _DesktopSummary = value;
            }
        }

        public bool IsWithinTimeSpan(TimeSpan currentTime)
        {
            return currentTime >= TimeSpan.FromMinutes(Convert.ToDouble(StartTime)) && currentTime < TimeSpan.FromMinutes(Convert.ToDouble(EndTime));
        }

        public bool IsWithinDates(DateTime currentDate)
        {
            DateInteger today = new DateInteger(currentDate, false);
            return today.IsBetween(StartDate, EndDate);
        }

        [XmlIgnore()]
        public DateTime PortalTime
        {
            get
            {
                return Utilities.GetCurrentPortalTime();
            }
        }

        [XmlIgnore()]
        public DateTime UserTime
        {
            get
            {
                return Utilities.ConvertPortalToUserTime(PortalTime);
            }
        }

        [XmlIgnore()]
        public DateTime UtcTime
        {
            get
            {
                return SystemDateTime.GetCurrentTimeUtc();
            }
        }

        #endregion

    }
}
