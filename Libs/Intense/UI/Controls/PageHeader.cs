using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Intense.UI.Controls
{
    /// <summary>
    /// The page header control with support for search.
    /// </summary>
    public class PageHeader
        : Control
    {
        /// <summary>
        /// Identifies the Icon dependency property.
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(PageHeader), null);
        /// <summary>
        /// Identifies the IconButtonStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty IconButtonStyleProperty = DependencyProperty.Register("IconButtonStyle", typeof(Style), typeof(PageHeader), null);
        /// <summary>
        /// Identifies the IconCommand dependency property.
        /// </summary>
        public static readonly DependencyProperty IconCommandProperty = DependencyProperty.Register("IconCommand", typeof(ICommand), typeof(PageHeader), null);
        /// <summary>
        /// Identifies the IsSearchBoxVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSearchBoxVisibleProperty = DependencyProperty.Register("IsSearchBoxVisible", typeof(bool), typeof(PageHeader), new PropertyMetadata(false));
        /// <summary>
        /// Identifies the SearchTerm dependency property.
        /// </summary>
        public static readonly DependencyProperty SearchTermProperty = DependencyProperty.Register("SearchTerm", typeof(string), typeof(PageHeader), null);
        /// <summary>
        /// Identifies the Title dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(PageHeader), null);

        /// <summary>
        /// Initializes a new instance of the <see cref="PageHeader"/> control.
        /// </summary>
        public PageHeader()
        {
            this.DefaultStyleKey = typeof(PageHeader);
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Gets or sets the icon button style.
        /// </summary>
        public Style IconButtonStyle
        {
            get { return (Style)GetValue(IconButtonStyleProperty); }
            set { SetValue(IconButtonStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the icon command.
        /// </summary>
        public ICommand IconCommand
        {
            get { return (ICommand)GetValue(IconCommandProperty); }
            set { SetValue(IconCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the search box is visible.
        /// </summary>
        public bool IsSearchBoxVisible
        {
            get { return (bool)GetValue(IsSearchBoxVisibleProperty); }
            set { SetValue(IsSearchBoxVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        public string SearchTerm
        {
            get { return (string)GetValue(SearchTermProperty); }
            set { SetValue(SearchTermProperty, value); }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
    }
}
