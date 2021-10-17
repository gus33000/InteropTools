// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Intense.Presentation
{
    /// <summary>
    /// The base <see cref="INotifyPropertyChanged"/> implementation for use in binding scenarios.
    /// </summary>
    public abstract class NotifyPropertyChanged
        : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Updates specified value, and raises the <see cref="PropertyChanged"/> event when the value has changed.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="storage">The current stored value</param>
        /// <param name="value">The new value</param>
        /// <param name="propertyName">The optional property name, automatically set to caller member name when not set.</param>
        /// <returns>Indicates whether the value has changed.</returns>
        protected bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(storage, value))
            {
                storage = value;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }
    }
}