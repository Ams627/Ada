using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web;

namespace Ada
{
    class Settings
    {
        private static string GetSettingsPath()
        {
            var company = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(
                            Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute), false))
                           .Company;
            var product = ((AssemblyProductAttribute)Attribute.GetCustomAttribute(
                Assembly.GetExecutingAssembly(), typeof(AssemblyProductAttribute), false))
               .Product;

            var settingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), company, product, "settings.ini");
            return settingsFilePath;
        }

        internal int Print()
        {
            throw new NotImplementedException();
        }

        internal int Edit()
        {
            var filename = GetSettingsPath();
            Console.WriteLine(filename);
            var procInfo = new ProcessStartInfo
            {
                Verb = "Edit",
                FileName = filename
            };
            Process.Start(procInfo);
            return 1;
        }

        internal int Default()
        {
            var fullSettingsPathname = GetSettingsPath();
            var directory = Path.GetDirectoryName(fullSettingsPathname);
            Directory.CreateDirectory(directory);
            var text = new[]
            {
                "# default settings - feel free to adjust.",
                "[paths]",
                "    bash-aliases-path=%HOME%/.aliases",
                "    bash-dir-aliases-path=%HOME%/.dir-aliases",
                "    bash-functions-path=%HOME%/.functions",
                "    bash-bashrc-path=%HOME%/.bashrc",
                "",
                @"    tcc-aliases-path=c:\tcc\aliases",
                @"    tcc-ini-path=c:\tcc\tcc.ini",
                @"    tcc-netmap-path=c:\tcc\net.map",
            };
            File.WriteAllLines(fullSettingsPathname, text);
            return 0;
        }

        internal void FirstRunCheck()
        {
            var fullSettingsPathname = GetSettingsPath();
            if (!File.Exists(fullSettingsPathname))
            {
                Default();
            }
        }
    }
}
