using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GfxlibWrapper
{
    /// <summary>
    /// vyjimka pro chybu pri nacteni obrazku
    /// </summary>
    class PictureLoadFailureException : Exception
    {
        /// <summary>
        /// nazev souboru daneho obrazku
        /// </summary>
        public readonly String filename;

        /// <summary>
        /// inicializujici konstruktor
        /// </summary>
        /// <param name="filename">nazev souboru daneho obrazku</param>
        public PictureLoadFailureException(String filename)
        {
            this.filename = filename;
        }
    }

    /// <summary>
    /// vyjimka pro chybu pri ulozeni obrazku
    /// </summary>
    class PictureSaveFailureException : Exception
    {
        /// <summary>
        /// nazev souboru daneho obrazku
        /// </summary>
        public readonly String filename;
        
        /// <summary>
        /// inicializujici konstruktor
        /// </summary>
        /// <param name="filename">nazev souboru daneho obrazku</param>
        public PictureSaveFailureException(String filename)
        {
            this.filename = filename;
        }
    }

    /// <summary>
    /// vyjimka pro chyby pri praci z obrazky
    /// </summary>
    abstract class PictureProcessException : Exception { }

    /// <summary>
    /// vyjimka pro chybu pri vytvareni obrazku
    /// </summary>
    class PictureCreationFailureException : PictureProcessException
    {
        /// <summary>
        /// sirka chteneho obrazku
        /// </summary>
        public readonly int width;
        /// <summary>
        /// vyska chteneho obrazku
        /// </summary>
        public readonly int height;

        /// <summary>
        /// inicializujici konstruktor
        /// </summary>
        /// <param name="width">sirka chteneho obrazku</param>
        /// <param name="height">vyska chteneho obrazku</param>
        public PictureCreationFailureException(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }

    /// <summary>
    /// vyjimka pro chybu pri zmene velikosti obrazku
    /// </summary>
    class PictureResizeFailureException : PictureProcessException 
    {
        /// <summary>
        /// nova sirka obrazku
        /// </summary>
        public readonly int width;
        /// <summary>
        /// nova vyska obrazku
        /// </summary>
        public readonly int height;

        /// <summary>
        /// inicializujici konstruktor
        /// </summary>
        /// <param name="width">nova sirka obrazku</param>
        /// <param name="height">nova vyska obrazku</param>
        public PictureResizeFailureException(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }

    /// <summary>
    /// vyjimka pro chybu pri orezavani obrazku
    /// </summary>
    class PictureClipFailureException : PictureProcessException
    {
        /// <summary>
        /// nova sirka obrazku
        /// </summary>
        public readonly int width;
        /// <summary>
        /// nova vyska obrazku
        /// </summary>
        public readonly int height;
        
        /// <summary>
        /// inicializujici konstruktor
        /// </summary>
        /// <param name="width">nova sirka obrazku</param>
        /// <param name="height">nova vyska obrazku</param>
        public PictureClipFailureException(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }

    /// <summary>
    /// vyjimka pro pripad, kdy format nacitaneho obrazku je chybny
    /// </summary>
    class PictureWrongFormatException : Exception
    {
        /// <summary>
        /// nazev souboru obrazku
        /// </summary>
        public readonly String filename;

        /// <summary>
        /// inicializujici konstruktor
        /// </summary>
        /// <param name="filename">nazev souboru obrazku</param>
        public PictureWrongFormatException(String filename)
        {
            this.filename = filename;
        }
    }

    /// <summary>
    /// vyjimka pro pripad, kdy se nekdo pokusi nad instanci tridy Picture zavolat Load nebo Ping, pokud jiz obrazek byl vytvoren nebo nacten metodou Load
    /// </summary>
    class PictureAlreadyCreatedException : Exception { }
}
