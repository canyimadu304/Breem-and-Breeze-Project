using BeamAndBreeze;
using BeamAndBreeze.BusinessLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Breeze_Beam.DataAccessLayer
{
    internal class DashboardManagement
    {
        public double hydroGen;

        // initializing charts 
        public void InitializeCharts(Chart energyGenerationCrt, Chart ConsAndProdCrt, Chart sourceGenerationCrt)
        {
            // ensuring crtEnergyGeneration has Series S1
            if (energyGenerationCrt.Series.IsUniqueName("S1"))
            {
                energyGenerationCrt.Series.Add("S1");
                energyGenerationCrt.Series["S1"].ChartType = SeriesChartType.Pie;
                energyGenerationCrt.Series["S1"]["PieLabelStyle"] = "Outside";
            }

            // ensuring crtConsAndProd has Series S1
            if (ConsAndProdCrt.Series.IsUniqueName("S1"))
            {
                ConsAndProdCrt.Series.Add("S1");
                ConsAndProdCrt.Series["S1"].ChartType = SeriesChartType.Pie;
                ConsAndProdCrt.Series["S1"]["PieLabelStyle"] = "Outside";
            }

            // ensuring crtSourceGeneration has Series for Solar, Wind and Hydro
            if (sourceGenerationCrt.Series.IsUniqueName("Solar"))
            {
                sourceGenerationCrt.Series.Add("Solar");
                sourceGenerationCrt.Series["Solar"].ChartType = SeriesChartType.Line;
                sourceGenerationCrt.Series["Solar"].Color = Color.FromArgb(255, 240, 162, 2);
                sourceGenerationCrt.Series["Solar"].BorderWidth = 4;
            }

            if (sourceGenerationCrt.Series.IsUniqueName("Wind"))
            {
                sourceGenerationCrt.Series.Add("Wind");
                sourceGenerationCrt.Series["Wind"].ChartType = SeriesChartType.Line;
                sourceGenerationCrt.Series["Wind"].Color = Color.FromArgb(255, 255, 181, 194);
                sourceGenerationCrt.Series["Wind"].BorderWidth = 4;
            }

            if (sourceGenerationCrt.Series.IsUniqueName("Hydro"))
            {
                sourceGenerationCrt.Series.Add("Hydro");
                sourceGenerationCrt.Series["Hydro"].ChartType = SeriesChartType.Line;
                sourceGenerationCrt.Series["Hydro"].Color = Color.FromArgb(255, 67, 87, 173);
                sourceGenerationCrt.Series["Hydro"].BorderWidth = 4;
            }
        }

        // getting value for current power generation of source
        public double GetCurrentCharge(ArrayList list, bool isDemandList)
        {
            DateTime now = DateTime.Now;
            double currentAmount = 0;
            int hourIndex = 0;

            foreach (string[] row in list)
            {
                if (isDemandList)
                {
                    if ((row.Length >= 3)
                    && (DateTime.TryParse(row[8], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    && (double.TryParse(row[9], NumberStyles.Any, CultureInfo.InvariantCulture, out double actual)))
                    {
                        // getting data from current date and hour
                        if (date.Day == now.Day && date.Month == 1)
                        {
                            if (hourIndex == now.Hour)
                            {
                                currentAmount = actual;
                            }

                            hourIndex++;
                        }
                    }
                }
                else
                {
                    if ((row.Length >= 3)
                    && (DateTime.TryParse(row[0], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    && (double.TryParse(row[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double actual)))
                    {
                        // getting data from current date and hour
                        if (date.Day == now.Day && date.Month == 1)
                        {
                            if (hourIndex == now.Hour)
                            {
                                currentAmount = actual;
                            }

                            hourIndex++;
                        }
                    }
                }
            }

            return currentAmount;
        }

        // getting solar power generation for current hour
        public double GetCurrentSolarCharge()
        {
            CSVHandler csvHandler = new CSVHandler();

            ArrayList solarList = csvHandler.LoadSolarData();
            double solarCharge = GetCurrentCharge(solarList, false);
            
            return solarCharge;
        }

        // getting wind power generation for current hour
        public double GetCurrentWindCharge()
        {
            CSVHandler csvHandler = new CSVHandler();

            ArrayList windList = csvHandler.LoadWindData();
            double windCharge = GetCurrentCharge(windList, false);

            return windCharge;
        }

        // getting power demand for current hour
        public double GetCurrentDemand()
        {
            CSVHandler csvHandler = new CSVHandler();

            ArrayList demandList = csvHandler.LoadConsumptionData();
            double demand = GetCurrentCharge(demandList, true);

            demand = Math.Round(demand * 3, 2);
            return demand;
        }

        // getting values for power generation of source for the day so far
        public double GetChargeForDaySoFar(ArrayList list, bool isDemand)
        {
            DateTime now = DateTime.Now;
            DateTime? lastDate = null;
            int hourIndex = 0;
            double sourceTotal = 0;

            foreach (string[] row in list)
            {
                if (isDemand)
                {
                    if ((row.Length >= 3)
                    && (DateTime.TryParse(row[8], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    && (double.TryParse(row[9], NumberStyles.Any, CultureInfo.InvariantCulture, out double actual)))
                    {
                        if (lastDate.HasValue && lastDate.Value != date)
                        {
                            hourIndex = 0;
                        }

                        // getting data from current date and only process up to the current hour
                        if (date.Day == now.Day && date.Month == 1 && hourIndex <= now.Hour)
                        {
                            sourceTotal += actual;

                        }

                        lastDate = date;
                        hourIndex++;
                    }
                }
                else
                {
                    if ((row.Length >= 3)
                    && (DateTime.TryParse(row[0], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    && (double.TryParse(row[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double actual)))

                    {
                        if (lastDate.HasValue && lastDate.Value != date)
                        {
                            hourIndex = 0;
                        }

                        // getting data from current date and only process up to the current hour
                        if (date.Day == now.Day && date.Month == 1 && hourIndex <= now.Hour)
                        {
                            sourceTotal += actual * 0.3;
                        }

                        lastDate = date;
                        hourIndex++;
                    }
                }
            }

            return sourceTotal;
        }

        public double[] GetTotalsForDaySoFar()
        {
            CSVHandler csvHandler = new CSVHandler();
            double[] totals = new double[3];

            ArrayList solarList = csvHandler.LoadSolarData();
            double solarActualTotal = GetChargeForDaySoFar(solarList, false);
            totals[0] = solarActualTotal;

            ArrayList windList = csvHandler.LoadWindData();
            double windActualTotal = GetChargeForDaySoFar(windList, false);
            totals[1] = windActualTotal;

            ArrayList demandList = csvHandler.LoadConsumptionData();
            double consumptionTotal = GetChargeForDaySoFar(demandList, true);
            totals[2] = consumptionTotal;

            return totals;
        }

        public double GetHydroGen()
        {
            HydroPlantController hydroPlantController = new HydroPlantController();

            double[] allData = GetTotalsForDaySoFar();
            double solarTotal = 0;
            double windTotal = 0;
            double demandTotal = 0;

            for (int i = 0; i <= allData.Length; i++)
            {
                solarTotal = allData[0];
                windTotal = allData[1];
                demandTotal = allData[2];
            }

            hydroGen = hydroPlantController.ManageDailyOperations(solarTotal + windTotal, demandTotal);

            return hydroGen;
        }

        // populating energy generation chart with solar, wind and hydro data for the day so far
        public void PopulateEnergyGenerationChart(Chart chart)
        {
            CSVHandler csvHandler = new CSVHandler();

            chart.Series["S1"].Points.Clear();

            ArrayList solarList = csvHandler.LoadSolarData();
            double solarActualTotal = GetChargeForDaySoFar(solarList, false);

            ArrayList windList = csvHandler.LoadWindData();
            double windActualTotal = GetChargeForDaySoFar(windList, false);

            double[] allData = GetTotalsForDaySoFar();
            double solarTotal = 0;
            double windTotal = 0;

            for (int i = 0; i <= allData.Length; i++)
            {
                solarTotal = allData[0];
                windTotal = allData[1];
            }

            // controlling autonomation of hydro dam based on production and consumption
            hydroGen = GetHydroGen();

            double totalGeneration = solarTotal + windTotal + hydroGen;

            if (totalGeneration > 0)
            {
                // calculating percentage generation for each energy source
                double solarPercentage = (solarActualTotal / totalGeneration) * 100;
                double windPercentage = (windActualTotal / totalGeneration) * 100;
                double hydroPercentage = (hydroGen / totalGeneration) * 100;

                // adding data points to chart with percentages
                var solarPoint = chart.Series["S1"].Points.AddXY("Solar", solarActualTotal);
                chart.Series["S1"].Points[solarPoint].Label = $"Solar: {solarPercentage:F2}%";
                chart.Series["S1"].Points[solarPoint].Color = Color.FromArgb(255, 240, 162, 2); 

                var windPoint = chart.Series["S1"].Points.AddXY("Wind", windActualTotal);
                chart.Series["S1"].Points[windPoint].Label = $"Wind: {windPercentage:F2}%";
                chart.Series["S1"].Points[windPoint].Color = Color.FromArgb(255, 255, 181, 194); 

                var hydroPoint = chart.Series["S1"].Points.AddXY("Hydro-Electric", hydroGen);
                chart.Series["S1"].Points[hydroPoint].Label = $"Hydro-Electric: {hydroPercentage:F2}%";
                chart.Series["S1"].Points[hydroPoint].Color = Color.FromArgb(255, 67, 87, 173);
            }
            else
            {
                // handling cases where total generation is zero
                chart.Series["S1"].Points.AddXY("Solar", 0);
                chart.Series["S1"].Points.AddXY("Wind", 0);
                chart.Series["S1"].Points.AddXY("Hydro-Electric", 0);
            }
        }

        // populating consumption and production chart with total production from all sources and total demand
        public void PopulateConsAndProdChart(Chart chart)
        {
            CSVHandler csvHandler = new CSVHandler();
            HydroPlantController hydroPlantController = new HydroPlantController();

            chart.Series["S1"].Points.Clear();

            ArrayList solarList = csvHandler.LoadSolarData();
            double solarActualTotal = GetChargeForDaySoFar(solarList, false);

            ArrayList windList = csvHandler.LoadWindData();
            double windActualTotal = GetChargeForDaySoFar(windList, false);

            ArrayList demandList = csvHandler.LoadConsumptionData();
            double consumptionTotal = GetChargeForDaySoFar(demandList, true);

            //solarActualTotal *= 0.45;
            //windActualTotal *= 0.45;

            double totalProduction = solarActualTotal + windActualTotal + hydroGen;
            double total = totalProduction + consumptionTotal;

            if (total > 0)
            {
                double productionPercentage = (totalProduction / total) * 100;
                double consumptionPercentage = (consumptionTotal / total) * 100;

                int productionPoint = chart.Series["S1"].Points.AddXY("Production", productionPercentage);
                chart.Series["S1"].Points[productionPoint].Label = $"Production: {totalProduction:F2}MW";
                chart.Series["S1"].Points[productionPoint].Color = Color.Green;

                int consumptionPoint = chart.Series["S1"].Points.AddXY("Consumption", consumptionPercentage);
                chart.Series["S1"].Points[consumptionPoint].Label = $"Consumption: {consumptionTotal:F2}MW";
                chart.Series["S1"].Points[consumptionPoint].Color = Color.Black;
            }
            else
            {
                chart.Series["S1"].Points.AddXY("Production", 0);
                chart.Series["S1"].Points.AddXY("Consumption", 0);
            }
        }

        // populating source generation chart with solar, wind and hydro data for every hour of the day so far
        public void PopulateSourceGenerationChart(Chart chart)
        {
            CSVHandler csvHandler = new CSVHandler();

            DateTime time = DateTime.Today;
            DateTime now = DateTime.Now;
            DateTime? lastDate = null;
            int hourIndex = 0;
            int counter = 0;

            chart.Series["Solar"].Points.Clear();
            chart.Series["Wind"].Points.Clear();
            chart.Series["Hydro"].Points.Clear();

            // plotting hourly solar actual power generation for the day
            ArrayList solarList = csvHandler.LoadSolarData();
            foreach (string[] row in solarList)
            {
                if ((row.Length >= 3)
                    && (DateTime.TryParse(row[0], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    && (double.TryParse(row[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double actual)))

                {
                    if (lastDate.HasValue && lastDate.Value != date)
                    {
                        hourIndex = 0;
                    }

                    // getting data from current date and only process up to the current hour
                    if (date.Day == now.Day && date.Month == 1 && hourIndex < now.Hour)
                    {
                        chart.Series["Solar"].Points.AddXY(time.ToString("HH:mm"), actual);
                        time = time.AddHours(1);
                        counter++;
                    }

                    lastDate = date;
                    hourIndex++;
                }
            }

            time = DateTime.Today;
            lastDate = null;
            hourIndex = 0;

            // plotting hourly wind actual power generation for the day
            ArrayList windList = csvHandler.LoadWindData();
            foreach (string[] row in windList)
            {
                if ((row.Length >= 3)
                    && (DateTime.TryParse(row[0], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    && (double.TryParse(row[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double actual)))
                {
                    if (lastDate.HasValue && lastDate.Value != date)
                    {
                        hourIndex = 0;
                    }

                    // getting data from current date and only process up to the current hour
                    if (date.Day == now.Day && date.Month == 1 && hourIndex < now.Hour)
                    {
                        chart.Series["Wind"].Points.AddXY(time.ToString("HH:mm"), actual);
                        time = time.AddHours(1);
                    }

                    lastDate = date;
                    hourIndex++;
                }
            }

            time = DateTime.Today;

            // plotting hourly hydro actual power generation for the day
            for (int i = 0; i < counter - 1; i++)
            {
                chart.Series["Hydro"].Points.AddXY(time.ToString("HH:mm"), 0);
                time = time.AddHours(1);
            }

            chart.Series["Hydro"].Points.AddXY(time.ToString("HH:mm"), hydroGen);
        }
    }
}
