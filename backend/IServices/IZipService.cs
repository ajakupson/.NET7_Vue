using backend.Models;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;

namespace backend.IServices
{
    public interface IZipService
    {
        public List<string> GetZipsList();
        public List<string> GetListEntries(string zipName);
        public List<string> GetListEntriesIFormFile(IFormFile zip);
        public List<ZipEntry> GetZipEntries(List<string> entries);
        public List<Node> BuildTree(List<ZipEntry> nodes, string parent);
        public List<Node> GetZipContent(string zipName);
        public List<Node> GetZipContentIFormFile(IFormFile zip);
        public List<string> ValidateZip(List<Node> tree);
        public bool DeleteZip(string zipName);
        public Task<Dictionary<string, string>> UploadFile(IFormFile zip);
    }
}
