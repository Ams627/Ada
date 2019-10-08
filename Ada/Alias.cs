using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ada
{
    internal class Alias
    {
        private ISettings settings;
        private bool haveBash;
        private bool haveTcc;
        private bool haveCmd;
        private bool havePs;

        private readonly List<string> bashAliases = new List<string>();
        private readonly List<string> bashDirAliases = new List<string>();
        private readonly List<string> tccAliases = new List<string>();
        private readonly List<string> tccDirAliases = new List<string>();
        private readonly List<string> cmdDirAliases = new List<string>();


        public Alias(ISettings settings)
        {
            this.settings = settings;
        }

        private void CheckSettings()
        {
            var sections = new HashSet<string>(settings.Sections);
            if (!sections.Contains("general"))
            {
                throw new Exception("The settings file does not contain a general section - do not know which shell to define aliases for.");
            }
            if (!sections.Contains("paths"))
            {
                throw new Exception("The settings file does not contain a path section - cannot find path to aliases files.");
            }

            if (!settings.DoesSettingExist("general", "shells"))
            {
                throw new Exception("The settings file does not contain the setting for shells in the general section.");
            }

            var shells = new HashSet<string>(settings.GetListSetting("general", "shells"));
            haveBash = shells.Contains("bash");
            haveTcc = shells.Contains("tcc");
            haveCmd = shells.Contains("cmd");
            havePs = shells.Contains("ps");
        }

        internal int Add(AddOptions addOptions)
        {
            CheckSettings();
            var bashDirAliasesPath = settings.GetSetting("paths", "bash-dir-aliases-path");
            if (!File.Exists(bashDirAliasesPath))
            {
                
            }
            return 0;
        }

        internal int List()
        {
            throw new NotImplementedException();
        }

        internal int Remove()
        {
            throw new NotImplementedException();
        }
    }
}