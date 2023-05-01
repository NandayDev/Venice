using System;
using System.Collections.Generic;
using System.Text;
using VeniceDomain.Enums;
using VeniceDomain.Models;

namespace VeniceTests
{
    internal class DataFetcher
    {
        internal DataFetcher(CandlePeriod period)
        {
            _period = period;
        }

        private CandlePeriod _period;

        private DateTime startDate = new DateTime(2010, 1, 1, 9, 0, 0);

        private FinancialInstrument _financialInstrument;
        internal FinancialInstrument FinancialInstrument
        {
            get
            {
                if (_financialInstrument == null)
                {
                    _financialInstrument = new FinancialInstrument
                    {
                        Ticker = "FCA",
                        CommercialName = "Fiat Chrysler Automobiles"
                    };
                }
                return _financialInstrument;
            }
        }

        private List<CandleValue> _candles;
        internal List<CandleValue> Candles
        {
            get
            {
                if (_candles == null)
                {
                    _candles = new List<CandleValue>()
                    {
                        GenerateCandleValue(11.364M, 11.448M, 11.448M, 11.154M), // 0 -- 
                        GenerateCandleValue(11.34M, 11.59M, 11.684M, 11.228M), // 1 -- 
                        GenerateCandleValue(12.08M, 12.22M, 12.274M, 12.018M), // 2 -- 
                        GenerateCandleValue(12.276M, 12.27M, 12.384M, 12.2M), // 3 -- 
                        GenerateCandleValue(12.268M, 12.248M, 12.32M, 12.186M), // 4 - SMA 5: 11,8656 - WMA 5: 12,04853

                        GenerateCandleValue(12.206M, 12.25M, 12.288M, 12.136M), // 5 - SMA 5: 12,034  - WMA 5: 12,162 
                        GenerateCandleValue(12.32M, 12.342M, 12.476M, 12.3M), // 6 - SMA 5: 12,230 - WMA 5: 12,2573
                        GenerateCandleValue(12.262M, 12.48M, 12.52M, 12.234M), // 7 - SMA 5: 12,2664 - WMA 5: 12,268 
                        GenerateCandleValue(12.432M, 12.464M, 12.502M, 12.294M), //8 - SMA 5: 12,2976 - WMA 5: 12,3232 
                        GenerateCandleValue(12.584M, 12.218M, 12.63M, 12.190M), // 9 - SMA 5: 12,3608 - WMA 5: 12,41867

                        GenerateCandleValue(12.132M, 12.06M, 12.302M, 12.048M), // 10 - SMA 5: 12,346  - WMA 5: 12,3424 
                        GenerateCandleValue(11.984M, 12.086M, 12.198M, 11.966M), // 11 - SMA 5: 12,2788  - WMA 5: 12,22173 
                        GenerateCandleValue(12.208M, 12.42M, 12.42M, 12.074M), // 12 - SMA 5: 12,268  - WMA 5: 12,19813
                        GenerateCandleValue(12.41M, 12.38M, 12.756M, 12.356M), // 13 - SMA 5: 12,2636  - WMA 5: 12,245467
                        GenerateCandleValue(12.31M, 11.96M, 12.334M, 11.906M), // 14 - SMA 5: 12,2088  - WMA 5: 12,26093
                        GenerateCandleValue(12.01M, 11.89M, 12.14M, 11.844M), // 15 - SMA 5: 12,1844 - WMA 5: 12,19467
                        GenerateCandleValue(11.822M, 11.82M, 11.918M, 11.68M), // 16 - SMA 5: 12,152  - WMA 5: 12,073867
                        GenerateCandleValue(11.732M, 11.952M, 11.952M, 11.69M), // 17 - SMA 5: 12,0568  - WMA 5: 11,933867
                        GenerateCandleValue(11.926M, 11.940M, 12.076M, 11.904M), // 18 - SMA 5: 11,960 - WMA 5: 11,890267

                        GenerateCandleValue(11.364M, 11.448M, 11.448M, 11.154M), // 19 - SMA_5: 11,7708 - WMA_5: 11,691619 - SMA_20: 12,0515 - WMA_20: 12,040790 - BBW(20,2): 1.45713056
                        GenerateCandleValue(11.34M, 11.59M, 11.684M, 11.228M), // 20 - SMA_5: 11,6368 - WMA_5: 11,54820 - SMA_20: 12,0503 - WMA_20: 11,973029 - BBW(20,2): 1.4667923327344
                        GenerateCandleValue(12.08M, 12.22M, 12.274M, 12.018M), // 21 - SMA_5: 11,6884 - WMA_5: 11,69573321 - SMA_20: 12,0873 - WMA_20: 11,975857 - BBW(20,2): 1.305489043272776M
                        GenerateCandleValue(12.276M, 12.27M, 12.384M, 12.2M), // 22 - SMA_5: 11,7972 - WMA_5: 11,891622 - SMA_20: 12,0971 - WMA_20: 11,993829 - BBW(20,2): 1.316292016873236M
                        GenerateCandleValue(12.268M, 12.248M, 12.32M, 12.186M), // 23 - SMA_5: 11,8656 - WMA_5: 12,04853323 - SMA_20: 12,0967 - WMA_20: 12,010105 - BBW(20,2): 1.31539554187284M
                        GenerateCandleValue(12.206M, 12.25M, 12.288M, 12.136M), // 24 -- SMA_5: 12,034 - WMA_5: 12,16224 - SMA_20: 12,0936 - WMA_20: 12,020514 - BBW(20,2): 1.309753146689544M
                        GenerateCandleValue(12.32M, 12.342M, 12.476M, 12.3M), // 25 -- SMA_5: 12,230 - WMA_5: 12,25733325 - SMA_20: 12,0993 - WMA_20: 12,042076 - BBW(20,2): 1.321904272352260M
                        GenerateCandleValue(12.262M, 12.48M, 12.52M, 12.234M), // 26 -- SMA_5: 12,2664 - WMA_5: 12,26826 - SMA_20: 12,0964 - WMA_20: 12,057571 - BBW(20,2): 1.314748340938296M
                        GenerateCandleValue(12.432M, 12.464M, 12.502M, 12.294M), // 27 -- SMA_5: 12,2976 - WMA_5: 12,323227 - SMA_20: 12,1049 - WMA_20: 12,089533 - BBW(20,2): 1.341304251367616M
                        GenerateCandleValue(12.584M, 12.218M, 12.63M, 12.190M), // 28 -- SMA_5: 12,3608 - WMA_5: 12,41866728 - SMA_20: 12,1125 - WMA_20: 12,135162 - BBW(20,2): 1.378882843006192M
                        GenerateCandleValue(12.132M, 12.06M, 12.302M, 12.048M), // 29 -- SMA_5: 12,346 - WMA_5: 12,342429 - SMA_20: 12,0899 - WMA_20: 12,137019 - BBW(20,2): 1.306072553799932M
                        GenerateCandleValue(11.984M, 12.086M, 12.198M, 11.966M), // 30 -- SMA_5: 12,2788 - WMA_5: 12,22173330 - SMA_20: 12,0825 - WMA_20: 12,126933 - BBW(20,2): 1.308760763792264M
                        GenerateCandleValue(12.208M, 12.42M, 12.42M, 12.074M), // 31 -- SMA_5: 12,268 - WMA_5: 12,19813331 - SMA_20: 12,0937 - WMA_20: 12,138886 - BBW(20,2): 1.309898895254696M
                        GenerateCandleValue(12.41M, 12.38M, 12.756M, 12.356M), // 32 -- SMA_5: 12,2636 - WMA_5: 12,24546732 - SMA_20: 12,1038 - WMA_20: 12,169010 - BBW(20,2): 1.336923475352036M
                        GenerateCandleValue(12.31M, 11.96M, 12.334M, 11.906M), // 33 -- SMA_5: 12,2088 - WMA_5: 12,26093333 - SMA_20: 12,0988 - WMA_20: 12,188648 - BBW(20,2): 1.32052786893408M
                        GenerateCandleValue(12.01M, 11.89M, 12.14M, 11.844M), // 34 -- SMA_5: 12,1844 - WMA_5: 12,19466734 - SMA_20: 12,0838 - WMA_20: 12,180190 - BBW(20,2): 1.307318734541892M
                        GenerateCandleValue(11.822M, 11.82M, 11.918M, 11.68M), // 35 -- SMA_5: 12,152 - WMA_5: 12,07386735 - SMA_20: 12,0744 - WMA_20: 12,155257 - BBW(20,2): 1.326923089033620M
                        GenerateCandleValue(11.732M, 11.952M, 11.952M, 11.69M), // 36 -- SMA_5: 12,0568 - WMA_5: 11,93386736 - SMA_20: 12,0699 - WMA_20: 12,122648 - BBW(20,2): 1.343675336726220M
                        GenerateCandleValue(11.926M, 11.940M, 12.076M, 11.904M), // 37 -- SMA_5: 11,960 - WMA_5: 11,89026737 - SMA_20: 12,0796 - WMA_20: 12,108943 - BBW(20,2): 1.313456444492524M
                        GenerateCandleValue(12.13M, 12.038M, 12.17M, 11.964M), // 38 -- SMA_5: 11,924 - WMA_5: 11,94693338 - SMA_20: 12,0898 - WMA_20: 12,113743 - BBW(20,2): 1.306019488852736M

                        GenerateCandleValue(12.014M, 11.918M, 12.052M, 11.772M), // 39 -- SMA_5: 11,9248 - WMA_5: 11,97693339 - SMA_20: 12,1223 - WMA_20: 12,106524 - BBW(20,2): 1.117643460707912M
                        GenerateCandleValue(11.856M, 11.948M, 12.07M, 11.786M), // 40 -- SMA_5: 11,9316 - WMA_5: 11,95440 - SMA_20: 12,1481 - WMA_20: 12,081162 - BBW(20,2): 0.884461322828164M
                        GenerateCandleValue(12.142M, 12.064M, 12.278M, 12.064M), // 41 -- SMA_5: 12,0136 - WMA_5: 12,02413341 - SMA_20: 12,1512 - WMA_20: 12,080581 - BBW(20,2): 0.882176829153168M
                        GenerateCandleValue(12.046M, 12.110M, 12.206M, 11.974M), // 42 -- SMA_5: 12,0376 - WMA_5: 12,03493342 - SMA_20: 12,1397 - WMA_20: 12,070562 - BBW(20,2): 0.878756154429300M
                        GenerateCandleValue(12.22M, 12.39M, 12.478M, 12.172M), // 43 -- SMA_5: 12,0556 - WMA_5: 12,09573343 - SMA_20: 12,1373 - WMA_20: 12,078210 - BBW(20,2): 0.873889888282332M
                        GenerateCandleValue(12.33M, 12.39M, 12.474M, 12.330M), // 44 -- SMA_5: 12,1188 - WMA_5: 12,187244 - SMA_20: 12,1435 - WMA_20: 12,096562 - BBW(20,2): 0.889006068299696M
                        GenerateCandleValue(12.458M, 12.402M, 12.504M, 12.362M), // 45 -- SMA_5: 12,2392 - WMA_5: 12,30026745 - SMA_20: 12,1504 - WMA_20: 12,126514 - BBW(20,2): 0.920102877543136M
                        GenerateCandleValue(12.432M, 12.534M, 12.578M, 12.43M), // 46 -- SMA_5: 12,2972 - WMA_5: 12,36453346 - SMA_20: 12,1589 - WMA_20: 12,153333 - BBW(20,2): 0.949558922536364M
                        GenerateCandleValue(12.454M, 12.48M, 12.61M, 12.344M), // 47 -- SMA_5: 12,3788 - WMA_5: 12,416847 - SMA_20: 12,160 - WMA_20: 12,181438 - BBW(20,2): 0.95507508660452M
                        GenerateCandleValue(12.472M, 12.666M, 12.708M, 12.426M), // 48 -- SMA_5: 12,4292 - WMA_5: 12,44786748 - SMA_20: 12,1544 - WMA_20: 12,211152 - BBW(20,2): 0.917727536679024M
                        GenerateCandleValue(12.712M, 12.798M, 12.8M, 12.66M), // 49 -- SMA_5: 12,5056 - WMA_5: 12,54213349 - SMA_20: 12,1834 - WMA_20: 12,264257 - BBW(20,2): 1.043773236110724M
                        GenerateCandleValue(12.75M, 12.910M, 12.91M, 12.708M), // 50 -- SMA_5: 12,564 - WMA_5: 12,623650 - SMA_20: 12,2217 - WMA_20: 12,318219 - BBW(20,2): 1.140885786986308M
                        GenerateCandleValue(12.83M, 12.848M, 12.992M, 12.79M), // // 51 -- SMA_5: 12,6436 - WMA_5: 12,71226751 - SMA_20: 12,2528 - WMA_20: 12,376152 - BBW(20,2): 1.263635923582588M
                        GenerateCandleValue(12.796M, 12.7M, 12.93M, 12.674M), //  // 52 -- SMA_5: 12,712 - WMA_5: 12,76306752 - SMA_20: 12,2721 - WMA_20: 12,427886 - BBW(20,2): 1.34839500693856M
                        GenerateCandleValue(12.96M, 12.984M, 13.046M, 12.918M), // 53 -- SMA_5: 12,8096 - WMA_5: 12,84573353 - SMA_20: 12,3046 - WMA_20: 12,4934 - BBW(20,2): 1.482450545904528M
                        GenerateCandleValue(12.90M, 12.92M, 12.934M, 12.658M), // 54 -- SMA_5: 12,8472 - WMA_5: 12,87586754 - SMA_20: 12,3491 - WMA_20: 12,550105 - BBW(20,2): 1.54588153696064M
                        GenerateCandleValue(13.078M, 13.254M, 13.266M, 13.002M), // 55 -- SMA_5: 12,9128 - WMA_5: 12,952855 - SMA_20: 12,4119 - WMA_20: 12,619524 - BBW(20,2): 1.592723400838624M
                        GenerateCandleValue(13.356M, 13.196M, 13.43M, 13.184M), // 56 -- SMA_5: 13,018 - WMA_5: 13,10053356 - SMA_20: 12,4931 - WMA_20: 12,709438 - BBW(20,2): 1.669441457178828M
                        GenerateCandleValue(13.194M, 13.38M, 13.394M, 13.194M), // 57 -- SMA_5: 13,0976 - WMA_5: 13,159257 - SMA_20: 12,5565 - WMA_20: 12,776190 - BBW(20,2): 1.691805886588276M
                        GenerateCandleValue(13.43M, 13.428M, 13.448M, 13.33M), // 58 -- SMA_5: 13,1916 - WMA_5: 13,27058 - SMA_20: 12,6215 - WMA_20: 12,859381 - BBW(20,2): 1.811186525893716M
                        GenerateCandleValue(13.506M, 13.552M, 13.566M, 13.454M), // 59 -- SMA_5: 13,3128 - WMA_5: 13,374859 - SMA_20: 12,6961 - WMA_20: 12,943619 - BBW(20,2): 1.88007878894252M
                        GenerateCandleValue(13.65M, 13.80M, 13.8M, 13.62M), // 60 -- SMA_5: 13,4272 - WMA_5: 13,487260 - SMA_20: 12,7858 - WMA_20: 13,034467 - BBW(20,2): 1.889736756938996M
                        GenerateCandleValue(13.602M, 13.70M, 13.72M, 13.554M), // 61 -- SMA_5: 13,4764 - WMA_5: 13,54546761 - SMA_20: 12,8588 - WMA_20: 13,1122 - BBW(20,2): 1.921799858137376M
                        GenerateCandleValue(13.604M, 13.838M, 13.848M, 13.558M), // 62 -- SMA_5: 13,5584 - WMA_5: 13,58862 - SMA_20: 12,9367 - WMA_20: 13,183171 - BBW(20,2): 1.871475005106092M
                        GenerateCandleValue(13.598M, 13.558M, 13.632M, 13.422M), // 63 -- SMA_5: 13,592 - WMA_5: 13,601263 - SMA_20: 13,0056 - WMA_20: 13,246152 - BBW(20,2): 1.832533316994464M
                        GenerateCandleValue(13.47M, 13.52M, 13.534M, 13.356M), // 64 -- SMA_5: 13,5848 - WMA_5: 13,56053364 - SMA_20: 13,0626 - WMA_20: 13,290381 - BBW(20,2): 1.760882266903368M
                    };
                };
                return _candles;
            }
        }

        private List<CandleValue> _candles2;
        internal List<CandleValue> Candles2
        {
            get
            {
                if (_candles2 == null)
                {
                    _candles2 = new List<CandleValue>
                    {
                        // ENI daily 2021-06-18 to ...
                        GenerateCandleValue(10.314M, 10.586M, 10.630M, 10.264M),// 0 
                        GenerateCandleValue(10.418M, 10.220m, 10.494m, 10.12m),// 1 
                        GenerateCandleValue(10.414m, 10.48m, 10.492m, 10.334m), // 2
                        GenerateCandleValue(10.422m, 10.46m, 10.57m, 10.408m),// 3
                        GenerateCandleValue(10.54m, 10.444m, 10.566m, 10.404m),// 4
                        GenerateCandleValue(10.556m, 10.54m, 10.59m, 10.51m),// 5
                        GenerateCandleValue(10.342m, 10.532m, 10.588m, 10.312m),// 6
                        GenerateCandleValue(10.338m, 10.304m, 10.406m, 10.268m),// 7
                        GenerateCandleValue(10.27m, 10.334m, 10.39m, 10.196m),// 8
                        GenerateCandleValue(10.46m, 10.27m, 10.528m, 10.270m),// 9
                        GenerateCandleValue(10.398m, 10.474m, 10.504m, 10.364m),// 10
                        GenerateCandleValue(10.456m, 10.438m, 10.518m, 10.336m),// 11
                        GenerateCandleValue(10.27m, 10.49m, 10.566m, 10.236m),// 12
                    };
                }
                return _candles2;
            }
        }

        private BrokerEnvironment _brokerEnvironment;
        internal BrokerEnvironment BrokerEnvironment
        {
            get
            {
                if (_brokerEnvironment == null)
                {
                    _brokerEnvironment = new BrokerEnvironment()
                    {
                        AnnualTax = 0.02M,
                        CommissionPlan = new PercentageMinMaxCommissionPlan()
                        {
                            MinCommission = 1.5M,
                            MaxCommission = 19,
                            CommissionPercentage = 0.0019M
                        },
                        NetEarningTaxation = 0.26M
                    };
                }
                return _brokerEnvironment;
            }
        }

        private CandleValue GenerateCandleValue(decimal close)
        {
            startDate = startDate.AddSeconds(_period.ConvertToSeconds());
            return new CandleValue(FinancialInstrument, startDate, _period, close, close, close, close);
        }


        private CandleValue GenerateCandleValue(decimal close, decimal open, decimal high, decimal low)
        {
            startDate = startDate.AddSeconds(_period.ConvertToSeconds());
            return new CandleValue(FinancialInstrument, startDate, _period, low, high, open, close);
        }
    }
}
