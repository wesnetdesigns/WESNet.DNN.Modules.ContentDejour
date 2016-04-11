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
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;

using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;

namespace WESNet.DNN.Modules.ContentDejour
{

    public class Consts
    {
        public const string Version = "06.00.00";
        public const string ModuleName = "WESNet_ContentDejour";
        public const string FriendlyName = "Content Dejour";
        public const string ModuleFolderPath = "DesktopModules/" + ModuleName + "/";
        public const string ContentTypeName = ModuleName + "Info";
        public const string ConfigurationCacheKey = "ContentDejourConfigCacheKey-{0}";
        public const int ConfigurationCacheDuration = 30; //minutes
        public const int MinTime = 0;
        public const int MaxTime = 23 * 60 + 59;
        public const int MaxDescLength = 100;
        public const string SortAscendingGlyph = "~/images/sortascending.gif";
        public const string SortDescendingGlyph = "~/images/sortdescending.gif";
    }

    public class Defaults
    {
        public const Enums.SelectBy SelectBy = Enums.SelectBy.Random;
        public const Enums.MultipleHandling MultipleHandling = Enums.MultipleHandling.Random;
        public const int Interval = 15;
        public const bool EnableUserTimeConversion = false;
        public const int CategoryID = -1;
        public const string ProfilePropertyName = "";
        public const bool IncludeDisabled = false;
        public const bool HideWhenNoContent = true;
        public const bool ReplaceTitle = true;
        public const bool ReplaceTokens = false;
    }

    [Serializable(), XmlRoot("configuration")]
    public class Configuration
    {

        #region Private Members
        private const string _v4ProfileResourceFile = "~/Admin/Users/App_LocalResources/Profile.ascx";
        private const string _v5ProfileResourceFile = "~/DesktopModules/Admin/Security/App_LocalResources/Profile.ascx";

        private ModuleInfo _ModuleConfiguration;
        private string _ModulePath = null;
        private string _LocalSharedResourceFile = null;

        private int _ModuleId = Null.NullInteger;
        private PortalSettings _portalSettings = null;
        private int _TabModuleID = Null.NullInteger;
        private int _TabID = Null.NullInteger;

        private Hashtable _Settings;

        private string _Version;
        private string _ModuleName;
        private Enums.SelectBy _SelectBy;
        private Enums.MultipleHandling _MultipleHandling;
        private int _Interval;
        private bool _EnableUserTimeConversion;
        private int _CategoryID;
        private string _ProfilePropertyName;
        private string _ProfileResourceFile;
        private bool _IncludeDisabled;
        private bool _HideWhenNoContent;
        private bool _ReplaceTitle;
        private bool _ReplaceTokens;
        #endregion

        #region Public Properties

        private int ModuleId
        {
            get
            {
                return _ModuleId;
            }
        }

        private int TabID
        {
            get
            {
                return _TabID;
            }
        }

        private int TabModuleID
        {
            get
            {
                return _TabModuleID;
            }
        }

        [XmlIgnore()]
        public PortalSettings PortalSettings
        {
            get
            {
                return _portalSettings;
            }
        }

        [XmlIgnore()]
        public Hashtable Settings
        {
            get
            {
                return _Settings;
            }
        }

        public string Version
        {
            get
            {
                if (_Version == null)
                {
                    if (_ModuleConfiguration == null)
                    {
                        if (ModuleId != Null.NullInteger && TabID != Null.NullInteger)
                        {
                            ModuleController mc = new ModuleController();
                            _Version = mc.GetModule(ModuleId, TabID).DesktopModule.Version;
                        }
                        else
                        {
                            _Version = Consts.Version;
                        }
                    }
                    else
                    {
                        _Version = _ModuleConfiguration.DesktopModule.Version;
                    }
                }
                return _Version;
            }
        }

        [XmlIgnore()]
        public string ModulePath
        {
            get
            {
                if (_ModulePath == null)
                {
                    string ApplicationPath = Globals.ApplicationPath;
                    string ControlSrc = _ModuleConfiguration.ModuleControl.ControlSrc;
                    _ModulePath = Path.GetDirectoryName(ApplicationPath + "/" + ControlSrc).Replace("\\", "/") + "/";
                }
                return _ModulePath;
            }
        }

        [XmlIgnore()]
        public string LocalSharedResourceFile
        {
            get
            {
                if (_LocalSharedResourceFile == null)
                {
                    _LocalSharedResourceFile = ModulePath + Localization.LocalResourceDirectory + "/" + Localization.LocalSharedResourceFile;
                }
                return _LocalSharedResourceFile;
            }
        }

        [XmlIgnore()]
        public string ProfileResourceFile
        {
            get
            {
                if (_ProfileResourceFile == null)
                {
                    _ProfileResourceFile = _v5ProfileResourceFile;
                }
                return _ProfileResourceFile;
            }
        }

        [XmlIgnore()]
        public string ModuleName
        {
            get
            {
                if (_ModuleName == null)
                {
                    if (_ModuleConfiguration == null)
                    {
                        if (ModuleId != Null.NullInteger && TabID != Null.NullInteger)
                        {
                            ModuleController mc = new ModuleController();
                            _ModuleName = mc.GetModule(ModuleId, TabID).DesktopModule.ModuleName;
                        }
                        else
                        {
                            _ModuleName = Consts.ModuleName;
                        }
                    }
                    else
                    {
                        _ModuleName = _ModuleConfiguration.DesktopModule.ModuleName;
                    }
                }
                return _ModuleName;
            }
        }

        public Enums.SelectBy SelectBy
        {
            get
            {
                return _SelectBy;
            }
            set
            {
                _SelectBy = value;
            }
        }

        public Enums.MultipleHandling MultipleHandling
        {
            get
            {
                return _MultipleHandling;
            }
            set
            {
                _MultipleHandling = value;
            }
        }

        public int Interval
        {
            get
            {
                return _Interval;
            }
            set
            {
                _Interval = value;
            }
        }

        public bool EnableUserTimeConversion
        {
            get
            {
                return _EnableUserTimeConversion;
            }
            set
            {
                _EnableUserTimeConversion = value;
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
                if (_CategoryID == -1)
                {
                    return "";
                }
                else
                {
                    ContentDejourController cdc = new ContentDejourController();
                    CategoryInfo objCategory = cdc.GetCategory(_CategoryID);
                    if (objCategory == null)
                    {
                        return "";
                    }
                    else
                    {
                        return objCategory.Category;
                    }
                }
            }
            set
            {
                //Read only properties are not serialized by default so must provide a dummy setter
                throw new NotImplementedException("Category property is read only");
            }
        }

        public string ProfilePropertyName
        {
            get
            {
                return _ProfilePropertyName;
            }
            set
            {
                _ProfilePropertyName = value;
            }
        }

        public bool IncludeDisabled
        {
            get
            {
                return _IncludeDisabled;
            }
            set
            {
                _IncludeDisabled = value;
            }
        }

        public bool HideWhenNoContent
        {
            get
            {
                return _HideWhenNoContent;
            }
            set
            {
                _HideWhenNoContent = value;
            }
        }

        public bool ReplaceTitle
        {
            get
            {
                return _ReplaceTitle;
            }
            set
            {
                _ReplaceTitle = value;
            }
        }

        public bool ReplaceTokens
        {
            get
            {
                return _ReplaceTokens;
            }
            set
            {
                _ReplaceTokens = value;
            }
        }

        #endregion

        #region Constructor
        public Configuration()
        {
            LoadDefaultSettings();
        }

        public Configuration(ModuleInfo ModuleConfiguration, Hashtable Settings)
        {
            _ModuleConfiguration = ModuleConfiguration;
            _ModuleId = _ModuleConfiguration.ModuleID;
            _TabID = _ModuleConfiguration.TabID;
            _portalSettings = new PortalSettings(_TabID, ModuleConfiguration.PortalID);
            _TabModuleID = _ModuleConfiguration.TabModuleID;
            _Settings = Settings;
            LoadSettings();
        }

        public Configuration(int ModuleId)
        {
            _ModuleId = ModuleId;
            _portalSettings = PortalController.GetCurrentPortalSettings();
            _TabID = _portalSettings.ActiveTab.TabID;
            var mc = new DotNetNuke.Entities.Modules.ModuleController();
            _ModuleConfiguration = mc.GetModule(_ModuleId, _TabID);
            _TabModuleID = _ModuleConfiguration.TabModuleID;

            Hashtable tabModuleSettings = mc.GetTabModuleSettings(_TabModuleID);
            Hashtable moduleSettings = mc.GetModuleSettings(_ModuleId);

            //Merge the TabModuleSettings and ModuleSettings
            _Settings = new Hashtable();
            foreach (string strKey in tabModuleSettings.Keys)
            {
                _Settings[strKey] = tabModuleSettings[strKey];
            }
            foreach (string strKey in moduleSettings.Keys)
            {
                _Settings[strKey] = moduleSettings[strKey];
            }

            LoadSettings();
        }

        public Configuration(int ModuleId, Hashtable Settings)
        {
            _ModuleId = ModuleId;
            _portalSettings = PortalController.GetCurrentPortalSettings();
            _TabID = _portalSettings.ActiveTab.TabID;
            DotNetNuke.Entities.Modules.ModuleController mc = new DotNetNuke.Entities.Modules.ModuleController();
            _ModuleConfiguration = mc.GetModule(_ModuleId, _TabID);
            _TabModuleID = _ModuleConfiguration.TabModuleID;
            _Settings = Settings;
            LoadSettings();
        }

        #endregion

        #region Public Methods
        public void LoadSettings()
        {
            if (Settings != null)
            {
                if (Settings.Count == 0)
                {
                    PreConfigureSettings(ModuleId, TabModuleID);
                }
                else
                {
                    _SelectBy = GetSetting("SelectBy", Defaults.SelectBy);
                    _MultipleHandling = GetSetting("MultipleHandling", Defaults.MultipleHandling);
                    _Interval = GetSetting("Interval", Defaults.Interval);
                    _EnableUserTimeConversion = GetSetting("EnableUserTimeConversion", Defaults.EnableUserTimeConversion);
                    _CategoryID = GetSetting("CategoryID", Defaults.CategoryID);
                    _ProfilePropertyName = GetSetting("ProfilePropertyName", Defaults.ProfilePropertyName);
                    _IncludeDisabled = GetSetting("IncludeDisabled", Defaults.IncludeDisabled);
                    _HideWhenNoContent = GetSetting("HideWhenNoContent", Defaults.HideWhenNoContent);
                    _ReplaceTitle = GetSetting("ReplaceTitle", Defaults.ReplaceTitle);
                    _ReplaceTokens = GetSetting("ReplaceTokens", Defaults.ReplaceTokens);
                }
            }
        }

        public void SaveSettings()
        {
            SaveSettings(_ModuleId, _TabModuleID);
        }

        public void SaveSettings(int ModuleId, int TabModuleId)
        {
            _ModuleId = ModuleId;
            _TabModuleID = TabModuleId;
            DotNetNuke.Entities.Modules.ModuleController mc = new DotNetNuke.Entities.Modules.ModuleController();
            mc.UpdateTabModuleSetting(TabModuleId, "SelectBy", _SelectBy.ToString());
            mc.UpdateTabModuleSetting(TabModuleId, "MultipleHandling", _MultipleHandling.ToString());
            mc.UpdateModuleSetting(ModuleId, "Interval", _Interval.ToString());
            mc.UpdateModuleSetting(ModuleId, "EnableUserTimeConversion", _EnableUserTimeConversion.ToString());
            mc.UpdateTabModuleSetting(TabModuleId, "CategoryID", _CategoryID.ToString());
            mc.UpdateTabModuleSetting(TabModuleId, "ProfilePropertyName", _ProfilePropertyName);
            mc.UpdateTabModuleSetting(TabModuleId, "IncludeDisabled", _IncludeDisabled.ToString());
            mc.UpdateTabModuleSetting(TabModuleId, "HideWhenNoContent", _HideWhenNoContent.ToString());
            mc.UpdateTabModuleSetting(TabModuleId, "ReplaceTitle", _ReplaceTitle.ToString());
            mc.UpdateTabModuleSetting(TabModuleId, "ReplaceTokens", _ReplaceTokens.ToString());
            DataCache.RemoveCache(string.Format(Consts.ConfigurationCacheKey, TabModuleId)); //Invalidate the tab modules's configuration cache
        }

        public void PreConfigureSettings(int ModuleId, int TabModuleId)
        {
            LoadDefaultSettings();
            SaveSettings(ModuleId, TabModuleId);
        }

        #endregion

        #region Private Methods

        private void LoadDefaultSettings()
        {
            _SelectBy = Defaults.SelectBy;
            _MultipleHandling = Defaults.MultipleHandling;
            _Interval = Defaults.Interval;
            _EnableUserTimeConversion = Defaults.EnableUserTimeConversion;
            _CategoryID = Defaults.CategoryID;
            _ProfilePropertyName = Defaults.ProfilePropertyName;
            _IncludeDisabled = Defaults.IncludeDisabled;
            _HideWhenNoContent = Defaults.HideWhenNoContent;
            _ReplaceTitle = Defaults.ReplaceTitle;
            _ReplaceTokens = Defaults.ReplaceTokens;
        }

        private T GetSetting<T>(string key, T defaultValue)
        {
            object obj = null;

            if (_Settings.ContainsKey(key))
            {
                obj = _Settings[key];
                if (defaultValue is System.Enum)
                {
                    try
                    {
                        return (T)Enum.Parse(typeof(T), Convert.ToString(obj));
                    }
                    catch (ArgumentException ex)
                    {
                        return defaultValue;
                    }
                }
                else if (defaultValue is System.DateTime)
                {
                    object objDateTime = null;
                    try
                    {
                        objDateTime = DateTime.Parse(Convert.ToString(obj));
                    }
                    catch (FormatException ex)
                    {
                        DateTime dt = DateTime.MinValue;
                        if (!(DateTime.TryParse(Convert.ToString(obj), System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None, out dt)))
                        {
                            dt = DateTime.Now;
                        }
                        objDateTime = dt;
                    }
                    return (T)objDateTime;
                }
                else
                {
                    try
                    {
                        return (T)Convert.ChangeType(obj, typeof(T));
                    }
                    catch (InvalidCastException ex)
                    {
                        return defaultValue;
                    }
                }
            }
            else
            {
                return defaultValue;
            }
        }
        #endregion

    }
}
