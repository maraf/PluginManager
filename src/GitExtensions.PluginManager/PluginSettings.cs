using GitUIPluginInterfaces;
using Neptuo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.PluginManager
{
    internal class PluginSettings : IEnumerable<ISetting>
    {
        /// <summary>
        /// Gets a property holding path to backup and restore bundle file.
        /// </summary>
        public static StringSetting PackageSourceUrlProperty { get; } = new StringSetting("Package Source Url", "Package Source Url", null);

        /// <summary>
        /// Gets a property holding if asking to close git extensions is required.
        /// </summary>
        public static BoolSetting AskToCloseInstancesProperty { get; } = new BoolSetting("AskToCloseInstances", "Ask to close all instances of GitExtensions before starting PluginManager", true);

        private readonly ISettingsSource source;

        /// <summary>
        /// Gets current value of <see cref="PackageSourceUrlProperty"/>.
        /// </summary>
        public string PackageSourceUrl => source.GetValue(PackageSourceUrlProperty.Name, PackageSourceUrlProperty.DefaultValue, t => t);
        public bool AskToCloseInstances => source.GetValue(AskToCloseInstancesProperty.Name, AskToCloseInstancesProperty.DefaultValue, t => Boolean.Parse(t));

        public PluginSettings(ISettingsSource source)
        {
            Ensure.NotNull(source, "source");
            this.source = source;
        }

        #region IEnumerable<ISetting>

        private static readonly List<ISetting> properties;

        static PluginSettings()
        {
            properties = new List<ISetting>(2)
            {
                PackageSourceUrlProperty,
                AskToCloseInstancesProperty
            };
        }

        public IEnumerator<ISetting> GetEnumerator()
            => properties.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        #endregion
    }
}
