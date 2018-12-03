using GitUIPluginInterfaces;
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
        /// Gets a property holding if asking to close git extensions is required.
        /// </summary>
        public static BoolSetting CloseInstancesProperty { get; } = new BoolSetting("CloseInstances", "Close all instances of GitExtensions before starting PluginManager", false);

        private readonly ISettingsSource source;

        /// <summary>
        /// Gets current value of <see cref="CloseInstancesProperty"/>.
        /// </summary>
        public bool CloseInstances => source.GetValue(CloseInstancesProperty.Name, CloseInstancesProperty.DefaultValue, t => Boolean.Parse(t));

        public PluginSettings(ISettingsSource source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }

        #region IEnumerable<ISetting>

        private static readonly List<ISetting> properties;

        static PluginSettings()
        {
            properties = new List<ISetting>(1)
            {
                CloseInstancesProperty
            };
        }

        public IEnumerator<ISetting> GetEnumerator()
            => properties.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        #endregion
    }
}
