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

using System.Data;

namespace WESNet.DNN.Modules.ContentDejour
{
    public abstract class DataProvider
    {

        #region Shared/Static Methods

        // singleton reference to the instantiated object 
        private static DataProvider objProvider = null;

        // constructor
        static DataProvider()
        {
            CreateProvider();
        }

        // dynamically create provider
        private static void CreateProvider()
        {
            objProvider = (DataProvider)DotNetNuke.Framework.Reflection.CreateObject("data", "WESNet.DNN.Modules.ContentDejour", "");
        }

        // return the provider
        public static DataProvider Instance()
        {
            return objProvider;
        }

        #endregion

        #region Abstract methods

        public abstract IDataReader GetContent(int KeyID, int ModuleID);

        public abstract IDataReader GetContents(int ModuleId, MonthArray Months, DayArray Days, DayofWeekArray DaysofWeek, DateInteger Today, int Time, int CategoryID, int GroupID, string ProfilePropertyValue, bool IncludeDisabled);

        public abstract IDataReader FindContents(int ModuleId, MonthArray Months, DayArray Days, DayofWeekArray DaysofWeek, DateInteger StartDate, DateInteger EndDate, int StartTime, int EndTime, int CategoryID, int GroupID, string ProfilePropertyValue, bool IncludeDisabled);

        public abstract int AddContent(int ModuleId, int PortalID, MonthArray Months, DayArray Days, DayofWeekArray DaysofWeek, DateInteger StartDate, DateInteger EndDate, int StartTime, int EndTime, int CategoryID, int GroupID, string ProfilePropertyValue, bool Disabled, string Title, string DesktopHtml, string DesktopSummary, int UserID);

        public abstract void UpdateContent(int KeyID, int ModuleID, int PortalID, int ContentItemID, MonthArray Months, DayArray Days, DayofWeekArray DaysofWeek, DateInteger StartDate, DateInteger EndDate, int StartTime, int EndTime, int CategoryID, int GroupID, string ProfilePropertyValue, bool Disabled, string Title, string DesktopHtml, string DesktopSummary, int UserID);

        public abstract void LinkContentItem(int KeyID, int ContentItemID);

        public abstract void DeleteContent(int KeyID);

        public abstract IDataReader GetCategories(int ModuleID);

        public abstract IDataReader GetCategory(int CategoryID);

        public abstract void AddCategory(CategoryInfo objCategory);

        public abstract void DeleteCategory(int CategoryID);

        public abstract void UpdateCategory(CategoryInfo objCategory);

        #endregion
    }
}
