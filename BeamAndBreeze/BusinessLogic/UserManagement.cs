using BeamAndBreeze.DataAccess;
using BeamAndBreeze.Presentation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace BeamAndBreeze.BusinessLogic
{
    internal class UserManagement
    {
        public bool Login(string eAddress, string pWord)
        {        
            bool existingAccount = CheckAccountExists(eAddress);

            if (existingAccount)
            {
                bool correctPassword = VerifyPassword(eAddress, pWord);

                if (correctPassword)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("The password you entered is invalid. Please enter the correct password.");
                }
            }
            else
            {
                MessageBox.Show("The email you entered does not exist in the system. Please enter a valid email.");
            }

            return false;
        }

        public bool CheckAccountExists(string eAddress)
        {
            DBAccess dba = new DBAccess();
            DataTable result = dba.GetEmail();

            foreach (DataRow dr in result.Rows)
            {
                if (dr.Field<string>("email") == eAddress) 
                {
                    return true;
                }
            }

            return false;
        }

        public bool VerifyPassword(string eAddress, string pWord)
        {
            DBAccess dba = new DBAccess();
            DataTable result = dba.GetPassword(eAddress);

            foreach (DataRow dr in result.Rows)
            {
                if (dr.Field<string>("password") == pWord)
                {
                    return true;
                }
            }
            
            return false;
        }

        public string CheckUserRole(string eAddress)
        {
            DBAccess dba = new DBAccess();
            DataTable result = dba.GetUserRole(eAddress);
            string role = "";

            foreach (DataRow dr in result.Rows)
            {
                role = dr.Field<string>("userRole");
            }

            return role;
        }

        public string CheckUsername(string eAddress)
        {
            DBAccess dba = new DBAccess();
            DataTable result = dba.GetUsername(eAddress);
            string uname = "";

            foreach (DataRow dr in result.Rows)
            {
                uname = dr.Field<string>("username");
            }
            return uname;
        }

        public bool RegisterNewUser(string uname, string eAddress, string pWord, string confirmPWord)
        {
            DBAccess dba = new DBAccess();

            bool correctEmailFormat = CheckEmailFormat(eAddress);

            if (correctEmailFormat)
            {
                bool uniqueEmail = CheckUniqueEmail(eAddress);

                if (uniqueEmail)
                {
                    bool matchingPassword = CheckPasswordMatch(pWord, confirmPWord);

                    if (matchingPassword)
                    {
                        dba.AddNewUser(uname, eAddress, pWord);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("The passwords do not match. Please enter the same password in the 'Confirm Password' field.");
                    }
                }
                else
                {
                    MessageBox.Show("The email you entered already exists in our system. Please enter a different email to register a new account.");
                }
            }
            else
            {
                MessageBox.Show("The email you entered is not valid. Please enter a valid email address.");
            }

            return false;
        }

        public bool CheckEmailFormat(string emailToCheck)
        {
            if (emailToCheck.Contains("@") && emailToCheck.Contains(".com") || emailToCheck.Contains(".co.za"))
            {
                return true;
            }

            return false;
        }

        public bool CheckUniqueEmail(string emailToCheck)
        {
            bool existingAccount = CheckAccountExists(emailToCheck);

            if (!existingAccount)
            {
                return true;
            }

            return false;
        }

        public bool CheckPasswordMatch(string password, string confirmPassword)
        {
            if (confirmPassword.Equals(password)) 
            { 
                return true; 
            }

            return false;
        }
    }
}
