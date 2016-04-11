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
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;

using System;
using System.Collections.Generic;
using System.Linq;

namespace WESNet.DNN.Modules.ContentDejour
{
	public class ContentDejourModuleBase : PortalModuleBase
	{
#region Private Members
		private WESNet.DNN.Modules.ContentDejour.Configuration _MyConfiguration;
		private System.Collections.Generic.List<ContentDejourInfo> _ContentsDejour;
		private ContentDejourInfo _CurrentItem = null;
		//private string _ProfileResourceFile = "~/Admin/Users/App_LocalResources/Profile.ascx";
		private int _ContentTypeID = Null.NullInteger;
#endregion

#region Public Properties

		public WESNet.DNN.Modules.ContentDejour.Configuration MyConfiguration
		{
			get
			{
				if (_MyConfiguration == null)
				{
					string CacheKey = string.Format(Consts.ConfigurationCacheKey, TabModuleId);
					object obj = DataCache.GetCache(CacheKey);
					if (obj == null || Consts.ConfigurationCacheDuration == 0)
					{
						_MyConfiguration = new Configuration(ModuleId, Settings);
						DataCache.SetCache(CacheKey, _MyConfiguration, new TimeSpan(0, 0, Consts.ConfigurationCacheDuration));
					}
					else
					{
						_MyConfiguration = (WESNet.DNN.Modules.ContentDejour.Configuration)obj;
					}
				}
				return _MyConfiguration;
			}
		}

		public int KeyID
		{
			get
			{
				return ViewState["KeyID"] == null ? Null.NullInteger : (int)ViewState["KeyID"];
			}
			set
			{
				ViewState["KeyID"] = value;
			}
		}

		public System.Collections.Generic.List<ContentDejourInfo> ContentsDejour
		{
			get
			{
				if (_ContentsDejour == null)
				{
					_ContentsDejour = new List<ContentDejourInfo>();
				}
				return _ContentsDejour;
			}
			set
			{
				_ContentsDejour = value;
			}
		}

		public ContentDejourInfo CurrentItem
		{
			get
			{
				if (_CurrentItem == null)
				{
					if (KeyID == Null.NullInteger)
					{
						_CurrentItem = new ContentDejourInfo(PortalId, ModuleId);
					}
					else
					{
						var cdc = new ContentDejourController();
						_CurrentItem = cdc.GetContent(KeyID, ModuleId);
						if (_CurrentItem == null)
						{
							Response.Redirect(Globals.AccessDeniedURL(string.Format(LocalizeSharedResource("No_Access_Invalid_KeyID"), KeyID)), true);
						}
					}
				}
				return _CurrentItem;
			}
			set
			{
                KeyID = value == null ? Null.NullInteger : value.KeyID;
				_CurrentItem = value;
			}
		}
#endregion

		internal void InvalidateConfiguration()
		{
			DataCache.RemoveCache(string.Format(Consts.ConfigurationCacheKey, TabModuleId));
			_MyConfiguration = null;
		}

		internal void InvalidateCurrentItem()
		{
			CurrentItem = null;
		}

		internal void RefreshCurrentItem()
		{
			_CurrentItem = null;
		}

		protected int GetContentTypeID()
		{
			if (_ContentTypeID == Null.NullInteger)
			{
				var ctc = new ContentTypeController();
                var contentTypes = ctc.GetContentTypes().Where(t => t.ContentType == Consts.ContentTypeName);
				if (contentTypes.Count() > 0)
				{
					var contentType = contentTypes.Single();
					_ContentTypeID = (contentType == null) ? CreateContentType() : contentType.ContentTypeId;
				}
				else
				{
					_ContentTypeID = CreateContentType();
				}
			}
			return _ContentTypeID;
		}

		private int CreateContentType()
		{
			ContentTypeController ctc = new ContentTypeController();
			ContentType contentType = new ContentType();
			contentType.ContentType = Consts.ContentTypeName;
			return ctc.AddContentType(contentType);
		}

		protected int SaveContentItem(ContentDejourInfo objContentDejour, bool update)
		{
			objContentDejour.Content = objContentDejour.DesktopHTML;
			objContentDejour.ContentKey = "mid=" + ModuleId.ToString() + "&KeyID=" + objContentDejour.KeyID.ToString();
			objContentDejour.ContentTypeId = GetContentTypeID();
			objContentDejour.ModuleID = ModuleId;
			objContentDejour.TabID = TabId;
			objContentDejour.Indexed = false;
			var cc = new ContentController();
			var cdc = new ContentDejourController();
			if (objContentDejour.ContentItemId == Null.NullInteger)
			{
				objContentDejour.ContentItemId = cc.AddContentItem(objContentDejour);
				cdc.LinkContentItem(objContentDejour.KeyID, objContentDejour.ContentItemId);
			}
			else if (update)
			{
				cc.UpdateContentItem(objContentDejour);
			}
			return objContentDejour.ContentItemId;
		}

        public string LocalizeSharedResource(string resourceKey)
        {
            return Localization.GetString(resourceKey, MyConfiguration.LocalSharedResourceFile);
        }
	}

}
