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

namespace WESNet.DNN.Modules.ContentDejour
{
	public class ContentDejourInfoComparer : System.Collections.Generic.IComparer<ContentDejourInfo>
	{
		private SortColumnList _SortColumns;

		public ContentDejourInfoComparer(SortColumnList SortColumns)
		{
			_SortColumns = SortColumns;
		}

		public ContentDejourInfoComparer(string SortExpression)
		{
			_SortColumns = new SortColumnList();
			_SortColumns.SetInitialSort(SortExpression);
		}

		public int Compare(ContentDejourInfo x, ContentDejourInfo y)
		{
			if (_SortColumns.Count == 0)
			{
				return 0;
			}
			return Compare(0, x, y);
		}

		private int Compare(int SortColumnIndex, ContentDejourInfo x, ContentDejourInfo y)
		{
			SortColumnInfo SortColumn = null;
			int Result = 0;

			if (SortColumnIndex >= _SortColumns.Count)
			{
				return 0;
			}

			SortColumn = _SortColumns[SortColumnIndex];

			if (SortColumn.Direction == SortColumnInfo.SortDirection.Ascending)
			{
				Result = CompareValues(SortColumn.ColumnName, x, y);
			}
			else if (SortColumn.Direction == SortColumnInfo.SortDirection.Descending)
			{
				Result = CompareValues(SortColumn.ColumnName, y, x);
			}

			// Difference not found, sort by next sort column
			if (Result == 0)
			{
				return Compare(SortColumnIndex + 1, x, y);
			}
			else
			{
				return Result;
			}
		}

		private int CompareValues(string ColumnName, ContentDejourInfo x, ContentDejourInfo y)
		{
			switch (ColumnName)
			{
				case "Month":
				case "Months":
					if (x.Months.CompareTo(y.Months) != 0)
					{
						return x.Months.CompareTo(y.Months);
					}
					break;
				case "Day":
				case "Days":
					if (x.Days.CompareTo(y.Days) != 0)
					{
						return x.Days.CompareTo(y.Days);
					}
					break;
				case "DayofWeek":
				case "DaysofWeek":
					if (x.DaysofWeek.CompareTo(y.DaysofWeek) != 0)
					{
						return x.DaysofWeek.CompareTo(y.DaysofWeek);
					}
					break;
				case "StartTime":
					if (x.StartTime.CompareTo(y.StartTime) != 0)
					{
						return x.StartTime.CompareTo(y.StartTime);
					}
					break;
				case "EndTime":
					if (x.EndTime.CompareTo(y.EndTime) != 0)
					{
						return x.EndTime.CompareTo(y.EndTime);
					}
					break;
				case "TimeDuration":
					if (x.TimeDuration.CompareTo(y.TimeDuration) != 0)
					{
						return x.TimeDuration.CompareTo(y.TimeDuration);
					}
					break;
				case "Category":
					if (x.Category.CompareTo(y.Category) != 0)
					{
						return x.Category.CompareTo(y.Category);
					}
					break;
				case "ProfileProperty":
					if (x.ProfilePropertyValue.CompareTo(y.ProfilePropertyValue) != 0)
					{
						return x.ProfilePropertyValue.CompareTo(y.ProfilePropertyValue);
					}
					break;
				case "Title":
					if (x.Title.CompareTo(y.Title) != 0)
					{
						return x.Title.CompareTo(y.Title);
					}
					break;
				case "CreatedDate":
					if (x.CreatedOnDate.CompareTo(y.CreatedOnDate) != 0)
					{
						return x.CreatedOnDate.CompareTo(y.CreatedOnDate);
					}
					break;
			}
			return 0;
		}
	}
}
