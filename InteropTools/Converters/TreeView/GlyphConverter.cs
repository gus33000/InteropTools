// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using Windows.UI.Xaml.Data;

namespace InteropTools.Converters.TreeView2
{
    public sealed class GlyphConverter : IValueConverter
    {
        public string CollapsedGlyph { get; set; }
        public string ExpandedGlyph { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool? isExpanded = value as bool?;

            if (isExpanded == true)
            {
                return ExpandedGlyph;
            }
            else
            {
                return CollapsedGlyph;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            throw new NotImplementedException();
    }
}
