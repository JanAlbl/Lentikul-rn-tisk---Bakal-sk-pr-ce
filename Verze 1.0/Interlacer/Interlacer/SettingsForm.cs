using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Interlacer
{
    /// <summary>
    /// Formulář pro zobrazení nastavení aplikace.
    /// </summary>
    public partial class SettingsForm : Form
    {
        /// <summary>
        /// Odkaz na rodičovský formulář
        /// </summary>
        private MainForm parent;
        /// <summary>
        /// instance tridy Settings s nastavenim aplikace
        /// </summary>
        private Settings settings;

        /// <summary>
        /// inicializacni konstruktor
        /// </summary>
        /// <param name="parent">instance hlavniho formulare, ze ktereho je tento otevren</param>
        /// <param name="settings">instance tridy Settings s nastavevnim aplikace</param>
        public SettingsForm(MainForm parent, Settings settings)
        {
            Localization.changeCulture();
            
            InitializeComponent();

            this.parent = parent;
            this.settings = settings;
        }

        /// <summary>
        /// Metoda, která se zavolá po načtení formuláře.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsForm_Load(object sender, EventArgs e)
        {            
            setOptions();
        }

        /// <summary>
        /// Metoda, která se zavolá po stisku tlačítka apply.
        /// Aktualizuje provedené změny ve formuláři.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applyButton_Click(object sender, EventArgs e)
        {
            applyChangedOptions();
        }

        /// <summary>
        /// aplikuje aktulani nastaveni
        /// </summary>
        private void applyChangedOptions()
        {
            Localization.currentLanguage = ((StringValuePair<String>)languageCombobox.SelectedItem).value;
            Localization.changeCulture();
            changeLanguage();
            settings.SetSelectedLanguageIndex(languageCombobox.SelectedIndex);
            settings.SetSelectedUnitsIndex(comboBox1.SelectedIndex);
            settings.SetSelectedResolutionUnitsIndex(comboBox2.SelectedIndex);
            parent.ApplySettings();
            setOptions();
        }

        /// <summary>
        /// Všem komponentám nastaví aktuální nastavení
        /// </summary>
        private void setOptions(){
            languageCombobox.Items.Clear();
            languageCombobox.Items.Add(settings.GetSettingOptions().languageOptions[0]);
            languageCombobox.Items.Add(settings.GetSettingOptions().languageOptions[1]);

            if (comboBox1.Items.Count == 0 && comboBox2.Items.Count == 0)
            {
                comboBox1.Items.Add("");
                comboBox1.Items.Add("");
                comboBox1.Items.Add("");

                comboBox2.Items.Add("");
                comboBox2.Items.Add("");
            }
            
            comboBox1.Items[0] = settings.GetSettingOptions().unitsOptions[0];
            comboBox1.Items[1] = settings.GetSettingOptions().unitsOptions[1];
            comboBox1.Items[2] = settings.GetSettingOptions().unitsOptions[2];            

            comboBox2.Items[0] = settings.GetSettingOptions().resolutionUnitsOptions[0];
            comboBox2.Items[1] = settings.GetSettingOptions().resolutionUnitsOptions[1];            

            comboBox1.SelectedIndex = settings.GetSelectedUnitsIndex();
            comboBox2.SelectedIndex = settings.GetSelectedResolutionUnitsIndex();
            languageCombobox.SelectedIndex = settings.GetSelectedLanguageIndex();
        }

        /// <summary>
        /// Metoda, která se zavolá po stisku tlačítka OK.
        /// Provede se to samé, jako při stisku tlačítka apply,
        /// po provedení se však formulář zavře.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
    
        private void okButton_Click(object sender, EventArgs e)
        {
            applyChangedOptions();
            this.Close();
        }

        /// <summary>
        /// Metoda pro změnu jazyka
        /// </summary>
        private void changeLanguage()
        {
            Localization.iterateOverControls(this, Localization.resourcesSettings);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
