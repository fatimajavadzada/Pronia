namespace Pronia.Helpers
{
    public static class ExtensionMethods
    {
        public static bool CheckType(this IFormFile file, string type)
        {
            return file.ContentType.Contains(type);
        }

        public static bool CheckSize(this IFormFile file, int size)
        {
            return file.Length < size * 1024 * 1024;
        }

        public static string SaveFile(this IFormFile file, string path)
        {
            string uniqueName = Guid.NewGuid().ToString() + file.FileName;

            string filePath = Path.Combine(path, uniqueName);

            using FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate);

            file.CopyTo(stream);

            return uniqueName;
        }
    }
}
