using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GhostYak
{
    public class Version
    {
        private Version()
        {

        }

        #region 어셈블리 특성 접근자

        public static string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public static string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public static string AssemblyVersionBig2
        {
            get
            {
                string fullVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                Match m = Regex.Match(fullVersion, @"^[0-9]+\.[0-9]+");
                return m.Value;
            }
        }

        public static string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public static string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        public static string AssemblyBuildDate
        {
            get
            {
                var dt = GetBuildDateTime();
                string month = (dt.Month < 10) ? $"0{dt.Month}" : $"{dt.Month}";
                string day = (dt.Day < 10) ? $"0{dt.Day}" : $"{dt.Day}";
                return $"{dt.Year}-{month}-{day}";
            }
        }
        #endregion

        private static System.DateTime GetBuildDateTime(System.Version version = null)
        {
            // 주.부.빌드.수정
            // 주 버전    Major Number
            // 부 버전    Minor Number
            // 빌드 번호  Build Number
            // 수정 버전  Revision NUmber

            //매개 변수가 존재할 경우
            if (version == null)
                version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            //세번째 값(Build Number)은 2000년 1월 1일부터
            //Build된 날짜까지의 총 일(Days) 수 이다.
            int day = version.Build;
            System.DateTime dtBuild = (new System.DateTime(2000, 1, 1)).AddDays(day);

            //네번째 값(Revision NUmber)은 자정으로부터 Build된
            //시간까지의 지나간 초(Second) 값 이다.
            int intSeconds = version.Revision;
            intSeconds = intSeconds * 2;
            dtBuild = dtBuild.AddSeconds(intSeconds);


            //시차 보정
            System.Globalization.DaylightTime daylingTime = System.TimeZone.CurrentTimeZone
                    .GetDaylightChanges(dtBuild.Year);
            if (System.TimeZone.IsDaylightSavingTime(dtBuild, daylingTime))
                dtBuild = dtBuild.Add(daylingTime.Delta);

            return dtBuild;
        }
    }
}
