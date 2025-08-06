
using System;
using System.ComponentModel.DataAnnotations;

namespace MusterprojektBie.Model
{
   
    /// <summary>
    /// Stellt einen Debitor dar, grundlegende Stammdaten sind enthalten, werden aus der PD-Datenbank ausgelesen.
    /// </summary>
    internal class Debitor
    {
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "BNR")]
        public string BetriebsNr { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Debitor")]
        public string DebitorNr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Name")]
        public string DebitorName { get; set; }

    }
}
