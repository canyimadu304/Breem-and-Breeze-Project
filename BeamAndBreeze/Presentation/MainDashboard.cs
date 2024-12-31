using BeamAndBreeze.BusinessLogic;
using Breeze_Beam.DataAccessLayer;
using Breeze_Beam.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace BeamAndBreeze
{
    public partial class MainDashboard : Form
    {
        private Admin _admin;
        private User _user;

        public MainDashboard(Admin admin)
        {
            InitializeComponent();

            // if the user logged in with an admin account, show the admin controls
            _admin = admin;
            pnlAdminControls.Visible = true;

            string uname = admin.Username;
            lblUsername.Text = "Hi, " + uname + "!";
        }

        public MainDashboard(User user)
        {
            InitializeComponent();

            // if the user logged in with a user account, do not show the admin controls
            _user = user;
            pnlAdminControls.Visible = false;

            string uname = user.Username;
            lblUsername.Text = "Hi, " + uname + "!";
        }

        private void MainDashboard_Load(object sender, EventArgs e)
        {
            DashboardManagement dashboardManagement = new DashboardManagement();

            // displaying data on the data overview panel 
            dashboardManagement.InitializeCharts(crtEnergyGeneration, crtConsAndProd, crtSourceGeneration);

            dashboardManagement.PopulateConsAndProdChart(crtConsAndProd);
            dashboardManagement.PopulateEnergyGenerationChart(crtEnergyGeneration);
            dashboardManagement.PopulateSourceGenerationChart(crtSourceGeneration);

            // displaying data on the power flow panel 
            double solarGen = dashboardManagement.GetCurrentSolarCharge();
            lblSolarCharge.Text = solarGen.ToString() + "MW";
            if (solarGen == 0)
            {
                lblSolarFlow1.ForeColor = Color.Black;
                lblSolarFlow2.ForeColor = Color.Black;
                lblSolarFlow3.ForeColor = Color.Black;
            }  
            else
            {
                lblSolarFlow1.ForeColor = Color.FromArgb(255, 0, 192, 0);
                lblSolarFlow2.ForeColor = Color.FromArgb(255, 0, 192, 0);
                lblSolarFlow3.ForeColor = Color.FromArgb(255, 0, 192, 0);
            }

            double windGen = dashboardManagement.GetCurrentWindCharge();
            lblWindCharge.Text = windGen.ToString() + "MW";
            if (windGen == 0)
            {
                lblWindFlow1.ForeColor = Color.Black;
                lblWindFlow2.ForeColor = Color.Black;
                lblWindFlow3.ForeColor = Color.Black;
            }
            else
            {
                lblWindFlow1.ForeColor = Color.FromArgb(255, 0, 192, 0);
                lblWindFlow2.ForeColor = Color.FromArgb(255, 0, 192, 0);
                lblWindFlow3.ForeColor = Color.FromArgb(255, 0, 192, 0);
            }

            double hydroGen = dashboardManagement.hydroGen;
            lblDamCharge.Text = Math.Round(hydroGen, 2).ToString() + "MW";
            if (hydroGen == 0)
            {
                lblDamFlow.ForeColor = Color.Black;
                lblPumpStatus.Text = "Pumps On";
            }
            else
            {
                lblDamFlow.ForeColor = Color.FromArgb(255, 0, 192, 0);
                lblFloodgateStatus.Text = "Floodgates Open";
            }

            double demand = dashboardManagement.GetCurrentDemand();
            lblDemand.Text = demand.ToString() + "MW";

            double batteryChargeRate = solarGen + windGen + hydroGen - demand;
            lblBatteryCharge.Text = Math.Round(batteryChargeRate, 2).ToString()  + "MW";

            double batteryPercentage = 50 + (batteryChargeRate / 20 * 100);
            lblBatteryPercentage.Text = Math.Round(batteryPercentage, 2).ToString() + "%";

            double HoursOnBattery = Math.Round(batteryPercentage / 100 * 20 / demand, 1);
            lblTimeOnBattery.Text = HoursOnBattery.ToString() + " Hours";

            if (lblFloodgateStatus.Text.Equals("Floodgates Closed"))
            {
                lblDamCharge.Text = "0MW";
                lblDamFlow.ForeColor = Color.Black;
            }
        }

        private void lblLogout_MouseEnter(object sender, EventArgs e)
        {
            lblLogout.ForeColor = Color.FromArgb(0, 192, 0);
        }

        private void lblLogout_MouseLeave(object sender, EventArgs e)
        {
            lblLogout.ForeColor = Color.Black;
        }

        private void lblLogout_Click(object sender, EventArgs e)
        {
            this.Hide();

            LoginForm lf = new LoginForm();
            lf.Show();
        }

        private void btnFloodgateOpen_Click(object sender, EventArgs e)
        {
            HydroPlantController hydroPlantController = new HydroPlantController();

            string presentStatus = lblFloodgateStatus.Text;

            if (presentStatus.Equals("Floodgates Open"))
            {
                MessageBox.Show("The floodgates are already open");
            }
            else
            {
                double hydroPowergenerated = hydroPlantController.ManuallyGeneratePower();
                if (hydroPowergenerated > 0)
                {
                    lblPumpStatus.Text = "Pumps Off";
                    lblFloodgateStatus.Text = "Floodgates Open";
                    lblDamFlow.ForeColor = Color.FromArgb(0, 192, 0);
                    lblDamCharge.Text = Math.Round(hydroPowergenerated, 2).ToString() + "MW";
                }
            }   
        }

        private void btnFloodgateClose_Click(object sender, EventArgs e)
        {
            string presentStatus = lblFloodgateStatus.Text;

            if (presentStatus.Equals("Floodgates Closed"))
            {
                MessageBox.Show("The floodgates are already closed");
            }
            else
            {
                lblFloodgateStatus.Text = "Floodgates Closed";
                lblDamCharge.Text = "0MW";
                lblDamFlow.ForeColor = Color.Black;
            }   
        }

        private void btnPumpOn_Click(object sender, EventArgs e)
        {
            HydroPlantController hydroPlantController = new HydroPlantController();

            string presentStatus = lblPumpStatus.Text;

            if (presentStatus.Equals("Pumps On"))
            {
                MessageBox.Show("The pumps are already on");
            }
            else
            {
                bool waterPumped = hydroPlantController.ManuallyPumpWater();
                if (!waterPumped)
                {
                    lblPumpStatus.Text = "Pumps Off";
                }
                else
                {
                    lblDamCharge.Text = "0MW";
                    lblDamFlow.ForeColor = Color.Black;
                    lblFloodgateStatus.Text = "Floodgates Closed";
                    lblPumpStatus.Text = "Pumps On";
                }
            }
        }

        private void btnPumpOff_Click(object sender, EventArgs e)
        {
            string presentStatus = lblPumpStatus.Text;
            

            if (presentStatus.Equals("Pumps Off"))
            {
                MessageBox.Show("The pumps are already off"); 
            }
            else
            {
                lblPumpStatus.Text = "Pumps Off";
            }     
        }

        private void MainDashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
