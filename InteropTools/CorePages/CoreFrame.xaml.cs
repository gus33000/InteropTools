// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using InteropTools.Providers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.CorePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    internal sealed partial class CoreFrame : Page
    {
        public CoreFrame()
        {
            InitializeComponent();
        }

        public delegate void CurrentContentChangedEvent(object sender);

        public event CurrentContentChangedEvent OnCurrentContentChanged;

        public UIElement FrameContent
        {
            get => FramePanel.Content as UIElement;

            set
            {
                UpdateCurrentContentChanged();
                FramePanel.Content = value;
            }
        }

        public IRegistryProvider provider { get; set; }

        private void UpdateCurrentContentChanged()
        {
            // Make sure someone is listening to event
            if (OnCurrentContentChanged == null)
            {
                return;
            }

            OnCurrentContentChanged(this);
        }
    }
}