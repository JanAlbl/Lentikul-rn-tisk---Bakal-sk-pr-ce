using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GfxlibWrapper
{
    /// <summary>
    /// trida reprezentujici typ filteru pouziteho pro resize
    /// </summary>
    public class FilterType
    {
        /// <summary>
        /// zadny filter - nejblizsi soused
        /// </summary>
        public static readonly FilterType None = new FilterType("Nearest neighbour", 1);
        /// <summary>
        /// trojuhelnikovy filter
        /// </summary>
        public static readonly FilterType Triangle = new FilterType("Triangle", 3);
        /// <summary>
        /// kubicky filter
        /// </summary>
        public static readonly FilterType Cubic = new FilterType("Cubic", 10);
        /// <summary>
        /// Lanczos filter
        /// </summary>
        public static readonly FilterType Lanczos = new FilterType("Lanczos", 22);

        /// <summary>
        /// nazev filteru
        /// </summary>
        private String name;
        /// <summary>
        /// cislo filteru odpovidajici danemu filteru v knihovne Magick++
        /// </summary>
        public readonly int filterNum;

        /// <summary>
        /// inicializijici konsturktor
        /// </summary>
        /// <param name="name">jmeno filtru</param>
        /// <param name="value">cislo fitrlu pro magick</param>
        private FilterType(String name, int value)
        {
            this.name = name;
            this.filterNum = value;
        }

        /// <summary>
        /// vraci nazev filteru
        /// </summary>
        /// <returns>nazev filteru</returns>
        public override string ToString()
        {
            return name;
        }
    }
}
