using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XesignNotes.App.Configuration
{
    public class DefaultDebugConfiguration
    {
        public List<Configuration> CreateDefaultDebugConfigurationList()
        {
            return new List<Configuration>
            {
                new Configuration() {Property = "ShowDebugOptions", DefaultValue = "True"},
                new Configuration() {Property = "ShowInternalOptions", DefaultValue = "True"},
                new Configuration() {Property = "InternalFeaturesEnabled", DefaultValue = "True"},
                new Configuration() {Property = "MainPage_MainDebugEntryPoint", DefaultValue = "True"},
                new Configuration() {Property = "MainPage_InternalEntryPoint", DefaultValue = "False"},

                new Configuration() {Property = "Internal_EnableModulesManagement", DefaultValue = "True"},
                new Configuration() {Property = "Internal_Modules_EnablePasswordProtectedModuleManagement", DefaultValue = "False"},

                new Configuration() {Property = "Internal_UpdatesEngine_Debug", DefaultValue = "True"},
            };
        }
    }
}
