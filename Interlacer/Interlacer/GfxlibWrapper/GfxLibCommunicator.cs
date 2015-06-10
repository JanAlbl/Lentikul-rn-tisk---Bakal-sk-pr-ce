using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GfxlibWrapper
{
    //definuje typ podle velikosti barevneho kanalu (Int32 - 8-bitovy kanal / Int64 - 16-bitovy kanal)
    using IntPx = Int32;

    /// <summary>
    /// staticka trida obsahujici funkce pro komunikaci s grafickou knihovnou prostrednictvim DLL
    /// </summary>
    static unsafe class GfxlibCommunicator
    {
        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void test();

        /// <summary>
        /// otestuje, zda lze volat funkce z Gfxlib.dll, pokud ne, indikuje to chybne nacteni souboru
        /// </summary>
        /// <returns>true, pokud je knihovna nactena spravne, false, pokud ne</returns>
        public static bool Test()
        {
            try
            {
                GfxlibCommunicator.test();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// nastaveni pametovych limitu knihovny
        /// </summary>
        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setResourceLimits();

        /// <summary>
        /// vytvori instanci tridy Magick::Image o dane sirce a vysce a vrati ji jako void* pointer
        /// </summary>
        /// <param name="w">sirka obrazku</param>
        /// <param name="h">vyska obrazku</param>
        /// <returns>vytvorena instance tridy Magick::Image</returns>
        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void* createImage(int w, int h);

        /// <summary>
        /// smaze danou instnanci tridy Magick::Image
        /// </summary>
        /// <param name="ptr">instance ke smazani</param>
        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void deleteImage(void* ptr);

        /// <summary>
        /// vrati pole pixelu dane instance tridy Magick::Image, ktera je predana parametrem
        /// </summary>
        /// <param name="ptr">instance tridy Magick::Image</param>
        /// <returns>ukazatel na pole pixelu</returns>
        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPx* getPixelDataPtr(void* ptr);

        /// <summary>
        /// nacte obrazek ze souboru, jehoz nazev je predan parametrem a vrati ho v podobe instance tridy Magick::Image
        /// </summary>
        /// <param name="filename">nazev souboru</param>
        /// <returns>vytvorena instance tridy Magick::Image</returns>
        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void* loadImage(char[] filename);

        /// <summary>
        /// nacte obrazek bez obrazovych dat (pouze info) ze souboru, jehoz nazev je predan parametrem a vrati ho v podobe instance tridy Magick::Image
        /// </summary>
        /// <param name="filename">nazev souboru</param>
        /// <returns>instance tridy Magick::Image</returns>
        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void* pingImage(char[] filename);

        /// <summary>
        /// ulozi obrazek do souboru, jehoz nazev je predan parametrem
        /// </summary>
        /// <param name="ptr">instance tridy Magick::Image</param>
        /// <param name="filename">nazev souboru</param>
        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void saveImage(void* ptr, char[] filename);

        /// <summary>
        /// vrati sirku v pixelech instance tridy Magick::Image, ktera je predana parametrem
        /// </summary>
        /// <param name="ptr">instnace tridy Magick::Image</param>
        /// <returns>sirka v px</returns>
        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getImageWidth(void* ptr);

        /// <summary>
        /// vrati vysku v pixelech instance tridy Magick::Image, ktera je predana parametrem
        /// </summary>
        /// <param name="ptr">instance tridy Magick::Image</param>
        /// <returns>vyska v px</returns>
        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getImageHeight(void* ptr);

        /// <summary>
        /// vrati rozliseni pro osu X instance tridy Magick::Image, ktera je preadana parametrem,
        /// druhy parametr je vystupni a je do nej ulozena informace o jednotkach
        /// </summary>
        /// <param name="ptr">instance tridy Magick::Image</param>
        /// <param name="unitType">vystupni parametr pro informaci o jednotkach</param>
        /// <returns>rozliseni na ose X</returns>
        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double getImageXResolution(void* ptr, int* unitType);

        /// <summary>
        /// vrati rozliseni pro osu Y instance tridy Magick::Image, ktera je preadana parametrem,
        /// druhy parametr je vystupni a je do nej ulozena informace o jednotkach
        /// </summary>
        /// <param name="ptr">instance tridy Magick::Image</param>
        /// <param name="unitType">vystupni parametr pro informaci o jednotkach</param>
        /// <returns>rozliseni na ose Y</returns>
        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double getImageYResolution(void* ptr, int* unitType);

        /// <summary>
        /// nastavi rozliseni pro instanci tridy Magick::Image, ktera je predana parametrem, posledni parametr je informace o jednotkach
        /// </summary>
        /// <param name="ptr">instance tridy Magick::Image</param>
        /// <param name="xRes">rozliseni na ose X</param>
        /// <param name="yRes">rozliseni na ose Y</param>
        /// <param name="unitType">informace o jednotkach (1 - DPI, 2 - DPCM)</param>
        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void setImageResolution(void* ptr, double xRes, double yRes, int unitType);

        /// <summary>
        /// zmeni velikost obrazku v instanci tridy Magick::Image predane parametrem, posledni parametr je cislo pouziteho filtru
        /// </summary>
        /// <param name="ptr">instance tridy Magick::Image</param>
        /// <param name="w">nova sirka</param>
        /// <param name="h">nova vyska</param>
        /// <param name="filterType">cislo filtru</param>
        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void resizeImage(void* ptr, int w, int h, int filterType);

        /// <summary>
        /// orizne obrazek v instanci tridy Magick::Image predane parametrem na dany obdelnik
        /// </summary>
        /// <param name="ptr">instance tridy Magick::Image</param>
        /// <param name="x">x souradnice vyriznuteho obdelniku</param>
        /// <param name="y">y souradnice vyriznuteho obdelniku</param>
        /// <param name="w">sirka vyryznuteho obdelniku</param>
        /// <param name="h">vyska vyriznuteho obdelniku</param>
        [DllImport("Gfxlib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void clipImage(void* ptr, int x, int y, int w, int h);
    }
}
