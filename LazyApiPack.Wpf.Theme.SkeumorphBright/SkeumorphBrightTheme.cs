using LazyApiPack.Collections;
using System;
using System.Linq;
using System.Windows.Media;

namespace LazyApiPack.Wpf.Theme
{
    public static class SkeumorphBrightTheme
    {
        /// <summary>
        /// Creates the Skeumorph Bright Blue theme
        /// </summary>
        /// <param name="name">Overrides the default name of this theme.</param>
        /// <param name="description">Overrides the default description of this theme.</param>
        /// <param name="preview">Preview image of this theme in your application (eg. Screenshot).</param>
        /// <param name="tags">Additional tags, if the theme dictionary should be searchable.</param>
        /// <param name="additionalResources">If you want to customize the theme with your own resources or you want to support additional theme information for your custom controls.</param>
        /// <returns>The theme you can add to the ThemeManager</returns>
        public static Theme CreateSkeumorphBrightBlueTheme(string? name, string? description, ImageSource? preview, string[]? tags, params Uri[]? additionalResources)
        {
          
            tags = new[] { "skeumorph", "blue", "bright", "classic", "2000" }.Append(tags);
            var resources = new[] { new Uri("pack://application:,,,/LazyApiPack.Wpf.Theme.SkeumorphBright;component/Theme/Blue/SkeumorphBrightBlueColors.xaml", UriKind.Absolute) };
            additionalResources = resources.Append(additionalResources);

            return new Theme(string.IsNullOrWhiteSpace(name) ? "Skeumorph Bright Blue" : name,
                string.IsNullOrWhiteSpace(description) ? "Bright Blue theme in skeumorph style." : description,
                preview,
                tags,
                new Uri("pack://application:,,,/LazyApiPack.Wpf.Theme.SkeumorphBright;component/Theme/SkeumorphBrightControlStyles.xaml", UriKind.Absolute),
                additionalResources);
        }

        /// <summary>
        /// Creates the Skeumorph Bright Green theme
        /// </summary>
        /// <param name="name">Overrides the default name of this theme.</param>
        /// <param name="description">Overrides the default description of this theme.</param>
        /// <param name="preview">Preview image of this theme in your application (eg. Screenshot).</param>
        /// <param name="tags">Additional tags, if the theme dictionary should be searchable.</param>
        /// <param name="additionalResources">If you want to customize the theme with your own resources or you want to support additional theme information for your custom controls.</param>
        /// <returns>The theme you can add to the ThemeManager</returns>
        public static Theme CreateSkeumorphBrightGreenTheme(string? name, string? description, ImageSource? preview, string[]? tags, params Uri[]? additionalResources)
        {

            tags = new[] { "skeumorph", "green", "bright", "classic", "2000" }.Append(tags);
            var resources = new[] { new Uri("pack://application:,,,/LazyApiPack.Wpf.Theme.SkeumorphBright;component/Theme/Green/SkeumorphBrightGreenColors.xaml", UriKind.Absolute) };
            additionalResources = resources.Append(additionalResources);

            return new Theme(string.IsNullOrWhiteSpace(name) ? "Skeumorph Bright Green" : name,
                string.IsNullOrWhiteSpace(description) ? "Bright Green theme in skeumorph style." : description,
                preview,
                tags,
                new Uri("pack://application:,,,/LazyApiPack.Wpf.Theme.SkeumorphBright;component/Theme/SkeumorphBrightControlStyles.xaml", UriKind.Absolute),
                additionalResources);
        }
    }
}
