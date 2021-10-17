// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InteropTools.Controls
{
    /// <summary>
    /// FlipView Extensions
    /// </summary>
    public class FlipViewExtensions
    {
        #region Classes

        /// <summary>
        /// FlipView list
        /// </summary>
        protected class FlipViewList : List<object>
        {
            /// <summary>
            /// Initializes a new instance of the FlipViewList class.
            /// </summary>
            /// <param name="collection">the initial collection</param>
            public FlipViewList(IEnumerable<object> collection) :
                base(collection)
            {
            }
        }

        #endregion Classes

        #region Fields

        /// <summary>
        /// IsLooping attached property for FlipView
        /// </summary>
        public static readonly DependencyProperty IsLoopingProperty =
            DependencyProperty.RegisterAttached(
                "IsLooping",
                typeof(bool),
                typeof(FlipViewExtensions),
                new PropertyMetadata(false, new PropertyChangedCallback(OnIsLoopingChanged)));

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets a value indicating whether this is a looping FlipView
        /// </summary>
        /// <param name="obj">the flipView</param>
        /// <returns>true if the list loops</returns>
        public static bool GetIsLooping(FlipView obj) => (bool)obj.GetValue(IsLoopingProperty);

        /// <summary>
        /// Sets a value indicating whether the FlipView loops
        /// </summary>
        /// <param name="obj">the FlipView</param>
        /// <param name="value">true if the list loops</param>
        public static void SetIsLooping(FlipView obj, bool value) => obj.SetValue(IsLoopingProperty, value);

        #endregion Methods

        #region Implementation

        /// <summary>
        /// swaps the selected index if the beginning or end is reached
        /// </summary>
        /// <param name="sender">the FlipView</param>
        /// <param name="e">the selection changed event arguments</param>
        private static void flipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FlipView flipView = sender as FlipView;

            int count;
            if (flipView.ItemsSource is not FlipViewList list)
            {
                count = Initialize(flipView);
            }
            else
            {
                count = list.Count;
            }

            if (count < 3)
            {
                return;
            }

            if (flipView.SelectedIndex == 0)
            {
                flipView.SelectedIndex = count - 2;
            }
            else if (flipView.SelectedIndex == count - 1)
            {
                flipView.SelectedIndex = 1;
            }
            else if (flipView.SelectedIndex == -1)
            {
                flipView.SelectedIndex = 1;
            }
        }

        /// <summary>
        /// Replace the FlipView.ItemsSource with a FlipView list that has
        /// duplicate items at the head and tail
        /// </summary>
        /// <param name="flipView">the FlipView</param>
        /// <returns>the number of items in the FlipView.ItemsSource</returns>
        private static int Initialize(FlipView flipView)
        {
            flipView.SelectionChanged -= flipView_SelectionChanged;

            if (flipView.ItemsSource is INotifyCollectionChanged collectionChanged)
            {
                collectionChanged.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) =>
                    UpdateList(flipView, sender as IEnumerable);
            }

            if (flipView.ItemsSource is IEnumerable enumerable)
            {
                IEnumerable<object> enumerableObjects = enumerable.OfType<object>();

                FlipViewList loopingList = new(enumerableObjects);

                loopingList.Insert(0, enumerableObjects.Last());

                loopingList.Add(enumerableObjects.First());

                flipView.ItemsSource = loopingList;

                flipView.SelectedItem = loopingList[1];

                flipView.SelectionChanged += flipView_SelectionChanged;

                return loopingList.Count;
            }

            return 0;
        }

        /// <summary>
        /// Initialize the selection changed handler and the list if the ItemsSource is set
        /// </summary>
        /// <param name="dependencyObject">the FlipView</param>
        /// <param name="args">the dependency property changed event arguments</param>
        private static void OnIsLoopingChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs args)
        {
            FlipView flipView = dependencyObject as FlipView;

            if ((bool)args.NewValue)
            {
                if (flipView.ItemsSource is IEnumerable enumerable)
                {
                    Initialize(flipView);
                }
                else
                {
                    flipView.SelectionChanged += flipView_SelectionChanged;
                }
            }
            else
            {
                flipView.SelectionChanged -= flipView_SelectionChanged;

                if (flipView.ItemsSource is FlipViewList flipViewList)
                {
                    object selectedItem = flipView.SelectedItem;

                    flipViewList.RemoveAt(0);
                    flipViewList.Remove(flipViewList.Last());

                    flipView.ItemsSource = flipViewList.ToArray();

                    flipView.SelectedItem = selectedItem;
                }
            }
        }

        /// <summary>
        /// Update the list
        /// </summary>
        /// <param name="flipView">the FlipView</param>
        /// <param name="enumerable">the enumerable collection</param>
        private static void UpdateList(FlipView flipView, IEnumerable enumerable)
        {
            object selection = flipView.SelectedItem;

            IEnumerable<object> enumerableObjects = enumerable.OfType<object>();

            FlipViewList flipViewList = flipView.ItemsSource as FlipViewList;

            flipViewList.Clear();
            flipViewList.Add(enumerableObjects.Last());
            flipViewList.AddRange(enumerableObjects);
            flipViewList.Add(enumerableObjects.First());

            flipView.SelectedItem = selection;
        }

        #endregion Implementation
    }
}
