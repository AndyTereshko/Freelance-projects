using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRates
{
    class LogClass
    {
      public LogClass ()
      {
      }
      public void write (string msg)
      {
         DateTime currtime = DateTime.Now;
         using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"log.txt", true))
         {
            string tmptxt = String.Format("{0:yyMMdd hh:mm:ss} {1}", currtime, msg);
            file.WriteLine(tmptxt);
            file.Close();
         }
      }
    }
}
