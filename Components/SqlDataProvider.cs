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


//INSTANT C# NOTE: Formerly VB project-level imports:
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using System.Web.Security;
using System.Web.Profile;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using DotNetNuke;
using DotNetNuke.Framework.Providers;
using DotNetNuke.Common.Utilities;

namespace WESNet.DNN.Modules.ContentDejour
{

	/// -----------------------------------------------------------------------------
	/// <summary>
	/// SQL Server implementation of the abstract DataProvider class
	/// </summary>
	/// <remarks>
	/// Written By Bill Severance, WESNet Designs - www.wesnetdesigns.com
	/// </remarks>
	/// <history>
	/// </history>
	/// -----------------------------------------------------------------------------
	public class SqlDataProvider : DataProvider
	{
#region Private Members

		private const string ProviderType = "data";
		private const string CompanyQualifier = "WESNet_";
		private const string ModuleQualifier = "ContentDejour_";

		private ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType);
		private string _connectionString;
		private string _providerPath;
		private string _objectQualifier;
		private string _databaseOwner;

#endregion

#region Constructors

		public SqlDataProvider()
		{

			// Read the configuration specific information for this provider
			Provider objProvider = (Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

			// This code handles getting the connection string from either the connectionString / appsetting section and uses the connectionstring section by default if it exists.  
			// Get Connection string from web.config
			_connectionString = DotNetNuke.Common.Utilities.Config.GetConnectionString();

			// If above funtion does not return anything then connectionString must be set in the dataprovider section.
			if (_connectionString == "")
			{
				// Use connection string specified in provider
				_connectionString = objProvider.Attributes["connectionString"];
			}

			_providerPath = objProvider.Attributes["providerPath"];

			_objectQualifier = objProvider.Attributes["objectQualifier"];
			if (_objectQualifier != "" && _objectQualifier.EndsWith("_") == false)
			{
				_objectQualifier += "_";
			}

			_databaseOwner = objProvider.Attributes["databaseOwner"];
			if (_databaseOwner != "" && _databaseOwner.EndsWith(".") == false)
			{
				_databaseOwner += ".";
			}

		}

#endregion

#region Properties

		public string ConnectionString
		{
			get
			{
				return _connectionString;
			}
		}

		public string ProviderPath
		{
			get
			{
				return _providerPath;
			}
		}

		public string ObjectQualifier
		{
			get
			{
				return _objectQualifier;
			}
		}

		public string DatabaseOwner
		{
			get
			{
				return _databaseOwner;
			}
		}

#endregion

#region Private Methods

		private string GetFullyQualifiedName(string name)
		{
			return DatabaseOwner + ObjectQualifier + CompanyQualifier + ModuleQualifier + name;
		}

		private object GetNull(object Field)
		{
			return DotNetNuke.Common.Utilities.Null.GetNull(Field, DBNull.Value);
		}

		private object GetNullTime(int value)
		{

			if (value <= Consts.MinTime || value >= Consts.MaxTime)
			{
				return DBNull.Value;
			}
			else
			{
				return value;
			}
		}

		private object GetNullDateInteger(DateInteger value)
		{
			if (value.IsNull)
			{
				return DBNull.Value;
			}
			else
			{
				return value.Value;
			}
		}

		private object GetNullDayofWeekArray(DayofWeekArray value)
		{
			return GetNullDayofWeekArray(value, false);
		}

		private object GetNullDayofWeekArray(DayofWeekArray value, bool TreatAllorEmptyAsNull)
		{
			if (value.IsNull || (TreatAllorEmptyAsNull && (value.IsAllDaysOfWeek || value.IsEmpty)))
			{
				return DBNull.Value;
			}
			else
			{
				return value.Value;
			}
		}

		private object GetNullMonthArray(MonthArray value)
		{
			return GetNullMonthArray(value, false);
		}

		private object GetNullMonthArray(MonthArray value, bool TreatAllorEmptyAsNull)
		{
			if (value.IsNull || (TreatAllorEmptyAsNull && (value.IsAllMonths || value.IsEmpty)))
			{
				return DBNull.Value;
			}
			else
			{
				return value.Value;
			}
		}

		private object GetNullDayArray(DayArray value)
		{
			return GetNullDayArray(value, false);
		}

		private object GetNullDayArray(DayArray value, bool TreatAllorEmptyAsNull)
		{
			if (value.IsNull || (TreatAllorEmptyAsNull && (value.IsAllDays || value.IsEmpty)))
			{
				return DBNull.Value;
			}
			else
			{
				return value.Value;
			}
		}


#endregion

#region Contents

		public override IDataReader GetContent(int KeyID, int ModuleID)
		{
			return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetContent"), KeyID, ModuleID);
		}

		public override IDataReader GetContents(int ModuleID, MonthArray Months, DayArray Days, DayofWeekArray DaysofWeek, DateInteger Today, int Time, int CategoryID, int GroupID, string ProfilePropertyValue, bool IncludeDisabled)
		{
			return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetContents"), ModuleID, GetNullMonthArray(Months, true), GetNullDayArray(Days, true), GetNullDayofWeekArray(DaysofWeek, true), GetNullDateInteger(Today), GetNullTime(Time), GetNull(CategoryID), GetNull(GroupID), GetNull(ProfilePropertyValue), IncludeDisabled);
		}

		public override IDataReader FindContents(int ModuleID, MonthArray Months, DayArray Days, DayofWeekArray DaysofWeek, DateInteger StartDate, DateInteger EndDate, int StartTime, int EndTime, int CategoryID, int GroupID, string ProfilePropertyValue, bool IncludeDisabled)
		{
			return SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("FindContents"), ModuleID, GetNullMonthArray(Months, true), GetNullDayArray(Days, true), GetNullDayofWeekArray(DaysofWeek, true), GetNullDateInteger(StartDate), GetNullDateInteger(EndDate), GetNullTime(StartTime), GetNullTime(EndTime), GetNull(CategoryID), GetNull(GroupID), GetNull(ProfilePropertyValue), IncludeDisabled);
		}

		public override int AddContent(int ModuleID, int PortalID, MonthArray Months, DayArray Days, DayofWeekArray DaysofWeek, DateInteger StartDate, DateInteger EndDate, int StartTime, int EndTime, int CategoryID, int GroupID, string ProfilePropertyValue, bool Disabled, string Title, string DesktopHtml, string DesktopSummary, int UserID)
		{

			return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, GetFullyQualifiedName("AddContent"), ModuleID, PortalID, GetNullMonthArray(Months), GetNullDayArray(Days), GetNullDayofWeekArray(DaysofWeek), GetNullDateInteger(StartDate), GetNullDateInteger(EndDate), GetNullTime(StartTime), GetNullTime(EndTime), GetNull(CategoryID), GetNull(GroupID), GetNull(ProfilePropertyValue), Disabled, GetNull(Title), DesktopHtml, GetNull(DesktopSummary), UserID));
		}

		public override void UpdateContent(int KeyID, int ModuleID, int PortalID, int ContentItemID, MonthArray Months, DayArray Days, DayofWeekArray DaysofWeek, DateInteger StartDate, DateInteger EndDate, int StartTime, int EndTime, int CategoryID, int GroupID, string ProfilePropertyValue, bool Disabled, string Title, string DesktopHtml, string DesktopSummary, int UserID)
		{

			SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("UpdateContent"), KeyID, ModuleID, PortalID, GetNull(ContentItemID), GetNullMonthArray(Months), GetNullDayArray(Days), GetNullDayofWeekArray(DaysofWeek), GetNullDateInteger(StartDate), GetNullDateInteger(EndDate), GetNullTime(StartTime), GetNullTime(EndTime), GetNull(CategoryID), GetNull(GroupID), GetNull(ProfilePropertyValue), Disabled, GetNull(Title), DesktopHtml, GetNull(DesktopSummary), UserID);
		}

		public override void LinkContentItem(int KeyID, int ContentItemID)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("LinkContentItem"), KeyID, ContentItemID);
		}

		public override void DeleteContent(int KeyID)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("DeleteContent"), KeyID);
		}

#endregion

#region Categories
		public override System.Data.IDataReader GetCategories(int ModuleID)
		{
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetCategories"), Null.GetNull(ModuleID, DBNull.Value));
		}

		public override IDataReader GetCategory(int CategoryID)
		{
			return (IDataReader)SqlHelper.ExecuteReader(ConnectionString, GetFullyQualifiedName("GetCategory"), CategoryID);
		}

		public override void AddCategory(CategoryInfo objCategory)
		{
			objCategory.CategoryID = Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, GetFullyQualifiedName("AddCategory"), objCategory.ModuleID, objCategory.Category, objCategory.ViewOrder));
		}

		public override void DeleteCategory(int CategoryID)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("DeleteCategory"), CategoryID);
		}

		public override void UpdateCategory(CategoryInfo objCategory)
		{
			SqlHelper.ExecuteNonQuery(ConnectionString, GetFullyQualifiedName("UpdateCategory"), Null.GetNull(objCategory.ModuleID, DBNull.Value), objCategory.CategoryID, objCategory.Category, objCategory.ViewOrder);
		}
#endregion

	}
}
