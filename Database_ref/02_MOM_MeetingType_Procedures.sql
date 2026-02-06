USE MOM
GO

-- ========================================
-- MOM_MeetingType CRUD Stored Procedures
-- ========================================

-- CREATE MeetingType
CREATE or alter PROCEDURE SP_MeetingType_Create
    @MeetingTypeName NVARCHAR(100),
    @Remarks NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO MOM_MeetingType (MeetingTypeName, Remarks, Modified)
    VALUES (@MeetingTypeName, @Remarks, GETDATE())
    
    SELECT SCOPE_IDENTITY() AS MeetingTypeID
END
GO

-- READ MeetingType (Get All)
CREATE or alter PROCEDURE SP_MeetingType_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        MeetingTypeID, 
        MeetingTypeName, 
        Remarks,
        Created, 
        Modified
    FROM MOM_MeetingType
    ORDER BY MeetingTypeID
END
GO

-- READ MeetingType (Get By ID)
CREATE  or alter PROCEDURE SP_MeetingType_GetByID
    @MeetingTypeID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        MeetingTypeID, 
        MeetingTypeName, 
        Remarks,
        Created, 
        Modified
    FROM MOM_MeetingType
    WHERE MeetingTypeID = @MeetingTypeID
END
GO

-- UPDATE MeetingType
CREATE or alter PROCEDURE SP_MeetingType_Update
    @MeetingTypeID INT,
    @MeetingTypeName NVARCHAR(100),
    @Remarks NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE MOM_MeetingType
    SET 
        MeetingTypeName = @MeetingTypeName,
        Remarks = @Remarks,
        Modified = GETDATE()
    WHERE MeetingTypeID = @MeetingTypeID
    
    SELECT @@ROWCOUNT AS RowsAffected
END
GO

-- DELETE MeetingType
CREATE or alter PROCEDURE SP_MeetingType_Delete
    @MeetingTypeID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if meeting type is referenced by meetings
    IF EXISTS (SELECT 1 FROM MOM_Meetings WHERE MeetingTypeID = @MeetingTypeID)
    BEGIN
        RAISERROR('Cannot delete meeting type. It is referenced by meeting records.', 16, 1)
        RETURN
    END
    
    DELETE FROM MOM_MeetingType
    WHERE MeetingTypeID = @MeetingTypeID
    
    SELECT @@ROWCOUNT AS RowsAffected
END
GO