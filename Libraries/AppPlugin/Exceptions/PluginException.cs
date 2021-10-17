using System;

namespace AppPlugin.Exceptions
{
    internal class PluginException : Exception
    {

        internal PluginException(string message) : base(message)
        {
        }

    }
}