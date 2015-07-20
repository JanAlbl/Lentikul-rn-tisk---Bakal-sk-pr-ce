using GfxlibWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlacer
{
    /// <summary>
    /// trida obsahujici parametry prokladani
    /// </summary>
    public class InterlacingData : IProcessData
    {
        /// <summary>
        /// sirka vystupniho obrazku v aktualne nastavenych jednotkach
        /// </summary>
        private double width;
        /// <summary>
        /// vyska vystupniho obrazku v aktualne nastavenych jednotkach
        /// </summary>
        private double height;
        /// <summary>
        /// tiskove rozliseni vystupniho obrazku v aktualne nastavenych jednotkach
        /// </summary>
        private double pictureResolution;
        /// <summary>
        /// hustota cocek lentikularni desky v aktualne nastavenych jednotkach
        /// </summary>
        private double lenticuleDensity;
        /// <summary>
        /// filtr pro prvotni zmenu velikosti vstupnich obrazku
        /// </summary>
        private FilterType initialResizeFilter;
        /// <summary>
        /// filtr pro finalni prevzorkovani vystupniho obrazku
        /// </summary>
        private FilterType finalResampleFilter;
        /// <summary>
        /// indikator, zda ma byt zachovat pomer stran pri zmene jednoho rozmeru
        /// </summary>
        private bool keepAspectRatio = false;
        /// <summary>
        /// delkove jednotky
        /// </summary>
        private Units units = Units.In;
        /// <summary>
        /// jednotky rozliseni (Units.In je DPI, Units.Cm je DPCM, pripadne Units.Mm je DPMM)
        /// </summary>
        private Units resolutionUnits = Units.In;
        /// <summary>
        /// smer prokladani (horizontalni nebo vertikalni)
        /// </summary>
        private Direction direction = Direction.Vertical;

        /// <summary>
        /// nastavi delkove jednotky a prepocita hodnoty
        /// </summary>
        /// <param name="units">nove jednotky</param>
        public void SetUnits(Units units)
        {
            width = UnitConverter.Transfer(width, this.units, units);
            height = UnitConverter.Transfer(height, this.units, units);
            this.units = units;
        }

        /// <summary>
        /// vrati aktualne nastavene jednotky
        /// </summary>
        /// <returns>aktualne nastavene jednotky</returns>
        public Units GetUnits()
        {
            return units;
        }

        /// <summary>
        /// nastavi jednotky rozliseni a prepocita hodnoty (Units.In - DPI, Units.Cm - DPCM)
        /// </summary>
        /// <param name="units">nove jednotky rozliseni (Units.In - DPI, Units.Cm - DPCM)</param>
        public void SetResolutionUnits(Units units)
        {
            //pouziva UnitConverter.Transfer s prohozenymi jednotkami (viz komentar nad deklaraci tridy UnitConverter)
            pictureResolution = UnitConverter.Transfer(pictureResolution, units, this.resolutionUnits);
            lenticuleDensity = UnitConverter.Transfer(lenticuleDensity, units, this.resolutionUnits);
            this.resolutionUnits = units;
        }

        /// <summary>
        /// vrati aktualne nastavene jednotky rozliseni (Units.In - DPI, Units.Cm - DPCM)
        /// </summary>
        /// <returns>aktualne nastavene jednotky rozliseni (Units.In - DPI, Units.Cm - DPCM)</returns>
        public Units GetResolutionUnits()
        {
            return resolutionUnits;
        }

        /// <summary>
        /// vraci sirku vystupniho obrazku v palcich
        /// </summary>
        /// <returns>sirka vystupniho obrazkuv palcich</returns>
        public double GetInchWidth()
        {
            return UnitConverter.GetInFromUnits(width, units);
        }

        /// <summary>
        /// vraci vysku vystupniho obrazku v palcich
        /// </summary>
        /// <returns>vyska vystupniho obrzaku v palcich</returns>
        public double GetInchHeight()
        {
            return UnitConverter.GetInFromUnits(height, units);
        }

        /// <summary>
        /// nastavi sirku vystupniho obrazku v aktualne nastavenych
        /// pokud je atribut keepAspektRatio nastaven na true, provede prepocet vysky
        /// </summary>
        /// <param name="width">nova sirka vystupniho obrazku v aktualne nastavenych jednotkach</param>
        public void SetWidth(double width)
        {
            if (keepAspectRatio && this.width > 0.00001)
            {
                double newWidth = width;
                double ratio = newWidth / this.width;
                this.width = newWidth;
                height *= ratio;
            }
            else
                this.width = width;
        }

        /// <summary>
        /// vrati sirku vystupniho obrazku v aktualne nastavenych jednotkach
        /// </summary>
        /// <returns>sirka vystupniho obrazku v aktualne nastavenych jednotkach</returns>
        public double GetWidth()
        {
            return width;
        }

        /// <summary>
        /// nastavi vysku vystupniho obrazku v aktualne nastavenych
        /// pokud je atribut keepAspektRatio nastaven na true, provede prepocet sirky
        /// </summary>
        /// <param name="height">nova vyska vystupniho obrazku v aktualne nastavenych jednotkach</param>
        public void SetHeight(double height)
        {
            if (keepAspectRatio && this.height > 0.00001)
            {
                double newHeight = height;
                double ratio = newHeight / this.height;
                this.height = newHeight;
                width *= ratio;
            }
            else
                this.height = height;
        }

        /// <summary>
        /// vrati vysku vystupniho obrazku v aktualne nastavenych jednotkach
        /// </summary>
        /// <returns>vyska vystupniho obrazku v aktualne nastavenych jednotkach</returns>
        public double GetHeight()
        {
            return height;
        }
        
        /// <summary>
        /// vrati tiskove rozliseni vystupniho obrazku v DPI
        /// </summary>
        /// <returns>tiskove rozliseni vystupniho obrazku v DPI</returns>
        public double GetDPI()
        {
            return pictureResolution / UnitConverter.GetInFromUnits(1, resolutionUnits);
        }

        /// <summary>
        /// nastavi tiskove rozliseni vystupniho obrazku v aktualne nastavenych jednotkach rozliseni
        /// </summary>
        /// <param name="resolution">nove tiskove rozliseni vystupniho obrazku v aktualne nastavenych jednotkach rozliseni</param>
        public void SetPictureResolution(double resolution)
        {
            pictureResolution = resolution;
        }

        /// <summary>
        /// vrati tiskove rozliseni vystupniho obrazku v aktualne nastavenych jednotkach rozliseni
        /// </summary>
        /// <returns>tiskove rozliseni vystupniho obrazku v aktualne nastavenych jednotkach rozliseni</returns>
        public double GetPictureResolution()
        {
            return pictureResolution;
        }

        /// <summary>
        /// vrati hustotu cocek lentikularni desky v LPI
        /// </summary>
        /// <returns>hustota cocek lentikularni desky v LPI</returns>
        public double GetLPI()
        {
            return lenticuleDensity / UnitConverter.GetInFromUnits(1, resolutionUnits);
        }

        /// <summary>
        /// nastavi hustotu cocek lentikularni desky v aktualne nastavenych jednotkach rozliseni
        /// </summary>
        /// <param name="density">nova hustota cocek lentikularni desky v aktualne nastavenych jednotkach rozliseni</param>
        public void SetLenticuleDensity(double density)
        {
            lenticuleDensity = density;
        }

        /// <summary>
        /// vrati hustotu cocek lentikularni desky v aktualne nastavenych jednotkach rozliseni
        /// </summary>
        /// <returns>hustota cocek lentikularni desky v aktualne nastavenych jednotkach rozliseni</returns>
        public double GetLenticuleDensity()
        {
            return lenticuleDensity;
        }
        
        /// <summary>
        /// vraci aktualne nastaveny filtr pro prvotni zmenu velikosti vstupnich obrazku
        /// </summary>
        /// <returns>aktualne nastaveny filtr pro prvotni zmenu velikosti vstupnich obrazku</returns>
        public FilterType GetInitialResizeFilter()
        {
            return initialResizeFilter;
        }

        /// <summary>
        /// nastavi filtr pro prvotni zmenu velikosti vstupnich obrazku
        /// </summary>
        /// <param name="filter">novy filtr pro prvnotni zmenu velikosti vstupnich obrazku</param>
        public void SetInitialResizeFilter(FilterType filter)
        {
            initialResizeFilter = filter;
        }

        /// <summary>
        /// vrati filtr pro finalni prevzorkovani vystupniho obrazku
        /// </summary>
        /// <returns>filtr pro finalni prevzorkovani vystupniho obrazku</returns>
        public FilterType GetFinalResampleFilter()
        {
            return finalResampleFilter;
        }

        /// <summary>
        /// nastavi filtr pro finalni prevzorkovani vystupniho obrazku
        /// </summary>
        /// <param name="filter">novy filtr pro finalni prevzorkovani vystupniho obrazku</param>
        public void SetFinalResampleFilter(FilterType filter)
        {
            finalResampleFilter = filter;
        }

        /// <summary>
        /// nastavi, zda ma byt zapnuta funkce pro zachovani pomeru stran
        /// </summary>
        /// <param name="keepAspectRatio">true pro zapnuti, false pro vypnuti funkce pro zachovana pomeru stran</param>
        public void KeepAspectRatio(bool keepAspectRatio)
        {
            this.keepAspectRatio = keepAspectRatio;
        }
        
        /// <summary>
        /// vraci true, pokud je zapnuta funkce pro zachovani pomeru stran, jinak false
        /// </summary>
        /// <returns>true, pokud je zapnuta funkce pro zachovani pomeru stran, jinak false</returns>
        public bool getKeepAspectRatio()
        {
            return this.keepAspectRatio;
        }

        /// <summary>
        /// nastavi smer prokladani
        /// </summary>
        /// <param name="direction">novy smer prokladani</param>
        public void SetDirection(Direction direction)
        {
            this.direction = direction;
        }

        /// <summary>
        /// vrati aktualne nastaveny smer prokladani
        /// </summary>
        /// <returns>aktualne nastaveny smer prokladani</returns>
        public Direction GetDirection()
        {
            return direction;
        }
    }
}
