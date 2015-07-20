using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GfxlibWrapper
{
    //definuje typ podle velikosti barevneho kanalu (Int32 - 8-bitovy kanal / Int64 - 16-bitovy kanal)
    using IntPx = Int32;
    //definuje typ podle velikosti barevneho kanalu pro samostatne kanaly
    using IntChannel = Byte;
    using System.IO;

    /// <summary>
    /// trida reprezentujici obrazek jako bitmapu
    /// </summary>
    unsafe public class Picture
    {
        /// <summary>
        /// sirka v pixelech
        /// </summary>
        private int width = 0;
        /// <summary>
        /// vyska v pixelech
        /// </summary>
        private int height = 0;
        /// <summary>
        /// rozliseni na ose x
        /// </summary>
        private double xResolution = 0;
        /// <summary>
        /// rozliseni na ose y
        /// </summary>
        private double yResolution = 0;
        /// <summary>
        /// jednotky pro rozliseni
        /// </summary>
        private Units resolutionUnits;
        /// <summary>
        /// pole obsahujici informace o jednotlivych pixelech
        /// </summary>
        private IntPx* pixelData = null;
        /// <summary>
        /// nazev souboru pro nacteni
        /// </summary>
        private String filename = null;
        /// <summary>
        /// ukazatel na objekt C++ tridy Magick::Image
        /// </summary>
        private void* imagePtr = null;

        /// <summary>
        /// prevede objekt tridy String na pole znaku a prida terminator
        /// </summary>
        /// <param name="str">objekt tridy String k prevedeni</param>
        /// <returns>pole charu ukoncene terminatorem</returns>
        private char[] stringToCharArray(String str)
        {
            return (str + '\0').ToCharArray();
        }

        /// <summary>
        /// nastavi vsechny atribury tridy obsahujici informaci o obrazku (vyska/sirka v px, dpi...)
        /// </summary>
        private void setData()
        {
            int unitType;
            width = GfxlibCommunicator.getImageWidth(imagePtr);
            height = GfxlibCommunicator.getImageHeight(imagePtr);
            xResolution = GfxlibCommunicator.getImageXResolution(imagePtr, &unitType);
            yResolution = GfxlibCommunicator.getImageYResolution(imagePtr, &unitType);
            resolutionUnits = unitType == 1 ? Units.In : Units.Cm;
        }

        /// <summary>
        /// nastavi pametove limity knihovny Magick++
        /// </summary>
        static Picture()
        {
            GfxlibCommunicator.setResourceLimits();
        }

        /// <summary>
        /// vytvori bitmapu o zadane velikosti
        /// </summary>
        /// <param name="width">sirka v pixelech</param>
        /// <param name="height">vyska v pixelech</param>
        public Picture(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new PictureCreationFailureException(width, height);
            this.width = width;
            this.height = height;
            try
            {
                imagePtr = GfxlibCommunicator.createImage(width, height);  //vytvoreni instance tridy Magick::Image
                pixelData = GfxlibCommunicator.getPixelDataPtr(imagePtr);  //ziskani pole pixelu
            }
            catch (SEHException)
            {
                Destroy();  //dealokuje pamet
                int errorCode = Marshal.GetExceptionCode();  //ziskani chyboveho kodu
                if (errorCode == (int)GfxlibErrors.OutOfMemory)
                    throw new OutOfMemoryException();
                throw new PictureCreationFailureException(width, height);
            }
        }

        /// <summary>
        /// nastavi jmeno vstupniho souboru
        /// </summary>
        /// <param name="filename">jmeno vstupniho souboru</param>
        public Picture(String filename)
        {
            this.filename = filename;
        }

        /// <summary>
        /// zjisti z obrazku dulezita data (vysku/sirku v px, dpi...) bez jeho kompletniho nacteni
        /// </summary>
        public void Ping()
        {
            if (imagePtr != null)
                throw new PictureAlreadyCreatedException();
            try
            {
                imagePtr = GfxlibCommunicator.pingImage(stringToCharArray(filename));  //ziskani instance tridy Magick::Image pingnutim souboru
                setData();  //nastaveni atributu
                GfxlibCommunicator.deleteImage(imagePtr);  //opetovne odstraneni instance
                imagePtr = null;
            }
            catch (SEHException)
            {
                int errorCode = Marshal.GetExceptionCode();  //ziskani chyboveho kodu
                if (errorCode == (int)GfxlibErrors.PictureWrongFormat)
                    throw new PictureWrongFormatException(filename);
                throw new PictureLoadFailureException(filename);
            }
        }

        /// <summary>
        /// nacte bitmapu ze zadaneho souboru
        /// </summary>
        public void Load()
        {
            if (imagePtr != null)
                throw new PictureAlreadyCreatedException();
            try
            {
                imagePtr = GfxlibCommunicator.loadImage(stringToCharArray(filename));  //ziskani instance tridy Magick::Image nactnim souboru
                setData();  //nastaveni potrebnych atributu
                pixelData = GfxlibCommunicator.getPixelDataPtr(imagePtr);  //ziskani pole pixelu
            }
            catch (SEHException)
            {
                int errorCode = Marshal.GetExceptionCode();  //ziskani chyboveho kodu
                if (errorCode == (int)GfxlibErrors.PictureLoadFailure)
                    throw new PictureLoadFailureException(filename);
                else if (errorCode == (int)GfxlibErrors.PictureWrongFormat)
                    throw new PictureWrongFormatException(filename);
                else
                {
                    Destroy();  //dealokace pameti pri chybe vznikle jinde nez pri nacitani souboru
                                //(pri chybe pri nacitani se o dealokaci stara sama funkce loadImage)
                    if (errorCode == (int)GfxlibErrors.OutOfMemory)
                        throw new OutOfMemoryException();
                    throw new PictureCreationFailureException(width, height);
                }
            }
        }

        /// <summary>
        /// vraci true, pokud je obrazek nacten nebo vytvoren, jinak false
        /// </summary>
        /// <returns>true, pokud je obrazek nacten nebo vytvoren, jinak false</returns>
        public bool IsCreated()
        {
            return imagePtr != null;
        }
        
        /// <summary>
        /// vraci true, pokud ma objekt nastaven cestu k nejakemu souboru, jinak false
        /// </summary>
        /// <returns>true, pokud ma objekt nastaven cestu k nejakemu souboru, jinak false</returns>
        public bool HasFilenameSet()
        {
            return filename != null;
        }

        /// <summary>
        /// pokud je nastavena cesta k nejakemu souboru, vraci hash teto cesty jako Stringu, jinak vraci hash kod tridy Object
        /// </summary>
        /// <returns>hash kod</returns>
        public override int GetHashCode()
        {
            if (filename != null)
                return filename.GetHashCode();
            return base.GetHashCode();
        }

        /// <summary>
        /// pokud je nastavena cesta k nejakemu souboru, vraci tuto cestu, jinak vraci "..." 
        /// </summary>
        /// <returns>cesta k souboru, pokud je nastavena, jinak "..."</returns>
        public override string ToString()
        {
            if (filename != null)
                return filename;
            return "...";
        }

        /// <summary>
        /// pokud je nastavena cesta k nejakemu souboru, porovna tuto cestu s cestou obrazku v parametru jako String, jinak pouziva base.Equals
        /// </summary>
        /// <param name="obj">objekt k porovnani</param>
        /// <returns>pri shode true, jinak false</returns>
        public override bool Equals(object obj)
        {
            if (filename != null)
                return filename.Equals(((Picture)obj).filename);
            return base.Equals(obj);
        }

        /// <summary>
        /// getter sirky v pixelech
        /// </summary>
        /// <returns>sirka bitmapy v pixelech</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetWidth()
        {
            return width;
        }

        /// <summary>
        /// getter vysky v pixelech
        /// </summary>
        /// <returns>vyska bitmapy v pixelech</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHeight()
        {
            return height;
        }

        /// <summary>
        /// vrati DPI na ose x
        /// </summary>
        /// <returns>dpi na ose x</returns>
        public double GetXDpi()
        {
            return UnitConverter.GetUnitsFromIn(xResolution, resolutionUnits);
        }

        /// <summary>
        /// vrati DPI na ose y
        /// </summary>
        /// <returns>dpi na ose y</returns>
        public double GetYDpi()
        {
            return UnitConverter.GetUnitsFromIn(yResolution, resolutionUnits);
        }

        /// <summary>
        /// nastavi DPI na obou osach
        /// </summary>
        /// <param name="xDpi">DPI na ose x</param>
        /// <param name="yDpi">DPI na ose y</param>
        public void SetDpi(double xDpi, double yDpi)
        {
            xResolution = xDpi;
            yResolution = yDpi;
            resolutionUnits = Units.In;
        }

        /// <summary>
        /// nastavi DPCM na obou osach
        /// </summary>
        /// <param name="xDpcm">DPCM na ose x</param>
        /// <param name="yDpcm">DPCM na ose y</param>
        public void SetDpcm(double xDpcm, double yDpcm)
        {
            xResolution = xDpcm;
            yResolution = yDpcm;
            resolutionUnits = Units.Cm;
        }

        /// <summary>
        /// ulozi bitmapu
        /// </summary>
        /// <param name="saveFilename">nazev vystupniho souboru</param>
        public void Save(String saveFilename)
        {
            try
            {
                GfxlibCommunicator.setImageResolution(imagePtr, xResolution, yResolution, resolutionUnits == Units.In ? 1 : 2);  //nastaveni rozliseni vystupniho obrazku
                GfxlibCommunicator.saveImage(imagePtr, stringToCharArray(saveFilename));
            }
            catch (SEHException)
            {
                throw new PictureSaveFailureException(saveFilename);
            }
        }

        /// <summary>
        /// nastavi hodnotu konkretniho pixelu
        /// </summary>
        /// <param name="x">x souradnice</param>
        /// <param name="y">y souradnice</param>
        /// <param name="pxValue">hodnota pixelu ve formatu ORGB, kde O je pruhlednost (opacity),
        /// cim je vetsi, tim je pixel pruhlednejsi, pro IntPx = Int32 je hodnota 4 bytová - napr. 0x00FF0000 je cervena</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixel(int x, int y, IntPx pxValue)
        {
            pixelData[y * width + x] = pxValue;
        }

        /// <summary>
        /// nastavi hodnotu konkretniho pixelu
        /// </summary>
        /// <param name="x">x souradnice</param>
        /// <param name="y">y souradnice</param>
        /// <param name="red">hodnota cerveneho kanalu (0 - 255)</param>
        /// <param name="green">hodnota zeleneho kanalu (0 - 255)</param>
        /// <param name="blue">hodnota modreho kanalu (0 - 255)</param>
        public void SetPixel(int x, int y, IntChannel red, IntChannel green, IntChannel blue)
        {
            Int16 multiplier = sizeof(IntChannel) == 1 ? 1 : 256;
            IntChannel* pxPtr = (IntChannel*)&pixelData[y * width + x];
            pxPtr[0] = (IntChannel)(blue * multiplier);
            pxPtr[1] = (IntChannel)(green * multiplier);
            pxPtr[2] = (IntChannel)(red * multiplier);
            pxPtr[3] = 0;
        }

        /// <summary>
        /// vrati hodnotu konkretniho pixelu
        /// </summary>
        /// <param name="x">x souradnice</param>
        /// <param name="y">y souradnice</param>
        /// <returns>hodnota pixelu na danych souradnicich</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntPx GetPixel(int x, int y)
        {
            return pixelData[y * width + x];
        }
        
        /// <summary>
        /// zmeni velikost bitmapy
        /// </summary>
        /// <param name="newWidth">nova sirka</param>
        /// <param name="newHeight">nova vyska</param>
        /// <param name="filterType">typ filteru</param>
        public void Resize(int newWidth, int newHeight, FilterType filterType)
        {
            if (newWidth <= 0 || newHeight <= 0)
                throw new PictureResizeFailureException(newWidth, newHeight);
            try
            {
                GfxlibCommunicator.resizeImage(imagePtr, newWidth, newHeight, filterType.filterNum);
                width = newWidth;
                height = newHeight;
                pixelData = GfxlibCommunicator.getPixelDataPtr(imagePtr);  //ziskani pole pixelu obrazku s novou velikosti
            }
            catch (SEHException)
            {
                int errorCode = Marshal.GetExceptionCode();
                if (errorCode == (int)GfxlibErrors.PictureCreationFailure)
                    throw new PictureCreationFailureException(newWidth, newHeight);
                if (errorCode == (int)GfxlibErrors.OutOfMemory)
                    throw new OutOfMemoryException();
                throw new PictureResizeFailureException(newWidth, newHeight);
            }
        }

        /// <summary>
        /// orizne obrazek na zadanou velikost
        /// </summary>
        /// <param name="x">pocatecni x souradnice oriznuteho obrazku</param>
        /// <param name="y">pocatecni y souradnice oriznuteho obrazku</param>
        /// <param name="newWidth">sirka oriznuteho obrazku</param>
        /// <param name="newHeight">vyska oriznuteho obrazku</param>
        public void Clip(int x, int y, int newWidth, int newHeight)
        {
            if (newWidth <= 0 || newHeight <= 0)
                throw new PictureClipFailureException(newWidth, newHeight);
            try
            {
                GfxlibCommunicator.clipImage(imagePtr, x, y, newWidth, newHeight);
                width = newWidth;
                height = newHeight;
                pixelData = GfxlibCommunicator.getPixelDataPtr(imagePtr);  //ziskani pole pixelu noveho, oriznuteho obrazku
            }
            catch (SEHException)
            {
                int errorCode = Marshal.GetExceptionCode();
                if (errorCode == (int)GfxlibErrors.PictureCreationFailure)
                    throw new PictureCreationFailureException(newWidth, newHeight);
                if (errorCode == (int)GfxlibErrors.OutOfMemory)
                    throw new OutOfMemoryException();
                throw new PictureClipFailureException(newWidth, newHeight);
            }
        }

        /// <summary>
        /// znici obrazek a dealokuje pamet, kterou vyuziva
        /// </summary>
        public void Destroy()
        {
            if (imagePtr != null)
                GfxlibCommunicator.deleteImage(imagePtr);
            imagePtr = null;
            pixelData = null;
            width = 0;
            height = 0;
            xResolution = 0;
            yResolution = 0;
        }

        /// <summary>
        /// destruktor, ktery pouze vola metodu Destroy
        /// </summary>
        ~Picture()
        {
            Destroy();
        }

        /// <summary>
        /// zkopiruje sloupec pixelu z jinoho obrazku do tohoto
        /// </summary>
        /// <param name="source">zdrojovy obrazek</param>
        /// <param name="sourceColumn">index zdrojoveho sloupce</param>
        /// <param name="destColumn">index ciloveho sloupce tohoto obrazku</param>
        /// <param name="destYBegin">index pocatecni y souradnice, od ktere se zacne sloupec kopirovat do tohoto obrazku</param>
        public void CopyColumn(Picture source, int sourceColumn, int destColumn, int destYBegin)
        {
            for (int i = 0; i < source.height; i++)
                SetPixel(destColumn, i + destYBegin, source.GetPixel(sourceColumn, i));
        }

        /// <summary>
        /// zkopiruje radek pixelu z jineho obrazku do tohoto
        /// </summary>
        /// <param name="source">zdrojovy obrazek</param>
        /// <param name="sourceRow">index zdrojoveho radku</param>
        /// <param name="destRow">index ciloveho radku tohoto obrazku</param>
        /// <param name="destXBegin">index pocatecni x souradnice, od ktere se zacne radek kopirovat do tohoto obrazku</param>
        public void CopyRow(Picture source, int sourceRow, int destRow, int destXBegin)
        {
            for (int i = 0; i < source.width; i++)
                SetPixel(i + destXBegin, destRow, source.GetPixel(i, sourceRow));
        }
    }
}
