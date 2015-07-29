using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GfxlibWrapper;

namespace Interlacer
{
    /// <summary>
    /// struktura, ktera obsahuje vsechny moznosti nastaveni
    /// </summary>
    public struct SettingOptions
    {
        /// <summary>
        /// moznosti nastaveni delkovych jednotek
        /// </summary>
        public List<StringValuePair<Units>> unitsOptions;
        /// <summary>
        /// moznosti nastaveni jednotek pro obrazove rozliseni a hustotu cocek
        /// </summary>
        public List<StringValuePair<Units>> resolutionUnitsOptions;
        /// <summary>
        /// moznosti nastaveni jazyka
        /// </summary>
        public List<StringValuePair<String>> languageOptions;
    }
}
