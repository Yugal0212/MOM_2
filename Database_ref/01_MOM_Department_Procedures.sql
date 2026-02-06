USE MOM_DB
GO

-- ========================================
-- MOM_Department CRUD Stored Procedures
-- ========================================

-- CREATE department
CREATE PROCEDURE SP_Department_Create
    @DepartmentName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO MOM_Department (DepartmentName, Modified)
    VALUES (@DepartmentName, GETDATE())
    
    SELECT SCOPE_IDENTITY() AS DepartmentID
END
GO

-- READ Department (Get All)    
CREATE PROCEDURE SP_Department_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        DepartmentID, 
        DepartmentName, 
        Created, 
        Modified
    FROM MOM_Department
    ORDER BY DepartmentID
END
GO

-- READ Department (Get By ID)
CREATE PROCEDURE SP_Department_GetByID
    @DepartmentID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        DepartmentID, 
        DepartmentName, 
        Created, 
        Modified
    FROM MOM_Department
    WHERE DepartmentID = @DepartmentID
END
GO

-- UPDATE Department
CREATE PROCEDURE SP_Department_Update
    @DepartmentID INT,
    @DepartmentName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE MOM_Department
    SET 
        DepartmentName = @DepartmentName,
        Modified = GETDATE()
    WHERE DepartmentID = @DepartmentID
    
    SELECT @@ROWCOUNT AS RowsAffected
END
GO

-- DELETE Department
CREATE PROCEDURE SP_Department_Delete
    @DepartmentID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if department is referenced by staff
    IF EXISTS (SELECT 1 FROM MOM_Staff WHERE DepartmentID = @DepartmentID)
    BEGIN
        RAISERROR('Cannot delete department. It is referenced by staff records.', 16, 1)
        RETURN
    END
    
    -- Check if department is referenced by meetings
    IF EXISTS (SELECT 1 FROM MOM_Meetings WHERE DepartmentID = @DepartmentID)
    BEGIN
        RAISERROR('Cannot delete department. It is referenced by meeting records.', 16, 1)
        RETURN
    END
    
    DELETE FROM MOM_Department
    WHERE DepartmentID = @DepartmentID
    
    SELECT @@ROWCOUNT AS RowsAffected
END
GO