using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;

namespace Interlacer
{
    /// <summary>
    /// Statická třída sloužící ke změně jazyka
    /// </summary>
    static class Localization
    {
        /// <summary>
        /// Udava, na jaky jazyk je program nastaven
        /// </summary>
        public static string currentLanguage = "cs-CZ";
        /// <summary>
        /// Resource manager pro tridu MainForm, obsahuje vsechny texty a preklady pro nazvy jednotlivych komponent
        /// </summary>
        public static ComponentResourceManager resourcesMain = new ComponentResourceManager(typeof(MainForm));
        /// <summary>
        /// Rseource manager pro tridu SettingsForm, obsahuje vsechny texty a preklady pro nazvy jednotlivych komponent
        /// </summary>
        public static ComponentResourceManager resourcesSettings = new ComponentResourceManager(typeof(SettingsForm));
        /// <summary>
        /// Resource manager slouzici k ulozeni vsech ostatnich textu(chybovych hlasek, tooltipu, polozek comboboxu...) vcetne jejich prekladu
        /// </summary>
        public static ComponentResourceManager resourcesStrings;

        static Localization()
        {
            changeCulture();
        }

        /// <summary>
        /// Meni nastaveni kultury programu podle promenne currentLanguage.
        /// Podle aktualniho nastaveni jazyka vybere, jaky soubor se pouzije pro resource manager.
        /// </summary>
        public static void changeCulture()
        {
            if(currentLanguage == "cs-CZ")
            {
                resourcesStrings = new ComponentResourceManager(typeof(StringRes_CZ));
            }
            else if (currentLanguage == "en")
            {
                resourcesStrings = new ComponentResourceManager(typeof(StringRes_EN));
            }
           
            Thread.CurrentThread.CurrentCulture = new CultureInfo(currentLanguage);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(currentLanguage);
        }

        /// <summary>
        /// Rekurzivne projde vsechny komponenty formu a nastavi jim aktualne pouzivany jazyk
        /// </summary>
        /// /// <param name="res">urcuje, pro jaky formular chceme zmenit jazyk</param>
        /// <param name="parent">Kopmonenta pres kterou se bude iterovat</param>
        public static void iterateOverControls(Control parent, ComponentResourceManager res)
        {

            foreach (Control c in parent.Controls)
            {
                /*tohle tu musi byt, jinak se po volani ApplyResources komponenty vrati na 
                puvodni pozici a velikost a nesouhlasi s aktualni velikosti formulare*/
                int x = c.Location.X;
                int y = c.Location.Y;
                int w = c.Size.Width;
                int h = c.Size.Height;
                res.ApplyResources(c, c.Name, new CultureInfo(Localization.currentLanguage));
                c.SetBounds(x, y, w, h); //nastaveni spravne velikosti a pozice
                if (c.GetType() == typeof(GroupBox))
                {
                    iterateOverControls(c, res);
                }

                if (c.GetType() == typeof(MenuStrip))
                {
                    foreach (ToolStripMenuItem it in ((MenuStrip)c).Items)
                    {
                        res.ApplyResources(it, it.Name, new CultureInfo(Localization.currentLanguage));

                        foreach (ToolStripItem f in it.DropDownItems)
                        {
                            if (typeof(ToolStripSeparator) != f.GetType())
                                res.ApplyResources(f, f.Name, new CultureInfo(Localization.currentLanguage));

                        }
                    }
                }

                else
                {
                    iterateOverControls(c, res);
                }
            }
        }
    }
}
