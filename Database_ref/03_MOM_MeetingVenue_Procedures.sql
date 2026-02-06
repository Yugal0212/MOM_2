USE MOM_DB
GO

-- ========================================
-- MOM_MeetingVenue CRUD Stored Procedures
-- ========================================

-- CREATE MeetingVenue
CREATE PROCEDURE SP_MeetingVenue_Create
    @MeetingVenueName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO MOM_MeetingVenue (MeetingVenueName, Modified)
    VALUES (@MeetingVenueName, GETDATE())
    
    SELECT SCOPE_IDENTITY() AS MeetingVenueID
END
GO

-- READ MeetingVenue (Get All)
CREATE PROCEDURE SP_MeetingVenue_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        MeetingVenueID, 
        MeetingVenueName, 
        Created, 
        Modified
    FROM MOM_MeetingVenue
    ORDER BY MeetingVenueID
END
GO

-- READ MeetingVenue (Get By ID)
CREATE PROCEDURE SP_MeetingVenue_GetByID
    @MeetingVenueID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        MeetingVenueID, 
        MeetingVenueName, 
        Created, 
        Modified
    FROM MOM_MeetingVenue
    WHERE MeetingVenueID = @MeetingVenueID
END
GO

-- UPDATE MeetingVenue
CREATE PROCEDURE SP_MeetingVenue_Update
    @MeetingVenueID INT,
    @MeetingVenueName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE MOM_MeetingVenue
    SET 
        MeetingVenueName = @MeetingVenueName,
        Modified = GETDATE()
    WHERE MeetingVenueID = @MeetingVenueID
    
    SELECT @@ROWCOUNT AS RowsAffected
END
GO

-- DELETE MeetingVenue
CREATE PROCEDURE SP_MeetingVenue_Delete
    @MeetingVenueID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if meeting venue is referenced by meetings
    IF EXISTS (SELECT 1 FROM MOM_Meetings WHERE MeetingVenueID = @MeetingVenueID)
    BEGIN
        RAISERROR('Cannot delete meeting venue. It is referenced by meeting records.', 16, 1)
        RETURN
    END
    
    DELETE FROM MOM_MeetingVenue
    WHERE MeetingVenueID = @MeetingVenueID
    
    SELECT @@ROWCOUNT AS RowsAffected
END
GO