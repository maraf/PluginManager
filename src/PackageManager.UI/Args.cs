using NuGet.Frameworks;
using PackageManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager
{
    public class Args : SelfUpdateService.IArgs
    {
        public string Path { get; set; }
        public IReadOnlyCollection<NuGetFramework> Monikers { get; set; }
        public (string id, string version)[] Dependencies { get; set; }
        public string SelfPackageId { get; set; }

        public bool IsSelfUpdate { get; set; }
        public string SelfOriginalPath { get; set; }

        public string[] ProcessNamesToKillBeforeChange { get; set; }

        public Args(string[] args)
        {
            Monikers = Array.Empty<NuGetFramework>();
            Dependencies = Array.Empty<(string id, string version)>();

            ParseParameters(args);

            if (ProcessNamesToKillBeforeChange == null)
                ProcessNamesToKillBeforeChange = new string[0];
        }

        #region Parse

        private void ParseParameters(string[] args)
        {
            List<string> items = args.ToList();
            foreach (string arg in items.ToList())
            {
                if (arg == "--selfupdate")
                {
                    IsSelfUpdate = true;
                    items.Remove(arg);
                }
            }

            args = items.ToArray();
            if (args.Length % 2 == 0)
            {
                for (int i = 0; i < args.Length; i += 2)
                {
                    string name = args[i];
                    string value = args[i + 1];
                    ParseParameter(name, value);
                }
            }

            if (Monikers.Count == 0)
            {
                Monikers = new List<NuGetFramework>()
                {
                    NuGetFramework.AnyFramework,
                    FrameworkConstants.CommonFrameworks.Net461
                };
            }
        }

        private bool ParseParameter(string name, string value)
        {
            switch (name)
            {
                case "--path":
                    Path = value;
                    return true;
                case "--monikers":
                    Monikers = ParseMonikers(value);
                    return true;
                case "--dependencies":
                    Dependencies = ParseDependencies(value);
                    return true;
                case "--selfpackageid":
                    SelfPackageId = value;
                    return true;
                case "--selforiginalpath":
                    SelfOriginalPath = value;
                    return true;
                case "--processnamestokillbeforechange":
                    ProcessNamesToKillBeforeChange = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    return true;
                default:
                    return false;
            }
        }

        private (string id, string version)[] ParseDependencies(string arg)
        {
            string[] dependencies = arg.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            (string id, string version)[] result = new (string id, string version)[dependencies.Length];

            for (int i = 0; i < dependencies.Length; i++)
            {
                string dependency = dependencies[i];

                string[] parts = dependency.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                    result[i] = (parts[0], null);
                else
                    result[i] = (parts[0], parts[1][0] == 'v' ? parts[1].Substring(1) : parts[1]);
            }

            return result;
        }

        private IReadOnlyCollection<NuGetFramework> ParseMonikers(string arg)
        {
            string[] values = arg.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<NuGetFramework> result = new List<NuGetFramework>();
            foreach (string value in values)
            {
                NuGetFramework framework = NuGetFramework.Parse(value, DefaultFrameworkNameProvider.Instance);
                result.Add(framework);
            }

            return result;
        }

        #endregion

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            if (!String.IsNullOrEmpty(Path))
                result.Append($"--path \"{Path}\"");

            if (Monikers.Count > 0)
            {
                result.Append($" --monikers ");
                result.Append(String.Join(",", Monikers.Select(m => m.GetShortFolderName(DefaultFrameworkNameProvider.Instance))));
            }

            if (Dependencies.Length > 0)
            {
                result.Append(" --dependencies ");
                result.Append(String.Join(",", Dependencies.Select(d => d.id + "-v" + d.version)));
            }

            if (!String.IsNullOrEmpty(SelfPackageId))
                result.Append($" --selfpackageid {SelfPackageId}");

            if (IsSelfUpdate)
                result.Append(" --selfupdate");

            if (!String.IsNullOrEmpty(SelfOriginalPath))
                result.Append($" --selforiginalpath \"{SelfOriginalPath}\"");

            if (ProcessNamesToKillBeforeChange != null && ProcessNamesToKillBeforeChange.Length > 0)
                result.Append($" --processnamestokillbeforechange \"{String.Join(",", ProcessNamesToKillBeforeChange)}\"");

            return result.ToString();
        }
    }
}
