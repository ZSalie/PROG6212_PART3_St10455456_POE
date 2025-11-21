-- Create the database
CREATE DATABASE UserManagementDB;
GO

USE UserManagementDB;
GO

-- Create UserTypes table for the specified roles
CREATE TABLE UserTypes (
    UserTypeID INT IDENTITY(1,1) PRIMARY KEY,
    UserTypeName NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(255),
    CreatedDate DATETIME2 DEFAULT GETDATE()
);
GO

-- Insert the specified user types
INSERT INTO UserTypes (UserTypeName, Description) VALUES
('Manager', 'Management staff with supervisory responsibilities'),
('Coordinator', 'Coordination and administrative staff'),
('Lecturer', 'Teaching and instructional staff'),
('HR', 'Human resources personnel');
GO

-- Create Users table with only the specified types
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    UserTypeID INT NOT NULL,
    JoinDate DATETIME2 DEFAULT GETDATE(),
    LastLogin DATETIME2,
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (UserTypeID) REFERENCES UserTypes(UserTypeID)
);
GO

-- Create indexes for better performance
CREATE INDEX IX_Users_UserTypeID ON Users(UserTypeID);
CREATE INDEX IX_Users_Username ON Users(Username);
CREATE INDEX IX_Users_Email ON Users(Email);
GO

-- Create a view for user information with their types
CREATE VIEW UserDetails AS
SELECT 
    u.UserID,
    u.Username,
    u.Email,
    u.FirstName,
    u.LastName,
    ut.UserTypeName,
    u.JoinDate,
    u.LastLogin,
    u.IsActive
FROM Users u
INNER JOIN UserTypes ut ON u.UserTypeID = ut.UserTypeID;
GO

-- Create stored procedure to get users by type
CREATE PROCEDURE GetUsersByType
    @UserTypeName NVARCHAR(50)
AS
BEGIN
    SELECT 
        u.UserID,
        u.Username,
        u.Email,
        u.FirstName,
        u.LastName,
        ut.UserTypeName,
        u.JoinDate,
        u.LastLogin
    FROM Users u
    INNER JOIN UserTypes ut ON u.UserTypeID = ut.UserTypeID
    WHERE ut.UserTypeName = @UserTypeName AND u.IsActive = 1
    ORDER BY u.LastName, u.FirstName;
END;
GO

-- Create stored procedure to add a new user
CREATE PROCEDURE AddUser
    @Username NVARCHAR(50),
    @Email NVARCHAR(255),
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
    @UserTypeName NVARCHAR(50)
AS
BEGIN
    DECLARE @UserTypeID INT;
    
    -- Get the UserTypeID for the given UserTypeName
    SELECT @UserTypeID = UserTypeID 
    FROM UserTypes 
    WHERE UserTypeName = @UserTypeName;
    
    -- If UserType doesn't exist, return error
    IF @UserTypeID IS NULL
    BEGIN
        RAISERROR('Invalid user type specified', 16, 1);
        RETURN;
    END
    
    -- Insert the new user
    INSERT INTO Users (Username, Email, FirstName, LastName, PasswordHash, UserTypeID)
    VALUES (@Username, @Email, @FirstName, @LastName, @PasswordHash, @UserTypeID);
    
    -- Return the new UserID
    SELECT SCOPE_IDENTITY() AS NewUserID;
END;
GO

-- Create stored procedure to update user last login
CREATE PROCEDURE UpdateUserLastLogin
    @UserID INT
AS
BEGIN
    UPDATE Users 
    SET LastLogin = GETDATE()
    WHERE UserID = @UserID;
END;
GO

-- Insert sample users for each type
INSERT INTO Users (Username, Email, FirstName, LastName, PasswordHash, UserTypeID) VALUES
('john.manager', 'john.manager@example.com', 'John', 'Smith', 'hashed_password_123', 1),
('sarah.coord', 'sarah.coord@example.com', 'Sarah', 'Johnson', 'hashed_password_456', 2),
('dr.lecturer', 'dr.lecturer@example.com', 'Robert', 'Brown', 'hashed_password_789', 3),
('lisa.hr', 'lisa.hr@example.com', 'Lisa', 'Davis', 'hashed_password_101', 4),
('mike.manager', 'mike.manager@example.com', 'Mike', 'Wilson', 'hashed_password_202', 1),
('anna.coord', 'anna.coord@example.com', 'Anna', 'Taylor', 'hashed_password_303', 2);
GO