using LazyApiPack.Collections;
using System;
using System.Linq;
using System.Windows.Media;

namespace LazyApiPack.Wpf.Theme
{
    public static class SkeumorphDarkTheme
    {
        /// <summary>
        /// Creates the Skeumorph Dark Night theme
        /// </summary>
        /// <param name="name">Overrides the default name of this theme.</param>
        /// <param name="description">Overrides the default description of this theme.</param>
        /// <param name="preview">Preview image of this theme in your application (eg. Screenshot).</param>
        /// <param name="tags">Additional tags, if the theme dictionary should be searchable.</param>
        /// <param name="additionalResources">If you want to customize the theme with your own resources or you want to support additional theme information for your custom controls.</param>
        /// <returns>The theme you can add to the ThemeManager</returns>
        public static Theme CreateSkeumorphNightTheme(string? name, string? description, ImageSource? preview, string[]? tags, params Uri[]? additionalResources)
        {

            tags = new[] { "skeumorph", "night", "dark", "classic", "2000" }.Append(tags);
            var resources = new[] { new Uri("pack://application:,,,/LazyApiPack.Wpf.Theme.SkeumorphDark;component/Theme/Night/SkeumorphDarkNightColors.xaml", UriKind.Absolute) };
            additionalResources = resources.Append(additionalResources);

            return new Theme(string.IsNullOrWhiteSpace(name) ? "Skeumorph Dark Night" : name,
                string.IsNullOrWhiteSpace(description) ? "Dark Night theme in skeumorph style." : description,
                preview,
                tags,
                new Uri("pack://application:,,,/LazyApiPack.Wpf.Theme.SkeumorphDark;component/Theme/SkeumorphDarkControlStyles.xaml", UriKind.Absolute),
                additionalResources);
        }
    }
}
