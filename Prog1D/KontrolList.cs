using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prog1D
{
   public class KontrolList
   {
        public string hangiBolum { get; set; }
        public string icerik { get; set; }
        public int sayfa { get; set; }
        public string numara { get; set; }
        public KontrolList(string hangiBolum, string icerik, int sayfa, string numara)
        {
            this.hangiBolum = hangiBolum;
            this.icerik = icerik;
            this.sayfa = sayfa;
            this.numara = numara;
        }


   }
}
