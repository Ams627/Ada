using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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

            if (haveBash)
            {
                AddBash(addOptions.Alias, addOptions.Replace);
            }

            if (haveTcc)
            {
                AddTcc(addOptions.Alias, addOptions.Replace);
            }

            if (haveCmd)
            {
                AddCmd(addOptions.Alias, addOptions.Replace);
            }

            if (havePs)
            {
                AddPs(addOptions.Alias, addOptions.Replace);
            }
           
            return 0;
        }

        private void AddPs(string alias, bool replace)
        {
            throw new NotImplementedException();
        }

        private void AddCmd(string alias, bool replace)
        {
            throw new NotImplementedException();
        }

        private void AddTcc(string alias, bool replace)
        {
            throw new NotImplementedException();
        }

        private void AddBash(string alias, bool replace)
        {
            var tmp = settings.GetSetting("paths", "bash-dir-aliases-path");
            var bashDirAliasesPath = Environment.ExpandEnvironmentVariables(tmp);
            var expanded = !bashDirAliasesPath.Contains("%");

            if (!expanded)
            {
                throw new Exception($"Couldn't expand environment variables in bash path {bashDirAliasesPath}");
            }

            var cwd = Directory.GetCurrentDirectory();

            var lines = new List<string>();
            if (File.Exists(bashDirAliasesPath))
            {
                var aliasPattern = @"^\s*alias\s+([A-Z0-9_-]+)\s*=";
                lines = File.ReadAllLines(bashDirAliasesPath).ToList();
                var aliases = lines
                        .Select((x, i) => new { LineNumber = i, Line = x, Match = Regex.Match(x, aliasPattern, RegexOptions.IgnoreCase) })
                        .Where(y => y.Match.Success && y.Match.Groups.Count > 1)
                        .Select(z => new { z.LineNumber, Alias = z.Match.Groups[1].Value });
                var aliasesDict = aliases.ToLookup(x => x.Alias);
                var corrupt = aliasesDict.Where(x => x.Count() > 1);
                foreach (var corruption in corrupt)
                {
                    Console.WriteLine($"Corrupt alias file {bashDirAliasesPath} - the alias {corruption.Key} appears more than once.");
                    Console.WriteLine($"Occurs at line(s) {string.Join(" ", corruption.Select(x=>x.LineNumber + 1))}");
                }
                if (corrupt.Count() > 0)
                {
                    throw new Exception("Exiting.");
                }
                   
                if (aliasesDict[alias].Count() > 0)
                {
                    var lineNumber = aliasesDict[alias].First().LineNumber;
                    if (!replace)
                    {
                        throw new Exception($"alias {alias} already defined at line {lineNumber + 1} in {bashDirAliasesPath}");
                    }
                    lines[lineNumber] = $"alias {alias}=\"cd '{cwd}'\"";
                }
                else
                {
                    lines.Add($"alias {alias}=\"cd '{cwd}'\"");
                }
            }
            else
            {
                lines.Add($"alias {alias}=\"cd '{cwd}'\"");
            }

            File.WriteAllLines(bashDirAliasesPath, lines);
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