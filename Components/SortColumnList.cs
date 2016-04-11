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

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace WESNet.DNN.Modules.ContentDejour
{
	[Serializable()]
	public class SortColumnList : System.Collections.Generic.List<SortColumnInfo>
	{
		private void AddSortColumn(string ColumnName)
		{
			Add(new SortColumnInfo(ColumnName, SortColumnInfo.SortDirection.Ascending));
		}

		private void AddSortColumn(string ColumnName, SortColumnInfo.SortDirection Direction)
		{
			Add(new SortColumnInfo(ColumnName, Direction));
		}

		private SortColumnInfo RemoveSortColumn(string ColumnName)
		{
			SortColumnInfo SortColumn = null;
			int i = FindColumnIndex(ColumnName);
			if (i != -1)
			{
				SortColumn = this[i];
				RemoveAt(i);
			}
			return SortColumn;
		}

		public void SetInitialSort(string SortExpression)
		{
			string ColumnName = null;
			string Direction = null;

			Clear();
			if (!(string.IsNullOrEmpty(SortExpression)))
			{
				Match m = Regex.Match(SortExpression, "\\s*([A-Za-z]{1}\\w*)(?:\\s*(ASC|DESC))?\\,?");
				while (m.Success)
				{
					ColumnName = "";
					Direction = "";
					if (m.Groups.Count > 1)
					{
						ColumnName = m.Groups[1].Value;
						if (m.Groups.Count > 2)
						{
							Direction = m.Groups[2].Value;
						}
						Add(new SortColumnInfo(ColumnName, Direction));
					}
					m = m.NextMatch();
				}
			}
		}

		public int FindColumnIndex(string ColumnName)
		{
			int i = 0;
			while (i < Count)
			{
				if (this[i].ColumnName == ColumnName)
				{
					return i;
				}
				i++;
			}
			return -1;
		}

		public void SortOn(string ColumnName)
		{
			int i = FindColumnIndex(ColumnName);
			if (i == -1)
			{
				AddSortColumn(ColumnName);
			}
			else
			{
				SortColumnInfo SortColumn = this[i];
				SortColumn.ToggleDirection();
				if (SortColumn.Direction == SortColumnInfo.SortDirection.NotSorted)
				{
					RemoveAt(i);
				}
			}
		}

		public string OrderByExpression()
		{
			StringBuilder sb = new StringBuilder();
			foreach (SortColumnInfo SortColumn in this)
			{
				sb.Append(SortColumn.OrderByExpression);
				sb.Append(",");
			}
			return sb.ToString().TrimEnd(',');
		}
	}
}
