using LazyApiPack.Theme;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace LazyApiPack.Wpf.Theme
{
    /// <summary>
    /// Metadata and methods to create a resource dictionary for a theme.
    /// </summary>
    public class Theme : ITheme
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
        public Theme([DisallowNull] string name, string? description,
                     ImageSource? preview, string[]? tags,
                     [DisallowNull] Uri source, params Uri[]? additionalResources)
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
        /// <inheritdoc />
        public string Name { get; }
        /// <inheritdoc />
        public string? Description { get; }
        /// <summary>
        /// Preview image of the theme.
        /// </summary>
        public ImageSource? Preview { get; }
        /// <inheritdoc />
        public string[] Tags { get; }
        /// <inheritdoc />
        public Uri Source { get; }
        /// <inheritdoc />
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
}
