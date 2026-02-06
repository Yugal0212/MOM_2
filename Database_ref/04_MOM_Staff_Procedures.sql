USE MOM
GO

-- ========================================
-- MOM_Staff CRUD Stored Procedures
-- ========================================

-- CREATE Staff
CREATE or alter PROCEDURE SP_Staff_Create
    @DepartmentID INT,
    @StaffName NVARCHAR(50),
    @MobileNo NVARCHAR(20),
    @EmailAddress NVARCHAR(50),
    @Remarks NVARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if department exists
    IF NOT EXISTS (SELECT 1 FROM MOM_Department WHERE DepartmentID = @DepartmentID)
    BEGIN
        RAISERROR('Department does not exist.', 16, 1)
        RETURN
    END
    
    INSERT INTO MOM_Staff (DepartmentID, StaffName, MobileNo, EmailAddress, Remarks, Modified)
    VALUES (@DepartmentID, @StaffName, @MobileNo, @EmailAddress, @Remarks, GETDATE())
    
    SELECT SCOPE_IDENTITY() AS StaffID
END
GO

-- READ Staff (Get All)
CREATE or alter PROCEDURE SP_Staff_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.StaffID, 
        s.DepartmentID,
        d.DepartmentName,
        s.StaffName, 
        s.MobileNo,
        s.EmailAddress,
        s.Remarks,
        s.Created, 
        s.Modified
    FROM MOM_Staff s
    INNER JOIN MOM_Department d ON s.DepartmentID = d.DepartmentID
    ORDER BY s.StaffID
END
GO

-- READ Staff (Get By ID)
CREATE  or alter PROCEDURE SP_Staff_GetByID
    @StaffID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.StaffID, 
        s.DepartmentID,
        d.DepartmentName,
        s.StaffName, 
        s.MobileNo,
        s.EmailAddress,
        s.Remarks,
        s.Created, 
        s.Modified
    FROM MOM_Staff s
    INNER JOIN MOM_Department d ON s.DepartmentID = d.DepartmentID
    WHERE s.StaffID = @StaffID
END
GO

-- READ Staff (Get By Department)
CREATE  or alter PROCEDURE SP_Staff_GetByDepartment
    @DepartmentID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.StaffID, 
        s.DepartmentID,
        d.DepartmentName,
        s.StaffName, 
        s.MobileNo,
        s.EmailAddress,
        s.Remarks,
        s.Created, 
        s.Modified
    FROM MOM_Staff s
    INNER JOIN MOM_Department d ON s.DepartmentID = d.DepartmentID
    WHERE s.DepartmentID = @DepartmentID
    ORDER BY s.StaffID
END
GO

-- UPDATE Staff
CREATE  or alter PROCEDURE SP_Staff_Update
    @StaffID INT,
    @DepartmentID INT,
    @StaffName NVARCHAR(50),
    @MobileNo NVARCHAR(20),
    @EmailAddress NVARCHAR(50),
    @Remarks NVARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if department exists
    IF NOT EXISTS (SELECT 1 FROM MOM_Department WHERE DepartmentID = @DepartmentID)
    BEGIN
        RAISERROR('Department does not exist.', 16, 1)
        RETURN
    END
    
    UPDATE MOM_Staff
    SET 
        DepartmentID = @DepartmentID,
        StaffName = @StaffName,
        MobileNo = @MobileNo,
        EmailAddress = @EmailAddress,
        Remarks = @Remarks,
        Modified = GETDATE()
    WHERE StaffID = @StaffID
    
    SELECT @@ROWCOUNT AS RowsAffected
END
GO

-- DELETE Staff
CREATE or alter PROCEDURE SP_Staff_Delete
    @StaffID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if staff is referenced by meeting members
    IF EXISTS (SELECT 1 FROM MOM_MeetingMember WHERE StaffID = @StaffID)
    BEGIN
        RAISERROR('Cannot delete staff. Staff is referenced by meeting member records.', 16, 1)
        RETURN
    END
    
    DELETE FROM MOM_Staff
    WHERE StaffID = @StaffID
    
    SELECT @@ROWCOUNT AS RowsAffected
END
GO