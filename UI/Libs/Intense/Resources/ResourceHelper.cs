using Windows.ApplicationModel.Resources;

namespace Intense.Resources
{
    internal static class ResourceHelper
    {
        private static ResourceLoader loader;

        private static ResourceLoader GetLoader()
        {
            return loader ??= ResourceLoader.GetForCurrentView("Intense/Resources");
        }

        /// <summary>
        /// Retrieves the specified resource, optionally formatting it using specified arguments.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetString(string name, params object[] args)
        {
            string value = GetLoader().GetString(name);
            if (args.Length > 0)
            {
                value = string.Format(value, args);
            }
            return value;
        }
    }
}
