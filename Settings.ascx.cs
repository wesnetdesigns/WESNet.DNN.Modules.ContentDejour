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


using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Client.ClientResourceManagement;

using System;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace WESNet.DNN.Modules.ContentDejour
{
    public partial class Settings : ModuleSettingsBase
    {
        #region Private Members
        private Configuration _MyConfiguration;
        #endregion

        #region Base Method Implementations

        public override void LoadSettings()
        {

            ListItem li = null;
            try
            {
                //_MyConfiguration = new Configuration(ModuleId, Settings);
                if (!IsPostBack)
                {
                    BindCategories();
                    BindProfilePropertyNames();
                    li = rblSelectBy.Items.FindByValue(Convert.ToInt32(_MyConfiguration.SelectBy).ToString());
                    if (li == null)
                    {
                        rblSelectBy.SelectedIndex = 0;
                    }
                    else
                    {
                        rblSelectBy.SelectedIndex = rblSelectBy.Items.IndexOf(li);
                    }
                    li = rblMultipleHandling.Items.FindByValue(Convert.ToInt32(_MyConfiguration.MultipleHandling).ToString());
                    if (li == null)
                    {
                        rblMultipleHandling.SelectedIndex = 0;
                    }
                    else
                    {
                        rblMultipleHandling.SelectedIndex = rblMultipleHandling.Items.IndexOf(li);
                    }
                    cbHideWhenNoContent.Checked = _MyConfiguration.HideWhenNoContent;
                    cbEnableUserTimeConversion.Checked = _MyConfiguration.EnableUserTimeConversion;
                    li = ddlCategory.Items.FindByValue(_MyConfiguration.CategoryID.ToString());
                    if (li != null)
                    {
                        ddlCategory.SelectedIndex = ddlCategory.Items.IndexOf(li);
                    }
                    li = ddlProfilePropertyName.Items.FindByValue(_MyConfiguration.ProfilePropertyName);
                    if (li != null)
                    {
                        ddlProfilePropertyName.SelectedIndex = ddlProfilePropertyName.Items.IndexOf(li);
                    }
                    cbIncludeDisabled.Checked = _MyConfiguration.IncludeDisabled;
                    cbReplaceTitle.Checked = _MyConfiguration.ReplaceTitle;
                    cbReplaceTokens.Checked = _MyConfiguration.ReplaceTokens;
                    tbNumericInterval.Text = _MyConfiguration.Interval.ToString();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        public override void UpdateSettings()
        {
            if (Page.IsValid)
            {
                try
                {
                    _MyConfiguration.SelectBy = (Enums.SelectBy)Convert.ToInt32(rblSelectBy.SelectedValue);
                    _MyConfiguration.MultipleHandling = (Enums.MultipleHandling)Convert.ToInt32(rblMultipleHandling.SelectedValue);
                    _MyConfiguration.HideWhenNoContent = cbHideWhenNoContent.Checked;
                    _MyConfiguration.EnableUserTimeConversion = cbEnableUserTimeConversion.Checked;
                    _MyConfiguration.CategoryID = int.Parse(ddlCategory.SelectedValue);
                    _MyConfiguration.ProfilePropertyName = ddlProfilePropertyName.SelectedValue;
                    _MyConfiguration.IncludeDisabled = cbIncludeDisabled.Checked;
                    _MyConfiguration.ReplaceTitle = cbReplaceTitle.Checked;
                    _MyConfiguration.ReplaceTokens = cbReplaceTokens.Checked;
                    var i = 0;
                    if (int.TryParse(tbNumericInterval.Text, out i))
                    {
                        _MyConfiguration.Interval = i;

                    }
                    // refresh cache
                    ModuleController.SynchronizeModule(ModuleId);
                    _MyConfiguration.SaveSettings();
                    // disable module caching if token replace is enabled
                    if (cbReplaceTokens.Checked)
                    {
                        var mc = new ModuleController();
                        DotNetNuke.Entities.Modules.ModuleInfo objModule = mc.GetModule(ModuleId, TabId, false);
                        if (objModule.CacheTime > 0)
                        {
                            objModule.CacheTime = 0;
                            mc.UpdateModule(objModule);
                        }
                    }

                }
                catch (Exception exc) //Module failed to load
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                }
            }
        }

        #endregion

        #region Private Methods
        private void BindCategories()
        {
            ContentDejourController cdc = new ContentDejourController();
            ddlCategory.DataSource = cdc.GetCategories(ModuleId);
            ddlCategory.DataBind();
            ddlCategory.Items.Insert(0, new ListItem(Localization.GetString("ANY", _MyConfiguration.LocalSharedResourceFile), "-1"));
        }

        private void BindProfilePropertyNames()
        {
            ProfilePropertyDefinitionCollection ppds = ProfileController.GetPropertyDefinitionsByPortal(PortalId);
            foreach (ProfilePropertyDefinition ppd in ppds)
            {
                string text = Localization.GetString("ProfileProperties_" + ppd.PropertyName + ".Text", _MyConfiguration.ProfileResourceFile, PortalSettings, UserInfo.Profile.PreferredLocale);
                if (text == null)
                {
                    text = ppd.PropertyName;
                }
                ListItem li = new ListItem(text.TrimEnd(':'), ppd.PropertyName);
                ddlProfilePropertyName.Items.Add(li);
            }
            ddlProfilePropertyName.Items.Insert(0, new ListItem(Localization.GetString("NONE_SELECTED", _MyConfiguration.LocalSharedResourceFile), ""));
        }
        #endregion

        #region Event Handlers
        protected void CategoryEditor_DataChanged(object sender, System.EventArgs e)
        {
            string strCategoryID = ddlCategory.SelectedValue;
            BindCategories();
            if (strCategoryID != "-1")
            {
                ListItem li = ddlCategory.Items.FindByValue(strCategoryID);
                if (li != null)
                {
                    ddlCategory.SelectedIndex = ddlCategory.Items.IndexOf(li);
                }
            }
        }

        protected void Page_Init(object sender, System.EventArgs e)
        {
            DotNetNuke.Framework.AJAX.RegisterPostBackControl(ctlCategoryEditor);
            JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            ClientResourceManager.RegisterStyleSheet(Page, Consts.ModuleFolderPath + "Themes/smoothness/jquery-ui-1.10.0.custom.css");

            _MyConfiguration = new Configuration(ModuleId, Settings);

            //INSTANT C# NOTE: Converted event handler wireups:
            ctlCategoryEditor.DataChanged += CategoryEditor_DataChanged;

        }

        #endregion


        public Settings()
        {
            this.Init += Page_Init;
        }
    }
}
