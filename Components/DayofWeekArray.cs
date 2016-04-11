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
    public struct DayofWeekArray : IComparable<DayofWeekArray>, IXmlSerializable, ISerializable
    {
        public static byte[] bitMasks = { 0x40, 0x20, 0x10, 0x8, 0x4, 0x2, 0x1 };

        private byte _DaysofWeek;

        private const byte _AllDaysofWeek = 0x7F;
        private const byte _Null = 0x80;
        public static DayofWeekArray AllDaysOfWeek;
        public static DayofWeekArray Empty;
        public static DayofWeekArray Null;

        static DayofWeekArray()
        {
            AllDaysOfWeek = new DayofWeekArray(_AllDaysofWeek);
            Empty = new DayofWeekArray();
            Null = new DayofWeekArray(_Null);
        }

        public DayofWeekArray(byte value)
        {
            _DaysofWeek = value;
        }

        private DayofWeekArray(SerializationInfo info, StreamingContext context)
            : this()
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            SerializationInfoEnumerator enumerator = info.GetEnumerator();
            bool flag = false;
            byte bits = 0;
            while (enumerator.MoveNext())
            {
                string name = enumerator.Name;
                if (name != null)
                {
                    if (name == "bits")
                    {
                        bits = Convert.ToByte(enumerator.Value, CultureInfo.InvariantCulture);
                        flag = true;
                    }
                }
            }
            if (!flag)
            {
                throw new SerializationException("Serialization Error: Missing DayOfWeekArray Data");
            }
            _DaysofWeek = bits;
        }

        public byte Value
        {
            get
            {
                return _DaysofWeek;
            }
        }

        public bool IsMultiDay
        {
            get
            {
                int n = 0;
                int bitCount = 0;
                while (n < 7)
                {
                    if ((_DaysofWeek & bitMasks[n]) != 0)
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

        public bool IsAllDaysOfWeek
        {
            get
            {
                return (_DaysofWeek & _AllDaysofWeek) == _AllDaysofWeek;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return _DaysofWeek == 0;
            }
        }

        public bool IsNull
        {
            get
            {
                return _DaysofWeek == _Null;
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

            string[] dayNames = DateTimeFormatInfo.GetInstance(provider).ShortestDayNames;
            int n = 0;
            int nLastHit = -1;
            int nConsecutive = 0;
            StringBuilder sb = new StringBuilder();
            while (n <= 7)
            {
                if ((n == 7) || (_DaysofWeek & bitMasks[n]) == 0)
                {
                    if (nConsecutive > 0)
                    {
                        //If nConsecutive = 1 Then
                        //    sb.Append(", ")
                        //Else
                        sb.Append("-");
                        //End If
                        sb.Append(dayNames[nLastHit]);
                        nConsecutive = 0;
                    }
                }
                else
                {
                    if (nLastHit == -1)
                    {
                        sb.Append(dayNames[n]);
                    }
                    else if (n - nLastHit == 1)
                    {
                        nConsecutive++;
                    }
                    else
                    {
                        sb.Append(", ");
                        sb.Append(dayNames[n]);
                        nConsecutive = 0;
                    }
                    nLastHit = n;
                }
                n++;
            }
            return sb.ToString();
        }

        public DayofWeekArray AddDay(int day)
        {
            if (day < 0 || day > 7)
            {
                throw new ArgumentOutOfRangeException("day", "Day number must be in range of 0 through 7");
            }
            else
            {
                return new DayofWeekArray((byte)(_DaysofWeek | bitMasks[day]));
            }
        }

        public DayofWeekArray RemoveDay(int day)
        {
            if (day < 0 || day > 7)
            {
                throw new ArgumentOutOfRangeException("day", "Day number must be in range of 0 through 7");
            }
            else
            {
                return new DayofWeekArray((byte)(_DaysofWeek & bitMasks[day]));
            }
        }

        public int CompareTo(DayofWeekArray other)
        {

            int tmp1 = 0;
            int tmp2 = 0;

            tmp1 = _DaysofWeek;
            tmp2 = other._DaysofWeek;
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
                } while (n < 7);
                return 0;
            }

        }

        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info != null)
            {
                info.AddValue("bits", _DaysofWeek);
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            _DaysofWeek = Convert.ToByte(reader.ReadContentAsInt());
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteString(_DaysofWeek.ToString());
        }
    }
}
