using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LazyApiPack.Theme
{

    public interface IThemeService : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Currently available themes.
        /// </summary>
        ObservableCollection<ITheme> AvailableThemes { get; }
        /// <summary>
        /// The currently active theme.
        /// </summary>
        ITheme? CurrentTheme { get; set; }
        /// <summary>
        /// Indicates if the object is aready disposed.
        /// </summary>
        bool IsDisposed { get; }
    }
}