namespace InteropTools.Controls
{
    public class AnimatedFlipView : FlipView
    {
        private bool _isRunningAnimation;
        private bool _reverseAnimation;
        private ScrollViewer _scrollViewer;

        public AnimatedFlipView()
        {
            DefaultStyleKey = typeof(AnimatedFlipView);

            Loaded += OnLoaded;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _scrollViewer = GetTemplateChild("ScrollingHost") as ScrollViewer;
            if (_scrollViewer == null)
            {
                throw new NullReferenceException("ScrollingHost and Mediator must not be null.");
            }

            SelectionChanged += OnSelectionChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_scrollViewer != null)
            {
                DispatcherTimer timer = new()
                {
                    Interval = TimeSpan.FromSeconds(3)
                };
                timer.Tick += (o, o1) =>
                {
                    if (!_isRunningAnimation)
                    {
                        Storyboard sb = new();

                        DoubleAnimation nextItemAnimation = new()
                        {
                            EnableDependentAnimation = true,
                            Duration = new Duration(TimeSpan.FromSeconds(10)),
                            From = _scrollViewer.HorizontalOffset,
                            To = _reverseAnimation ? _scrollViewer.HorizontalOffset - 1 : _scrollViewer.HorizontalOffset + 1,
                            FillBehavior = FillBehavior.HoldEnd,
                            EasingFunction = new ExponentialEase
                            {
                                EasingMode = EasingMode.EaseOut
                            }
                        };

                        Storyboard.SetTarget(nextItemAnimation, _scrollViewer);
                        Storyboard.SetTargetProperty(nextItemAnimation, "HorizontalOffset");

                        sb.Children.Add(nextItemAnimation);

                        sb.Completed += (sender1, o2) =>
                        {
                            if (_reverseAnimation)
                            {
                                if (SelectedIndex > 0)
                                {
                                    SelectedIndex--;
                                }

                                if (SelectedIndex == 0)
                                {
                                    _reverseAnimation = false;
                                }
                            }
                            else
                            {
                                if (Items != null && SelectedIndex < Items.Count - 1)
                                {
                                    SelectedIndex++;
                                }

                                if (Items != null && SelectedIndex == Items.Count - 1)
                                {
                                    _reverseAnimation = true;
                                }
                            }

                            _isRunningAnimation = false;
                        };

                        sb.Begin();

                        _isRunningAnimation = true;
                    }
                };
                timer.Start();
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            _scrollViewer.ScrollToHorizontalOffset(_reverseAnimation
                                                                    ? (_scrollViewer.HorizontalOffset - 1)
                                                                    : (_scrollViewer.HorizontalOffset + 1));
        }
    }
}