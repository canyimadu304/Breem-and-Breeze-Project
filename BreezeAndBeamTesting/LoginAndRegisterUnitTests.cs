using Moq;
using System.Data;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BreezeAndBeamTesting
{
    public class UserManagementTests
    {
        private readonly Mock<DBAccess> _mockDBAccess;
        private readonly UserManagement _userManagement;

        public UserManagementTests()
        {
            // Create a mock of the DBAccess class
            _mockDBAccess = new Mock<DBAccess>();
            // Create the UserManagement instance, passing the mocked DBAccess
            _userManagement = new UserManagement();
        }

        // testing Login method
        [Fact]
        public void Login_ValidCredentials_ReturnsTrue()
        {
            // arrange
            string email = "test@example.com";
            string password = "password123";

            _mockDBAccess.Setup(dba => dba.GetEmail()).Returns(GetMockEmails());
            _mockDBAccess.Setup(dba => dba.GetPassword(email)).Returns(GetMockPasswords(email));

            // act
            bool result = _userManagement.Login(email, password);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Login_InvalidPassword_ReturnsFalse()
        {
            // arrange
            string email = "test@example.com";
            string password = "wrongpassword";

            _mockDBAccess.Setup(dba => dba.GetEmail()).Returns(GetMockEmails());
            _mockDBAccess.Setup(dba => dba.GetPassword(email)).Returns(GetMockPasswords(email));

            // act
            bool result = _userManagement.Login(email, password);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Login_NonExistentEmail_ReturnsFalse()
        {
            // arrange
            string email = "nonexistent@example.com";
            string password = "password123";

            _mockDBAccess.Setup(dba => dba.GetEmail()).Returns(GetMockEmails());
            _mockDBAccess.Setup(dba => dba.GetPassword(email)).Returns(GetMockPasswords(email));

            // act
            bool result = _userManagement.Login(email, password);

            // assert
            Assert.False(result);
        }

        // testing CheckAccountExists method
        [Fact]
        public void CheckAccountExists_EmailExists_ReturnsTrue()
        {
            // arrange
            string email = "test@example.com";

            _mockDBAccess.Setup(dba => dba.GetEmail()).Returns(GetMockEmails());

            // act
            bool result = _userManagement.CheckAccountExists(email);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void CheckAccountExists_EmailDoesNotExist_ReturnsFalse()
        {
            // arrange
            string email = "nonexistent@example.com";

            _mockDBAccess.Setup(dba => dba.GetEmail()).Returns(GetMockEmails());

            // act
            bool result = _userManagement.CheckAccountExists(email);

            // assert
            Assert.False(result);
        }

        // testing CheckEmailFormat method
        [Fact]
        public void CheckEmailFormat_ValidEmail_ReturnsTrue()
        {
            // arrange
            string email = "test@example.com";

            // act
            bool result = _userManagement.CheckEmailFormat(email);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void CheckEmailFormat_InvalidEmail_ReturnsFalse()
        {
            // arrange
            string email = "test@com";

            // act
            bool result = _userManagement.CheckEmailFormat(email);

            // assert
            Assert.False(result);
        }

        // testing RegisterNewUser method
        [Fact]
        public void RegisterNewUser_ValidData_ReturnsTrue()
        {
            // arrange
            string username = "newuser";
            string email = "newuser@example.com";
            string password = "password123";
            string confirmPassword = "password123";

            _mockDBAccess.Setup(dba => dba.GetEmail()).Returns(GetMockEmails());
            _mockDBAccess.Setup(dba => dba.GetPassword(email)).Returns(GetMockPasswords(email));
            _mockDBAccess.Setup(dba => dba.GetUsername(email)).Returns(GetMockUsernames(email));

            // act
            bool result = _userManagement.RegisterNewUser(username, email, password, confirmPassword);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void RegisterNewUser_PasswordMismatch_ReturnsFalse()
        {
            // arrange
            string username = "newuser";
            string email = "newuser@example.com";
            string password = "password123";
            string confirmPassword = "differentpassword";

            // act
            bool result = _userManagement.RegisterNewUser(username, email, password, confirmPassword);

            // assert
            Assert.False(result);
        }

        // creating helper methods to simulate database data
        private DataTable GetMockEmails()
        {
            var table = new DataTable();
            table.Columns.Add("email", typeof(string));
            table.Rows.Add("test@example.com");
            table.Rows.Add("another@example.com");
            return table;
        }

        private DataTable GetMockPasswords(string email)
        {
            var table = new DataTable();
            table.Columns.Add("password", typeof(string));
            if (email == "test@example.com")
            {
                table.Rows.Add("password123");
            }
            return table;
        }

        private DataTable GetMockUsernames(string email)
        {
            var table = new DataTable();
            table.Columns.Add("username", typeof(string));
            if (email == "test@example.com")
            {
                table.Rows.Add("testuser");
            }
            return table;
        }
    }
}