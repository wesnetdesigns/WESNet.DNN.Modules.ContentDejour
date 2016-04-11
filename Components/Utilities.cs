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

using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;

using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

namespace WESNet.DNN.Modules.ContentDejour
{
  public class Utilities
  {

	public static string GetSummary(string desktopSummary, string desktopHTML)
	{
	  if (!(string.IsNullOrEmpty(desktopSummary)))
	  {
		return HttpUtility.HtmlDecode(desktopSummary);
	  }
	  else
	  {
		return ContentDejour.Utilities.TruncateString(HttpUtility.HtmlDecode(desktopHTML), Consts.MaxDescLength);
	  }
	}

	public static string TruncateString(string Text, int MaxLength)
	{
	  string s = string.Empty;
	  int p = 0;
	  if (!(string.IsNullOrEmpty(Text)))
	  {
		Text = StripHTMLTags(Text, false);
		if (Text.Length > MaxLength)
		{
		  p = Text.LastIndexOfAny(new char[] { '.', ' ', '\r' }, MaxLength - 1);
		  if (p > 0)
		  {
			  s = Text.Substring(0, p) + "...";
		  }
		}
		else
		{
		  s = Text;
		}
	  }
	  return s;
	}

	public static string StripImageTags(string Text)
	{
	  Regex rgx = new Regex("<img[^>]*>", RegexOptions.IgnoreCase);
	  return rgx.Replace(Text, "");
	}

	public static string StripAnchorTags(string Text)
	{
	  Regex rgx = new Regex("<a\\s+href='(?<href>[^']*)'[^>]*>(?<inner>[^>]*)</a>", RegexOptions.IgnoreCase);
	  return rgx.Replace(Text, "${inner} (${href})");
	}

	public static string ReplaceBreaks(string Text)
	{
	  return Regex.Replace(Text, "<\\s*br\\s*/?\\s*>", Environment.NewLine, RegexOptions.IgnoreCase);
	}

	public static string ReplacevbCrlf(string Text)
	{
	  Text = Text.Replace(Environment.NewLine, "<br />");
	  return Text.Replace("\r", "<br />");
	}

	public static string StripHTMLTags(string Text, bool RetainWhiteSpace)
	{
	  DotNetNuke.Security.PortalSecurity ps = new DotNetNuke.Security.PortalSecurity();
	  Text = ps.InputFilter(Text, PortalSecurity.FilterFlag.NoScripting);
	  Text = Regex.Replace(Text, "<(.|\\n)*?>", " ");
	  if (!RetainWhiteSpace)
	  {
		  Text = Regex.Replace(Text, "\\s{2,}", " ").Trim();
	  }
	  return Text;
	}

	public static string CleanMultiLineHTML(string Text)
	{
	  Text = Regex.Replace(Text, "<b\\s*/?><p>", "<p>");
	  Text = Regex.Replace(Text, "<b\\s*/?></p>", "</p>");
	  Text = Regex.Replace(Text, "<p><br\\s*/?>", "<p>");
	  Text = Regex.Replace(Text, "</p>(<br\\s*/?>)+", "</p>");
	  Text = Regex.Replace(Text, "<p>\\s*</p>", "");
	  return Text;
	}

	public static void SelectCBLItems(CheckBoxList CBL, int Bits)
	{
	  int nBits = CBL.Items.Count;
	  int allBitsMask = Convert.ToInt32(((Math.Pow(2, nBits))) - 1);
	  ListItem li = null;
	  if ((Bits & allBitsMask) == allBitsMask)
	  {
		SelectAllCBLItems(CBL);
	  }
	  else
	  {
		int n = nBits - 1;
		CBL.ClearSelection();
		do
		{
		  if ((Bits & 0x1) != 0)
		  {
			li = CBL.Items.FindByValue(n.ToString());
			if (li != null && li.Enabled)
			{
				li.Selected = true;
			}
		  }
		  Bits >>= 1;
		  n--;
		} while (n >= 0);
	  }
	}

	public static int GetSelectedCBLItems(CheckBoxList CBL)
	{
	  int nBits = CBL.Items.Count;
	  int n = 0;
	  foreach (ListItem li in CBL.Items)
	  {
		if (li.Selected && li.Enabled)
		{
		  n = n | Convert.ToInt32((Math.Pow(2, (nBits - 1 - Convert.ToInt32(li.Value)))));
		}
	  }
	  return n;
	}

	public static void SelectAllCBLItems(CheckBoxList CBL)
	{
	  foreach (ListItem li in CBL.Items)
	  {
		li.Selected = true;
	  }
	}

	public static void ClearAllSelectedCBLItems(CheckBoxList CBL)
	{
	  CBL.ClearSelection();
	}

	public static DateTime GetCurrentPortalTime()
	{
	  PortalSettings ps = PortalController.GetCurrentPortalSettings();
	  TimeZoneInfo portalTimeZone = ps.TimeZone;
	  DateTime systemCurrentTime = DateTime.Now;
	  DateTime portalCurrentTime = TimeZoneInfo.ConvertTime(systemCurrentTime, portalTimeZone);
	  return portalCurrentTime;
	}

	public static DateTime ConvertPortalToUserTime(DateTime Value)
	{
	  DateTime result = Value;
	  PortalSettings ps = PortalController.GetCurrentPortalSettings();
	  TimeZoneInfo portalTimeZone = ps.TimeZone;
	  UserInfo user = UserController.GetCurrentUserInfo();
	  if (user.UserID == -1)
	  {
		result = Value;
	  }
	  else
	  {
		TimeZoneInfo userTimeZone = user.Profile.PreferredTimeZone;
		result = TimeZoneInfo.ConvertTime(Value, portalTimeZone, userTimeZone);
	  }
	  return result;
	}

  }
}
