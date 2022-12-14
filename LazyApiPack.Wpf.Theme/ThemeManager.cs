using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LazyApiPack.Wpf.Theme
{
    /// <summary>
    /// Used to notify when a theme was changed.
    /// </summary>
    /// <param name="source">Theme manager intance that changed the theme.</param>
    /// <param name="oldTheme">The theme that was replaced.</param>
    /// <param name="newTheme">The current theme.</param>
    public delegate void ThemeChangedDelegate(ThemeManager source, Theme? oldTheme, Theme? newTheme);
    /// <summary>
    /// Metadata and methods to create a resource dictionary for a theme.
    /// </summary>
    public class Theme
    {
        /// <summary>
        /// Creates an instance of a theme.
        /// </summary>
        /// <param name="name">Name of the theme.</param>
        /// <param name="description">Description of the theme.</param>
        /// <param name="preview">Preview image of the theme (eg. Screenshot).</param>
        /// <param name="tags">Tags, if the theme dictionary should be searchable.</param>
        /// <param name="source">Uri source of the theme dictionary.</param>
        /// <param name="additionalResources">Additional resource dictionaries, that will be merged into the current theme.</param>
        public Theme([NotNull] string name, string? description,
                     ImageSource? preview, string[]? tags,
                     [NotNull] Uri source, params Uri[]? additionalResources)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name), $"Parameter {nameof(name)} must not be null or empty.");
            }
            Name = name;
            if (source == null || string.IsNullOrEmpty(source.ToString()))
            {
                throw new ArgumentNullException(nameof(source), $"Parameter {nameof(source)} and its value must not be empty.");
            }
            Source = source;
            if (additionalResources != null)
            {
                var invalidResources = additionalResources.Where(r => r == null || string.IsNullOrWhiteSpace(r.ToString()));
                if (invalidResources.Any())
                {
                    var indices = string.Join(", ", invalidResources.Select(i => Array.IndexOf(additionalResources, i)));

                    throw new ArgumentNullException(nameof(additionalResources), $"The additional resources at indices {indices} must not be null or emty.");
                }
                AdditionalResources = new ReadOnlyCollection<Uri>(additionalResources);
            }
            else
            {
                AdditionalResources = new ReadOnlyCollection<Uri>(Array.Empty<Uri>());
            }

            Description = description;
            Preview = preview;
            Tags = tags ?? Array.Empty<string>();

        }
        /// <summary>
        /// Name of the theme.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Description of the theme.
        /// </summary>
        public string? Description { get; }
        /// <summary>
        /// Preview image of the theme.
        /// </summary>
        public ImageSource? Preview { get; }

        /// <summary>
        /// Tags for the theme.
        /// </summary>
        public string[] Tags { get; }

        /// <summary>
        /// Location of the theme resource dictionary.
        /// </summary>
        public Uri Source { get; }

        /// <summary>
        /// Locations of the additional resource dictionaries for this theme.
        /// </summary>
        public ReadOnlyCollection<Uri> AdditionalResources { get; }

        /// <summary>
        /// Creates a resource dictionary for this theme consisting of the Source and AdditionalResources.
        /// </summary>
        /// <returns></returns>
        public ResourceDictionary CreateResource()
        {
            var dictionary = new ResourceDictionary() { Source = this.Source };

            foreach (var uri in AdditionalResources)
            {
                dictionary.MergedDictionaries.Add(new ResourceDictionary() { Source = uri });

            }

            return dictionary;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// Manages the themes in a WPF application
    /// </summary>
    public sealed class ThemeManager : IDisposable, INotifyPropertyChanged
    {
        private Application _app;
        /// <summary>
        /// Raised, when the theme was changed.
        /// </summary>
        public event ThemeChangedDelegate ThemeChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Creates a singleton instance of the ThemeManager or restarts it.
        /// </summary>
        /// <param name="themes">A list of themes that are currently supported.</param>
        public static void Start([NotNull] Application app, params Theme[] themes)
        {
            if (Instance != null)
            {
                Stop();
            }

            Instance = new ThemeManager(app, themes);

        }

        /// <summary>
        /// Disposes the current singleton of the theme manager.
        /// </summary>
        public static void Stop()
        {
            if (Instance != null)
            {
                Instance.Dispose();
                Instance = null;
            }
        }

        /// <summary>
        /// The current instance of the ThemeManager singleton.
        /// </summary>
        public static ThemeManager Instance { get; private set; }

        /// <summary>
        /// Creates an instance of the ThemeManager. Use Start() to create an instance.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <param name="themes">A list of available themes.</param>
        private ThemeManager([NotNull] Application app, Theme[] themes)
        {
            _app = app ?? throw new ArgumentNullException(nameof(app));
            AvailableThemes = new ObservableCollection<Theme>(themes ?? new Theme[0]);
        }

        ObservableCollection<Theme> _availableThemes;
        /// <summary>
        /// Currently available themes.
        /// </summary>
        public ObservableCollection<Theme> AvailableThemes
        {
            get => _availableThemes;
            private set
            {
                if (_availableThemes != null)
                {
                    _availableThemes.CollectionChanged -= AvailableThemes_CollectionChanged;
                }
                SetPropertyValue(ref _availableThemes, value);
                if (_availableThemes != null)
                {
                    _availableThemes.CollectionChanged += AvailableThemes_CollectionChanged;
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
            if (CurrentTheme != null && e.OldItems != null)
            {
                var current = e.OldItems.OfType<Theme>().FirstOrDefault(t => t.Source == CurrentTheme.Source);
                if (current != null)
                {
                    throw new InvalidOperationException(
                        $"Can not remove theme {CurrentTheme.Name} because it is the active theme. Please change the theme first before removing it.");
                }


            }

            if (e.NewItems != null)
            {
                var grouped = AvailableThemes.GroupBy(t => t.Source).Where(g => g.Count() > 1).Select(g => g.First().Source);
                if (grouped.Any())
                {
                    throw new ArgumentException(
@$"One or more items with the same key have already been added.
Duplicate item(s):
{string.Join(",", grouped)}.");

                }
            }
        }


        private Theme _currentTheme;
        /// <summary>
        /// The currently active theme.
        /// </summary>
        public Theme CurrentTheme
        {
            get => _currentTheme;
            set
            {
                if (value == _currentTheme) return;
                var oldTheme = _currentTheme;
                if (value== null)
                {
                    UnsetTheme(_currentTheme);
                }
                else
                {
                    if (!AvailableThemes.Contains(value))
                    {
                        AvailableThemes.Add(value);
                    }
                    UnsetTheme(_currentTheme);

                    _app.Resources.MergedDictionaries.Add(value.CreateResource());
                    SetPropertyValue(ref _currentTheme, value);
                }
                OnThemeChanged(oldTheme, value);

            }
        }

        /// <summary>
        /// Sets the currently active theme.
        /// </summary>
        /// <param name="themeName">The name of the theme (must be known in AvailableThemes).</param>
        /// <returns>True if the theme was successfully set. Otherwise false.</returns>
        public bool SetTheme(string themeName)
        {
            var theme = AvailableThemes.FirstOrDefault(t => t.Name == themeName);
            if (theme == null)
            {
                return false;
            }
            else
            {
                CurrentTheme = theme;
                return true;
            }
        }


        /// <summary>
        /// Removes the current theme from the applications resource dictionary without setting a new one. Does not change the theme changed event.
        /// </summary>
        /// <param name="theme">The theme that will be removed.</param>
        private void UnsetTheme(Theme theme)
        {
            if (theme != null)
            {
                var current = _app?.Resources?.MergedDictionaries.FirstOrDefault(r => r.Source == theme.Source);
                if (current != null)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(current);

                }
            }
        }

        /// <summary>
        /// Is raised when the current theme was changed
        /// </summary>
        /// <param name="oldTheme"></param>
        /// <param name="newTheme"></param>
        private void OnThemeChanged(Theme? oldTheme, Theme? newTheme)
        {
            ThemeChanged?.Invoke(this, oldTheme, newTheme);
        }

        private void SetPropertyValue<TValue>(ref TValue backingField, TValue newValue, [CallerMemberName] string? propertyName = null)
        {
            backingField = newValue;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ThemeManager()
        {
            Dispose(false);
        }
        private void Dispose(bool isDisposing)
        {
            if (IsDisposed) return;
            if (isDisposing)
            {
                UnsetTheme(CurrentTheme);
                _currentTheme = null;
            }
            AvailableThemes = null;
            _app = null;
            IsDisposed = true;


        }
    }
}
