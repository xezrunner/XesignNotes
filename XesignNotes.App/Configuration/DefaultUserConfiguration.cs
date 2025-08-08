using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XesignNotes.App.Configuration
{
    public class DefaultUserConfiguration
    {
        public List<Configuration> CreateDefaultUserConfigurationList()
        {
            return new List<Configuration>
            {
                new Configuration() {Property = "Theme", DefaultValue = "Dark"},
                new Configuration() {Property = "Accent", DefaultValue = "Blue"},
                new Configuration() {Property = "RevealFX", DefaultValue = true},
                new Configuration() {Property = "AcrylicFX", DefaultValue = true},
                new Configuration() {Property = "RecentSearches", DefaultValue = ""},
                new Configuration() {Property = "IsTelemetryEnabled", DefaultValue = "question"}
            };
        }
    }
}
