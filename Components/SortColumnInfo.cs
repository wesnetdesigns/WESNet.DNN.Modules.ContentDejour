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

namespace WESNet.DNN.Modules.ContentDejour
{
	[Serializable()]
	public class SortColumnInfo
	{
		private string _ColumnName;
		private SortDirection _Direction = SortDirection.NotSorted;

		public enum SortDirection: int
		{
			NotSorted = 0,
			Ascending,
			Descending
		}

		public SortColumnInfo()
		{
			//Default Constructor
		}

		public SortColumnInfo(string ColumnName)
		{
			_ColumnName = ColumnName;
		}

		public SortColumnInfo(string ColumnName, SortDirection Direction)
		{
			_ColumnName = ColumnName;
			_Direction = Direction;
		}

		public SortColumnInfo(string ColumnName, string Direction) : this(ColumnName)
		{
			switch (Direction)
			{
				case "":
					_Direction = SortDirection.NotSorted;
					break;
				case "ASC":
					_Direction = SortDirection.Ascending;
					break;
				case "DESC":
					_Direction = SortDirection.Descending;
					break;
				default:
					throw new ArgumentOutOfRangeException("Direction", "Sort direction must be '', 'ASC' or 'DESC'");
			}
		}

		public string ColumnName
		{
			get
			{
				return _ColumnName;
			}
			set
			{
				_ColumnName = value;
			}
		}

		public SortDirection Direction
		{
			get
			{
				return _Direction;
			}
			set
			{
				_Direction = value;
			}
		}

		public string DirectionString
		{
			get
			{
				string s = "";
				switch (_Direction)
				{
					case SortDirection.Ascending:
					s = "ASC";
					break;
					case SortDirection.Descending:
					s = "DESC";
					break;
				}
				return s;
			}
		}

		public string OrderByExpression
		{
			get
			{
				return (ColumnName + " " + DirectionString).TrimEnd(' ');
			}
		}

		public string DirectionGlyph
		{
			get
			{
				string Glyph = "spacer.gif";
				switch (_Direction)
				{
					case SortDirection.Ascending:
					Glyph = Consts.SortAscendingGlyph;
					break;
					case SortDirection.Descending:
					Glyph = Consts.SortDescendingGlyph;
					break;
				}
				return string.Format("<img src='{0}' alt='{1}' />", DotNetNuke.Common.Globals.ResolveUrl(Glyph), _Direction.ToString());
			}
		}

		public void ToggleDirection()
		{
			Direction = (SortDirection)((Convert.ToInt32(Direction) + 1) % 3);
		}
	}
}
