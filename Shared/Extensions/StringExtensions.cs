using System;
using System.Globalization;
using System.Linq;

namespace Shared.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts the first character to lowercase
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns>string</returns>
        public static string ToCamelCase(string phrase)
        {
            var characters = phrase.ToCharArray();
            var firstCharacter = characters[0].ToString(CultureInfo.InvariantCulture).ToLower();
            var otherCharacters = new string(characters).Remove(0, 1);
            return firstCharacter + otherCharacters;
        }

        /// <summary>
        /// Converts the first character to uppercase
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns>string</returns>
        public static string ToPascalCase(string phrase)
        {
            var characters = phrase.ToCharArray();
            var firstCharacter = characters[0].ToString(CultureInfo.InvariantCulture).ToUpper();
            var otherCharacters = new string(characters).Remove(0, 1);
            return firstCharacter + otherCharacters;
        }

        public static string GetFriendlyName(Type type)
        {
            if (type == typeof(int))
                return "int";
            
            if (type == typeof(short))
                return "short";
            
            if (type == typeof(byte))
                return "byte";
            
            if (type == typeof(bool))
                return "bool";
            
            if (type == typeof(long))
                return "long";
            
            if (type == typeof(float))
                return "float";
            
            if (type == typeof(double))
                return "double";
            
            if (type == typeof(decimal))
                return "decimal";
            
            if (type == typeof(string))
                return "string";

            if (type.IsGenericType)
            {
                string genericType = type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(GetFriendlyName)) + ">";

                if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    genericType = GetFriendlyName(type.GetGenericArguments()[0]) + "?";
                }

                return genericType;
            }

            return type.Name;
        }
    }
}
