using System.Globalization;

namespace backend.IServices
{
    public interface ICultureService
    {
        public CultureInfo[] GetAllCultures();
        public string[] GetLangsIsoCodes();
    }
}
