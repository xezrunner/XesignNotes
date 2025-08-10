using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XesignNotes.App.Configuration
{
    class DefaultApplicationConfiguration
    {
        public List<Configuration> CreateDefaultApplicationConfigurationList()
        {
            return new List<Configuration>
            {
                // Theming engine
                new Configuration() {Property = "IsThemingEnabled", DefaultValue = "True"},
                new Configuration() {Property = "IsAccentColorizationEnabled", DefaultValue = "True"},
                new Configuration() {Property = "IsFluentDesignChoice", DefaultValue = "True"},
                new Configuration() {Property = "IsFluentDesignForced", DefaultValue = "False"},
                new Configuration() {Property = "ShowFluentDesignIncompatibleNotice", DefaultValue = "Unintrusive"},

                // Note taking
                new Configuration() {Property = "IsNoteColorizationEnabled", DefaultValue = "True"},
                new Configuration() {Property = "NotesCanHaveCustomTitles", DefaultValue = "True"},
                new Configuration() {Property = "UseNewNotesListExperience", DefaultValue = "False"},
                new Configuration() {Property = "EnableEditorCustomization", DefaultValue = "True"},
                new Configuration() {Property = "InEditorCustomizations", DefaultValue = "True"},
                
                // Configuration import & export
                new Configuration() {Property = "EnableTxtFileSupport", DefaultValue = "True"},
                new Configuration() {Property = "EnableTxtFileImport", DefaultValue = "True"},
                new Configuration() {Property = "EnableTxtFileExport", DefaultValue = "True"},
                new Configuration() {Property = "DisableConfigurationBackupRestore", DefaultValue = "False"},
                new Configuration() {Property = "UseBackupRestoreProgressUI", DefaultValue = "True"},

                // Updates engine
                new Configuration() {Property = "EnableUpdates", DefaultValue = "False"},
                new Configuration() {Property = "EnableUpdateNotifications", DefaultValue = "True"},

                // Debug & internal
                // ----- logging -----
                new Configuration() {Property = "IsDebugLoggingEnabled", DefaultValue = "Auto"},
                new Configuration() {Property = "AddDebugInfoToNotes", DefaultValue = "False"}
            };
        }
    }
}
