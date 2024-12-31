using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Breeze_Beam.DataAccessLayer
{
    internal class CSVHandler
    {
        public ArrayList LoadSolarData()
        {
            string solarFilePath = @"Data\UVIndexData.csv";
            ArrayList solarDataList = new ArrayList();

            try
            {
                using (var reader = new StreamReader(solarFilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        solarDataList.Add(values);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading solar data: {ex.Message}");
            }

            return solarDataList;
        }

        public ArrayList LoadWindData()
        {
            string windFilePath = @"Data\WindSpeedData.csv";
            ArrayList windDataList = new ArrayList();

            try
            {
                using (var reader = new StreamReader(windFilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        windDataList.Add(values);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading wind data: {ex.Message}");
            }

            return windDataList;
        }

        public ArrayList LoadHydroData() 
        {
            string hydroFilePath = @"Data\WindSpeedData.csv";
            ArrayList hydroDataList = new ArrayList();

            try
            {
                using (var reader = new StreamReader(hydroFilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        hydroDataList.Add(values);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading hydro data: {ex.Message}");
            }

            return hydroDataList;
        }

        public ArrayList LoadConsumptionData()
        {
            string demandFilePath = @"Data\PowerUsageData.csv";
            ArrayList usageDataList = new ArrayList();

            try
            {
                using (var reader = new StreamReader(demandFilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        usageDataList.Add(values);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading power consumption data: {ex.Message}");
            }

            return usageDataList;
        }
    }
}
