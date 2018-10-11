using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace DataCollector
{
    public class CurrencyDataReader
    {

        public enum TimeFrame { m1,m5,m15,m30,h1};

        private string FileAsString { get; set; }
        private string Record { get; set; }
        private char[] DocumentCharArray { get; set; }
        private DateTime LastCandleDate { get; set; }
        private DateTime CurrCandleDate { get; set; }
        List<Candle> data { get; set; }
        private List<double> CandleOpenHighLowClose { get; set; }


        public string dateFormat
        {
            get
            {
                return @"yyyyMMdd HHmmss";
            }
            private set { }
        }

        /// <summary>
        /// Extracts candle data from a specified CSV file.
        /// </summary>
        /// <param name="path">Path of the CSV file that contains historical data.</param>
        /// <returns></returns>
        public List<Candle> ParseCSVFile(string path)
        {
            data = new List<Candle>();
            CandleOpenHighLowClose = new List<double>();

            try
            {
                FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(file);

                //reads entire document and saves it to a single string
                FileAsString = reader.ReadToEnd();
                DocumentCharArray = FileAsString.ToCharArray();

                //cycles through every character in the current file
                foreach (char ch in DocumentCharArray)
                {
                    //if the character is a comma, it indicates the start of a new Record
                    if (ch == ';')
                    {
                        //checks if the record is a price 
                        if (Record.Length == 8 || Record.Length == 6)
                        {
                            //if we have all current candle data, we add it to our candle list
                            if (CandleOpenHighLowClose != null && CandleOpenHighLowClose.Count == 4 && CurrCandleDate != null)
                            {
                                SaveCandle();
                            }

                            CandleOpenHighLowClose.Add(double.Parse(Record,CultureInfo.GetCultureInfo("US")));
                            Record = string.Empty;
                        }
                       //else, the record is a date
                        else
                        {
                            //if we have all current candle data, we add it to our candle list
                            if (CandleOpenHighLowClose != null && CandleOpenHighLowClose.Count == 4 && CurrCandleDate != null)
                            {
                                SaveCandle();
                            }
                            CurrCandleDate = ParseDate(Record);
                        }

                        Record = string.Empty;
                    }
                    //else, you add the charracter to a buffer and continue adding until you hit a comma
                    else
                    {
                        Record += ch.ToString();
                    }
                }

                reader.Close();
            }
            catch (FileNotFoundException fileNotFound)
            {
                Console.WriteLine(fileNotFound.Message);
                Console.WriteLine(fileNotFound.GetBaseException());

                return null;
            }
            SaveCandle();
            return data;
        }

        public List<Candle> ChangeDataTimeFrame(List<Candle> m1Candles, TimeFrame timeFrame)
        {
            if(m1Candles != null)
            {
                List<Candle> newTimeframeCandles = new List<Candle>();
                List<Candle> buffer = new List<Candle>();

                switch (timeFrame)
                {
                    case TimeFrame.m5:                     
                        foreach(Candle candle in m1Candles)
                        {
                            if(buffer.Count == 5)
                            {
                                newTimeframeCandles.Add(FindHighest(buffer));
                                buffer.Clear();
                            }

                            buffer.Add(candle);
                        }
                        break;

                    case TimeFrame.m15:
                        foreach (Candle candle in m1Candles)
                        {
                            if (buffer.Count == 15)
                            {
                                newTimeframeCandles.Add(FindHighest(buffer));
                                buffer.Clear();
                            }

                            buffer.Add(candle);
                        }
                        break;

                    case TimeFrame.m30:
                        foreach (Candle candle in m1Candles)
                        {
                            if (buffer.Count == 30)
                            {
                                newTimeframeCandles.Add(FindHighest(buffer));
                                buffer.Clear();
                            }

                            buffer.Add(candle);
                        }
                        break;

                    case TimeFrame.h1:
                        foreach (Candle candle in m1Candles)
                        {
                            if (buffer.Count == 60)
                            {
                                newTimeframeCandles.Add(FindHighest(buffer));
                                buffer.Clear();
                            }

                            buffer.Add(candle);
                        }
                        break;
                }
                return newTimeframeCandles;
            }
            return null;
        }

        //parses date from the data file
        private DateTime ParseDate(string date)
        {
            if(date.StartsWith("0"))
            {
                date = date.Remove(0, 1);
            }
            if (date.Contains("\n"))
            {
                string[] dateSplit = date.Split('\n');
                date = dateSplit[0] + dateSplit[1];
            }
            LastCandleDate = CurrCandleDate;
            return DateTime.ParseExact(date,dateFormat, CultureInfo.CurrentCulture);
        }

        //saves a candle with the current data
        private void SaveCandle()
        {
            data.Add(
                                    new Candle(
                                        CurrCandleDate,
                                        CandleOpenHighLowClose[0],
                                        CandleOpenHighLowClose[1],
                                        CandleOpenHighLowClose[2],
                                        CandleOpenHighLowClose[3])
                                        );

            CandleOpenHighLowClose.Clear();
        }

        private Candle FindHighest(List<Candle> candles)
        {
            double open = 0, high = 0, low = 0, close = 0;
            DateTime candleDate = candles[candles.Count - 1].CandleDate;

            open = candles[0].CandleOpen;
            close = candles[candles.Count-1].CandleClose;

            foreach (Candle candle in candles)
            {
                if (candle.CandleHigh > high)
                    high = candle.CandleHigh;
                else if (candle.CandleLow < low)
                    low = candle.CandleLow;
            }

            return new Candle(candleDate, open, high, low, close);
        }
    }
}
