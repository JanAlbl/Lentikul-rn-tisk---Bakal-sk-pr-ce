using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlacer
{
    /// <summary>
    /// umoznuje vytvorit pojmenovani libovolne promenne ci objektu, obsahuje atribut "name" typu String a atribut "value",
    /// ktery je generickeho typu
    /// metoda ToString vraci hodnotu atributu "name"
    /// </summary>
    /// <typeparam name="ValueType">typ hodnoty prirazene danemu retezci</typeparam>
    public class StringValuePair<ValueType>
    {
        /// <summary>
        /// jmeno
        /// </summary>
        public readonly String name;
        /// <summary>
        /// hodnota
        /// </summary>
        public readonly ValueType value;

        /// <summary>
        /// kontruktor nastavujici jmeno a hodnotu
        /// </summary>
        /// <param name="name">jmeno</param>
        /// <param name="value">hodnota</param>
        public StringValuePair(String name, ValueType value)
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// vraci jmeno ulozene v atributu "name"
        /// </summary>
        /// <returns>jmeno</returns>
        public override string ToString()
        {
            return name;
        }
    }
}
