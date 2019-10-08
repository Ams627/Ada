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

            var aliasPattern = @"^\s*alias\s+([A-Z0-9_-]+)\s*=";
            var creationPattern = "alias @1=\"cd '@2'\"";

            ProcessAlias(alias, bashDirAliasesPath, aliasPattern, creationPattern, replace);

        }

        /// <summary>
        /// Add an alias to the specified file
        /// </summary>
        /// <param name="alias">The alias</param>
        /// <param name="dirAliasesPath">The dir aliases file</param>
        /// <param name="aliasPattern">The regex to extract existing aliases from the file (to check for existence or dups)</param>
        /// <param name="creationPattern">The entire line used to define an alias with the literal string @1 marking the alias and the 
        /// literal string @2 marking the expansion</param>
        /// <param name="replace">If true, replace an existing alias with the same name</param>
        private void ProcessAlias(string alias, string dirAliasesPath, string aliasPattern, string creationPattern, bool replace)
        {
            var cwd = Directory.GetCurrentDirectory();
            var outputLine = creationPattern.Replace("@1", alias).Replace("@2", cwd);

            var lines = File.Exists(dirAliasesPath) ? File.ReadAllLines(dirAliasesPath).ToList() : new List<string>();
            var aliases = lines
                    .Select((x, i) => new { LineNumber = i, Line = x, Match = Regex.Match(x, aliasPattern, RegexOptions.IgnoreCase) })
                    .Where(y => y.Match.Success && y.Match.Groups.Count > 1)
                    .Select(z => new { z.LineNumber, Alias = z.Match.Groups[1].Value });

            var aliasLookup = aliases.ToLookup(x => x.Alias);

            // find aliases which have a count greater than 1 - this indicates a corrupt alias file - we should never define
            // an alias more than once:
            var corrupt = aliasLookup.Where(x => x.Count() > 1);
            foreach (var corruption in corrupt)
            {
                Console.Error.WriteLine($"Corrupt alias file {dirAliasesPath} - the alias {corruption.Key} appears more than once.");
                Console.Error.WriteLine($"Occurs at line(s) {string.Join(" ", corruption.Select(x => x.LineNumber + 1))}");
            }
            if (corrupt.Count() > 0)
            {
                throw new Exception("Exiting.");
            }

            // if the alias exists once and only once already:
            if (aliasLookup[alias].Count() > 0)
            {
                var lineNumber = aliasLookup[alias].First().LineNumber;
                if (!replace)
                {
                    // it exists and we were not asked to replace it:
                    throw new Exception($"alias {alias} already defined at line {lineNumber + 1} in {dirAliasesPath}");
                }
                lines[lineNumber] = outputLine;
            }
            else
            {
                // the alias didn't exist already so add it to the end of the file:
                lines.Add(outputLine);
            }

            File.WriteAllLines(dirAliasesPath, lines);
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