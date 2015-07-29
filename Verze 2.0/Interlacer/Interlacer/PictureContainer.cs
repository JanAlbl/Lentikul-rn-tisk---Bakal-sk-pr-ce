using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GfxlibWrapper;

namespace Interlacer
{
    /// <summary>
    /// trida pro prokladani obrazku
    /// </summary>
    public class PictureContainer
    {
        /*DULEZITE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
         pokud ma metoda na konci nazvu velke V, znamena to, ze je to metoda pouze pro vertikalni prokladani
         pokud ma metoda na konci nazvu velke H, znamena to, ze je to metoda pouze pro horizontalni prokladani
         ----------------------------------------------------------------------------------------------------*/

        /// <summary>
        /// List indexu - klicem je vzdy cesta k souboru, hodnotou je seznam pozic, na kterych se obrazek ma objevit
        /// </summary>
        private List<KeyValuePair<Picture, List<int>>> indexList = new List<KeyValuePair<Picture, List<int>>>();
        /// <summary>
        /// obsahuje informace o vystupnim obrazku
        /// </summary>
        private InterlacingData interlacingData;
        /// <summary>
        /// obsahuje informace o pasovacích značkách
        /// </summary>
        private LineData lineData;
        /// <summary>
        /// pocet obrazku k prolozeni
        /// </summary>
        private int pictureCount;
        /// <summary>
        /// vysledny obrazek
        /// </summary>
        private Picture result = null;
        /// <summary>y
        /// sirka samotneho obrazku bez ramecku pred poslednim resizem
        /// </summary>
        private int preResamplePictureWidth = -1;
        /// <summary>y
        /// vyska samotneho obrazku bez ramecku pred poslednim resizem
        /// </summary>
        private int preResamplePictureHeight = -1;
        /// <summary>
        /// sirka vystupniho obrzazku bez ramecku
        /// </summary>
        private double outputPictureWidth = -1.0;
        /// <summary>
        /// vyska vystupniho obrzazku bez ramecku
        /// </summary>
        private double outputPictureHeight = -1.0;
        /// <summary>
        /// sirka vstupnich obrazku
        /// </summary>
        private int inputPictureWidth = -1;
        /// <summary>
        /// vyska vstupnich obrazku
        /// </summary>
        private int inputPictureHeight = -1;
        /// <summary>
        /// instance progress baru, ktery bude pouzit pro zobrazeni postupu prokladani
        /// </summary>
        private ProgressBar progressBar;

        /// <summary>
        /// konstruktor, ktery z Listu cest k souborum vytvori List indexu
        /// </summary>
        /// <param name="pictures">seznam obrazku, bud nenactenych s nastavenou cestou k souboru nebo vytvorenych programove</param>
        /// <param name="interlacingData">data potrebna k prolozeni</param>
        /// <param name="lineData">data potrebna k pridani pasovacich znacek</param>
        /// <param name="progressBar">instance progress baru, ktery bude pouzit pro zobrazeni postupu prokladani</param>
        public PictureContainer(List<Picture> pictures, InterlacingData interlacingData, LineData lineData, ProgressBar progressBar = null)
        {
            pictureCount = pictures.Count;
            this.interlacingData = interlacingData;
            this.lineData = lineData;
            this.progressBar = progressBar;
            Dictionary<Picture, List<int>> indexTable = new Dictionary<Picture, List<int>>();
            for (int i = 0; i < pictures.Count; i++)  //vytvoreni seznamu indexu
            {
                if (indexTable.ContainsKey(pictures[i]))  //pokud se obrazek jiz vyskytuje ve skovniku...
                {
                    indexTable[pictures[i]].Add(i);  //...je k nemu pridan index
                }
                else  //pokud seznam neobsahuje dany obrazek...
                {
                    indexTable.Add(pictures[i], new List<int>{i});  //...je pridan seznam k danemu obrazku a do nej prvni index
                }
            }
            indexList = indexTable.ToList();  //vytvoreni seznamu ze slovniku
        }

        /// <summary>
        /// nastavi progress bar na vychozi hodnoty
        /// </summary>
        private void resetProgressBar()
        {
            if (progressBar != null)
            {
                progressBar.Maximum = pictureCount + 3;  //jeden krok pro kazdy obrazek + 2 x konecny resize + vykresleni pasovacich znacek
                progressBar.Step = 1;  //velikost kroku
                progressBar.Value = 0;
            }
        }

        /// <summary>
        /// udela jeden krok na progess baru
        /// </summary>
        private void makeProgressBarStep()
        {
            if (progressBar != null)
            {
                progressBar.PerformStep();
                Application.DoEvents();
                progressBar.Refresh();
            }
        }

        /// <summary>
        /// orizne obrazek na sirku nejuzsiho a vysku nejnizsiho, obe hodnoty musi byt nejdrive zjisteny metodou CheckPictures
        /// </summary>
        /// <param name="picture">obrazek k oriznuti</param>
        private void adjustPictureSize(Picture picture)
        {
            if (picture.GetHeight() > inputPictureHeight || picture.GetWidth() > inputPictureWidth)
                picture.Clip(0, 0, inputPictureWidth, inputPictureHeight);
        }

        /// <summary>
        /// zkontroluje vsechny obrazky, pokud maji stejne rozmery, vrati true
        /// pokud nemaji stejne rozmery, vrati false a 
        /// </summary>
        /// <returns>true, pokud jsou obrazky stejne velke, jinak false</returns>
        public bool CheckPictures()
        {
            bool sameSizes = true;
            for (int i = 0; i < indexList.Count; i++)  //cyklus pro kontrolu vsech obrazku
            {
                Picture pic = indexList[i].Key;
                if (!pic.IsCreated())  //pokud obrazek neni vytvoren, je potreba ho pingnout
                    pic.Ping();
                if (i == 0)
                {
                    inputPictureHeight = pic.GetHeight();  //pocatecni nastaveni vysky vstupnich obrazku
                    inputPictureWidth = pic.GetWidth();  //pocatecni nastaveni sirky vstupnich obrazku
                }
                else if (pic.GetHeight() != indexList[i - 1].Key.GetHeight() ||  //kontrola, zda je aktualni obrazek stejne velky
                         pic.GetWidth() != indexList[i - 1].Key.GetWidth())      //jako predchozi obrazek
                {
                    sameSizes = false;
                    if (pic.GetWidth() < inputPictureWidth)  //pokud je sirka mensi nez aktualne nastavena sirka vstupnich obrazku, je zmenena
                        inputPictureWidth = pic.GetWidth();
                    if (pic.GetHeight() < inputPictureHeight)  //pokud je vyska mensi nez aktualne nastavena vyska vstupnich obrazku, je zmenena
                        inputPictureHeight = pic.GetHeight();
                }
            }
            return sameSizes;
        }

        /// <summary>
        /// Vrátí prázdný obrázek, pokud jsou nastaveny pasovací značky připočte jejich šířku či výšku
        /// </summary>
        /// <returns>Vrátí prázdný obrázek, pokud jsou nastaveny pasovací značky připočte jejich šířku či výšku</returns>
        private Picture getPictureForMarkToLine()
        {
  
            int addWidth = 0;
            int addHeight = 0;
            if (lineData == null)
                return new Picture(preResamplePictureWidth, preResamplePictureHeight);
            
            if (lineData.GetLeft())
                addWidth = getAddWidthForLineAndIndent();   //připočtu šířku pasovacích značek vůči zmenšenému obrázku 
            if (lineData.GetRight())
                addWidth = addWidth + getAddWidthForLineAndIndent();  //připočtu výšku pasovacích značek vůči zmenšenému obrázku 
            if (lineData.GetTop())
                addHeight = getAddHeightForLineAndIndent();
            if (lineData.GetBottom())
                addHeight = addHeight + getAddHeightForLineAndIndent();

            return new Picture(preResamplePictureWidth + addWidth, preResamplePictureHeight + addHeight);
        }

        /// <summary>
        ///  Vrací šířku rámečku s odsazením v px pro přidání před finálním resizem vydělený poměrem
        /// </summary>
        /// <param name="ratio">parametr dělení vysledné šířky rámečku</param>
        /// <returns>poměr šířky rámečku / parametr</returns>
        private int getAddSizeForLineAndIndentRatio(double ratio)
        {
            double widthFL = lineData.GetFrameInchWidth() + lineData.GetInchIndent();
            double widthFLpixel = widthFL * interlacingData.GetDPI();
            return (int)(widthFLpixel / ratio);
        }

        /// <summary>
        ///  Vrací šířku rámečku bez odsazením v px pro pridani pred finalnim resizem vydeleny pomerem
        /// </summary>
        /// <param name="ratio">parametr dělení vysledné šířky rámečku</param>
        /// <returns>poměr šířky rámečku / parametr</returns>
        private int getAddSizeForLineRatio(double ratio)
        {
            double widthFL = lineData.GetFrameInchWidth();
            double widthFLpixel = widthFL * interlacingData.GetDPI();
            return (int)(widthFLpixel / ratio);
        }


        /// <summary>
        ///  Vrací šířku postraního rámečku s pomocnými čarami v px pro pridani pred finalnim resizem
        /// </summary>
        /// <returns>šířku rámečku s pomocnými čarami v px</returns>
        private int getAddWidthForLineAndIndent()
        {
            double ratio = outputPictureWidth / (double)(preResamplePictureWidth);
            return getAddSizeForLineAndIndentRatio(ratio);
        }

        /// <summary>
        ///  Vrací šířku postraního rámečku na levé straně s pomocnými čarami v px pro pridani pred finalnim resizem
        /// </summary>
        /// <returns>šířku rámečku s pomocnými čarami v px</returns>
        private int getAddWidthForLineAndIndentLeft()
        {
            if (lineData.GetLeft())
                return this.getAddWidthForLineAndIndent();
            else
                return 0;
        }

        /// <summary>
        /// Vrací výšku horního nebo spodního rámečku s pomocnými čarami v px pro pridani pred finalnim resizem
        /// </summary>
        /// <returns>výšku rámečku s pomocnými čarami v px</returns>
        private int getAddHeightForLineAndIndent()
        {
            double ratio = outputPictureHeight / (double)(preResamplePictureHeight);
            return getAddSizeForLineAndIndentRatio(ratio);
        }

        /// <summary>
        ///  Vrací výšku horního rámečku na levé straně s pomocnými čarami v px pro pridani pred finalnim resizem
        /// </summary>
        /// <returns>výšku rámečku s pomocnými čarami v px</returns>
        private int getAddHeightForLineAndIndentTop()
        {
            if (lineData.GetTop())
                return this.getAddHeightForLineAndIndent();
            else
                return 0;
        }

        /// <summary>
        ///  Vrací výšku čar horního rámečku bez osazení v px pro pridani pred finalnim resizem
        /// </summary>
        /// <returns>výšku rámečku s pomocnými čarami v px</returns>
        private int getAddHeightForLineTop()
        {
            double ratio = outputPictureHeight / (double)(preResamplePictureHeight);
            return getAddSizeForLineRatio(ratio);
        }

        /// <summary>
        ///  Vrací šířku čar na stranách bez odsazení v px pro pridani pred finalnim resizem
        /// </summary>
        /// <returns>výšku rámečku s pomocnými čarami v px</returns>
        private int getAddWidthForLine()
        {
            double ratio = outputPictureWidth / (double)(preResamplePictureWidth);
            return getAddSizeForLineRatio(ratio);
        }

        /// <summary>
        /// metoda, ktera provede prolozeni obrazku a vytvori vystupni obrazek
        /// </summary>
        public void Interlace()
        {
            switch (interlacingData.GetDirection())
            {
                case Direction.Vertical:  //pokud je smer vertikalni...
                    InterlaceV();  //...provede se vertikalni prolozeni
                    break;
                case Direction.Horizontal:  //pokud je smer horizontalni...
                    InterlaceH();  //...provede se horizontalni prolozeni
                    break;
            }
            double pictureResolution = interlacingData.GetPictureResolution();
            switch (interlacingData.GetResolutionUnits())  //nastaveni vystupniho rozliseni v korektnich jednotkach
            {
                case Units.In:
                    result.SetDpi(pictureResolution, pictureResolution);
                    break;
                case Units.Cm:
                    result.SetDpcm(pictureResolution, pictureResolution);
                    break;
                case Units.Mm:
                    result.SetDpcm(pictureResolution * 10, pictureResolution * 10);
                    break;
            }
        }

        /// <summary>
        /// metoda pro vertikalni prokladani
        /// </summary>
        private void InterlaceV()
        {
            if (inputPictureHeight < 0 || inputPictureWidth < 0)  //pokud jsou rozmery vstupnich obrazku zaporne, znamena to, ze metoda CheckPictures nebyla zavolana
                throw new InvalidOperationException("CheckPictures was not called");
            int lenses = (int)(interlacingData.GetInchWidth() * interlacingData.GetLPI());  //pocet cocek na cely obrazek bez desetinne casti
            double lensesError = (interlacingData.GetInchWidth() * interlacingData.GetLPI() - lenses) * pictureCount;  //pocet pixelu, ktere jsou navic v sirce obrazku pred finalnim prevzorkovanim, vlivem zaokrouhlovaci chyby
            double errorRatio = lensesError / ((lenses * pictureCount) + lensesError);  //pomer poctu pixelu, ktere jsou navic a sirky obrazku pred finalnim prevzorkovanim
            outputPictureWidth = interlacingData.GetInchWidth() * interlacingData.GetDPI();  //sirka vystupniho obrazku
            double finalError = outputPictureWidth * errorRatio; //pocet pixelu, ktere jsou navic v sirce vystupniho obrazku
            outputPictureWidth -= finalError;  //odecteni pixelu navic od sirky vystupniho obrazku
            outputPictureHeight = interlacingData.GetInchHeight() * interlacingData.GetDPI();  //vypocet vysky vystupniho obrazku
            preResamplePictureWidth = lenses * pictureCount;  //sirka obrazku pred finalnim prevzorkovanim
            preResamplePictureHeight = inputPictureHeight;  //vyska obrazku pred finalnim prevzorkovanim
            resetProgressBar();
            for (int i = 0; i < indexList.Count; i++)  //cyklus, ktery projde vsechny obrazky a da jejich spravne soupce na spravne misto
            {
                Picture currentPic = indexList[i].Key;  //vytazeni daneho obrazku
                List<int> indexes = indexList[i].Value;  //seznam indexu, na jejichz pozice ma byt vlozen
                bool loadPic = !currentPic.IsCreated();  //zjisti, zda se obrazek ma nacist
                if (loadPic)
                    currentPic.Load();
                adjustPictureSize(currentPic);  //pokud je to potreba, orizne obrazek
                if (result == null)
                    result = getPictureForMarkToLine();  //vytvoreni prazdneho obrazku pro prolozeni s mistem pro pripadne pasovaci znacky
                currentPic.Resize(lenses, preResamplePictureHeight, interlacingData.GetInitialResizeFilter());  //zmeni sirku obrazku na pocet cocek (jedna cocka, jeden sloupec)
                for (int j = 0; j < indexes.Count; j++)  //cyklus, ktery da jednotlive sloupce obrazku na spravne indexy
                {
                    for (int k = 0; k < lenses; k++)  //cyklus, ktery da konkretni sloupce, na spravny dany index
                    {
                        int column = k * pictureCount + (pictureCount - 1 - indexes[j]) + this.getAddWidthForLineAndIndentLeft(); //vypocet indexu sloupce
                        result.CopyColumn(currentPic, k, column, this.getAddHeightForLineAndIndentTop());  //prekopirovani sloupce
                    }
                    makeProgressBarStep();
                }
                if (loadPic)  //pokud byl obrazek nacten v teto metode, je oped smazan
                    currentPic.Destroy();
            }
            drawLines();  //vykresleni pasovacich znacek
            makeProgressBarStep();
            ResizeResultV();  //finalni prevzorkovani
            makeProgressBarStep();
        }

        /// <summary>
        /// metoda pro horizontalni prokladani
        /// </summary>
        private void InterlaceH()
        {
            if (inputPictureHeight < 0 || inputPictureWidth < 0)  //pokud jsou rozmery vstupnich obrazku zaporne, znamena to, ze metoda CheckPictures nebyla zavolana
                throw new InvalidOperationException("CheckPictures was not called");
            int lenses = (int)(interlacingData.GetInchHeight() * interlacingData.GetLPI());  //pocet cocek na cely obrazek bez desetinne casti
            double lensesError = (interlacingData.GetInchHeight() * interlacingData.GetLPI() - lenses) * pictureCount;  //pocet pixelu, ktere jsou navic ve vysce obrazku pred finalnim prevzorkovanim, vlivem zaokrouhlovaci chyby
            double errorRatio = lensesError / ((lenses * pictureCount) + lensesError);  //pomer poctu pixelu, ktere jsou navic a vysky obrazku pred finalnim prevzorkovanim
            outputPictureHeight = interlacingData.GetInchHeight() * interlacingData.GetDPI();  //vyska vystupniho obrazku
            double finalError = outputPictureHeight * errorRatio; //pocet pixelu, ktere jsou navic ve vysce vystupniho obrazku
            outputPictureHeight -= finalError;  //odecteni pixelu navic od vysky vystupniho obrazku
            outputPictureWidth = interlacingData.GetInchWidth() * interlacingData.GetDPI();  //vypocet sirky vystupniho obrazku
            preResamplePictureHeight = lenses * pictureCount;  //vyska obrazku pred finalnim prevzorkovanim
            preResamplePictureWidth = inputPictureWidth;  //sirka obrazku pred finalnim prevzorkovanim
            resetProgressBar();
            for (int i = 0; i < indexList.Count; i++)  //cyklus, ktery projde vsechny obrazky a da jejich spravne radky na spravne misto
            {
                Picture currentPic = indexList[i].Key;  //vytazeni daneho obrazku
                List<int> indexes = indexList[i].Value;  //seznam indexu, na jejichz pozice ma byt vlozen
                bool loadPic = !currentPic.IsCreated();  //zjisti, zda se obrazek ma nacist
                if (loadPic)
                    currentPic.Load();
                adjustPictureSize(currentPic);  //pokud je to potreba, orizne obrazek
                if (result == null)
                    result = getPictureForMarkToLine();  //vytvoreni prazdneho obrazku pro prolozeni s mistem pro pripadne pasovaci znacky
                currentPic.Resize(preResamplePictureWidth, lenses, interlacingData.GetInitialResizeFilter());
                for (int j = 0; j < indexes.Count; j++)  //cyklus, ktery da jednotlive radky obrazku na spravne indexy
                {
                    for (int k = 0; k < lenses; k++)  //cyklus, ktery da konkretni radek, na spravny dany index
                    {
                        int row = k * pictureCount + (pictureCount - 1 - indexes[j]) + this.getAddHeightForLineAndIndentTop(); //vypocet indexu radku
                        result.CopyRow(currentPic, k, row, this.getAddWidthForLineAndIndentLeft());  //prekopirovani radku
                    }
                    makeProgressBarStep();
                }
                if (loadPic)  //pokud byl obrazek nacten v teto metode, je oped smazan
                    currentPic.Destroy();
            }
            drawLines();  //vykresleni pasovacich znacek
            makeProgressBarStep();
            ResizeResultH();  //finalni prevzorkovani
            makeProgressBarStep();
        }

        /// <summary>
        /// Zjistí zda na indexu sloupečku může být vykreslena pasovací značka
        /// </summary>
        /// <param name="index">index sloupečku obrázku</param>
        /// <returns>Vratí true pokud na daném indexu může být vykreslena značka</returns>
        private bool getCanBeLineV(int index)
        {
            if (!lineData.GetCenterPosition())  // pokud čára není zarovnaná na střed lentikule
            {
                // zde se testuje zda je na indexu sloupečku obrázku možné vykreslit čáru
                // getAddWidthForLineAndIndentLeft() % pictureCount - pro zarovnání čar k prvnímu sloupečku obrázku se musí odečíst kolik sloupečku pod letikulí je navíc v levé části obrázku a aby nedošlo k % záporného čísla musím připočíst alespon 2*počet čar
                // celkové číslo % počtem obrázků a zjistím na jakém sloupečku v lentikuli se nacházím, pokud jsem na indexu sloupečku menším než je počet čar mohu zde vykreslit čáru

                if (((index + 2*pictureCount - getAddWidthForLineAndIndentLeft() % pictureCount) % pictureCount) < lineData.GetLineThickness())
                {
                    return true;    // mohu na indexu vykreslit čáru
                }
                else
                    return false;
            }
            else                                // pokud čára je zarovnaná na střed  lentikule
            {
                // zde se testuje zda je na indexu sloupečku obrázku možné vykreslit čáru
                // getAddWidthForLineAndIndentLeft() % pictureCount - pro zarovnání čar k prvnímu sloupečku obrázku se musí odečíst kolik sloupečku pod letikulí je navíc v levé části obrázku a aby nedošlo k % záporného čísla musím připočíst alespon 2*počet čar
                // (pictureCount / 2 - lineData.GetLineThickness() / 2) - odečku velikost posunu čáry od levé strany lentikule a mohu počítat stejným způsobem jako při počítání zarovnání čáry nalevo
                // celkové číslo % počtem obrázků a zjistím na jakém sloupečku v lentikuli se nacházím, pokud jsem na indexu sloupečku menším než je počet čar mohu zde vykreslit čáru

                if (((index + 2*pictureCount - (pictureCount / 2 - lineData.GetLineThickness() / 2)  - getAddWidthForLineAndIndentLeft() % pictureCount) % pictureCount) < lineData.GetLineThickness())
                    return true;    // mohu na indexu vykreslit čáru
                else
                    return false;
            }
        }

        /// <summary>
        /// Zjistí zda na indexu řádku může být vykreslena pasovací značka
        /// </summary>
        /// <param name="index">index řádku obrázku</param>
        /// <returns>Vratí true pokud na daném indexu může být vykreslena značka</returns>
        private bool getCanBeLineH(int index)
        {
            if (!lineData.GetCenterPosition())  //pokud čára není zarovnaná na střed lentikule
            {
                // zde se testuje zda je na indexu řádku obrázku možné vykreslit čáru
                // getAddHeightForLineAndIndentTop() % pictureCount - pro zarovnání čar k prvnímu řádku obrázku se musí odečíst kolik řádku pod letikulí je navíc v horní části obrázku a aby nedošlo k % záporného čísla musím připočíst alespon 2*počet čar
                // celkové číslo % počtem obrázků a zjistím na jakém řádku v lentikuli se nacházím, pokud jsem na indexu řádku menším než je počet čar mohu zde vykreslit čáru
             
                if (((index + 2*pictureCount - getAddHeightForLineAndIndentTop() % pictureCount) % pictureCount) < lineData.GetLineThickness())
                {
                    return true;    // mohu na indexu vykreslit čáru
                }
                else
                    return false;
            }
            else                              // pokud čára není zarovnaná na střed lentikule
            {
                // zde se testuje zda je na indexu řádku obrázku možné vykreslit čáru
                // getAddHeightForLineAndIndentTop() % pictureCount - pro zarovnání čar k prvnímu řádku obrázku se musí odečíst kolik řádku pod letikulí je navíc v horní části obrázku a aby nedošlo k % záporného čísla musím připočíst alespon 2*počet čar
                // (pictureCount / 2 - lineData.GetLineThickness() / 2) - odečku velikost posunu čáry v lentikuli a mohu počítat stejným způsobem
                // celkové číslo % počtem obrázků a zjistím na jakém řádku v lentikuli se nacházím, pokud jsem na indexu řádku menším než je počet čar mohu zde vykreslit čáru

                if (((index + 2*pictureCount - (pictureCount / 2 - lineData.GetLineThickness() / 2) - getAddHeightForLineAndIndentTop() % pictureCount) % pictureCount) < lineData.GetLineThickness())
                    return true;    // mohu na indexu vykreslit čáru
                else
                    return false;
            }
        }

        /// <summary>
        /// Vykreslí vertikální pasovací značky na levé straně obrázku
        /// </summary>
        private void drawLinesLeftV()
        {
            int colorValue;
            if (lineData.GetLeft())
            {
                for (int i = 0; i < this.getAddWidthForLineAndIndent(); i++)    // začínám kreslit od zleva do prava do začátku již proloženého obrázku
                {
                    if (i < this.getAddWidthForLine())                          // testuji zda jsem ještě v prosturu kde se mohou vykreslit pasovací značky a né v odsazení pasovacích značek od obrázku
                    {
                        if (getCanBeLineV(i))                                   // zda může být na daném indexu vykreslena pasovací značka  
                            colorValue = lineData.GetLineColor().ToArgb();      // pokud ano přidám barvu do proměné pro vykreslení pixelu pasovací značky
                        else
                            colorValue = lineData.GetBackgroundColor().ToArgb();    // pokud né přidám barvu do proměné pro vykreslení pixelu pozadí vedle pasovacích značek
                    }
                    else
                        colorValue = Color.White.ToArgb();                      // jsem již za hranicí pasovacích značek a přidám bílou default barvu
                    for (int j = 0; j < result.GetHeight(); j++) {              // obarvím celý sloupeček
                        result.SetPixel(i, j, colorValue);
                    }
                }
            } 
        }

        /// <summary>
        ///  Vykreslí vertikální pasovací značky na pravé straně obrázku
        /// </summary>
        private void drawLinesRightV()
        {
            int colorValue = 0;
            if (lineData.GetRight())
            {
                for (int i = result.GetWidth()-1; i >= (preResamplePictureWidth + this.getAddWidthForLineAndIndentLeft()); i--) // začínám kreslit od posledního sloupečku do prvního sloupečku již vykreslené časti obrázku
                {
                    if (i >= (result.GetWidth() - this.getAddWidthForLine()))       // testuji zda jsem ještě v prosturu kde se mohou vykreslit pasovací značky a né v odsazení pasovacích značek od obrázku
                    {
                        if (getCanBeLineV(i))                                       // zda může být na daném indexu vykreslena pasovací značka  
                            colorValue = lineData.GetLineColor().ToArgb();          // pokud ano přidám barvu do proměné pro vykreslení pixelu pasovací značky
                        else
                            colorValue = lineData.GetBackgroundColor().ToArgb();    // pokud né přidám barvu do proměné pro vykreslení pixelu pozadí vedle pasovacích značek
                    }
                    else
                        colorValue = Color.White.ToArgb();                      // jsem již za hranicí pasovacích značek a přidám bílou default barvu
                    for (int j = 0; j < result.GetHeight(); j++)                // obarvím celý sloupeček
                        result.SetPixel(i, j, colorValue);
                }
            }
        }

        /// <summary>
        ///  Vykreslí vertikální pasovací značky nahoře obrázku
        /// </summary>
        private void drawLinesTopV(){
            int colorValue = 0;
            if (lineData.GetTop())
            {
                int pomLeft = 0;
                int pomRight = 0;
                if (lineData.GetLeft())
                    pomLeft = this.getAddWidthForLine();    // pokud jsou na levé straně už pasovací značky nekreslím je znova a posunu se na indexu začátku nevykreslené části
                if (lineData.GetRight())
                    pomRight = this.getAddWidthForLine();   // pokud jsou na pravé straně už pasovací značky nekreslím je znova a posunu se na indexu konce nevykreslené části

                for (int i = pomLeft; i < result.GetWidth() - pomRight; i++)    // začínám kreslit z leva do prava
                {
                    if (getCanBeLineV(i))                                // zda může být na daném indexu vykreslena pasovací značka  
                        colorValue = lineData.GetLineColor().ToArgb();  // pokud ano přidám barvu do proměné pro vykreslení pixelu pasovací značky
                    else
                        colorValue = lineData.GetBackgroundColor().ToArgb();    // pokud né přidám barvu do proměné pro vykreslení pixelu pozadí vedle pasovacích značek

                    for (int j = 0; j <= getAddHeightForLineTop(); j++)     // barvou obarvním celý sloupeček do odsazení
                        result.SetPixel(i, j, colorValue);

                    colorValue = Color.White.ToArgb();              // jsem již za hranicí pasovacích značek a přidám bílou default barvu

                    for (int j = getAddHeightForLineTop()+1; j < getAddHeightForLineAndIndent(); j++)   // barvou obarvím sloupeček od osazení do začátku již proloženého obrázku
                        result.SetPixel(i, j, colorValue);
                }
            }
        }

        /// <summary>
        ///  Vykreslí vertikální pasovací značky v dolní části obrázku
        /// </summary>
        private void drawLinesBottomV()
        {
            int colorValue = 0;
            if (lineData.GetBottom())
            {
                int pomLeft = 0;
                int pomRight = 0;
                if (lineData.GetLeft())
                    pomLeft = this.getAddWidthForLine();    // pokud jsou na levé straně už pasovací značky nekreslím je znova a posunu se na indexu začátku nevykreslené části
                if (lineData.GetRight())
                    pomRight = this.getAddWidthForLine();   // pokud jsou na pravé straně už pasovací značky nekreslím je znova a posunu se na indexu konce nevykreslené části
                for (int i = pomLeft; i < result.GetWidth() - pomRight; i++)
                {
                    if (getCanBeLineV(i))                                // zda může být na daném indexu vykreslena pasovací značka
                        colorValue = lineData.GetLineColor().ToArgb();      // pokud ano přidám barvu do proměné pro vykreslení pixelu pasovací značky
                    else
                        colorValue = lineData.GetBackgroundColor().ToArgb();    // pokud né přidám barvu do proměné pro vykreslení pixelu pozadí vedle pasovacích značek

                    for (int j = result.GetHeight() - 1; j >= (result.GetHeight() - getAddHeightForLineTop()); j--) // barvou obarvním celý sloupeček do odsazení
                        result.SetPixel(i, j, colorValue);

                    colorValue = Color.White.ToArgb();                             // jsem již za hranicí pasovacích značek a přidám bílou default barvu

                    for (int j = (result.GetHeight() - getAddHeightForLineTop() - 1); j >= (result.GetHeight() - getAddHeightForLineAndIndent()); j--)    // barvou obarvím sloupeček od osazení do začátku již proloženého obrázku
                        result.SetPixel(i, j, colorValue);
                }
            }
        }

        /// <summary>
        ///  Vykreslí horizontální pasovací značky na levé straně obrázku
        /// </summary>
        private void drawLinesLeftH()
        {
            int colorValue;
            if (lineData.GetLeft())
            {

                for (int i = 0; i < result.GetHeight(); i++)            // kreslím postupně od prvního řádku do posledního
                {
                    for (int j = 0; j < this.getAddWidthForLineAndIndent(); j++) // vykresluji řádky od prvního sloupečku obrázku do prvního sloupečku již proloženého obrázku
                    {
                        if (j < this.getAddWidthForLine()) {              // testuji zda jsem ještě v prosturu kde se mohou vykreslit pasovací značky a né v odsazení pasovacích značek od obrázku
                            if (getCanBeLineH(i))                                           // zda na daném indexu řádku může být vykreslena pasovací značka
                                colorValue = lineData.GetLineColor().ToArgb();              // pokud ano přidám barvu do proměné pro vykreslení pixelu pasovací značky
                            else
                                colorValue = lineData.GetBackgroundColor().ToArgb();        // pokud né přidám barvu do proměné pro vykreslení pixelu pozadí vedle pasovacích značek
                            }
                        else
                            colorValue = Color.White.ToArgb();          // jsem již za hranicí pasovacích značek a přidám bílou default barvu
                        result.SetPixel(j, i, colorValue);               // vykreslím celý řádek touto barvou
                    }  
                }
            }
        }

        /// <summary>
        ///  Vykreslí horizontální pasovací značky na pravé straně obrázku
        /// </summary>
        private void drawLinesRightH()
        {
            int colorValue = 0;
            if (lineData.GetRight())
            {
                for (int i = 0; i < result.GetHeight(); i++)        // kreslím postupně od prvního řádku do posledního
                {
                    for (int j = result.GetWidth()-1; j >= (preResamplePictureWidth + this.getAddWidthForLineAndIndentLeft()); j--) // vykresluji řádky od posledního sloupečku obrázku do posledního sloupečku již proloženého obrázku
                    {
                        if (j >= (result.GetWidth() - this.getAddWidthForLine()))   // testuji zda jsem ještě v prosturu kde se mohou vykreslit pasovací značky a né v odsazení pasovacích značek od obrázku
                        {
                            if (getCanBeLineH(i))                                   // zda na daném indexu řádku může být vykreslena pasovací značka
                                colorValue = lineData.GetLineColor().ToArgb();       // pokud ano přidám barvu do proměné pro vykreslení pixelu pasovací značky
                            else
                                colorValue = lineData.GetBackgroundColor().ToArgb();   // pokud né přidám barvu do proměné pro vykreslení pixelu pozadí vedle pasovacích značek
                        }
                        else
                            colorValue = Color.White.ToArgb();          // jsem již za hranicí pasovacích značek a přidám bílou default barvu

                        result.SetPixel(j, i, colorValue);              // vykreslím celý řádek touto barvou
                    }
                        
                }
            }
        }

        /// <summary>
        ///  Vykreslí horizontální pasovací značky v horní části obrázku
        /// </summary>
        private void drawLinesTopH()
        {
            int colorValue = 0;
            if (lineData.GetTop())
            {
                int pomLeft = 0;
                int pomRight = 0;
                if (lineData.GetLeft())
                    pomLeft = this.getAddWidthForLine();    // pokud je na levé straně pasovací značky nekreslím znova a posunu se na indexu začátku nevykreslené části
                if (lineData.GetRight())
                    pomRight = this.getAddWidthForLine();   // pokud je na pravé straně pasovací značky nekreslím znova a posunu se na indexu konce nevykreslené části

                for (int i = 0; i < getAddHeightForLineAndIndentTop(); i++) // kreslím od prvního řádku obrázku do prvního řádku již proloženého obrázku
                {
                     if (i < getAddHeightForLineTop())                       // testuji zda jsem ještě v prosturu kde se mohou vykreslit pasovací značky a né v odsazení pasovacích značek od obrázku
                     {
                         if (getCanBeLineH(i))                                  // zda na daném indexu řádku může být vykreslena pasovací značka
                             colorValue = lineData.GetLineColor().ToArgb();      // pokud ano přidám barvu do proměné pro vykreslení pixelu pasovací značky
                         else
                             colorValue = lineData.GetBackgroundColor().ToArgb();     // pokud né přidám barvu do proměné pro vykreslení pixelu pozadí vedle pasovacích značek
                     }
                     else
                         colorValue = Color.White.ToArgb();                 // jsem již za hranicí pasovacích značek a přidám bílou default barvu
                    
                     for (int j = pomLeft; j < result.GetWidth() - pomRight; j++){      // danou barvou vykreslím doposud nevykreslený řádek
                          result.SetPixel(j, i, colorValue);
                     }
                }
            }
        }

        /// <summary>
        ///  Vykreslí horizontální pasovací značky v dolní části obrázku
        /// </summary>
        private void drawLinesBottomH()
        {
            int colorValue = 0;
            if (lineData.GetBottom())
            {
                int pomLeft = 0;
                int pomRight = 0;
                if (lineData.GetLeft())
                    pomLeft = this.getAddWidthForLine();    // pokud jsou na levé straně pasovací značky nekreslím znova a posunu se na indexu začátku nevykreslené části
                if (lineData.GetRight())
                    pomRight = this.getAddWidthForLine();   // pokud jsou na pravé straně pasovací značky nekreslím znova a posunu se na indexu konec nevykreslené části

                for (int i = result.GetHeight() - 1; i >= (result.GetHeight() - getAddHeightForLineAndIndent()); i--)   // kreslím od posledního řádku obrázku nahoru do začátku dolního řádku již proloženého obrázku
                {
                    if (i > (result.GetHeight() - getAddHeightForLineTop())) {      // testuji zda řádek ještě náleží do rámečku pro vykreslení pasovacích značek
                        if (getCanBeLineH(i))                                       // testuji zda na indexu řádku může být pasovací značka 
                            colorValue = lineData.GetLineColor().ToArgb();
                        else
                            colorValue = lineData.GetBackgroundColor().ToArgb();    // pokud né přidám barvu pozadí
                    }
                    else
                        colorValue = Color.White.ToArgb();                          // barva v odsazení pasovacích značek od obrázku je default bílá

                    for (int j = pomLeft; j < result.GetWidth() - pomRight; j++)    // samotné vykreslení již zjištěné barvy na příslušném pixelu celkového obrázku
                        result.SetPixel(j, i, colorValue);
                }
            }
        }

        /// <summary>
        /// Vykreslí pasovací značky, pokud někde mají být, zavoláním dalších metod
        /// </summary>
        private void drawLines()
        {
            if (lineData != null)
            {
                switch (interlacingData.GetDirection()) // zjistím v jakém směru se provádí prokládání
                {
                    case Direction.Vertical:        // vertikální prokládání 
                        drawLinesLeftV();           // vykreslení pasovacích značek v levé části obrázku
                        drawLinesRightV();          // vykreslení pasovacích značek v pravé části obrázku
                        drawLinesTopV();            // vykreslení pasovacích značek v horní části obrázku
                        drawLinesBottomV();         // vykreslení pasovacích značek v dolní části obrázku
                        break;
                    case Direction.Horizontal:      // horizontální prokládání 
                        drawLinesLeftH();           // vykreslení pasovacích značek v levé části obrázku
                        drawLinesRightH();          // vykreslení pasovacích značek v pravé části obrázku
                        drawLinesTopH();            // vykreslení pasovacích značek v horní části obrázku
                        drawLinesBottomH();         // vykreslení pasovacích značek v dolní části obrázku
                        break;
                }
       
                
            }
        }

        /// <summary>
        /// finalni prevzorkovani pro vertikalni prokladani
        /// </summary>
        private void ResizeResultV()
        {
            int outputHeight = (int)outputPictureHeight;
            if (lineData != null)
            {
                if (lineData.GetTop())
                    outputHeight += (int)((lineData.GetFrameInchWidth() + lineData.GetInchIndent()) * interlacingData.GetDPI());
                if (lineData.GetBottom())
                    outputHeight += (int)((lineData.GetFrameInchWidth() + lineData.GetInchIndent()) * interlacingData.GetDPI());
            }
            int originalWidth = (int)(interlacingData.GetInchWidth() * interlacingData.GetLPI()) * pictureCount;  //sirka obrazku pred finalnim prevzorkovanim
            double widthRatio = outputPictureWidth / originalWidth;  //pomer sirky vystupniho obrazku a sirky obrazku pred finalnim prevzorkovanim
            int outputWidth = (int)(result.GetWidth() * widthRatio);  //vypocet sirky vystupniho obrazku - timto zpusobem se zbavime zaokrouhlovacich chyb, vzniklych pridanim ramecku pro pasovaci znacky
            result.Resize(outputWidth, result.GetHeight(), interlacingData.GetFinalResampleFilter());  //prevzorkovani na finalni sirku
            makeProgressBarStep();
            result.Resize(outputWidth, outputHeight,  //prevzorkovani na finalni vysku
                interlacingData.GetInitialResizeFilter() == FilterType.None ? FilterType.None : FilterType.Triangle);
        }

        /// <summary>
        /// finalni prevzorkovani pro horizontalni prokladani
        /// </summary>
        private void ResizeResultH()
        {
            int outputWidth = (int)outputPictureWidth;
            if (lineData != null)
            {
                if (lineData.GetLeft())
                    outputWidth += (int)((lineData.GetFrameInchWidth() + lineData.GetInchIndent()) * interlacingData.GetDPI());
                if (lineData.GetRight())
                    outputWidth += (int)((lineData.GetFrameInchWidth() + lineData.GetInchIndent()) * interlacingData.GetDPI());
            }
            int originalHeight = (int)(interlacingData.GetInchHeight() * interlacingData.GetLPI()) * pictureCount;  //vyska obrazku pred finalnim prevzorkovanim
            double heightRatio = outputPictureHeight / originalHeight;  //pomer vysky vystupniho obrazku a vysky obrazku pred finalnim prevzorkovanim
            int outputHeight = (int)(result.GetHeight() * heightRatio);  //vypocet vysky vystupniho obrazku - timto zpusobem se zbavime zaokrouhlovacich chyb, vzniklych pridanim ramecku pro pasovaci znacky
            result.Resize(result.GetWidth(), outputHeight, interlacingData.GetFinalResampleFilter());  //prevzorkovani na finalni vysku
            makeProgressBarStep();
            result.Resize(outputWidth, outputHeight,  //prevzorkovani na finalni sirku
                interlacingData.GetInitialResizeFilter() == FilterType.None ? FilterType.None : FilterType.Triangle);
        }

        /// <summary>
        /// vrati vysledny obrazek, pokud byl jiz proveden interlacing
        /// </summary>
        /// <returns>vysledny obrazek</returns>
        public Picture GetResult()
        {
            return result;
        }
    }
}
