using System.IO;
using System.Runtime.CompilerServices;

namespace MyRazorClassLib
{
    public static class Locator
    {
        public static string Root => GetRoot();

        public static string GetRoot([CallerFilePath] string path = null)
            => Path.GetDirectoryName(path);
    }
}