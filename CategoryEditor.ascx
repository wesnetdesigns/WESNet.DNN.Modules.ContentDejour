<%@ Control Language="C#" AutoEventWireup="false" Inherits="WESNet.DNN.Modules.ContentDejour.CategoryEditor" Codebehind="CategoryEditor.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>

<div class="WESNet_ContentDejour_CategoryEditor">
	 <asp:GridView id="gvList" runat="server" AutoGenerateColumns="False" GridLines="None">
		 <Columns>
			<asp:TemplateField ShowHeader="False" ItemStyle-CssClass="ItemNumber">
			  <ItemTemplate>
				<asp:Label ID="lblItemNo" runat="server"></asp:Label>
			  </ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField ShowHeader="False" ItemStyle-CssClass="CommandField">
			  <ItemTemplate>
				 <dnn:DnnImageButton runat="server" ID="btnEdit" resourcekey="btnEdit" CommandName="Edit" IconKey="Edit"  />
				 <dnn:DnnImageButton runat="server" ID="btnDelete" resourcekey="btnDelete" CommandName="Delete" IconKey="Delete" />
				 <dnn:DnnImageButton runat="server" ID="btnCancel" resourcekey="btnCancel" CommandName="Cancel" IconKey="Cancel" />
				 <dnn:DnnImageButton runat="server" ID="btnUpdate" resourcekey="btnUpdate" CommandName="Update" IconKey="Save" />
			  </ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField ShowHeader="True" HeaderText="Category" HeaderStyle-CssClass="dnnGridHeader" ItemStyle-CssClass="Category">
				<ItemTemplate>
				   <asp:Label ID="lblName" runat="server"></asp:Label>
				</ItemTemplate>
				<EditItemTemplate>
					<asp:TextBox ID="tbName" runat="server"></asp:TextBox>
				</EditItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField ShowHeader="False" ItemStyle-CssClass="CommandField">
			  <ItemTemplate>
				 <dnn:DnnImageButton Runat="server" ID="btnUp" resourcekey="btnUp" CommandName="MoveUp" IconKey="Up" />
				 <dnn:DnnImageButton Runat="server" ID="btnDown" resourcekey="btnDown" CommandName="MoveDown" IconKey="Dn" /> 
			  </ItemTemplate>
		  </asp:TemplateField>
		</Columns>
	   </asp:GridView>
	   <asp:LinkButton id="btnAdd" runat="server" resourcekey="btnAdd"></asp:LinkButton>
</div>