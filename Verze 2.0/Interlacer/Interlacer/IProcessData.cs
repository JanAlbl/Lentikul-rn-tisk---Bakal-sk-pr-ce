using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GfxlibWrapper;

namespace Interlacer
{
    /// <summary>
    /// interface pro tridy LineData a InterlacingData
    /// </summary>
    public interface IProcessData
    {
        /// <summary>
        /// metoda pro nastaveni jednotek
        /// </summary>
        /// <param name="units">nove jednotky</param>
        void SetUnits(Units units);

        /// <summary>
        /// metoda pro zjisteni aktualne nastavenych jednotek
        /// </summary>
        /// <returns>aktualne nastavene jednotky</returns>
        Units GetUnits();
    }
}
