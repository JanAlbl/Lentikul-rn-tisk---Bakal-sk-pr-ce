using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GfxlibWrapper;
using System.IO;

namespace Interlacer
{
    /// <summary>
    /// obsahuje informace o aktualnim nastaveni
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// informace o vsech moznostech nastaveni
        /// </summary>
        private SettingOptions settingOptions;
        /// <summary>
        /// aktualne vybrany index v nastaveni delkovych jednotek
        /// </summary>
        private int selectedUnitsIndex;
        /// <summary>
        /// aktualne vybrany index v nastaveni jednotek pro rozliseni
        /// </summary>
        private int selectedResolutionUnitsIndex;
        /// <summary>
        /// aktualne vybrany index v nastaveni jazyka
        /// </summary>
        private int selectedLanguageIndex;

        /// <summary>
        /// konstruktor, ktery nastavi moznosti nastaveni
        /// </summary>
        /// <param name="settingOptions">moznosti nastaveni</param>
        public Settings(SettingOptions settingOptions)
        {
            this.settingOptions = settingOptions;
        }

        /// <summary>
        /// nastavi vse na vychozi nastaveni
        /// </summary>
        public void SetToDefault()
        {
            selectedLanguageIndex = 0;
            selectedUnitsIndex = 0;
            selectedResolutionUnitsIndex = 0;
        }

        /// <summary>
        /// ulozi aktualni nastaveni do souboru
        /// </summary>
        /// <param name="filename">nazev souboru, do ktereho bude nastaveni ulozeno</param>
        public void Save(String filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine(selectedLanguageIndex);
                sw.WriteLine(selectedUnitsIndex);
                sw.WriteLine(selectedResolutionUnitsIndex);
            }
        }

        /// <summary>
        /// nacte nastaveni ze souboru
        /// </summary>
        /// <param name="filename">soubor, ze ktereho ma byt nastaveni nacteno</param>
        public void Load(String filename)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                selectedLanguageIndex = int.Parse(sr.ReadLine());  //nateni indexu jazyka
                if (selectedLanguageIndex >= settingOptions.languageOptions.Count)  //kontrola, zda je index v mezich
                    throw new Exception();
                selectedUnitsIndex = int.Parse(sr.ReadLine());  //nacteni indexu delkovych jednotek
                if (selectedUnitsIndex >= settingOptions.unitsOptions.Count)  //kontrola, zda je index v mezich
                    throw new Exception();
                selectedResolutionUnitsIndex = int.Parse(sr.ReadLine());  //nacteni indexu jednotek rozliseni
                if (selectedResolutionUnitsIndex >= settingOptions.resolutionUnitsOptions.Count)  //kontrola, zda je index v mezich
                    throw new Exception();
            }
        }

        /// <summary>
        /// nastavi moznosti nastaveni
        /// </summary>
        /// <param name="settingOptions">moznosti nastaveni</param>
        public void SetSettingOptions(SettingOptions settingOptions)
        {
            this.settingOptions = settingOptions;
        }

        /// <summary>
        /// vrati moznosti nastaveni
        /// </summary>
        /// <returns>moznosti nastaveni</returns>
        public SettingOptions GetSettingOptions()
        {
            return settingOptions;
        }

        /// <summary>
        /// nastavi index nastaveni delkovych jednotek
        /// </summary>
        /// <param name="index">novy index nastaveni delkovych jednotek</param>
        public void SetSelectedUnitsIndex(int index)
        {
            this.selectedUnitsIndex = index;
        }

        /// <summary>
        /// vrati aktualne vybrany index nastaveni delkovych jednotek 
        /// </summary>
        /// <returns>aktualne vybrany index nastaveni delkovych jednotek</returns>
        public int GetSelectedUnitsIndex()
        {
            return selectedUnitsIndex;
        }

        /// <summary>
        /// nastavi index nastaveni jednotek pro rozliseni
        /// </summary>
        /// <param name="index">novy index nastaveni jednotek pro rozliseni</param>
        public void SetSelectedResolutionUnitsIndex(int index)
        {

            this.selectedResolutionUnitsIndex = index;
        }

        /// <summary>
        /// vrati aktualne vybrany index nastaveni jednotek pro rozliseni
        /// </summary>
        /// <returns>aktualne vybrany index nastaveni jednotek pro rozliseni</returns>
        public int GetSelectedResolutionUnitsIndex()
        {
            return selectedResolutionUnitsIndex;
        }

        /// <summary>
        /// nastavi index nastaveni jazyka
        /// </summary>
        /// <param name="index">novy index nastaveni jazyka</param>
        public void SetSelectedLanguageIndex(int index)
        {
            this.selectedLanguageIndex = index;
        }

        /// <summary>
        /// vrati aktualne vybrany index nastaveni jazyka
        /// </summary>
        /// <returns>aktualne vybrany index nastaveni jazyka</returns>
        public int GetSelectedLanguageIndex()
        {
            return selectedLanguageIndex;
        }

        /// <summary>
        /// vrati aktualne vybrane nastaveni delkovych jednotek
        /// </summary>
        /// <returns>aktualne vybrane nastaveni delkovych jednotek</returns>
        public StringValuePair<Units> GetSelectedUnits()
        {
            return settingOptions.unitsOptions[selectedUnitsIndex];
        }

        /// <summary>
        /// vrati aktualne vybrane nastaveni jednotek pro rozliseni
        /// </summary>
        /// <returns>aktualne vybrane nastaveni jednotek pro rozliseni</returns>
        public StringValuePair<Units> GetSelectedResolutionUnits()
        {
            return settingOptions.resolutionUnitsOptions[selectedResolutionUnitsIndex];
        }

        /// <summary>
        /// vrati aktualne vybrane nastaveni jazyka
        /// </summary>
        /// <returns>aktualne vybrane nastaveni jazyka</returns>
        public StringValuePair<String> GetSelectedLanguage()
        {
            return settingOptions.languageOptions[selectedLanguageIndex];
        }
    }
}
