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
//

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Modules;
using DotNetNuke.Web.UI.WebControls;

using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WESNet.DNN.Modules.ContentDejour
{
    public partial class CategoryEditor : DotNetNuke.Framework.UserControlBase
    {
        private int _ModuleId = -1;
        private string _LocalResourceFile;
        private string _NewItem;

        public bool ShowRowNumbers
        {
            get
            {
                return ViewState["ShowRowNumbers"] == null ? true : Convert.ToBoolean(ViewState["ShowRowNumbers"]);
            }
            set
            {
                ViewState["ShowRowNumbers"] = value;
            }
        }

        public int ModuleId
        {
            get
            {
                if (_ModuleId == Null.NullInteger)
                {
                    _ModuleId = GetModuleId(this);
                }
                return _ModuleId;
            }
        }

        public string LocalResourceFile
        {
            get
            {
                if (_LocalResourceFile == null)
                {
                    _LocalResourceFile = GetLocalResourceFile(null);
                }
                return _LocalResourceFile;
            }
        }

        private int GetModuleId(Control ctl)
        {
            if (ctl == null)
            {
                return -1;
            }
            else
            {
                int mid = -1;
                Control Parent = ctl.Parent;
                if (Parent != null)
                {
                    if (Parent is IModuleControl)
                    {
                        mid = ((IModuleControl)Parent).ModuleContext.ModuleId;
                    }
                    else
                    {
                        System.Reflection.PropertyInfo pi = Parent.GetType().GetProperty("ModuleId");
                        if (pi != null)
                        {
                            //If Parent has a ModuleId property use this
                            mid = Convert.ToInt32(pi.GetValue(Parent, null));
                        }
                        else
                        {
                            //Otherwise, look further up the control hierarchy
                            mid = GetModuleId(Parent);
                        }
                    }
                }
                return mid;
            }
        }

        private string GetLocalResourceFile(Control ctl)
        {

            if (ctl == null)
            {
                return this.TemplateSourceDirectory + "/App_LocalResources/CategoryEditor.ascx.resx";
            }
            else
            {
                string LocalResourceFile = string.Empty;
                Control Parent = ctl.Parent;
                if (Parent != null)
                {
                    if (Parent is IModuleControl)
                    {
                        LocalResourceFile = ((IModuleControl)Parent).LocalResourceFile;
                    }
                    else
                    {
                        System.Reflection.PropertyInfo pi = Parent.GetType().GetProperty("LocalResourceFile");
                        if (pi != null)
                        {
                            //If Parent has a LocalResourceFile property use it
                            LocalResourceFile = Convert.ToString(pi.GetValue(Parent, null));
                        }
                        else
                        {
                            //Otherwise, look further up the control hierarchy
                            LocalResourceFile = GetLocalResourceFile(Parent);
                        }
                    }
                }
                return LocalResourceFile;
            }
        }

        private void BindList()
        {
            var cdc = new ContentDejourController();
            gvList.DataKeyNames = new [] { "CategoryID", "ViewOrder" };
            gvList.DataSource = cdc.GetCategories(ModuleId);
            gvList.DataBind();
            if (gvList.Rows.Count > 0 && gvList.EditIndex == -1)
            {
                gvList.Rows[gvList.Rows.Count - 1].FindControl("btnDown").Visible = false;
            }
        }

        private void SaveRow(GridViewRow row)
        {
            int ViewOrder = Convert.ToInt32(gvList.DataKeys[row.RowIndex].Values[1]);
            SaveRow(row, ViewOrder);
        }

        private void SaveRow(GridViewRow row, int ViewOrder)
        {
            if (gvList.DataKeys.Count > row.RowIndex)
            {
                int ID = Convert.ToInt32(gvList.DataKeys[row.RowIndex].Value);
                string Name = null;
                if ((row.RowState & DataControlRowState.Edit) != 0)
                {
                    Name = ((TextBox)row.FindControl("tbName")).Text;
                }
                else
                {
                    Name = ((Label)row.FindControl("lblName")).Text;
                }
                Save(ID, Name, ModuleId, ViewOrder);
            }
        }

        private void Save(int ID, string Name, int ModuleID, int ViewOrder)
        {
            var cdc = new ContentDejourController();
            var objCategoryInfo = new CategoryInfo();
            objCategoryInfo.ModuleID = ModuleID;
            objCategoryInfo.CategoryID = ID;
            objCategoryInfo.Category = Name;
            objCategoryInfo.ViewOrder = ViewOrder;
            if (ID == -1)
            {
                cdc.AddCategory(objCategoryInfo);
            }
            else
            {
                cdc.UpdateCategory(objCategoryInfo);
            }
        }

        protected void btnAdd_Click(object sender, System.EventArgs e)
        {
            int Idx = gvList.Rows.Count - 1;
            int ViewOrder = 0;

            if (Idx >= 0)
            {
                ViewOrder = Convert.ToInt32(gvList.DataKeys[Idx].Values[1]) + 1;
            }

            Save(-1, _NewItem, ModuleId, ViewOrder);
            gvList.EditIndex = Idx + 1;
            BindList();
            btnAdd.Visible = false;
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            _NewItem = Localization.GetString("New_Item", LocalResourceFile);
            if (!IsPostBack)
            {
                gvList.Columns[0].Visible = ShowRowNumbers;
                Localization.LocalizeGridView(ref gvList, LocalResourceFile);
                BindList();
            }
        }

        protected void gvList_RowCancelingEdit(object sender, System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
        {
            gvList.EditIndex = -1;
            btnAdd.Visible = true;
            BindList();
        }

        protected void gvList_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName.StartsWith("Move"))
            {
                int Idx = Convert.ToInt32(e.CommandArgument);
                switch (e.CommandName)
                {
                    case "MoveUp":
                        SwapOrder(Idx, Idx - 1);
                        break;
                    case "MoveDown":
                        SwapOrder(Idx, Idx + 1);
                        break;
                }
            }
        }

        private void SwapOrder(int Idx, int Idx2)
        {
            if (Idx2 >= 0 && Idx2 < gvList.DataKeys.Count)
            {
                int ViewOrder1 = Convert.ToInt32(gvList.DataKeys[Idx].Values[1]);
                int ViewOrder2 = Convert.ToInt32(gvList.DataKeys[Idx2].Values[1]);
                SaveRow(gvList.Rows[Idx], ViewOrder2);
                SaveRow(gvList.Rows[Idx2], ViewOrder1);
            }
            BindList();
            OnDataChanged(new System.EventArgs());
        }

        protected void gvList_RowCreated(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (ShowRowNumbers && e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[0].Text = (e.Row.RowIndex + 1).ToString();
            }
        }

        protected void gvList_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            TextBox tbName = null;
            Label lblName = null;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DnnImageButton btnUp = (DnnImageButton)e.Row.FindControl("btnUp");
                btnUp.CommandArgument = e.Row.RowIndex.ToString();
                DnnImageButton btnDown = (DnnImageButton)e.Row.FindControl("btnDown");
                btnDown.CommandArgument = e.Row.RowIndex.ToString();

                if (gvList.EditIndex != -1)
                {
                    btnUp.Visible = false;
                    btnDown.Visible = false;
                }
                else if (e.Row.RowIndex == 0)
                {
                    btnUp.Visible = false;
                    btnDown.Style.Add("margin-left", "24px");
                }

                Control btnEdit = e.Row.FindControl("btnEdit");
                Control btnDelete = e.Row.FindControl("btnDelete");
                Control btnCancel = e.Row.FindControl("btnCancel");
                Control btnUpdate = e.Row.FindControl("btnUpdate");

                if ((e.Row.RowState & DataControlRowState.Edit) != 0)
                {
                    tbName = (TextBox)e.Row.FindControl("tbName");
                    if (tbName != null)
                    {
                        tbName.Text = ((CategoryInfo)e.Row.DataItem).Category;
                    }
                    btnEdit.Visible = false;
                    btnDelete.Visible = false;
                    btnCancel.Visible = true;
                    btnUpdate.Visible = true;
                }
                else
                {
                    lblName = (Label)e.Row.FindControl("lblName");
                    if (lblName != null)
                    {
                        lblName.Text = ((CategoryInfo)e.Row.DataItem).Category;
                    }
                    btnEdit.Visible = true;
                    btnDelete.Visible = true;
                    btnCancel.Visible = false;
                    btnUpdate.Visible = false;
                }
            }
        }

        protected void gvList_RowDeleting(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            if (gvList.DataKeys.Count > e.RowIndex)
            {
                int ID = Convert.ToInt32(gvList.DataKeys[e.RowIndex].Value);
                ContentDejourController cdc = new ContentDejourController();
                cdc.DeleteCategory(ID);
                BindList();
                OnDataChanged(new System.EventArgs());
            }
        }

        protected void gvList_RowEditing(object sender, System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            gvList.EditIndex = e.NewEditIndex;
            btnAdd.Visible = false;
            BindList();
        }

        protected void gvList_RowUpdating(object sender, System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            SaveRow(gvList.Rows[e.RowIndex]);
            gvList.EditIndex = -1;
            btnAdd.Visible = true;
            BindList();
            OnDataChanged(new System.EventArgs());
        }

        #region Public Events
        public event System.EventHandler DataChanged;

        public void OnDataChanged(System.EventArgs e)
        {
            if (DataChanged != null)
                DataChanged(this, e);
        }
        #endregion

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnAdd.Click += btnAdd_Click;
            this.Load += Page_Load;
            gvList.RowCancelingEdit += gvList_RowCancelingEdit;
            gvList.RowCommand += gvList_RowCommand;
            gvList.RowCreated += gvList_RowCreated;
            gvList.RowDataBound += gvList_RowDataBound;
            gvList.RowDeleting += gvList_RowDeleting;
            gvList.RowEditing += gvList_RowEditing;
            gvList.RowUpdating += gvList_RowUpdating;
        }
    }
}
