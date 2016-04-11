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
    public struct DayArray : IComparable<DayArray>, IXmlSerializable, ISerializable
    {
        public static Int32[] bitMasks = { 0x40000000, 0x20000000, 0x10000000, 0x8000000, 0x4000000, 0x2000000, 0x1000000, 0x800000, 0x400000, 0x200000, 0x100000, 0x80000, 0x40000, 0x20000, 0x10000, 0x8000, 0x4000, 0x2000, 0x1000, 0x800, 0x400, 0x200, 0x100, 0x80, 0x40, 0x20, 0x10, 0x8, 0x4, 0x2, 0x1 };

        private Int32 _Day;

        private const Int32 _AllDays = 0x7FFFFFFF;
        private const Int32 _Null = unchecked((Int32)0x80000000);
        private const Int32 _ModeFlag = unchecked((Int32)0x80000000);
        public static DayArray AllDays;
        public static DayArray Empty;
        public static DayArray Null;

        static DayArray()
        {
            AllDays = new DayArray(_AllDays);
            Empty = new DayArray();
            Null = new DayArray(_Null);
        }

        public DayArray(Int32 value)
            : this()
        {
            _Day = value;
        }

        private DayArray(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            SerializationInfoEnumerator enumerator = info.GetEnumerator();
            bool flag = false;
            Int32 bits = 0;
            while (enumerator.MoveNext())
            {
                string name = enumerator.Name;
                if (name != null)
                {
                    if (name == "bits")
                    {
                        bits = Convert.ToInt32(enumerator.Value, CultureInfo.InvariantCulture);
                        flag = true;
                    }
                }
            }
            if (!flag)
            {
                throw new SerializationException("Serialization Error: Missing DayArray Data");
            }
            _Day = bits;
        }

        public Int32 Value
        {
            get
            {
                return _Day;
            }
        }

        public bool IsDayOfYear
        {
            get
            {
                return (_Day & _ModeFlag) != 0;
            }
        }

        public bool IsDayOfMonth
        {
            get
            {
                return !((_Day & _ModeFlag) != 0);
            }
        }

        public bool IsMultiDayofMonth
        {
            get
            {
                if (IsDayOfMonth)
                {
                    int n = 0;
                    int bitCount = 0;
                    while (n < 32)
                    {
                        if ((_Day & bitMasks[n]) != 0)
                        {
                            bitCount++;
                            if (bitCount > 1)
                            {
                                return true;
                            }
                        }
                        n++;
                    }
                }
                return false;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return _Day == 0;
            }
        }

        public bool IsNull
        {
            get
            {
                return _Day == _Null;
            }
        }

        public bool IsAllDays
        {
            get
            {
                return (_Day & _AllDays) == _AllDays;
            }
        }

        public static int ModeFlag
        {
            get
            {
                return _ModeFlag;
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
            if (IsDayOfYear)
            {
                return (_Day & ~_ModeFlag).ToString();
            }

            int n = 0;
            int nLastHit = -1;
            int nConsecutive = 0;
            StringBuilder sb = new StringBuilder();
            while (n <= 31)
            {
                if ((n == 31) || (_Day & bitMasks[n]) == 0)
                {
                    if (nConsecutive > 0)
                    {
                        //If nConsecutive = 1 Then
                        //    sb.Append(", ")
                        //Else
                        sb.Append("-");
                        //End If
                        sb.Append(nLastHit + 1);
                        nConsecutive = 0;
                    }
                }
                else
                {
                    if (nLastHit == -1)
                    {
                        sb.Append(n + 1);
                    }
                    else if (n - nLastHit == 1)
                    {
                        nConsecutive++;
                    }
                    else
                    {
                        sb.Append(", ");
                        sb.Append(n + 1);
                        nConsecutive = 0;
                    }
                    nLastHit = n;
                }
                n++;
            }
            return sb.ToString();
        }

        public DayArray AddDay(int Day)
        {
            if (Day < 1 || Day > 31)
            {
                throw new ArgumentOutOfRangeException("day", "Day number must be in range of 1 through 31");
            }
            else
            {
                return new DayArray(_Day | bitMasks[Day - 1]);
            }
        }

        public DayArray RemoveDay(int Day)
        {
            if (Day < 1 || Day > 31)
            {
                throw new ArgumentOutOfRangeException("month", "Day number must be in range of 1 through 31");
            }
            else
            {
                return new DayArray(_Day & bitMasks[Day - 1]);
            }
        }

        public int CompareTo(DayArray other)
        {

            int tmp1 = 0;
            int tmp2 = 0;

            tmp1 = _Day;
            tmp2 = other._Day;
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
                } while (n < 32);
                return 0;
            }

        }

        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info != null)
            {
                info.AddValue("bits", _Day);
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            _Day = Convert.ToInt32(reader.ReadContentAsInt());
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteString(_Day.ToString());
        }
    }
}
