USE UserDB;


CREATE TABLE Users (
    userID INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50) NOT NULL,
    email VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(50) NOT NULL,
    userRole VARCHAR(10) NOT NULL
);

INSERT INTO Users (username, email, password, userRole)
VALUES
('alex_jones', 'alex.jones@example.com', 'P@ssword123', 'admin'),
('maria_garcia', 'maria.garcia@example.com', 'MyS3cretPass', 'user')

SELECT * FROM Users;

DROP TABLE Users