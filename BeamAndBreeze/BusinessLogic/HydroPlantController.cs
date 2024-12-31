using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeamAndBreeze.BusinessLogic
{
    internal class HydroPlantController
    {
        private double reservoirLevel;                  // Current water level in cubic meters
        private double maxReservoirCapacity = 500.0;    // Maximum capacity in cubic meters
        private double minReservoirLevel = 50.0;        // Minimum level before generation
        private double pumpRate = 5.0;                  // Water added per MW of surplus energy (cubic meters)
        private double turbineRate = 10.0;              // Water used per MW generated (cubic meters)
        private Random random = new Random();

        public HydroPlantController()
        {
            this.reservoirLevel = 117.2; // Initial water level in the reservoir, will get from previous day's last level
        }

        // Method to manage the hydroplant actions based on production and consumption predictions
        public double ManageDailyOperations(double dailyProductionMW, double dailyConsumptionMW)// sub in the wind+solar, consumption
        {
            double surplus = (dailyProductionMW - dailyConsumptionMW) / 2;

            double hydroCharge = 0;

            // Determine actions based on surplus or demand
            if (surplus > 0)
            {
                AutomaticallyPumpWater(surplus);
            }
            else
            {
                hydroCharge = AutomaticallyGeneratePower(Math.Abs(surplus / 2));
            }

            return hydroCharge;
        }

        // simulating water pumping based on surplus energy
        private void AutomaticallyPumpWater(double surplusMW)
        {
            if (reservoirLevel < maxReservoirCapacity)
            {
                double waterAdded = Math.Min(surplusMW * pumpRate, maxReservoirCapacity - reservoirLevel);
                reservoirLevel += waterAdded;
                MessageBox.Show($"Current power generation exceeds demand and water is being pumped for storage in reservoir." +
                    $"\n\nWater added: {Math.Round(waterAdded, 2)} cubic meters." +
                    $"\nReservoir level: {Math.Round(reservoirLevel, 2)} cubic meters."); 
            }
            else
            {
                MessageBox.Show("Current power generation exceeds demand but the reservoir is already at capacity.");
            }
        }

        // simulating power generation using stored water
        private double AutomaticallyGeneratePower(double demandMW)
        {
            if (reservoirLevel > minReservoirLevel)
            {
                double waterUsed = Math.Min(demandMW * turbineRate, reservoirLevel - minReservoirLevel);
                reservoirLevel -= waterUsed;
                MessageBox.Show($"Current demand exceeds power generation and floodgates have been opened to generate additional power." +
                    $"\n\nWater used: {Math.Round(waterUsed, 2)} cubic meters." +
                    $"\nReservoir level: {Math.Round(reservoirLevel, 2)} cubic meters.");
                double powerGenerated = SimulatePowerGen();
                return powerGenerated;
            }
            else
            {
                MessageBox.Show("Current demand exceeds power generation but there is insufficient water in reservoir to generate power.");
                return 0;
            }
        }

        // simulating water pumping based on surplus energy
        public bool ManuallyPumpWater()
        {
            bool successfulPump = false;

            if (reservoirLevel < maxReservoirCapacity)
            {
                MessageBox.Show($"Water is being pumped for storage in reservoir.");
                successfulPump = true;
            }
            else
            {
                MessageBox.Show("Cannot pump water to reservoir because it is already at capacity.");
            }

            return successfulPump;
        }

        // simulating power generation using stored water
        public double ManuallyGeneratePower()
        {
            if (reservoirLevel > minReservoirLevel)
            {
                MessageBox.Show($"Floodgates have been opened to generate additional power.");
                double powerGenerated = SimulatePowerGen();
                return powerGenerated;
            }
            else
            {
                MessageBox.Show("There is insufficient water in reservoir to generate power.");
                return 0;
            }
        }

        // generating random value to simulate power generation by hydro-electric dam
        private double SimulatePowerGen()
        {
            double minPowerOutput = 5.0833; 
            double maxPowerOutput = 7.1667;
            double efficiencyFactor = 0.75; 

            double simulatedPowerOutput = random.NextDouble() * (maxPowerOutput - minPowerOutput) + minPowerOutput;
            simulatedPowerOutput *= efficiencyFactor;

            return simulatedPowerOutput;
        }
    }
}
