using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using GfxlibWrapper;
using System.Globalization;
using System.Threading;

namespace Interlacer
{
    /// <summary>
    /// trida obsahujici parametry prokladani a pasovacich znacek v podobe instanci trid InterlacingData a Line data
    /// stara se o nacitani a ukladani projektu
    /// </summary>
    public class ProjectData
    {
        /// <summary>
        /// Zde se uchovávají data pro prokládání obrázků
        /// </summary>
        private InterlacingData interlacingData = new InterlacingData();
        
        /// <summary>
        /// Data pro práci s pasovacími značkami 
        /// </summary>
        private LineData lineData = new LineData();

        /// <summary>
        /// Vrátí data pro prokládání obrázků
        /// </summary>
        /// <returns>Vrátí data pro prokládání obrázků</returns>
        public InterlacingData GetInterlacingData()
        {
            return interlacingData;
        }
        
        /// <summary>
        /// Vrátí data pro práci s pasovacími značkami
        /// </summary>
        /// <returns>Vrátí data pro práci s pasovacími značkami</returns>
        public LineData GetLineData()
        {
            return lineData;
        }
        
        /// <summary>
        /// Uloží projekt do filename
        /// </summary>
        /// <param name="filename">Cesta s názvem souboru kam se má soubor uložit</param>
        /// <param name="picturesPaths">Cesty obrázků které se uloží do souboru</param>
        public void Save(String filename, List<String> picturesPaths)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");  //nastaveni kultury na ceskou, aby se hodnoty ukladaly vzdy s desetinou carkou
            try
            {
                String pathFile = "NUMBER_OF_PICTURES\t" + picturesPaths.Count + Environment.NewLine;   //do souboru se uloží počet obrázků
                for (int i = 0; i < picturesPaths.Count; i++)
                    pathFile = pathFile + "PATH_" + i + "\t" + picturesPaths[i] + Environment.NewLine;
                
                File.WriteAllText(filename, InterlacingDataToString() + LineDataToString() + pathFile); // uložení samotného souboru s daty
            }
            catch{
                Localization.changeCulture();  //vraceni zpet na kultury, ktera je aktualne nastavena
                throw new Exception(string.Format(Localization.resourcesStrings.GetString("imageSaveError2"), filename));
            }
           
            Localization.changeCulture();  //vraceni zpet na kultury, ktera je aktualne nastavena
        }

        /// <summary>
        /// Načte a zvaliduje soubor, pokud se soubor nepodaří načít vyhodí vyjímku.
        /// Načtená data se uloží do proměných interlacingData a lineData, pokud nedošlo k chybě.
        /// Vrátí List cest obrázků, které se načetly ze souboru
        /// </summary>
        /// <param name="filename">Soubor, který se má načíst</param>
        /// <returns>List cest obrázků, které se načetly ze souboru</returns>
        public List<String> Load(String filename)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");  //nastaveni kultury na ceskou, aby se hodnoty nacetly vzdy spravne
            Dictionary<string, string> dictionary = new Dictionary<string, string>();   // slovník kam se uloží data ze souboru
            List<String> picsPath;
            try
            {
                String pom = File.ReadAllText(filename);

                string[] words = pom.Split('\n');
                foreach (string word in words)  // zde se nahrávají data do slovníku dictionary
                {
                    string[] line = word.Split('\t');
                    if (line.Length > 1)
                        dictionary.Add(line[0], line[1].Trim());
                    else
                        dictionary.Add(line[0], "");
                }

                picsPath = getListPathPictures(dictionary);     //načtení listu cest obrázků
            }
           
            catch
            {
                Localization.changeCulture();  //vraceni zpet na kultury, ktera je aktualne nastavena
                throw new Exception(string.Format(Localization.resourcesStrings.GetString("imageLoadError"), filename));
            }

            // zde se zvaliduje soubor zda není poškozen nebo si s ním někdo nehrál a uloží data do LineData a InterlacingData
            if (validateLoadDictionary(dictionary))
            {
                setLineData(dictionary);
                setInterlacingData(dictionary);
            }
            else
            {
                Localization.changeCulture();  //vraceni zpet na kultury, ktera je aktualne nastavena
                throw new Exception(string.Format(Localization.resourcesStrings.GetString("configFileCorruptError"), filename));
            }
            
            Localization.changeCulture();  //vraceni zpet na kultury, ktera je aktualne nastavena
            return picsPath;
        }
        
        /// <summary>
        /// Z dictonary vezme cesty obrázků a uloží je do listu, který pak vrátí
        /// </summary>
        /// <param name="dictionary">Slovník ve kterém jsou uloženy cesty obrázků</param>
        /// <returns>list cest obrázků načtených ze slovníku</returns>
        private  List<String> getListPathPictures(Dictionary<string, string> dictionary){
            List<String> picsPath = new List<string>();
            if (dictionary.ContainsKey("NUMBER_OF_PICTURES") && IsNumeric(dictionary["NUMBER_OF_PICTURES"]))
            {
                for (int i = 0; i < Convert.ToInt32(dictionary["NUMBER_OF_PICTURES"]); i++)
                {
                    if (dictionary.ContainsKey("PATH_" + i))
                    {
                        picsPath.Add(dictionary["PATH_" + i]);
                    }
                }
            }
            
            return picsPath;
        }

        /// <summary>
        /// Zvaliduje zda se všechna potřebná data uložila do slovníku, pokud ne vráti false
        /// </summary>
        /// <param name="dictionary">Slovník, který se má zvalidovat</param>
        /// <returns>Vrátí true, pokud nebyly nalezeny žádné chyby</returns>
        private bool validateLoadDictionary(Dictionary<string, string> dictionary)
        {
            if (
                    !dictionary.ContainsKey("UNITS_INTERLACING")
                 || !dictionary.ContainsKey("RESOLUTION_UNITS")
                 || !dictionary.ContainsKey("WIDTH")
                 || !dictionary.ContainsKey("HEIGHT")
                 || !dictionary.ContainsKey("PICURE_RESOLUTION")
                 || !dictionary.ContainsKey("LENTICULE_DENSITY")
                 || !dictionary.ContainsKey("DIRECTION")
                 || !dictionary.ContainsKey("INITIAL_RESIZE_FILTER")
                 || !dictionary.ContainsKey("FINAL_RESAMPLE_FILTER")
                 || !dictionary.ContainsKey("UNITS_LINE")
                 || !dictionary.ContainsKey("LINE_COLOR")
                 || !dictionary.ContainsKey("BACKGROUND_COLOR")
                 || !dictionary.ContainsKey("FRAME_WIDTH")
                 || !dictionary.ContainsKey("INDENT")
                 || !dictionary.ContainsKey("LEFT")
                 || !dictionary.ContainsKey("TOP")
                 || !dictionary.ContainsKey("RIGHT")
                 || !dictionary.ContainsKey("BOTTOM")
                 || !dictionary.ContainsKey("CENTER_POSITION")
                )
                return false;
            if (!validateLoadUnits(dictionary))
                return false;
            if (!validateLoadFilters(dictionary))
                return false;
            if (!validateLoadNumbers(dictionary))
                return false;
            return true;
        }

        /// <summary>
        /// Otestuje zda se dá object převést na číslo
        /// </summary>
        /// <param name="Expression">Objekt, který je testován</param>
        /// <returns>Vrátí true pokud je možné převést objekt na číslo</returns>
        private static bool IsNumeric(object Expression)
        {
            double retNum;
            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        /// <summary>
        /// Otestuje zda jsou ve slovníku správně uloženy čísla potřebných dat
        /// </summary>
        /// <param name="dictionary">Slovník naplněný daty</param>
        /// <returns>Vrátí true pokud nebyla nalezena žádná chyby</returns>
        private bool validateLoadNumbers(Dictionary<string, string> dictionary)
        {
            if (!IsNumeric(dictionary["WIDTH"])
                || !IsNumeric(dictionary["HEIGHT"])
                || !IsNumeric(dictionary["PICURE_RESOLUTION"])
                || !IsNumeric(dictionary["LENTICULE_DENSITY"])
                || !IsNumeric(dictionary["BACKGROUND_COLOR"])
                || !IsNumeric(dictionary["INDENT"])
                || !IsNumeric(dictionary["FRAME_WIDTH"])
                || !IsNumeric(dictionary["LINE_COLOR"]))
                return false;
            return true;
        }
        
        /// <summary>
        /// Zvaliduje potřebné filry zda nejsou poškozeny a jsou správně nahrány ve slovníku
        /// </summary>
        /// <param name="dictionary">Slovník, který je testován</param>
        /// <returns>Vrátí true pokud nebyla nalezena žádná chyba</returns>
        private bool validateLoadFilters(Dictionary<string, string> dictionary)
        {
            if (!dictionary["INITIAL_RESIZE_FILTER"].Equals("1")
                && !dictionary["INITIAL_RESIZE_FILTER"].Equals("3")
                && !dictionary["INITIAL_RESIZE_FILTER"].Equals("10")
                && !dictionary["INITIAL_RESIZE_FILTER"].Equals("22"))
                return false;
            if (!dictionary["FINAL_RESAMPLE_FILTER"].Equals("1")
                  && !dictionary["FINAL_RESAMPLE_FILTER"].Equals("3")
                  && !dictionary["FINAL_RESAMPLE_FILTER"].Equals("10")
                  && !dictionary["FINAL_RESAMPLE_FILTER"].Equals("22"))
                return false;
            return true;
        }

        /// <summary>
        /// Zvaliduje zda jsou ve slovníku načteny správně jednotky
        /// </summary>
        /// <param name="dictionary">Slovník, který je testován</param>
        /// <returns>Vráti true pokud nebyla nalezena žádná chyba</returns>
        private bool validateLoadUnits(Dictionary<string, string> dictionary)
        {
            if (!dictionary["UNITS_INTERLACING"].Equals("Mm")
                && !dictionary["UNITS_INTERLACING"].Equals("Cm")
                && !dictionary["UNITS_INTERLACING"].Equals("In"))
                return false;
            if (!dictionary["UNITS_LINE"].Equals("Mm")
                && !dictionary["UNITS_LINE"].Equals("Cm")
                && !dictionary["UNITS_LINE"].Equals("In"))
                return false;
            if (!dictionary["RESOLUTION_UNITS"].Equals("Mm")
                && !dictionary["RESOLUTION_UNITS"].Equals("Cm")
                && !dictionary["RESOLUTION_UNITS"].Equals("In"))
                return false;
            return true;
        }

        /// <summary>
        /// Nastaví pozice pasovacích značek do lineDat podle načtených dat ze slovníku
        /// </summary>
        /// <param name="dictionary">Slovník ve kterém jsou uloženy pozice pasovacích značek</param>
        private void setLinePosition(Dictionary<string, string> dictionary)
        {
            if (dictionary["LEFT"].Equals("True"))
                this.lineData.SetLeft(true);
            else
                this.lineData.SetLeft(false);
            if (dictionary["TOP"].Equals("True"))
                this.lineData.SetTop(true);
            else
                this.lineData.SetTop(false);
            if (dictionary["RIGHT"].Equals("True"))
                this.lineData.SetRight(true);
            else
                this.lineData.SetRight(false);
            if (dictionary["BOTTOM"].Equals("True"))
                this.lineData.SetBottom(true);
            else
                this.lineData.SetBottom(false);
            if (dictionary["CENTER_POSITION"].Equals("True"))
                this.lineData.SetCenterPosition(true);
            else
                this.lineData.SetCenterPosition(false);
        }

        /// <summary>
        /// Nastaví data pro LineData načtených ze slovníku
        /// </summary>
        /// <param name="dictionary">Slovník, ve kterém jsou nahrány data o pasovacích značkách</param>
        private void setLineData(Dictionary<string, string> dictionary)
        {
            switch (dictionary["UNITS_LINE"])
            {
                case "Mm":
                    this.lineData.SetUnits(Units.Mm);
                    break;
                case "Cm":
                    this.lineData.SetUnits(Units.Cm);
                    break;
                case "In":
                    this.lineData.SetUnits(Units.In);
                    break;
                default: break;
            }

            if (dictionary["FRAME_WIDTH"] != null)
                this.lineData.SetFrameWidth(Convert.ToDouble(dictionary["FRAME_WIDTH"]));
            if (dictionary["INDENT"] != null)
                this.lineData.SetIndent(Convert.ToDouble(dictionary["INDENT"]));

            if (dictionary["LINE_COLOR"] != null)
                this.lineData.SetLineColor(Color.FromArgb(Convert.ToInt32(dictionary["LINE_COLOR"])));
            if (dictionary["BACKGROUND_COLOR"] != null)
                this.lineData.SetBackgroundColor(Color.FromArgb(Convert.ToInt32(dictionary["BACKGROUND_COLOR"])));

            setLinePosition(dictionary);
        }

        /// <summary>
        ///  Podle dat ze slovníku načte potřebné filty a uloží je do InterlacingData
        /// </summary>
        /// <param name="dictionary">Slovník, který obsahuje potřebná data pro načtení filtrů</param>
        private void setFilter(Dictionary<string, string> dictionary)
        {
            switch (dictionary["INITIAL_RESIZE_FILTER"])
            {
                case "3":
                    this.interlacingData.SetInitialResizeFilter(FilterType.Triangle);
                    break;
                case "1":
                    this.interlacingData.SetInitialResizeFilter(FilterType.None);
                    break;
                case "22":
                    this.interlacingData.SetInitialResizeFilter(FilterType.Lanczos);
                    break;
                case "10":
                    this.interlacingData.SetInitialResizeFilter(FilterType.Cubic);
                    break;
                default: break;
            }

            switch (dictionary["FINAL_RESAMPLE_FILTER"])
            {
                case "3":
                    this.interlacingData.SetFinalResampleFilter(FilterType.Triangle);
                    break;
                case "1":
                    this.interlacingData.SetFinalResampleFilter(FilterType.None);
                    break;
                case "22":
                    this.interlacingData.SetFinalResampleFilter(FilterType.Lanczos);
                    break;
                case "10":
                    this.interlacingData.SetFinalResampleFilter(FilterType.Cubic);
                    break;
                default: break;
            }
        }

        /// <summary>
        /// Načte ze slovníku data o jednotkách pro prokládání a uloží je do InterlacingData
        /// </summary>
        /// <param name="dictionary">Slovník, který obsahuje data o jednotkách pro interlacing</param>
        private void setInterlacingUnits(Dictionary<string, string> dictionary)
        {
            switch (dictionary["UNITS_INTERLACING"])
            {
                case "Mm":
                    this.interlacingData.SetUnits(Units.Mm);
                    break;
                case "Cm":
                    this.interlacingData.SetUnits(Units.Cm);
                    break;
                case "In":
                    this.interlacingData.SetUnits(Units.In);
                    break;
                default: break;
            }

            switch (dictionary["RESOLUTION_UNITS"])
            {
                case "Mm":
                    this.interlacingData.SetResolutionUnits(Units.Mm);
                    break;
                case "Cm":
                    this.interlacingData.SetResolutionUnits(Units.Cm);
                    break;
                case "In":
                    this.interlacingData.SetResolutionUnits(Units.In);
                    break;
                default: break;
            }
        }

        /// <summary>
        /// Ze slovníku načte data o interlacingu a uloží je do InterlacingData
        /// </summary>
        /// <param name="dictionary">Slovník, ve kterém jsou data o interlacingu</param>
        private void setInterlacingData(Dictionary<string, string> dictionary)
        {
            setInterlacingUnits(dictionary);
            setFilter(dictionary);
            
            bool keepRatio = false;                         
            if (this.interlacingData.getKeepAspectRatio())
                keepRatio = true;
            this.interlacingData.KeepAspectRatio(false); // zachování poměru na false pro snadné uložení dat šířky a výšky
            
            if (dictionary["WIDTH"] != null)
                this.interlacingData.SetWidth(Convert.ToDouble(dictionary["WIDTH"]));
            if (dictionary["HEIGHT"] != null)
                this.interlacingData.SetHeight(Convert.ToDouble(dictionary["HEIGHT"]));
            if (dictionary["PICURE_RESOLUTION"] != null)
                this.interlacingData.SetPictureResolution(Convert.ToDouble(dictionary["PICURE_RESOLUTION"]));
            if (dictionary["LENTICULE_DENSITY"] != null)
                this.interlacingData.SetLenticuleDensity(Convert.ToDouble(dictionary["LENTICULE_DENSITY"]));
            if (dictionary["LENTICULE_DENSITY"] != null)
                this.interlacingData.SetLenticuleDensity(Convert.ToDouble(dictionary["LENTICULE_DENSITY"]));
            if (dictionary["DIRECTION"].Equals("Vertical"))
                this.interlacingData.SetDirection(Direction.Vertical);
            else
                this.interlacingData.SetDirection(Direction.Horizontal);

            this.interlacingData.KeepAspectRatio(keepRatio); // vrátím předešlou hodnotu zachování poměru do interlacingDat
        }
        
        /// <summary>
        /// Převede data o pasovacích značkách do stringové podoby, kterou pak vrátí
        /// </summary>
        /// <returns>Vrátí data o pasovacích značkách převedená na řetězec znaků</returns>
        private String LineDataToString()
        {
            return
                "UNITS_LINE\t" + this.lineData.GetUnits() + Environment.NewLine +
                "LINE_COLOR\t" + this.lineData.GetLineColor().ToArgb() + Environment.NewLine +
                "BACKGROUND_COLOR\t" + this.lineData.GetBackgroundColor().ToArgb() + Environment.NewLine +
                "FRAME_WIDTH\t" + this.lineData.GetFrameWidth() + Environment.NewLine +
                "INDENT\t" + this.lineData.GetIndent() + Environment.NewLine +
                "LEFT\t" + this.lineData.GetLeft() + Environment.NewLine +
                "TOP\t" + this.lineData.GetTop() + Environment.NewLine +
                "RIGHT\t" + this.lineData.GetRight() + Environment.NewLine +
                "BOTTOM\t" + this.lineData.GetBottom() + Environment.NewLine +
                "CENTER_POSITION\t" + this.lineData.GetCenterPosition() + Environment.NewLine;
        }

        /// <summary>
        /// Převede data o samotném interlacingu do stringové podoby, kterou pak vrátí
        /// </summary>
        /// <returns>Vrátí data o interlacingu převedená na řetězec znaků</returns>
        private String InterlacingDataToString()
        {
            return
                "UNITS_INTERLACING\t" + this.interlacingData.GetUnits() + Environment.NewLine +
                "RESOLUTION_UNITS\t" + this.interlacingData.GetResolutionUnits() + Environment.NewLine +
                "WIDTH\t" + this.interlacingData.GetWidth() + Environment.NewLine +
                "HEIGHT\t" + this.interlacingData.GetHeight() + Environment.NewLine +
                "PICURE_RESOLUTION\t" + this.interlacingData.GetPictureResolution() + Environment.NewLine +
                "LENTICULE_DENSITY\t" + this.interlacingData.GetLenticuleDensity() + Environment.NewLine +
                "DIRECTION\t" + this.interlacingData.GetDirection() + Environment.NewLine +
                "INITIAL_RESIZE_FILTER\t" + this.interlacingData.GetInitialResizeFilter().filterNum + Environment.NewLine +
                "FINAL_RESAMPLE_FILTER\t" + this.interlacingData.GetFinalResampleFilter().filterNum + Environment.NewLine;
        }
    }
}
