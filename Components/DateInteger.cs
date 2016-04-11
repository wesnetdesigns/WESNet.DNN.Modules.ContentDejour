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
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace WESNet.DNN.Modules.ContentDejour
{
    [Serializable()]
    public struct DateInteger : IComparable<DateInteger>
    {
        private Int32 _DateInteger;

        public const int YearShift = 9;
        public const int MonthShift = 5;
        public const int YearMask = 0x7FFE00;
        public const int MonthMask = 0x1E0;
        public const int DayMask = 0x1F;
        public const int MonthDayMask = MonthMask | DayMask;
        public const int YearDayMask = YearMask | DayMask;
        public const int YearMonthMask = YearMask | MonthMask;

        public static DateInteger Null;
        public static DateInteger MinValue;
        public static DateInteger MaxValue;

        static DateInteger()
        {
            Null = new DateInteger();
            MinValue = new DateInteger(1, 1, 1);
            MaxValue = new DateInteger(9999, 12, 31);
        }

        public DateInteger(Int32 value)
            : this()
        {
            this.Value = value;
        }

        public DateInteger(int Year, int Month, int Day)
            : this()
        {
            this.Year = Year;
            this.Month = Month;
            if (Month == 2)
            {
                Day = Constrain(Day, 1, (IsLeapYear ? 29 : 28));
            }
            this.Day = Day;
        }

        public DateInteger(DateTime value, bool IgnoreYear)
            : this()
        {

            if (value == DateTime.MinValue)
            {
                _DateInteger = MinValue.Value;
            }
            else if (value == DateTime.MaxValue)
            {
                _DateInteger = MaxValue.Value;
            }
            else if (IgnoreYear)
            {
                Year = 0;
                Month = value.Month;
                Day = value.Day;
            }
            else
            {
                Year = value.Year;
                Month = value.Month;
                Day = value.Day;
            }
        }

        public Int32 Value
        {
            get
            {
                return _DateInteger;
            }
            set
            {
                if (value != 0)
                {
                    value = Constrain(value, Null.Value, MaxValue.Value);
                }
                _DateInteger = value;
            }
        }

        [XmlIgnore()]
        public Int32 Year
        {
            get
            {
                return (_DateInteger & YearMask) >> YearShift;
            }
            set
            {
                value = Constrain(value, 0, 9999);
                _DateInteger = (_DateInteger & MonthDayMask) | (value << YearShift);
            }
        }

        [XmlIgnore()]
        public Int32 Month
        {
            get
            {
                return (_DateInteger & MonthMask) >> MonthShift;
            }
            set
            {
                value = Constrain(value, 1, 12);
                _DateInteger = (_DateInteger & YearDayMask) | (value << MonthShift);
            }
        }

        [XmlIgnore()]
        public Int32 Day
        {
            get
            {
                return _DateInteger & DayMask;
            }
            set
            {
                value = Constrain(value, 1, 31);
                _DateInteger = (_DateInteger & YearMonthMask) | value;
            }
        }

        [XmlIgnore()]
        public bool HasYear
        {
            get
            {
                return Year != 0;
            }
        }

        [XmlIgnore()]
        public bool IsLeapYear
        {
            get
            {
                int y = Year;
                return (((y % 4 == 0) & (y % 100 != 0)) | (y % 400 == 0));
            }
        }

        [XmlIgnore()]
        public bool IsNull
        {
            get
            {
                return _DateInteger == 0;
            }
        }

        public DateTime ToDateTime()
        {
            if (IsNull)
            {
                return DateTime.MinValue.Date;
            }
            else if (Year == 0)
            {
                return (new DateTime(DateTime.Today.Year, Month, Day)).Date;
            }
            else
            {
                return (new DateTime(Year, Month, Day)).Date;
            }
        }

        new public string ToString()
        {
            return ToDateTime().ToString();
        }

        public string ToString(string format)
        {
            return ToDateTime().ToString(format);
        }

        public string ToString(IFormatProvider provider)
        {
            return ToDateTime().ToString(provider);
        }

        public string ToString(string format, IFormatProvider provider)
        {
            return ToDateTime().ToString(format, provider);
        }

        public static DateInteger Parse(string s)
        {

            int v1 = 0;
            int v2 = 0;
            int v3 = 0;

            if (string.IsNullOrEmpty(s))
            {
                return new DateInteger();
            }
            else
            {
                Regex dateRgx = new Regex("^\\s*(((\\d{1,2})[/\\-.](\\d{1,2})(?:[/\\-.](\\d{2,4})?))|(((?:\\d{4})[/\\-.])?(\\d{1,2})[/\\-.](\\d{1,2}))|((\\d{4})(\\d{2})(\\d{2})))\\s*$");
                Match m = dateRgx.Match(s);
                if (m.Success)
                {
                    string ShortDatePattern = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
                    bool MonthBeforeDay = ShortDatePattern.IndexOf("M") < ShortDatePattern.IndexOf("d");
                    if (m.Groups[2].Success) //Month-Day-Year or Day-Month-Year
                    {
                        v1 = int.Parse(m.Groups[3].Value);
                        v2 = int.Parse(m.Groups[4].Value);
                        int.TryParse(m.Groups[5].Value, out v3); //In case no year then v3 will be returned as 0


                        if (MonthBeforeDay && v1 <= 12)
                        {
                            return new DateInteger(v3, v1, v2);
                        }
                        else
                        {
                            return new DateInteger(v3, v2, v1);
                        }

                    }
                    else if (m.Groups[6].Success) //Year-Month-Day
                    {
                        int.TryParse(m.Groups[7].Value, out v1); //In case no year then v1 will be returned as 0
                        v2 = int.Parse(m.Groups[8].Value);
                        v3 = int.Parse(m.Groups[9].Value);
                        if (MonthBeforeDay && v1 <= 12)
                        {
                            return new DateInteger(v1, v2, v3);
                        }
                        else
                        {
                            return new DateInteger(v1, v3, v2);
                        }
                    }
                    else if (m.Groups[10].Success) //########'
                    {
                        v1 = int.Parse(m.Groups[11].Value);
                        v2 = int.Parse(m.Groups[12].Value);
                        v3 = int.Parse(m.Groups[13].Value);
                        return new DateInteger(v1, v2, v3);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("s", "Unrecognzable or ambiguous date string format");
                    }
                }
                else
                {
                    throw new ArgumentException("s", "Invalid date string");
                }
            }
        }

        public static DateInteger Parse(string s, bool IgnoreYear)
        {
            DateInteger result = Parse(s);
            if (!result.IsNull && IgnoreYear)
            {
                return new DateInteger(result.Value & MonthDayMask);
            }
            else
            {
                return result;
            }
        }

        public int CompareTo(DateInteger other)
        {
            int tmp1 = 0;
            int tmp2 = 0;

            if (!(HasYear && other.HasYear))
            {
                tmp1 = _DateInteger & MonthDayMask;
                tmp2 = other.Value & MonthDayMask;
            }
            else
            {
                tmp1 = _DateInteger;
                tmp2 = other.Value;
            }
            if (tmp1 == tmp2)
            {
                return 0;
            }
            else if (IsNull)
            {
                return 1;
            }
            else
            {
                return Math.Sign(tmp1 - tmp2);
            }
        }

        public bool IsBetween(DateInteger value1, DateInteger value2)
        {
            int tmp0 = 0;
            int tmp1 = 0;
            int tmp2 = 0;

            if (!(HasYear && value1.HasYear && value2.HasYear))
            {
                tmp0 = _DateInteger & MonthDayMask;
                tmp1 = value1.Value & MonthDayMask;
                tmp2 = value2.Value & MonthDayMask;
            }
            else
            {
                tmp0 = _DateInteger;
                tmp1 = value1.Value;
                tmp2 = value2.Value;
            }

            if (tmp1 > tmp2)
            {
                return (tmp0 >= tmp1) & (tmp0 <= tmp2);
            }
            else
            {
                return (tmp0 >= tmp1) | (tmp0 <= tmp2);
            }
        }

        private Int32 Constrain(Int32 value, Int32 min, Int32 max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }
            return value;
        }

    }
}
