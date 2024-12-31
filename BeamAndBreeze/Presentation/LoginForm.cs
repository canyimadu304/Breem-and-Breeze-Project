using BeamAndBreeze.BusinessLogic;
using BeamAndBreeze.Presentation;
using Breeze_Beam.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeamAndBreeze
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            tbxEmail.Text = Properties.Settings.Default.email;

            if (tbxEmail.Text != "")
            {
                cbxRememberMe.Checked = true;
            }
            else
            {
                cbxRememberMe.Checked = false;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            String email = tbxEmail.Text;
            String password = tbxPassword.Text;

            if (!email.Equals("") && !password.Equals(""))
            {
                UserManagement userManagement = new UserManagement();

                if (userManagement.Login(email, password))
                {
                    if (cbxRememberMe.Checked == true)
                    {
                        Properties.Settings.Default.email = email;
                        Properties.Settings.Default.Save();
                    }
                    else 
                    {
                        Properties.Settings.Default.email = "";
                        Properties.Settings.Default.Save();
                    }

                    string userRole = userManagement.CheckUserRole(email);

                    if (userRole.Equals("admin"))
                    {
                        string username = userManagement.CheckUsername(email);

                        Admin ad = new Admin(username, email);

                        this.Hide();
                        MainDashboard umd = new MainDashboard(ad);
                        umd.Show();
                    }

                    if (userRole.Equals("user"))
                    {
                        string username = userManagement.CheckUsername(email);

                        User ur = new User(username, email);

                        this.Hide();
                        MainDashboard umd = new MainDashboard(ur);
                        umd.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("You cannot leave any fields blank.");
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            this.Hide();
            RegisterForm rf = new RegisterForm();
            rf.Show();
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
