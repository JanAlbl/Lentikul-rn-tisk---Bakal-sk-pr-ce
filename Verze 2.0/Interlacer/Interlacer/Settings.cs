using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GfxlibWrapper;
using System.IO;
using System.Windows.Forms;

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
            Properties.Settings.Default.selectedLanguageIndex = 0;
            Properties.Settings.Default.selectedUnitsIndex = 0;
            Properties.Settings.Default.selectedResolutionUnitsIndex = 0; 
            /*selectedLanguageIndex = 0;
            selectedUnitsIndex = 0;
            selectedResolutionUnitsIndex = 0;*/
        }

        /// <summary>
        /// ulozi aktualni nastaveni do souboru
        /// </summary>
        /// <param name="filename">nazev souboru, do ktereho bude nastaveni ulozeno</param>
        public void Save(TreeView driveTree)
        {
            saveTreeState(driveTree.Nodes);
            Properties.Settings.Default.expandedNodes = expandedNodes;
            Properties.Settings.Default.Save();
            /*using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine(selectedLanguageIndex);
                sw.WriteLine(selectedUnitsIndex);
                sw.WriteLine(selectedResolutionUnitsIndex);
            }*/
        }

        String expandedNodes = "";
        String[] nodesToExpand;
        int nodesToExpandIndex = 0;
        private void saveTreeState(TreeNodeCollection treeNodeCollection)
        {
            foreach (TreeNode child in treeNodeCollection)
            {
                if (child.IsExpanded)
                {
                    expandedNodes += child.Name + "|";
                    saveTreeState(child.Nodes);
                }
            }
        }

        private void loadTreeState(TreeNodeCollection treeNodeCollection)
        {

            foreach (TreeNode child in treeNodeCollection)
            {
                if (nodesToExpand[nodesToExpandIndex].Equals(child.Name))
                {
                    
                    child.Expand();
                    nodesToExpandIndex += 1;
                    loadTreeState(child.Nodes);
                }
            }
        }

        /// <summary>
        /// nacte nastaveni ze souboru
        /// </summary>
        /// <param name="filename">soubor, ze ktereho ma byt nastaveni nacteno</param>
        public void Load(TreeView driveTree)
        {
            nodesToExpand = (Properties.Settings.Default.expandedNodes).Split('|');
            loadTreeState(driveTree.Nodes);

            selectedLanguageIndex = Properties.Settings.Default.selectedLanguageIndex;
            selectedUnitsIndex = Properties.Settings.Default.selectedUnitsIndex;
            selectedResolutionUnitsIndex = Properties.Settings.Default.selectedResolutionUnitsIndex;
            /*using (StreamReader sr = new StreamReader(filename))
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
            }*/
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
            Properties.Settings.Default.selectedUnitsIndex = index;
            this.selectedUnitsIndex = index;
        }

        /// <summary>
        /// vrati aktualne vybrany index nastaveni delkovych jednotek 
        /// </summary>
        /// <returns>aktualne vybrany index nastaveni delkovych jednotek</returns>
        public int GetSelectedUnitsIndex()
        {
            return Properties.Settings.Default.selectedUnitsIndex;
            //return selectedUnitsIndex;
        }

        /// <summary>
        /// nastavi index nastaveni jednotek pro rozliseni
        /// </summary>
        /// <param name="index">novy index nastaveni jednotek pro rozliseni</param>
        public void SetSelectedResolutionUnitsIndex(int index)
        {
            Properties.Settings.Default.selectedResolutionUnitsIndex = index;
            this.selectedResolutionUnitsIndex = index;
        }

        /// <summary>
        /// vrati aktualne vybrany index nastaveni jednotek pro rozliseni
        /// </summary>
        /// <returns>aktualne vybrany index nastaveni jednotek pro rozliseni</returns>
        public int GetSelectedResolutionUnitsIndex()
        {
            return Properties.Settings.Default.selectedResolutionUnitsIndex;
            //return selectedResolutionUnitsIndex;
        }

        /// <summary>
        /// nastavi index nastaveni jazyka
        /// </summary>
        /// <param name="index">novy index nastaveni jazyka</param>
        public void SetSelectedLanguageIndex(int index)
        {
            Properties.Settings.Default.selectedLanguageIndex = index;
            this.selectedLanguageIndex = index;
        }

        /// <summary>
        /// vrati aktualne vybrany index nastaveni jazyka
        /// </summary>
        /// <returns>aktualne vybrany index nastaveni jazyka</returns>
        public int GetSelectedLanguageIndex()
        {
            return Properties.Settings.Default.selectedLanguageIndex; 
            //return selectedLanguageIndex;
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
