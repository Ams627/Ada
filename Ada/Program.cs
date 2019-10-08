using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Ada
{
    [Verb("settings", HelpText = "Print settings on the command line.")]
    class PrintSettings
    {
    }

    [Verb("esettings", HelpText = "Use the Windows configured editor to edit the settings or the editor configured in the settings itself.")]
    class EditSettings
    {
    }

    [Verb("defsettings", HelpText = "Go back to the default settings.")]
    class DefaultSettings
    {
    }

    [Verb("add", HelpText = "Add a directory alias.")]
    class AddOptions
    {
        [Option(
          Default = false,
          HelpText = "Replace the directory alias if it already exists.")]
        public bool Replace { get; set; }

        [Value(0, MetaName = "alias",
        HelpText = "alias to be added.",
        Required = true)]
        public string Alias { get; set; }
    }

    [Verb("list", HelpText = "List all directory aliases.")]
    class ListOptions
    {
    }

    [Verb("listthis", HelpText = "List the directory aliases for the current directory.")]
    class ListThisOptions
    {
    }

    [Verb("remove", HelpText = "Remove a directory alias.")]
    class RemoveOptions
    {
    }



    class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                var settings = new Settings();
                settings.FirstRunCheck();

                return CommandLine.Parser.Default.ParseArguments<PrintSettings, EditSettings, DefaultSettings, AddOptions, ListOptions, RemoveOptions>(args)
                .MapResult(
                    (PrintSettings opts) => settings.Print(),
                    (EditSettings opts) => settings.Edit(),
                    (DefaultSettings opts) => settings.Default(),
                    (AddOptions opts) => { settings.ReadSettings(); return new Alias(settings).Add(opts); },
                    (ListOptions opts) => new Alias(settings).List(),
                    (RemoveOptions opts) => new Alias(settings).Remove(),
                    errs => 1);;

            }
            catch (Exception ex)
            {
                var fullname = System.Reflection.Assembly.GetEntryAssembly().Location;
                var progname = Path.GetFileNameWithoutExtension(fullname);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
            }
            return -1;
        }
    }
}
