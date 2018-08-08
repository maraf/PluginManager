namespace PackageManager.Services
{
    internal partial class SelfUpdateService
    {
        public interface IApplication
        {
            IArgs Args { get; }
            void Shutdown();
        }
    }
}
