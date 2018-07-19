using PackageManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager
{
    public partial class Args : IPackageSourceProvider
    {
        public string Path { get; set; }
        public string PackageSourceUrl { get; set; }
        public string SelfPackageId { get; set; }

        public bool IsUpdateCount { get; set; }

        string IPackageSourceProvider.Url => PackageSourceUrl;

        public Args(string[] args)
        {
            ParseParameters(args);
        }

        #region Parse

        private bool ParseParameters(string[] args)
        {
            int skipped = 0;
            if (args[0] == "update")
            {
                skipped++;
                if (args[1] == "--count")
                {
                    IsUpdateCount = true;
                    skipped++;
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
                default:
                    return false;
            }
        }

        #endregion
    }
}
