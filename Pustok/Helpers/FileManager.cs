namespace Pustok.Helpers
{
    public static class FileManager
    {
        public static string SaveFile(this IFormFile file , string rootPath , string folderName )
        {
            string filename = file.FileName;
            filename =  filename.Length > 64 ? filename.Substring(filename.Length - 64, 64) : filename;

            filename = Guid.NewGuid().ToString() + filename;
            string path = Path.Combine(rootPath, folderName ,filename);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return filename;
        }

        public static void DeleteFile(string rootPath , string folderName , string fileName)
        {
            string deletePath = Path.Combine(rootPath, folderName ,fileName);
            if (System.IO.File.Exists(deletePath))
            {
                System.IO.File.Delete(deletePath);
            }
        }

        
    }
}
