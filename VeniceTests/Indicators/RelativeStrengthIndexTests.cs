using System.Collections.Generic;
using VeniceDomain.Extensions;
using VeniceDomain.Models;
using VeniceDomain.Services.TechnicalAnalysis;
using Xunit;

namespace VeniceTests.Indicators
{
    public class RelativeStrengthIndexTests
    {
        [Fact]
        public void TestRsi()
        {
            // See xlsx file for data and comparisons //
            var candles = new List<CandleValue>
            {
                new CandleValue { Close = 44.3389M },
                new CandleValue { Close = 44.0902m },
                new CandleValue { Close = 44.1497m },
                new CandleValue { Close = 43.6124M },
                new CandleValue { Close = 44.3278M },
                new CandleValue { Close = 44.8264m },
                new CandleValue { Close = 45.0955m },
                new CandleValue { Close = 45.4245m },
                new CandleValue { Close = 45.8433m },
                new CandleValue { Close = 46.0826m },
                new CandleValue { Close = 45.8931m },
                new CandleValue { Close = 46.0328m },
                new CandleValue { Close = 45.6140m },
                new CandleValue { Close = 46.2820m },
                new CandleValue { Close = 46.2820m },
                new CandleValue { Close = 46.0028m },
                new CandleValue { Close = 46.0328m },
                new CandleValue { Close = 46.4116m },
                new CandleValue { Close = 46.2222m },
                new CandleValue { Close = 45.6439m },
                new CandleValue { Close = 46.2122m },
                new CandleValue { Close = 46.2521m },
                new CandleValue { Close = 45.7137m },
                new CandleValue { Close = 46.4515m },
                new CandleValue { Close = 45.7835m },
                new CandleValue { Close = 45.3548m },
                new CandleValue { Close = 44.0288m },
                new CandleValue { Close = 44.1783m },
                new CandleValue { Close = 44.2181m },
                new CandleValue { Close = 44.5672m },
                new CandleValue { Close = 43.4205m },
                new CandleValue { Close = 42.6628m },
                new CandleValue { Close = 43.1314m },
            };
            new RelativeStrengthIndexCalc(candles, 14).GetAll();
            Assert.Equal(70.5328m, candles[14].GetRelativeStrengthIndex(14).Value, 4);
            Assert.Equal(66.3186m, candles[15].GetRelativeStrengthIndex(14).Value, 4);
            Assert.Equal(66.5498m, candles[16].GetRelativeStrengthIndex(14).Value, 4);
            Assert.Equal(69.4063m, candles[17].GetRelativeStrengthIndex(14).Value, 4);
            Assert.Equal(66.3552m, candles[18].GetRelativeStrengthIndex(14).Value, 4);
            Assert.Equal(57.9749m, candles[19].GetRelativeStrengthIndex(14).Value, 4);
            Assert.Equal(62.9296m, candles[20].GetRelativeStrengthIndex(14).Value, 4);
            Assert.Equal(63.2571m, candles[21].GetRelativeStrengthIndex(14).Value, 4);
            Assert.Equal(56.0593m, candles[22].GetRelativeStrengthIndex(14).Value, 4);
            Assert.Equal(62.3771m, candles[23].GetRelativeStrengthIndex(14).Value, 4);
            Assert.Equal(54.7076m, candles[24].GetRelativeStrengthIndex(14).Value, 4);
            Assert.Equal(50.4228m, candles[25].GetRelativeStrengthIndex(14).Value, 4);
            Assert.Equal(39.9898m, candles[26].GetRelativeStrengthIndex(14).Value, 4);
            Assert.Equal(41.4605m, candles[27].GetRelativeStrengthIndex(14).Value, 4);
            Assert.Equal(41.8689m, candles[28].GetRelativeStrengthIndex(14).Value, 4);
            Assert.Equal(45.4632m, candles[29].GetRelativeStrengthIndex(14).Value, 4);
            Assert.Equal(37.3040m, candles[30].GetRelativeStrengthIndex(14).Value, 4);
            Assert.Equal(33.0795m, candles[31].GetRelativeStrengthIndex(14).Value, 4);
            Assert.Equal(37.7730m, candles[32].GetRelativeStrengthIndex(14).Value, 4);

        }
    }
}
