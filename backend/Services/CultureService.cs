using backend.IServices;
using System.Globalization;

namespace backend.Services
{
    public class CultureService : ICultureService
    {
        public CultureInfo[] GetAllCultures()
        {
            return CultureInfo.GetCultures(CultureTypes.AllCultures);
        }

        public string[] GetLangsIsoCodes()
        {
            CultureInfo[] cultures = GetAllCultures();
            return cultures.Select(c => c.TwoLetterISOLanguageName).ToArray();
        }
    }
}
