using System;

namespace Shared
{
    public class Ensure
    {
        public static void ArgumentNotNull(object argument, string name)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(name, "Cannot be null");
            }
        }

        public static void ArgumentNotNullOrEmpty(string argument, string name)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentException("Cannot be null or empty", name);
            }
        }

        public static void ArgumentTypeValid(object argument, string name, Type type)
        {
            ArgumentNotNull(argument, name);

            if (argument.GetType() != type)
            {
                throw new ArgumentException(
                    string.Format(
                        "Invalid argument type. Expecting an argument of type '{0}' but received '{1}'.",
                        type.Name,
                        argument.GetType().Name),
                    name);
            }
        }
    }
}