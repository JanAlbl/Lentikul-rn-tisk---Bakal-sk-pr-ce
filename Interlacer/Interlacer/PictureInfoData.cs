using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GfxlibWrapper;

namespace Interlacer
{
    /// <summary>
    /// trida, ktera uchovava informace o obrazcich
    /// </summary>
    public class PictureInfoData
    {
        /// <summary>
        /// slovnik, ktery obsahuje jako klice cesty k souborum a jako hodnoty "pingnute" obrazky
        /// </summary>
        private Dictionary<String, Picture> pictures = new Dictionary<String, Picture>();

        /// <summary>
        /// vrati instanci "pingnutou" tridy Picture podle zadane cesty
        /// </summary>
        /// <param name="path">cesta k souboru</param>
        /// <returns>instance tridy Picture, na kter byla zavolana metoda Ping</returns>
        public Picture GetInfo(String path)
        {
            if (!pictures.ContainsKey(path))  //pokud obrazek jeste neni obsazen ve slovniku
            {
                Picture pic = new Picture(path);
                pic.Ping();
                pictures.Add(path, pic);
            }
            return pictures[path];
        }
    }
}
