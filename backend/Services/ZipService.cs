using backend.Constants;
using backend.IServices;
using backend.Models;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.Security.Cryptography.Xml;

namespace backend.Services
{
    public class ZipService : IZipService
    {
        private string zipsFolder = "zips";
        private readonly ICultureService _cultureService;

        public ZipService(ICultureService cultureService)
        {
            _cultureService = cultureService;
        }

        public List<string> GetZipsList()
        {
            List<string> zipNames = new List<string>();

            if (Directory.Exists(zipsFolder))
            {
                DirectoryInfo di = new DirectoryInfo(zipsFolder);
                FileInfo[] zips = di.GetFiles("*.zip");
                foreach (FileInfo file in zips)
                {
                    zipNames.Add(file.Name);
                }
            } 
            else
            {
                throw new DirectoryNotFoundException(); 
            }

            return zipNames;
        }

        public List<string> GetListEntries(string zipName)
        {
            List<string> entries = new List<string>();

            string zipPath = Path.Combine(zipsFolder, zipName);
            if (File.Exists(zipPath))
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        entries.Add(entry.FullName);
                    }
                }
            } else
            {
                throw new FileNotFoundException();
            }

            return entries;
        }

        public List<string> GetListEntriesIFormFile(IFormFile zip)
        {
            List<string> entries = new List<string>();

            using (var stream = zip.OpenReadStream())
            using (var archive = new ZipArchive(stream))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    entries.Add(entry.FullName);
                }
            }

            return entries;
        }

        public List<ZipEntry> GetZipEntries(List<string> entries)
        {
            List<ZipEntry> nodes = new List<ZipEntry>();

            foreach (string entry in entries)
            {
                string[] parts = entry.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                Array.Reverse(parts);

                ZipEntry node = new ZipEntry();
                node.Name = parts[0];
                if (parts.Length > 1)
                {
                    node.Parent = parts[1];
                }

                nodes.Add(node);
            }

            if (nodes.First().Parent != null)
            {
                ZipEntry parent = new ZipEntry();
                parent.Name = entries.First().Split('/')[0];
                nodes.Insert(0, parent);
            }

            return nodes;
        }

        public List<Node> BuildTree(List<ZipEntry> nodes, string parent = null)
        {
            List<Node> tree = new List<Node>();
            foreach (ZipEntry node in nodes)
            {
                if (node.Parent == parent)
                {
                    Node n = new Node(node);
                    n.Children = BuildTree(nodes, node.Name);
                    tree.Add(n);
                }
            }

            return tree;
        }

        public List<Node> GetZipContent(string zipName)
        {
            List<string> entries = GetListEntries(zipName);
            List<ZipEntry> nodes = GetZipEntries(entries);
            List<Node> tree = BuildTree(nodes);

            return tree;
        }

        public List<Node> GetZipContentIFormFile(IFormFile zip)
        {
            List<string> entries = GetListEntriesIFormFile(zip);
            List<ZipEntry> nodes = GetZipEntries(entries);
            List<Node> tree = BuildTree(nodes);

            return tree;
        }

        public List<string> ValidateZip(List<Node> tree)
        {
            List<string> errorMessages = new List<string>();

            Node root = tree.First();
            List<Node> rootChildren = root.Children;

            List<string> missingFolders = new List<string> { "dlls", "images", "languages" };
            bool hasDll = false;
            string[] allowedImages = { "jpg", "png" };
            bool hasImage = false;
            bool invalidImagesFolder = false;
            string[] allowedLanguages = { "xml" };
            bool hasLanguage = false;
            bool invalidLanguagesFolder = false;
            string[] langIsoCodes = _cultureService.GetLangsIsoCodes();

            foreach (Node child in rootChildren)
            {
                if (child.Name == "dlls")
                {
                    missingFolders.Remove(child.Name);
                    foreach (Node dllChild in child.Children)
                    {
                        if (root.Name + ".dll" == dllChild.Name)
                        {
                            hasDll = true;
                        }
                    }
                }

                if (child.Name == "images")
                {
                    missingFolders.Remove("images");
                    foreach (Node imagesChild in child.Children)
                    {
                        string[] parts = imagesChild.Name.Split('.');
                        if (parts.Length == 0)
                        {
                            invalidImagesFolder = true;
                        }
                        else
                        {
                            string ext = parts[parts.Length - 1];
                            if (allowedImages.Contains(ext))
                            {
                                hasImage = true;
                            }
                            else
                            {
                                invalidImagesFolder = true;
                            }
                        }
                    }
                }

                if (child.Name == "languages")
                {
                    missingFolders.Remove("languages");
                    foreach (Node languagesChild in child.Children)
                    {

                        string lName = languagesChild.Name;
                        int idx = lName.LastIndexOf('.') + 1;
                        string name = "";
                        if (idx > 1) {
                            name = lName.Substring(0, idx - 1);
                        }
                        string ext = lName.Substring(idx, lName.Length - idx);

                        if (ext == "xml")
                        {
                            idx = name.LastIndexOf("_") + 1;
                            string lCode = name.Substring(idx, name.Length - idx);
                            if (langIsoCodes.Contains(lCode))
                            {
                                if (root.Name + "_" + lCode + ".xml" == languagesChild.Name)
                                {
                                    hasLanguage = true;
                                }
                            }
                            else
                            {
                                invalidLanguagesFolder = true;
                            }
                        } 
                        else
                        {
                            invalidLanguagesFolder = true;
                        }
                    }
                }
            }

            foreach (string folder in missingFolders)
            {
                errorMessages.Add(folder + " " + EM.FOLDER_REQUIRED);
            }

            if (!missingFolders.Contains("dlls"))
            {
                if (!hasDll)
                {
                    errorMessages.Add(EM.DLL_REQUIRED);
                }
            }

            if (!missingFolders.Contains("images"))
            {
                if (!hasImage)
                {
                    errorMessages.Add(EM.IMG_REQUIRED);
                }

                if (invalidImagesFolder)
                {
                    errorMessages.Add(EM.JPG_PNG_ONLY);
                }
            }

            if (!missingFolders.Contains("languages"))
            {
                if (!hasLanguage)
                {
                    errorMessages.Add(EM.XML_REQUIRED);
                }

                if (hasLanguage && invalidLanguagesFolder)
                {
                    errorMessages.Add(EM.XML_ONLY);
                }
            }

            return errorMessages;
        }

        public bool DeleteZip(string zipName)
        {
            string path = Path.Combine(zipsFolder, zipName);
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            } 
            else
            {
                throw new FileNotFoundException();
            }
        }

        public async Task<Dictionary<string, string>> UploadFile(IFormFile zip)
        {
            Dictionary<string, string> result = new Dictionary<string, string>
            {
                { "status", "error" },
                { "message", "" }
            };

            try
            {
                string path = Path.Combine(zipsFolder, zip.FileName.Trim('"'));
                using (FileStream output = File.Create(path))
                {
                    await zip.CopyToAsync(output);
                    result["status"] = "success";
                }
            }
            catch (Exception ex)
            {
                result["message"] = ex.Message;
            }

            return result;
        }
    }
}
