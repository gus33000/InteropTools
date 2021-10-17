// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Intense.UI
{
    /// <summary>
    /// Provides various XAML helper functions.
    /// </summary>
    internal static class XamlHelper
    {
        /// <summary>
        /// Creates a <see cref="SolidColorBrush"/> instance from given color attribute value.
        /// </summary>
        /// <param name="colorAttr"></param>
        /// <returns></returns>
        public static SolidColorBrush CreateSolidColorBrush(string colorAttr) => (SolidColorBrush)XamlReader.Load(
            $"<SolidColorBrush Color=\"{colorAttr}\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"/>");
    }
}
