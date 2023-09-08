using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xaml;

namespace LazyApiPack.XamlPreview
{
    internal class ResourceManager
    {
        private ResourceDictionary _resources;
        public ResourceManager(string dictionaryFileName)
        {
            return;
            _resources = (ResourceDictionary)XamlServices.Load(dictionaryFileName);
            _resourceKeys = _resources.Keys.OfType<string>().OrderBy(k => k);

        }
        private IEnumerable<string> _resourceKeys;
        public IEnumerable<string> ResourceKeys { get => _resourceKeys; }

        public object GetResource(string key)
        {
            return _resources[key];
        }

    }
}
