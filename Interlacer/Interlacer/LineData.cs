using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using GfxlibWrapper;

namespace Interlacer
{
    /// <summary>
    /// Třída, která v sobě drží všechny potřebná data o pasovacích značkách
    /// </summary>
    public class LineData : IProcessData
    {
        /// <summary>
        ///  šírka čáry na lentikuli kolik je obrázků
        /// </summary>
        private int lineThickness;
        
        /// <summary>
        /// barva pasovacích značek
        /// </summary>
        private Color lineColor;

        /// <summary>
        /// pozadí vedle pasovacích značek
        /// </summary>
        private Color backgroundColor;

        /// <summary>
        ///  šířka rámečku čár
        /// </summary>
        private double frameWidth;

        /// <summary>
        /// odsazení obrázku od pasovacích značek
        /// </summary>
        private double indent;

        /// <summary>
        /// na jakých pozicích se mají čáry vykreslit
        /// </summary>
        private bool left, top, right, bottom;

        /// <summary>
        /// je čára zarovnaná na střed
        /// </summary>
        private bool centerPosition;

        /// <summary>
        /// v jakých jednotkách jsou míry uvedeny
        /// </summary>
        private Units units = Units.In;

        /// <summary>
        /// Nastaví jednotky pro pasovací značky
        /// </summary>
        /// <param name="units">Do jakých jednotek se mají hodnoty převést</param>
        public void SetUnits(Units units)
        {
            frameWidth = UnitConverter.Transfer(frameWidth, this.units, units);
            indent = UnitConverter.Transfer(indent, this.units, units);
            this.units = units;
        }

        /// <summary>
        /// Vrátí jednotky pasovacích čar
        /// </summary>
        /// <returns>Jednotky pasovacích čar</returns>
        public Units GetUnits()
        {
            return this.units;
        }

        /// <summary>
        /// Vrátí šírku pasovacích čar, kolik zabírá sloupečků pod lentikulí
        /// </summary>
        /// <returns>Šírku pasovacích čar, kolik zabírá sloupečků pod lentikulí</returns>
        public int GetLineThickness()
        {
            return lineThickness;
        }

        /// <summary>
        /// Nastaví šírku pasovacích čar, kolik zabírá jedna pasovací značka sloupečků obrázku pod lentikulí
        /// </summary>
        /// <param name="lineThickness">Nastaví šírku pasovacích čar, kolik zabírá jedna pasovací značka sloupečků obrázku pod lentikulí</param>
        public void SetLineThickness(int lineThickness)
        {
            this.lineThickness = lineThickness;
        }

        /// <summary>
        /// Vrátí barvu pasovacích čar
        /// </summary>
        /// <returns>Vrátí barvu pasovacích čar</returns>
        public Color GetLineColor()
        {
            return this.lineColor;
        }

        /// <summary>
        /// Nastaví barvu pasovacích čar
        /// </summary>
        /// <param name="lineColor">Barva pasovacích čar</param>
        public void SetLineColor(Color lineColor)
        {
            this.lineColor = lineColor;
        }

        /// <summary>
        /// Vrátí pozadí vedle pasovacíc čar
        /// </summary>
        /// <returns>Vrátí pozadí vedle pasovacíc čar</returns>
        public Color GetBackgroundColor()
        {
            return this.backgroundColor;
        }

        /// <summary>
        /// Nastaví pozadí vedle pasovacích čar
        /// </summary>
        /// <param name="backgroundColor">Barva pozadí vedle pasovacích čar</param>
        public void SetBackgroundColor(Color backgroundColor)
        {
            this.backgroundColor = backgroundColor;
        }

        /// <summary>
        /// Vrátí šířka rámečku pasovacích značek v palcích
        /// </summary>
        /// <returns>Vrátí šířka rámečku pasovacích značek v palcích</returns>
        public double GetFrameInchWidth()
        {
            return UnitConverter.GetInFromUnits(frameWidth, units);
        }

        /// <summary>
        /// Nastaví šířku rámečku pasovacích značek
        /// </summary>
        /// <param name="frameWidth">Šířka rámečku pasovacích značek</param>
        public void SetFrameWidth(double frameWidth)
        {
            this.frameWidth = frameWidth;
        }

        /// <summary>
        /// Vrátí šířku rámčku pasovacích značke v nastavených jednotkách
        /// </summary>
        /// <returns>Vrátí šířku rámčku pasovacích značke v nastavených jednotkách</returns>
        public double GetFrameWidth()
        {
            return frameWidth;
        }

        /// <summary>
        /// Vrátí šířku odsazení pasovacích značek od obrázku v palcích
        /// </summary>
        /// <returns>Vrátí šířku odsazení pasovacích značek od obrázku v palcích</returns>
        public double GetInchIndent()
        {
            return UnitConverter.GetInFromUnits(indent, units);
        }

        /// <summary>
        /// Nastaí šířku odsazení pasovacích značek od obrázku v nastavených jednotkách
        /// </summary>
        /// <param name="indent">Šířku odsazení pasovacích značek od obrázku v nastavených jednotkách</param>
        public void SetIndent(double indent)
        {
            this.indent = indent;
        }

        /// <summary>
        /// Vrátí šířku odsazení pasovacích značek od obrázku
        /// </summary>
        /// <returns>Vrátí šířku odsazení pasovacích značek od obrázku</returns>
        public double GetIndent()
        {
            return indent;
        }

        /// <summary>
        /// Vrátí true, pokud se mají pasovací značky vykreslit nalevo od obrázku
        /// </summary>
        /// <returns>Vrátí true, pokud se mají pasovací znakčy vykreslit nalevo od obrázku</returns>
        public bool GetLeft()
        {
            return this.left;
        }

        /// <summary>
        /// Nastaví zda se mají vykreslit pasovacích značek nalevo vedle obrázku
        /// </summary>
        /// <param name="left">Nastavení vykreslení pasovacích značek nalevo vedle obrázku</param>
        public void SetLeft(bool left)
        {
            this.left = left;
        }

        /// <summary>
        /// Vrátí true pokud se pasovací značky mají vykreslit nahoře nad obrázkem
        /// </summary>
        /// <returns>Vrátí true pokud se pasovací značky mají vykreslit nahoře nad obrázkem</returns>
        public bool GetTop()
        {
            return this.top;
        }

        /// <summary>
        /// Nastaví  zda se mají vykreslit pasovacích značek nahoře nad obrázkem
        /// </summary>
        /// <param name="top">Nastaví vykreslení pasovacích značek nahoře nad obrázkem</param>
        public void SetTop(bool top)
        {
            this.top = top;
        }

        /// <summary>
        /// Vrátí true pokud se pasovací značky mají vykreslit napravo vedel obrázku
        /// </summary>
        /// <returns>Vrátí true pokud se pasovací značky mají vykreslit napravo vedel obrázku</returns>
        public bool GetRight()
        {
            return this.right;
        }

        /// <summary>
        /// Nastaví zda se mají vykreslit pasovacích čar napravo od obrázku
        /// </summary>
        /// <param name="right">Nastavení vykreslení pasovacích čar napravo od obrázku</param>
        public void SetRight(bool right)
        {
            this.right = right;
        }

        /// <summary>
        /// Vrátí true pokud se pasovací značky mají vykreslit pod obrázkem
        /// </summary>
        /// <returns>Vrátí true pokud se pasovací značky mají vykreslit pod obrázkem</returns>
        public bool GetBottom()
        {
            return this.bottom;
        }

        /// <summary>
        /// Nastaví zda se mají vykreslit pasovací čary pod obrázkem
        /// </summary>
        /// <param name="bottom">Nastavení vykreslení pasovacích čar pod obrázkem</param>
        public void SetBottom(bool bottom)
        {
            this.bottom = bottom;
        }

        /// <summary>
        /// Vrátí true, pokud se pasovací značka vykresluje na středu lentikule
        /// </summary>
        /// <returns>Vrátí true, pokud se pasovací značka vykresluje na středu lentikule</returns>
        public bool GetCenterPosition()
        {
            return this.centerPosition;
        }

        /// <summary>
        /// Nastaví pozici pasovacích značek na střed lentikule
        /// </summary>
        /// <param name="centerPosition">Nastaví pozici pasovacích značek na střed lentikule</param>
        public void SetCenterPosition(bool centerPosition)
        {
            this.centerPosition = centerPosition;
        }

    }
}
