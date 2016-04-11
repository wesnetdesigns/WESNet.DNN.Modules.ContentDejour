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


using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Search;

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;

namespace WESNet.DNN.Modules.ContentDejour
{
    public class ContentDejourController : ISearchable, IPortable, IUpgradeable
    {
        #region Contents
        public ContentDejourInfo GetContent(int KeyID, int ModuleID)
        {
            return CBO.FillObject<ContentDejourInfo>(DataProvider.Instance().GetContent(KeyID, ModuleID));
        }

        public List<ContentDejourInfo> GetContents(int ModuleId, MonthArray Months, DayArray Days, DayofWeekArray DaysofWeek, DateInteger Today, int Time, int CategoryID, int GroupID, string ProfilePropertyValue, bool IncludeDisabled)
        {
            return CBO.FillCollection<ContentDejourInfo>(DataProvider.Instance().GetContents(ModuleId, Months, Days, DaysofWeek, Today, Time, CategoryID, GroupID, ProfilePropertyValue, IncludeDisabled));
        }

        public System.Collections.Generic.List<ContentDejourInfo> FindContents(int ModuleId, MonthArray Months, DayArray Days, DayofWeekArray DaysofWeek, DateInteger StartDate, DateInteger EndDate, int StartTime, int EndTime, int CategoryID, int GroupID, string ProfilePropertyValue, bool IncludeDisabled)
        {
            return CBO.FillCollection<ContentDejourInfo>(DataProvider.Instance().FindContents(ModuleId, Months, Days, DaysofWeek, StartDate, EndDate, StartTime, EndTime, CategoryID, GroupID, ProfilePropertyValue, IncludeDisabled));
        }

        public int AddContent(ContentDejourInfo objContentDejourInfo, int UserId)
        {
            objContentDejourInfo.KeyID = DataProvider.Instance().AddContent(objContentDejourInfo.ModuleID, objContentDejourInfo.PortalID, objContentDejourInfo.Months, objContentDejourInfo.Days, objContentDejourInfo.DaysofWeek, objContentDejourInfo.StartDate, objContentDejourInfo.EndDate, objContentDejourInfo.StartTime, objContentDejourInfo.EndTime, objContentDejourInfo.CategoryID, objContentDejourInfo.GroupID, objContentDejourInfo.ProfilePropertyValue, objContentDejourInfo.Disabled, objContentDejourInfo.Title, objContentDejourInfo.DesktopHTML, objContentDejourInfo.DesktopSummary, UserId);
            return objContentDejourInfo.KeyID;
        }

        public void UpdateContent(ContentDejourInfo objContentDejourInfo, int UserId)
        {
            DataProvider.Instance().UpdateContent(objContentDejourInfo.KeyID, objContentDejourInfo.ModuleID, objContentDejourInfo.PortalID, objContentDejourInfo.ContentItemId, objContentDejourInfo.Months, objContentDejourInfo.Days, objContentDejourInfo.DaysofWeek, objContentDejourInfo.StartDate, objContentDejourInfo.EndDate, objContentDejourInfo.StartTime, objContentDejourInfo.EndTime, objContentDejourInfo.CategoryID, objContentDejourInfo.GroupID, objContentDejourInfo.ProfilePropertyValue, objContentDejourInfo.Disabled, objContentDejourInfo.Title, objContentDejourInfo.DesktopHTML, objContentDejourInfo.DesktopSummary, UserId);
        }

        public void LinkContentItem(int KeyID, int ContentItemID)
        {
            DataProvider.Instance().LinkContentItem(KeyID, ContentItemID);
        }

        public void DeleteContent(int KeyID)
        {
            DataProvider.Instance().DeleteContent(KeyID);
        }

        #endregion

        #region Categories
        public System.Collections.Generic.List<CategoryInfo> GetCategories(int ModuleID)
        {
            return CBO.FillCollection<CategoryInfo>(DataProvider.Instance().GetCategories(ModuleID));
        }

        public CategoryInfo GetCategory(int CategoryID)
        {
            return (CategoryInfo)CBO.FillObject(DataProvider.Instance().GetCategory(CategoryID), typeof(CategoryInfo));
        }

        public void AddCategory(CategoryInfo objCategory)
        {
            DataProvider.Instance().AddCategory(objCategory);
        }

        public void UpdateCategory(CategoryInfo objCategory)
        {
            DataProvider.Instance().UpdateCategory(objCategory);
        }

        public void DeleteCategory(int CategoryID)
        {
            DataProvider.Instance().DeleteCategory(CategoryID);
        }
        #endregion

        public DotNetNuke.Services.Search.SearchItemInfoCollection GetSearchItems(ModuleInfo ModInfo)
        {
            var SearchItemCollection = new SearchItemInfoCollection();
            SearchItemInfo SearchItem = null;
            string summary = null;
            string title = null;
            var cdc = new ContentDejourController(); 

            var objContents = cdc.GetContents(ModInfo.ModuleID, MonthArray.Null, DayArray.Null, DayofWeekArray.Null, DateInteger.Null, Null.NullInteger, Null.NullInteger, Null.NullInteger, "", false);

            foreach (ContentDejourInfo objContent in objContents)
            {
                title = HttpUtility.HtmlDecode(objContent.Title);
                summary = Utilities.GetSummary(objContent.DesktopSummary, objContent.DesktopHTML);
                SearchItem = new SearchItemInfo(ModInfo.ModuleTitle + " - " + title, summary, objContent.CreatedByUserID, objContent.CreatedOnDate, ModInfo.ModuleID, objContent.KeyID.ToString(), HttpUtility.HtmlDecode(objContent.DesktopHTML), "KeyID=" + objContent.KeyID.ToString());
                SearchItemCollection.Add(SearchItem);
            }
            return SearchItemCollection;
        }

        public string ExportModule(int ModuleID)
        {

            StringBuilder strXML = new StringBuilder();
            ContentDejourController cdc = new ContentDejourController();
            strXML.AppendLine("<contentdejour>");

            Configuration MyConfiguration = new Configuration(ModuleID);
            AppendSerializedObject(strXML, "configuration", MyConfiguration);

            strXML.AppendLine("<categories>");
            System.Collections.Generic.List<CategoryInfo> objCategories = cdc.GetCategories(ModuleID);
            foreach (CategoryInfo objCategory in objCategories)
            {
                AppendSerializedObject(strXML, "category", objCategory);
            }
            strXML.AppendLine("</categories>");

            System.Collections.Generic.List<ContentDejourInfo> objContents = cdc.GetContents(ModuleID, MonthArray.Null, DayArray.Null, DayofWeekArray.Null, DateInteger.Null, Null.NullInteger, Null.NullInteger, Null.NullInteger, "", true);
            if (objContents.Count > 0)
            {
                strXML.AppendLine("<contents>");
                foreach (ContentDejourInfo objContent in objContents)
                {
                    AppendSerializedObject(strXML, "content", objContent);
                }
                strXML.AppendLine("</contents>");
                strXML.AppendLine("</contentdejour>");
            }

            return strXML.ToString();

        }

        private void AppendSerializedObject(StringBuilder sb, string Tag, object obj)
        {

            try
            {
                System.Xml.Serialization.XmlSerializer xser = null;
                System.IO.StringWriter sw = new System.IO.StringWriter();
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                System.Xml.XmlNode xmlNode = null;
                Type objType = obj.GetType();

                xser = new System.Xml.Serialization.XmlSerializer(objType);
                xser.Serialize(sw, obj);
                xmlDoc.LoadXml(sw.GetStringBuilder().ToString());
                xmlNode = xmlDoc.SelectSingleNode(Tag);
                xmlNode.Attributes.Remove(xmlNode.Attributes["xmlns:xsd"]);
                xmlNode.Attributes.Remove(xmlNode.Attributes["xmlns:xsi"]);
                sb.Append(xmlNode.OuterXml);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        private void AppendElement(StringBuilder sb, string Tag, object Value)
        {
            sb.AppendFormat("<{0}>", Tag);
            sb.Append(Value);
            sb.AppendFormat("</{0}>", Tag);
        }

        /// ImportModule implements the IPortable ImportModule Interface
        /// -----------------------------------------------------------------------------
        public void ImportModule(int ModuleID, string Content, string Version, int UserId)
        {

            try
            {
                System.Collections.Specialized.NameValueCollection ForeignKeyTranslator = new System.Collections.Specialized.NameValueCollection();
                XmlNode Node = null;
                XmlNode xmlDocuments = Globals.GetContent(Content, "contentdejour");

                var MyConfiguration = new Configuration(ModuleID);
                Node = xmlDocuments.SelectSingleNode("configuration");
                string category = string.Empty;
                string profilePropertyName = string.Empty;
                MyConfiguration.SelectBy = (Enums.SelectBy)Enum.Parse(typeof(Enums.SelectBy), Node["SelectBy"].InnerText);
                MyConfiguration.MultipleHandling = (Enums.MultipleHandling)Enum.Parse(typeof(Enums.MultipleHandling), Node["MultipleHandling"].InnerText);
                MyConfiguration.Interval = Convert.ToInt32(Node["Interval"].InnerText);
                MyConfiguration.IncludeDisabled = Convert.ToBoolean(Node["IncludeDisabled"].InnerText);
                MyConfiguration.HideWhenNoContent = Convert.ToBoolean(Node["HideWhenNoContent"].InnerText);
                MyConfiguration.ReplaceTitle = Convert.ToBoolean(Node["ReplaceTitle"].InnerText);
                MyConfiguration.ReplaceTokens = Convert.ToBoolean(Node["ReplaceTokens"].InnerText);
                if (Version == "04.00.00")
                {
                    MyConfiguration.ProfilePropertyName = Defaults.ProfilePropertyName;
                    MyConfiguration.EnableUserTimeConversion = Defaults.EnableUserTimeConversion;
                }
                else
                {
                    MyConfiguration.ProfilePropertyName = Node["ProfilePropertyName"].InnerText;
                    MyConfiguration.EnableUserTimeConversion = Convert.ToBoolean(Node["EnableUserTimeConversion"].InnerText);
                }

                category = Node["Category"].InnerText;
                var cdc = new ContentDejourController();

                CategoryInfo objCategory = null;
                foreach (XmlNode NodeWithinLoop in xmlDocuments.SelectNodes("categories/category"))
                {
                    Node = NodeWithinLoop;
                    objCategory = new CategoryInfo();
                    objCategory.ModuleID = ModuleID;
                    objCategory.Category = NodeWithinLoop["Category"].InnerText;
                    objCategory.ViewOrder = int.Parse(NodeWithinLoop["ViewOrder"].InnerText);
                    try
                    {
                        cdc.AddCategory(objCategory);
                    }
                    catch
                    {
                    }
                }
                ForeignKeyTranslator.Add("CAT-", "-1");
                foreach (CategoryInfo objCategoryWithinLoop in cdc.GetCategories(ModuleID))
                {
                    objCategory = objCategoryWithinLoop;
                    ForeignKeyTranslator.Add("CAT-" + objCategoryWithinLoop.Category, objCategoryWithinLoop.CategoryID.ToString());
                    if (objCategoryWithinLoop.Category == category)
                    {
                        MyConfiguration.CategoryID = objCategoryWithinLoop.CategoryID;
                    }
                }

                MyConfiguration.SaveSettings();

                var rc = new RoleController();
                var portalID = MyConfiguration.PortalSettings.PortalId;

                foreach (XmlNode NodeWithinLoop in xmlDocuments.SelectNodes("contents/content"))
                {
                    Node = NodeWithinLoop;
                    var objContent = new ContentDejourInfo();
                    objContent.ModuleID = ModuleID;
                    if (Version == "04.00.00")
                    {
                        objContent.Months = (new MonthArray()).AddMonth(Convert.ToInt16(NodeWithinLoop["Month"].InnerText));
                        objContent.Days = (new DayArray()).AddDay(Convert.ToInt32(NodeWithinLoop["Day"].InnerText));
                        objContent.DaysofWeek = (new DayofWeekArray()).AddDay(Convert.ToInt32(NodeWithinLoop["DayofWeek"].InnerText));
                        objContent.ProfilePropertyValue = string.Empty;
                    }
                    else
                    {
                        objContent.Months = new MonthArray(Convert.ToInt16(NodeWithinLoop["Months"].InnerText));
                        objContent.Days = new DayArray(Convert.ToInt32(NodeWithinLoop["Days"].InnerText));
                        objContent.DaysofWeek = new DayofWeekArray(Convert.ToByte(NodeWithinLoop["DaysofWeek"].InnerText));
                        objContent.ProfilePropertyValue = NodeWithinLoop["ProfilePropertyValue"].InnerText;
                    }
                    objContent.StartDate = new DateInteger(Convert.ToInt32(NodeWithinLoop["StartDate"].InnerText));
                    objContent.EndDate = new DateInteger(Convert.ToInt32(NodeWithinLoop["EndDate"].InnerText));
                    objContent.StartTime = Convert.ToInt32(NodeWithinLoop["StartTime"].InnerText);
                    objContent.EndTime = Convert.ToInt32(NodeWithinLoop["EndTime"].InnerText);
                    objContent.CategoryID = Convert.ToInt32(ForeignKeyTranslator["CAT-" + NodeWithinLoop["Category"].InnerText]);

                    var groupName = NodeWithinLoop["GroupName"] == null ? "": NodeWithinLoop["GroupName"].InnerText;
                    if (groupName != "")
                    {
                        var role = rc.GetRoleByName(portalID, groupName);
                        if (role != null && role.SecurityMode != SecurityMode.SecurityRole && role.Status == RoleStatus.Approved)
                        {
                            objContent.GroupID = role.RoleID;
                        }
                    }
                    
                    objContent.Title = HttpUtility.HtmlEncode(NodeWithinLoop["Title"].InnerText);
                    objContent.DesktopHTML = NodeWithinLoop["DesktopHTML"].InnerText;
                    objContent.DesktopSummary = NodeWithinLoop["DesktopSummary"].InnerText;
                    cdc.AddContent(objContent, UserId); //Note all content will be have CreatedByUserID set to UserID of current user
                }
            }
            catch (Exception exc)
            {
                string Msg = exc.Message;
            }
        }

        public string UpgradeModule(string Version)
        {
            StringBuilder results = new StringBuilder();

            //Nothing to do - stub included for later if needed

            results.Append(Version);
            return results.ToString();
        }
    }
}
