using BeamAndBreeze.BusinessLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeamAndBreeze.Presentation
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = tbxUsername.Text;
            string email = tbxEmail.Text;
            string password = tbxPassword.Text;
            string confirmPassword = tbxConfirmPassword.Text;

            if (!username.Equals("") && !email.Equals("") && !password.Equals("") && !confirmPassword.Equals(""))
            {
                UserManagement userManagement = new UserManagement();

                if (userManagement.RegisterNewUser(username, email, password, confirmPassword))
                {
                    MessageBox.Show("You have successfully registered your account!");

                    this.Hide();

                    LoginForm lf = new LoginForm();
                    lf.ShowDialog();
                }        
            }
            else
            {
                MessageBox.Show("You cannot leave any fields blank.");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();

            LoginForm lf = new LoginForm();
            lf.ShowDialog();
        }
    }
}
