using LazyApiPack.Theme;
using LazyApiPack.Utils.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace LazyApiPack.Wpf.Theme
{
    /// <summary>
    /// Manages the themes in a WPF application
    /// </summary>
    public class ThemeService : NotifyObject, IThemeService, IDisposable
    {
        [DisallowNull] private Application _app;


        /// <summary>
        /// Creates an instance of the ThemeManager. Use Start() to create an instance.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <param name="themes">A list of available themes.</param>
        public ThemeService([DisallowNull] Application app, IEnumerable<Theme> themes)
        {
            _app = app ?? throw new ArgumentNullException(nameof(app));
            AvailableThemes = new ObservableCollection<ITheme>(themes ?? new Theme[0]);
        }

        private ObservableCollection<ITheme> _availableThemes;
        /// <inheritdoc/>
        public ObservableCollection<ITheme> AvailableThemes
        {
            get => _availableThemes;
            private set
            {
                if (AvailableThemes != null)
                {
                    AvailableThemes.CollectionChanged -= AvailableThemes_CollectionChanged;
                }
                SetPropertyValue(ref _availableThemes, value);
                if (AvailableThemes != null)
                {
                    AvailableThemes.CollectionChanged += AvailableThemes_CollectionChanged;
                }
            }
        }

        /// <summary>
        /// Checks that no item gets removed if it is in use and not item gets added that has the same source.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="InvalidOperationException">Raised when an item is removed that is still in use.</exception>
        /// <exception cref="ArgumentException">Raised when an item is added that has the same source as an item already in the list.</exception>
        private void AvailableThemes_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            var currentTheme = CurrentTheme as Theme;
            if (currentTheme != null && e.OldItems != null)
            {
                var current = e.OldItems.OfType<Theme>().FirstOrDefault(t => t.Source == currentTheme.Source);
                if (current != null)
                {
                    throw new InvalidOperationException(
                        $"Can not remove theme {CurrentTheme.Name} because it is the active theme. Please change the theme first before removing it.");
                }
            }

            if (e.NewItems != null)
            {
                var grouped = AvailableThemes.OfType<Theme>().GroupBy(t => t.Source).Where(g => g.Count() > 1).Select(g => g.First().Source);
                if (grouped.Any())
                {
                    throw new ArgumentException(
@$"One or more items with the same key have already been added.
Duplicate item(s):
{string.Join(",", grouped)}.");

                }
            }
        }


        private ITheme? _currentTheme;
        /// <inheritdoc/>
        public ITheme? CurrentTheme
        {
            get => _currentTheme;
            set
            {
                if (value == _currentTheme) return;
                var oldTheme = _currentTheme;

                // Remove current theme if set.
                if (_currentTheme != null)
                {
                    var current = _app?.Resources?.MergedDictionaries.FirstOrDefault(r => r.Source == ((Theme)_currentTheme).Source);
                    if (current != null)
                    {
                        Application.Current.Resources.MergedDictionaries.Remove(current);
                    }
                }

                if (value != null)
                {
                    if (!AvailableThemes.Contains(value))
                    {
                        AvailableThemes.Add(value);
                    }

                    _app.Resources.MergedDictionaries.Add(((Theme)value).CreateResource());
                    SetPropertyValue(ref _currentTheme, value);
                }

            }
        }

        /// <inheritdoc/>
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ThemeService()
        {
            Dispose(false);
        }
        private void Dispose(bool isDisposing)
        {
            if (IsDisposed) return;
            if (isDisposing)
            {
                CurrentTheme = null;
            }
            AvailableThemes.Clear();
            IsDisposed = true;


        }
    }
}
