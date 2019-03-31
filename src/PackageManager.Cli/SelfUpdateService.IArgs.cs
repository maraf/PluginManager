using Neptuo;

namespace PackageManager.Services
{
    internal partial class SelfUpdateService
    {
        public interface IArgs : ICloneable<IArgs>
        {
            string Path { get; }

            bool IsSelfUpdate { get; set; }
            string SelfOriginalPath { get; set; }
        }
    }
}
