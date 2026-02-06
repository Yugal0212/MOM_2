USE MOM_DB
GO

-- ========================================
-- MOM_Meetings CRUD Stored Procedures
-- ========================================

-- CREATE Meeting
CREATE PROCEDURE SP_Meeting_Create
    @MeetingDate DATETIME,
    @MeetingVenueID INT,
    @MeetingTypeID INT,
    @DepartmentID INT,
    @MeetingDescription NVARCHAR(250) = NULL,
    @DocumentPath NVARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validate foreign key references
    IF NOT EXISTS (SELECT 1 FROM MOM_MeetingVenue WHERE MeetingVenueID = @MeetingVenueID)
    BEGIN
        RAISERROR('Meeting venue does not exist.', 16, 1)
        RETURN
    END
    
    IF NOT EXISTS (SELECT 1 FROM MOM_MeetingType WHERE MeetingTypeID = @MeetingTypeID)
    BEGIN
        RAISERROR('Meeting type does not exist.', 16, 1)
        RETURN
    END
    
    IF NOT EXISTS (SELECT 1 FROM MOM_Department WHERE DepartmentID = @DepartmentID)
    BEGIN
        RAISERROR('Department does not exist.', 16, 1)
        RETURN
    END
    
    INSERT INTO MOM_Meetings (
        MeetingDate, MeetingVenueID, MeetingTypeID, DepartmentID,
        MeetingDescription, DocumentPath, Modified, IsCancelled
    )
    VALUES (
        @MeetingDate, @MeetingVenueID, @MeetingTypeID, @DepartmentID,
        @MeetingDescription, @DocumentPath, GETDATE(), 0
    )
    
    SELECT SCOPE_IDENTITY() AS MeetingID
END
GO

-- READ Meetings (Get All)
CREATE PROCEDURE SP_Meeting_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        m.MeetingID,
        m.MeetingDate,
        m.MeetingVenueID,
        mv.MeetingVenueName,
        m.MeetingTypeID,
        mt.MeetingTypeName,
        m.DepartmentID,
        d.DepartmentName,
        m.MeetingDescription,
        m.DocumentPath,
        m.Created,
        m.Modified,
        m.IsCancelled,
        m.CancellationDateTime,
        m.CancellationReason
    FROM MOM_Meetings m
    INNER JOIN MOM_MeetingVenue mv ON m.MeetingVenueID = mv.MeetingVenueID
    INNER JOIN MOM_MeetingType mt ON m.MeetingTypeID = mt.MeetingTypeID
    INNER JOIN MOM_Department d ON m.DepartmentID = d.DepartmentID
    ORDER BY m.MeetingID
END
GO

-- READ Meeting (Get By ID)
CREATE PROCEDURE SP_Meeting_GetByID
    @MeetingID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        m.MeetingID,
        m.MeetingDate,
        m.MeetingVenueID,
        mv.MeetingVenueName,
        m.MeetingTypeID,
        mt.MeetingTypeName,
        m.DepartmentID,
        d.DepartmentName,
        m.MeetingDescription,
        m.DocumentPath,
        m.Created,
        m.Modified,
        m.IsCancelled,
        m.CancellationDateTime,
        m.CancellationReason
    FROM MOM_Meetings m
    INNER JOIN MOM_MeetingVenue mv ON m.MeetingVenueID = mv.MeetingVenueID
    INNER JOIN MOM_MeetingType mt ON m.MeetingTypeID = mt.MeetingTypeID
    INNER JOIN MOM_Department d ON m.DepartmentID = d.DepartmentID
    WHERE m.MeetingID = @MeetingID
END
GO

-- READ Meetings (Get By Department)
CREATE PROCEDURE SP_Meeting_GetByDepartment
    @DepartmentID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        m.MeetingID,
        m.MeetingDate,
        m.MeetingVenueID,
        mv.MeetingVenueName,
        m.MeetingTypeID,
        mt.MeetingTypeName,
        m.DepartmentID,
        d.DepartmentName,
        m.MeetingDescription,
        m.DocumentPath,
        m.Created,
        m.Modified,
        m.IsCancelled,
        m.CancellationDateTime,
        m.CancellationReason
    FROM MOM_Meetings m
    INNER JOIN MOM_MeetingVenue mv ON m.MeetingVenueID = mv.MeetingVenueID
    INNER JOIN MOM_MeetingType mt ON m.MeetingTypeID = mt.MeetingTypeID
    INNER JOIN MOM_Department d ON m.DepartmentID = d.DepartmentID
    WHERE m.DepartmentID = @DepartmentID
    ORDER BY m.MeetingID
END
GO

-- READ Meetings (Get Upcoming)
CREATE PROCEDURE SP_Meeting_GetUpcoming
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        m.MeetingID,
        m.MeetingDate,
        m.MeetingVenueID,
        mv.MeetingVenueName,
        m.MeetingTypeID,
        mt.MeetingTypeName,
        m.DepartmentID,
        d.DepartmentName,
        m.MeetingDescription,
        m.DocumentPath,
        m.Created,
        m.Modified,
        m.IsCancelled,
        m.CancellationDateTime,
        m.CancellationReason
    FROM MOM_Meetings m
    INNER JOIN MOM_MeetingVenue mv ON m.MeetingVenueID = mv.MeetingVenueID
    INNER JOIN MOM_MeetingType mt ON m.MeetingTypeID = mt.MeetingTypeID
    INNER JOIN MOM_Department d ON m.DepartmentID = d.DepartmentID
    WHERE m.MeetingDate >= GETDATE() AND (m.IsCancelled IS NULL OR m.IsCancelled = 0)
    ORDER BY m.MeetingID
END
GO

-- UPDATE Meeting
CREATE PROCEDURE SP_Meeting_Update
    @MeetingID INT,
    @MeetingDate DATETIME,
    @MeetingVenueID INT,
    @MeetingTypeID INT,
    @DepartmentID INT,
    @MeetingDescription NVARCHAR(250) = NULL,
    @DocumentPath NVARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validate foreign key references
    IF NOT EXISTS (SELECT 1 FROM MOM_MeetingVenue WHERE MeetingVenueID = @MeetingVenueID)
    BEGIN
        RAISERROR('Meeting venue does not exist.', 16, 1)
        RETURN
    END
    
    IF NOT EXISTS (SELECT 1 FROM MOM_MeetingType WHERE MeetingTypeID = @MeetingTypeID)
    BEGIN
        RAISERROR('Meeting type does not exist.', 16, 1)
        RETURN
    END
    
    IF NOT EXISTS (SELECT 1 FROM MOM_Department WHERE DepartmentID = @DepartmentID)
    BEGIN
        RAISERROR('Department does not exist.', 16, 1)
        RETURN
    END
    
    UPDATE MOM_Meetings
    SET 
        MeetingDate = @MeetingDate,
        MeetingVenueID = @MeetingVenueID,
        MeetingTypeID = @MeetingTypeID,
        DepartmentID = @DepartmentID,
        MeetingDescription = @MeetingDescription,
        DocumentPath = @DocumentPath,
        Modified = GETDATE()
    WHERE MeetingID = @MeetingID
    
    SELECT @@ROWCOUNT AS RowsAffected
END
GO

-- CANCEL Meeting
CREATE PROCEDURE SP_Meeting_Cancel
    @MeetingID INT,
    @CancellationReason NVARCHAR(250)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE MOM_Meetings
    SET 
        IsCancelled = 1,
        CancellationDateTime = GETDATE(),
        CancellationReason = @CancellationReason,
        Modified = GETDATE()
    WHERE MeetingID = @MeetingID
    
    SELECT @@ROWCOUNT AS RowsAffected
END
GO

-- RESTORE Meeting (Un-cancel)
CREATE PROCEDURE SP_Meeting_Restore
    @MeetingID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE MOM_Meetings
    SET 
        IsCancelled = 0,
        CancellationDateTime = NULL,
        CancellationReason = NULL,
        Modified = GETDATE()
    WHERE MeetingID = @MeetingID
    
    SELECT @@ROWCOUNT AS RowsAffected
END
GO

-- DELETE Meeting
CREATE PROCEDURE SP_Meeting_Delete
    @MeetingID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if meeting has members
    IF EXISTS (SELECT 1 FROM MOM_MeetingMember WHERE MeetingID = @MeetingID)
    BEGIN
        RAISERROR('Cannot delete meeting. It has associated meeting members.', 16, 1)
        RETURN
    END
    
    DELETE FROM MOM_Meetings
    WHERE MeetingID = @MeetingID
    
    SELECT @@ROWCOUNT AS RowsAffected
END
GO