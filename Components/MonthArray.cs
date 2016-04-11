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
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Xml.Serialization;

namespace WESNet.DNN.Modules.ContentDejour
{
    [Serializable()]
    public struct MonthArray : IComparable<MonthArray>, IXmlSerializable, ISerializable
    {
        public static Int16[] bitMasks = { 0x1000, 0x800, 0x400, 0x200, 0x100, 0x80, 0x40, 0x20, 0x10, 0x8, 0x4, 0x2, 0x1 };

        private Int16 _Months;

        private const Int16 _AllMonths = 0x1FFF;
        private const Int16 _Null = 0x2000;
        public static MonthArray AllMonths;
        public static MonthArray Empty;
        public static MonthArray Null;

        static MonthArray()
        {
            AllMonths = new MonthArray(_AllMonths);
            Empty = new MonthArray();
            Null = new MonthArray(_Null);
        }

        public MonthArray(Int16 value)
            : this()
        {
            _Months = value;
        }

        private MonthArray(SerializationInfo info, StreamingContext context)
            : this()
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            SerializationInfoEnumerator enumerator = info.GetEnumerator();
            bool flag = false;
            Int16 bits = 0;
            while (enumerator.MoveNext())
            {
                string name = enumerator.Name;
                if (name != null)
                {
                    if (name == "bits")
                    {
                        bits = Convert.ToInt16(enumerator.Value, CultureInfo.InvariantCulture);
                        flag = true;
                    }
                }
            }
            if (!flag)
            {
                throw new SerializationException("Serialization Error: Missing MonthArray Data");
            }
            _Months = bits;
        }

        public Int16 Value
        {
            get
            {
                return _Months;
            }
        }

        public bool IsMultiMonth
        {
            get
            {
                int n = 0;
                int bitCount = 0;
                while (n < 13)
                {
                    if ((_Months & bitMasks[n]) != 0)
                    {
                        bitCount++;
                        if (bitCount > 1)
                        {
                            return true;
                        }
                    }
                    n++;
                }
                return false;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return _Months == 0;
            }
        }

        public bool IsNull
        {
            get
            {
                return _Months == _Null;
            }
        }

        public bool IsAllMonths
        {
            get
            {
                return (_Months & _AllMonths) == _AllMonths;
            }
        }

        public override string ToString()
        {
            return ToString(DateTimeFormatInfo.CurrentInfo);
        }

        public string ToString(IFormatProvider provider)
        {
            if (IsNull || IsEmpty)
            {
                return "";
            }

            string[] monthNames = DateTimeFormatInfo.GetInstance(provider).AbbreviatedMonthNames;

            //The AbbreviatedMonthNames array will contain 13 elements. Depending on culture, the item at n=12
            //may be empty string, so adjust upper bound used for iteration

            //Dim nUpperBound As Integer = monthNames.Length - 1
            //If String.IsNullOrEmpty(monthNames(nUpperBound)) Then nUpperBound -= 1

            int n = 0;
            int nLastHit = -1;
            int nConsecutive = 0;
            StringBuilder sb = new StringBuilder();
            while (n <= 12)
            {
                if ((n == 12) || (_Months & bitMasks[n]) == 0)
                {
                    if (nConsecutive > 0)
                    {
                        //If nConsecutive = 1 Then
                        //    sb.Append(", ")
                        //Else
                        sb.Append("-");
                        //End If
                        sb.Append(monthNames[nLastHit]);
                        nConsecutive = 0;
                    }
                }
                else
                {
                    if (nLastHit == -1)
                    {
                        sb.Append(monthNames[n]);
                    }
                    else if (n - nLastHit == 1)
                    {
                        nConsecutive++;
                    }
                    else
                    {
                        sb.Append(", ");
                        sb.Append(monthNames[n]);
                        nConsecutive = 0;
                    }
                    nLastHit = n;
                }
                n++;
            }
            return sb.ToString();
        }

        public MonthArray AddMonth(int Month)
        {
            if (Month < 1 || Month > 13)
            {
                throw new ArgumentOutOfRangeException("month", "Month number must be in range of 1 through 12");
            }
            else
            {
                return new MonthArray((short)(_Months | bitMasks[Month - 1]));
            }
        }

        public MonthArray RemoveMonth(int Month)
        {
            if (Month < 1 || Month > 13)
            {
                throw new ArgumentOutOfRangeException("month", "Month number must be in range of 1 through 12");
            }
            else
            {
                return new MonthArray((short)(_Months & bitMasks[Month - 1]));
            }
        }

        public int CompareTo(MonthArray other)
        {

            int tmp1 = 0;
            int tmp2 = 0;

            tmp1 = _Months;
            tmp2 = other._Months;
            if (tmp1 == tmp2)
            {
                return 0;
            }
            else
            {
                if (IsEmpty)
                {
                    return -1;
                }
                if (IsNull)
                {
                    return 1;
                }
                int n = 0;
                do
                {
                    int mask = bitMasks[n];
                    int rslt = ((tmp1 & mask) - (tmp2 & mask));
                    if (rslt != 0)
                    {
                        return rslt;
                    }
                    n++;
                } while (n < 13);
                return 0;
            }

        }

        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info != null)
            {
                info.AddValue("bits", _Months);
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            _Months = Convert.ToByte(reader.ReadContentAsInt());
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteString(_Months.ToString());
        }
    }
}
