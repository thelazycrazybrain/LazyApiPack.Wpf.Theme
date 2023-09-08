using System.Collections.ObjectModel;

namespace LazyApiPack.Theme
{
    public interface ITheme
    {
        /// <summary>
        /// Name of the theme.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Description of the theme.
        /// </summary>
        string? Description { get; }
        /// <summary>
        /// Tags for the theme.
        /// </summary>
        string[] Tags { get; }

        /// <summary>
        /// Location of the theme resource dictionary.
        /// </summary>
        Uri Source { get; }

        /// <summary>
        /// Locations of the additional resource dictionaries for this theme.
        /// </summary>
        ReadOnlyCollection<Uri> AdditionalResources { get; }

    }
}