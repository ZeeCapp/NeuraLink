using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollector
{
    public class Candle
    {
        public DateTime CandleDate { get; private set; }
        public double CandleOpen { get; private set; }
        public double CandleHigh { get; private set; }
        public double CandleLow { get; private set; }
        public double CandleClose { get; private set; }

        public Candle(DateTime candleDate, double open, double high, double low, double close)
        {
            this.CandleDate = candleDate;
            this.CandleOpen = open;
            this.CandleHigh = high;
            this.CandleLow = low;
            this.CandleClose = close;
        }
    }
}
