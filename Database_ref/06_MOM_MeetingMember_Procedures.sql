USE MOM
GO

-- ========================================
-- MOM_MeetingMember CRUD Stored Procedures
-- ========================================

-- CREATE MeetingMember
CREATE  or alter PROCEDURE SP_MeetingMember_Create
    @MeetingID INT,
    @StaffID INT,
    @IsPresent BIT,
    @Remarks NVARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validate foreign key references
    IF NOT EXISTS (SELECT 1 FROM MOM_Meetings WHERE MeetingID = @MeetingID)
    BEGIN
        RAISERROR('Meeting does not exist.', 16, 1)
        RETURN
    END
    
    IF NOT EXISTS (SELECT 1 FROM MOM_Staff WHERE StaffID = @StaffID)
    BEGIN
        RAISERROR('Staff does not exist.', 16, 1)
        RETURN
    END
    
    -- Check if staff is already added to this meeting
    IF EXISTS (SELECT 1 FROM MOM_MeetingMember WHERE MeetingID = @MeetingID AND StaffID = @StaffID)
    BEGIN
        RAISERROR('Staff is already added to this meeting.', 16, 1)
        RETURN
    END
    
    INSERT INTO MOM_MeetingMember (MeetingID, StaffID, IsPresent, Remarks, Modified)
    VALUES (@MeetingID, @StaffID, @IsPresent, @Remarks, GETDATE())
    
    SELECT SCOPE_IDENTITY() AS MeetingMemberID
END
GO

-- READ MeetingMembers (Get All by Meeting)
CREATE or alter  PROCEDURE SP_MeetingMember_GetByMeeting
    @MeetingID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        mm.MeetingMemberID,
        mm.MeetingID,
        mm.StaffID,
        s.StaffName,
        s.EmailAddress,
        s.MobileNo,
        d.DepartmentName,
        mm.IsPresent,
        mm.Remarks,
        mm.Created,
        mm.Modified
    FROM MOM_MeetingMember mm
    INNER JOIN MOM_Staff s ON mm.StaffID = s.StaffID
    INNER JOIN MOM_Department d ON s.DepartmentID = d.DepartmentID
    WHERE mm.MeetingID = @MeetingID
    ORDER BY mm.MeetingMemberID
END
GO

-- READ MeetingMember (Get By ID)
CREATE  or alter PROCEDURE SP_MeetingMember_GetByID
    @MeetingMemberID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        mm.MeetingMemberID,
        mm.MeetingID,
        mm.StaffID,
        s.StaffName,
        s.EmailAddress,
        s.MobileNo,
        d.DepartmentName,
        mm.IsPresent,
        mm.Remarks,
        mm.Created,
        mm.Modified
    FROM MOM_MeetingMember mm
    INNER JOIN MOM_Staff s ON mm.StaffID = s.StaffID
    INNER JOIN MOM_Department d ON s.DepartmentID = d.DepartmentID
    WHERE mm.MeetingMemberID = @MeetingMemberID
END
GO

-- READ MeetingMembers (Get by Staff - all meetings for a staff member)
CREATE  or alter PROCEDURE SP_MeetingMember_GetByStaff
    @StaffID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        mm.MeetingMemberID,
        mm.MeetingID,
        m.MeetingDate,
        mt.MeetingTypeName,
        mv.MeetingVenueName,
        m.MeetingDescription,
        mm.StaffID,
        s.StaffName,
        mm.IsPresent,
        mm.Remarks,
        mm.Created,
        mm.Modified
    FROM MOM_MeetingMember mm
    INNER JOIN MOM_Meetings m ON mm.MeetingID = m.MeetingID
    INNER JOIN MOM_MeetingType mt ON m.MeetingTypeID = mt.MeetingTypeID
    INNER JOIN MOM_MeetingVenue mv ON m.MeetingVenueID = mv.MeetingVenueID
    INNER JOIN MOM_Staff s ON mm.StaffID = s.StaffID
    WHERE mm.StaffID = @StaffID
    ORDER BY mm.MeetingMemberID
END
GO

-- READ MeetingMembers (Get Present/Absent by Meeting)
CREATE or alter PROCEDURE SP_MeetingMember_GetAttendanceByMeeting
    @MeetingID INT,
    @IsPresent BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        mm.MeetingMemberID,
        mm.MeetingID,
        mm.StaffID,
        s.StaffName,
        s.EmailAddress,
        s.MobileNo,
        d.DepartmentName,
        mm.IsPresent,
        mm.Remarks,
        mm.Created,
        mm.Modified
    FROM MOM_MeetingMember mm
    INNER JOIN MOM_Staff s ON mm.StaffID = s.StaffID
    INNER JOIN MOM_Department d ON s.DepartmentID = d.DepartmentID
    WHERE mm.MeetingID = @MeetingID AND mm.IsPresent = @IsPresent
    ORDER BY mm.MeetingMemberID
END
GO

-- UPDATE MeetingMember
CREATE or alter PROCEDURE SP_MeetingMember_Update
    @MeetingMemberID INT,
    @IsPresent BIT,
    @Remarks NVARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE MOM_MeetingMember
    SET 
        IsPresent = @IsPresent,
        Remarks = @Remarks,
        Modified = GETDATE()
    WHERE MeetingMemberID = @MeetingMemberID
    
    SELECT @@ROWCOUNT AS RowsAffected
END
GO

-- UPDATE MeetingMember Attendance (Bulk update for multiple members)
CREATE  or alter PROCEDURE SP_MeetingMember_UpdateAttendance
    @MeetingMemberID INT,
    @IsPresent BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE MOM_MeetingMember
    SET 
        IsPresent = @IsPresent,
        Modified = GETDATE()
    WHERE MeetingMemberID = @MeetingMemberID
    
    SELECT @@ROWCOUNT AS RowsAffected
END
GO

-- DELETE MeetingMember
CREATE or alter PROCEDURE SP_MeetingMember_Delete
    @MeetingMemberID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM MOM_MeetingMember
    WHERE MeetingMemberID = @MeetingMemberID
    
    SELECT @@ROWCOUNT AS RowsAffected
END
GO

-- DELETE All MeetingMembers for a specific meeting
CREATE  or alter PROCEDURE SP_MeetingMember_DeleteByMeeting
    @MeetingID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM MOM_MeetingMember
    WHERE MeetingID = @MeetingID
    
    SELECT @@ROWCOUNT AS RowsAffected
END
GO

-- ADD Multiple Staff to Meeting (Bulk insert)
CREATE or alter PROCEDURE SP_MeetingMember_AddMultipleStaff
    @MeetingID INT,
    @StaffIDs NVARCHAR(MAX) -- Comma-separated staff IDs like "1,2,3,4"
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validate meeting exists
    IF NOT EXISTS (SELECT 1 FROM MOM_Meetings WHERE MeetingID = @MeetingID)
    BEGIN
        RAISERROR('Meeting does not exist.', 16, 1)
        RETURN
    END
    
    -- Create a table variable to hold staff IDs
    DECLARE @StaffIDTable TABLE (StaffID INT)
    
    -- Parse the comma-separated string into table
    INSERT INTO @StaffIDTable (StaffID)
    SELECT CAST(value AS INT)
    FROM STRING_SPLIT(@StaffIDs, ',')
    WHERE TRIM(value) != ''
    
    -- Insert only staff that are not already in the meeting
    INSERT INTO MOM_MeetingMember (MeetingID, StaffID, IsPresent, Modified)
    SELECT 
        @MeetingID,
        s.StaffID,
        0, -- Default to absent
        GETDATE()
    FROM @StaffIDTable sit
    INNER JOIN MOM_Staff s ON sit.StaffID = s.StaffID
    WHERE NOT EXISTS (
        SELECT 1 FROM MOM_MeetingMember 
        WHERE MeetingID = @MeetingID AND StaffID = s.StaffID
    )
    
    SELECT @@ROWCOUNT AS MembersAdded
END
GO