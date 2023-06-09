namespace backend.Constants
{
    public class EM
    {
        public static readonly string FOLDER_REQUIRED = "folder is required";
        public static readonly string DLL_REQUIRED = "Folder 'dlls' must contain RootFolder.dll file";
        public static readonly string IMG_REQUIRED = "At least one image of type [.jpg, .png] is required";
        public static readonly string JPG_PNG_ONLY = "Folder 'images' must contain only images of types [.jpg, .png]";
        public static readonly string XML_REQUIRED = "Folder 'languages' must contain RootFolder_en.xml file";
        public static readonly string XML_ONLY = "Folder 'languages' must contain only RootFolder_xx.xml files, where xx - 2 letter language code";
    }
}
