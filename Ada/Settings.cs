using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Ada
{
    interface ISettings
    {
        string[] Sections { get;}
        bool DoesSettingExist(string section, string setting);
        string GetSetting(string section, string setting);
        IEnumerable<string> GetListSetting(string section, string setting);
    }
    class Settings : ISettings
    {
        readonly Dictionary<string, Dictionary<string, string>> iniOptions = new Dictionary<string, Dictionary<string, string>>();

        string[] ISettings.Sections => iniOptions.Keys.ToArray();

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
                "[general]",
                "# shells can be one of bash, cmd, tc, ps",
                "shells=bash, cmd, tc, ps",
                "",
                "[paths]",
                "    bash-aliases-path=%HOME%/.aliases",
                "    bash-dir-aliases-path=%HOME%/.dir-aliases",
                "    bash-functions-path=%HOME%/.functions",
                "    bash-bashrc-path=%HOME%/.bashrc",
                "",
                @"    tcc-aliases-path=c:\tcc\aliases",
                @"    tcc-ini-path=c:\tcc\tcc.ini",
                @"    tcc-netmap-path=c:\tcc\net.map",
                @"     cmd-aliases-path=c:\"
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

        public void ReadSettings()
        {
            var fullSettingsPathname = GetSettingsPath();
            if (!File.Exists(fullSettingsPathname))
            {
                throw new Exception($"Settings file {fullSettingsPathname} does not exist. Please run ada defsettings.");
            }

            var lines = File.ReadAllLines(fullSettingsPathname).Select(x=>x.Trim()).Where(x=>!string.IsNullOrEmpty(x));
            var sections = lines.Where(x => x.StartsWith("[") && x.EndsWith("]")).ToLookup(x=>x);
            var plurals = sections.Where(x => x.Count() > 1);
            foreach (var pluralSection in plurals)
            {
                Console.WriteLine($"Section name {pluralSection.Key} defined more than once.");
            }
            if (plurals.Count() > 0)
            {
                throw new Exception("Exiting - please modify the settings file using ada esettings.");
            }

            var currentSection = "";
            foreach (var line in lines.Where(x=>!x.StartsWith("#")))
            {
                // test for section name in square brackets and if found store in current section:
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSection = line.Substring(1, line.Length - 2);
                    iniOptions[currentSection] = new Dictionary<string, string>();
                }
                // options are name=value pairs so test for a line containing a single equals:
                else if (line.Count(x=>x == '=') == 1)
                {
                    var split = line.Split('=').Select(x => x.Trim()).ToArray();
                    if (iniOptions[currentSection].ContainsKey(split[0]))
                    {
                        Console.Error.WriteLine($"Warning: key {split[0]} defined more than once in section {currentSection}.");
                        Console.Error.WriteLine($"Second definition ignored.");
                    }
                    else
                    {
                        iniOptions[currentSection].Add(split[0], split[1]);
                    }
                }
            }
        }

        public string GetSetting(string section, string setting)
        {
            var result = "";
            if (iniOptions.ContainsKey(section) && iniOptions[section].ContainsKey(setting))
            {
                result = iniOptions[section][setting];
            }
            return result;
        }

        /// <summary>
        /// return an IEnumerable<string> from a setting composed of a comma-separated list.
        /// spaces are stripped from each item.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public IEnumerable<string> GetListSetting(string section, string setting)
        {
            var result = "";
            if (iniOptions.ContainsKey(section) && iniOptions[section].ContainsKey(setting))
            {
                result = iniOptions[section][setting];
            }

            return result.Split(',').Select(x => x.Trim());
        }

        public bool DoesSettingExist(string section, string setting)
        {
            return iniOptions.ContainsKey(section) && iniOptions[section].ContainsKey(setting);
        }
    }
}
