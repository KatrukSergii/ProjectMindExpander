using System;
using System.Reflection;
using Autofac;

namespace Infrastructure
{
    /// <summary>
    /// Autofac container
    /// </summary>
    public class Container
    {
        /// <summary>
        /// singleton class for container
        /// </summary>
        private static IContainer _container;

        /// <summary>
        /// Gets the singleton instance
        /// </summary>
        public static IContainer Instance
        {
            get
            {
                if (_container == null)
                {
                    try
                    {
                        var builder = new ContainerBuilder();

                        var communication = Assembly.Load("Communication");
                        //var shared = Assembly.Load("Shared");

                        builder.RegisterAssemblyModules(communication);

                        // Store the AutoFac container on a static variable to be used inside legacy code
                        _container = builder.Build();
                    }
                    catch (Exception ex)
                    {
                        string errorString = "Exception Information\n============\n" + GenerateErrorString(ex);
                        throw new Exception(errorString, ex);
                    }
                }

                return _container;
            }
        }

        /// <summary>
        /// Generate an Exception Message to see why the Modules couldn't be loaded
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>Detailed Exception Message of the issue.</returns>
        private static string GenerateErrorString(Exception ex)
        {
            if (ex.InnerException != null)
            {
                return GenerateErrorString(ex.InnerException) + "\n============\n";
            }

            var errorString = "Exceptions of type - " + ex.GetType() + "\n";
            errorString += "Message: " + ex.Message + "\n";
            errorString += "StackTrace: " + ex.StackTrace + "\n";

            var reflectionError = ex as ReflectionTypeLoadException;
            if (reflectionError != null)
            {
                foreach (var loaderExcption in reflectionError.LoaderExceptions)
                {
                    errorString += loaderExcption.Message + "\n";
                    errorString += loaderExcption.StackTrace + "\n";
                }
            }
            return errorString + "============\n";
        }
    }
}
