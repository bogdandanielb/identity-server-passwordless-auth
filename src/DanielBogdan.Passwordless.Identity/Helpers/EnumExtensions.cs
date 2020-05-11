using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DanielBogdan.Passwordless.Identity.Helpers
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Generic get attibute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Enum enumValue) where T : Attribute
        {
            T attribute;

            MemberInfo memberInfo = enumValue.GetType().GetMember(enumValue.ToString())
                                            .FirstOrDefault();

            if (memberInfo != null)
            {
                attribute = (T)memberInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
                return attribute;
            }
            return null;
        }

        /// <summary>
        /// Retrieve enum item DescriptionAttribute
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();

            return attribute == null ? string.Empty : attribute.Description;
        }

        /// <summary>
        /// Retrieve enum item DisplayNameAttribute
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDisplayName(this Enum value)
        {
            var attribute = value.GetAttribute<DisplayNameAttribute>();

            return attribute == null ? string.Empty : attribute.DisplayName;
        }

        /// <summary>
        /// Returns kvp with EnumValue.ToString() as key and DescriptionAttribute value as description
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> DescriptionList<T>()
        {
            var list = new List<KeyValuePair<string, string>>();

            foreach (Enum field in Enum.GetValues(typeof(T)))
            {
                list.Add(new KeyValuePair<string, string>(field.ToString(), field.GetDescription()));
            }

            return list;
        }

        /// <summary>
        ///  Returns kvp with EnumValue as key and EnumValue.ToString() value as description
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<int, string>> List<T>()
        {
            var list = new List<KeyValuePair<int, string>>();

            foreach (Enum field in Enum.GetValues(typeof(T)))
            {
                list.Add(new KeyValuePair<int, string>(Convert.ToInt32(field), field.ToString()));
            }

            return list;
        }
    }
}
