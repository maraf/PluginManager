using PackageManager.Services;
using PackageManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager
{
    public partial class Args : IPackageSourceProvider, SelfUpdateService.IArgs
    {
        public string Path { get; set; }
        public string PackageSourceUrl { get; set; }
        public string SelfPackageId { get; set; }

        public bool IsUpdateCount { get; set; }

        public bool IsUpdatePackage { get; set; }
        public string PackageId { get; set; }

        public bool IsSelfUpdate { get; set; }
        public string SelfOriginalPath { get; set; }

        string IPackageSourceProvider.Url => PackageSourceUrl;

        public Args(string[] args)
        {
            ParseParameters(args);
        }

        #region Parse

        private bool ParseParameters(string[] args)
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

            int skipped = 0;
            if (args[0] == "update")
            {
                skipped++;
                if (args[1] == "--count")
                {
                    IsUpdateCount = true;
                    skipped++;
                }
                else if(args[1] == "--package")
                {
                    IsUpdatePackage = true;
                    PackageId = args[2];
                    skipped += 2;
                }
            }

            if ((args.Length - skipped) % 2 == 0)
            {
                for (int i = skipped; i < args.Length; i += 2)
                {
                    string name = args[i];
                    string value = args[i + 1];
                    ParseParameter(name, value);
                }
            }

            return true;
        }

        private bool ParseParameter(string name, string value)
        {
            switch (name)
            {
                case "--path":
                    Path = value;
                    return true;
                case "--packagesource":
                    PackageSourceUrl = value;
                    return true;
                case "--selfpackageid":
                    SelfPackageId = value;
                    return true;
                case "--selforiginalpath":
                    SelfOriginalPath = value;
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            if (!String.IsNullOrEmpty(Path))
                result.Append($"--path \"{Path}\"");

            if (!String.IsNullOrEmpty(PackageSourceUrl))
                result.Append($" --packagesource {PackageSourceUrl}");

            if (!String.IsNullOrEmpty(SelfPackageId))
                result.Append($" --selfpackageid {SelfPackageId}");

            if (IsSelfUpdate)
                result.Append(" --selfupdate");

            if (!String.IsNullOrEmpty(SelfOriginalPath))
                result.Append($" --selforiginalpath \"{SelfOriginalPath}\"");

            return result.ToString();
        }
    }
}
