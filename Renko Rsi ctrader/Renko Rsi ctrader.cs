// Rsi Breakout for Renko by paolo panicali 2022
// answer to hamijonzsince: 09 May 2022  06 Jun 2022, 12:11

using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class RenkoRsictrader : Indicator
    {
        [Parameter("Period", Group = "RSI", DefaultValue = 14)]
        public int RsiPeriod { get; set; }

        [Parameter("Rsi Min Value", Group = "RSI", DefaultValue = 60)]
        public int RsiMin { get; set; }

        [Parameter("Rsi Max Value", Group = "RSI", DefaultValue = 100)]
        public int RsiMax { get; set; }

        [Output("Breakout Top Signal", Color = Colors.Aqua, Thickness = 5, PlotType = PlotType.Points)]
        public IndicatorDataSeries BreakOutTop { get; set; }

        [Output("RSI", Color = Colors.Red, Thickness = 1, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries Rsi_Value { get; set; }

        RelativeStrengthIndex RSI;
        double prevRsi_Value, TopRsi_Value, LastRsi_Value;
        int LookBack;

        IndicatorDataSeries RelativeTop_Value_Series, RelativeTop_Top_Series;

        protected override void Initialize()
        {
            RSI = Indicators.RelativeStrengthIndex(Bars.ClosePrices, RsiPeriod);

            RelativeTop_Value_Series = CreateDataSeries();
            RelativeTop_Top_Series = CreateDataSeries();

            LookBack = 24;
        }

        public override void Calculate(int index)
        {

            Rsi_Value[index] = RSI.Result[index];

            prevRsi_Value = RSI.Result[index - 2];
            TopRsi_Value = RSI.Result[index - 1];
            LastRsi_Value = RSI.Result[index];

            if (TopRsi_Value > prevRsi_Value && TopRsi_Value > LastRsi_Value)
            {
                RelativeTop_Top_Series[index - 1] = 1;
                RelativeTop_Value_Series[index - 1] = TopRsi_Value;
            }
            else
            {
                RelativeTop_Top_Series[index - 1] = 0;
            }


            if (RelativeTop_Top_Series[index - 1] == 1 && RelativeTop_Value_Series[index - 1] >= RsiMin && RelativeTop_Value_Series[index - 1] <= RsiMax)
            {
                bool IsBreakout = true;

                for (int i = 2; i <= 2 + LookBack; i++)
                {
                    if (RelativeTop_Top_Series[index - i] == 1 && RelativeTop_Value_Series[index - i] > RelativeTop_Value_Series[index - 1])
                    {
                        IsBreakout = false;
                        break;
                    }
                }

                if (IsBreakout)
                {
                    BreakOutTop[index - 1] = TopRsi_Value;
                }
                else
                {
                    //BreakOutTop[index - 1] = 0;
                }
            }
        }
    }
}
