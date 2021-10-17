// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using Windows.ApplicationModel.Resources;

namespace Intense.Resources
{
    internal static class ResourceHelper
    {
        private static ResourceLoader loader;

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

        private static ResourceLoader GetLoader() => loader ??= ResourceLoader.GetForCurrentView("Intense/Resources");
    }
}
