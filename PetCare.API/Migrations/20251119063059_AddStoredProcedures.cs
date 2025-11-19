using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCare.API.Migrations
{
    /// <inheritdoc />
    public partial class AddStoredProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. sp_AddPet
            // Creates a new pet and returns the new PetId immediately.
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_AddPet
                    @UserId INT,
                    @Name NVARCHAR(100),
                    @Species NVARCHAR(50),
                    @Breed NVARCHAR(100),
                    @DOB DATETIME,
                    @Sex NVARCHAR(10),
                    @PhotoUrl NVARCHAR(500),
                    @Notes NVARCHAR(MAX)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    INSERT INTO Pet (UserId, Name, Species, Breed, DOB, Sex, PhotoUrl, Notes, IsDeleted)
                    VALUES (@UserId, @Name, @Species, @Breed, @DOB, @Sex, @PhotoUrl, @Notes, 0);

                    SELECT SCOPE_IDENTITY() AS NewPetId;
                END");

            // 2. sp_GetPetsByUser
            // Lists active pets for a specific user.
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_GetPetsByUser
                    @UserId INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- Selects all columns from the 'Pets' table
                    SELECT PetId, UserId, Name, Species, Breed, DOB, Sex, PhotoUrl, Notes, IsDeleted
                    FROM Pets 
                    WHERE UserId = @UserId 
                        AND IsDeleted = 0;
                END");

            // 3. sp_GetPetDetails
            // Gets pet profile + counts for Next Appt and Active Meds.
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_GetPetDetails
                    @PetId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT 
                        p.PetId, p.UserId, p.Name, p.Species, p.Breed, p.DOB, p.Sex, p.PhotoUrl, p.Notes,
                        (SELECT TOP 1 StartedDateTime FROM Appointments a 
                         WHERE a.PetId = p.PetId AND a.StartedDateTime > GETDATE() 
                         ORDER BY a.StartedDateTime ASC) AS NextAppointment,
                        (SELECT COUNT(*) FROM Medication m 
                         WHERE m.PetId = p.PetId AND (m.EndDate IS NULL OR m.EndDate >= GETDATE())) AS ActiveMedsCount
                    FROM Pets p
                    WHERE p.PetId = @PetId;
                END");

            // 4. sp_UpdatePet
            // Updates specific pet details.
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_UpdatePet
                    @PetId INT,
                    @Name NVARCHAR(100),
                    @Species NVARCHAR(50),
                    @Breed NVARCHAR(100),
                    @DOB DATETIME,
                    @Sex NVARCHAR(10),
                    @PhotoUrl NVARCHAR(500),
                    @Notes NVARCHAR(MAX)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE Pets
                    SET Name = @Name, Species = @Species, Breed = @Breed, 
                        DOB = @DOB, Sex = @Sex, PhotoUrl = @PhotoUrl, Notes = @Notes
                    WHERE PetId = @PetId;
                END");

            // 5. sp_DeletePet
            // Handles Soft Delete (marking IsDeleted=1) or Hard Delete.
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_DeletePet
                    @PetId INT,
                    @SoftDelete BIT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    IF @SoftDelete = 1
                        UPDATE Pets SET IsDeleted = 1 WHERE PetId = @PetId;
                    ELSE
                        DELETE FROM Pets WHERE PetId = @PetId;
                END");

            // 6. sp_AddAppointment
            // Adds an appointment. Maps @StartDateTime to your column 'StartedDateTime'.
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_AddAppointment
                    @PetId INT,
                    @UserId INT,
                    @Provider NVARCHAR(200),
                    @Type NVARCHAR(50),
                    @StartDateTime DATETIME,
                    @EndDateTime DATETIME,
                    @Notes NVARCHAR(MAX),
                    @Status NVARCHAR(20)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    INSERT INTO Appointments (PetId, UserId, Provider, Type, StartedDateTime, EndDateTime, Notes, Status, CreatedAt)
                    VALUES (@PetId, @UserId, @Provider, @Type, @StartDateTime, @EndDateTime, @Notes, @Status, GETDATE());
                END");

            // 7. sp_GetAppointmentsByUser
            // Gets appointments with optional date filters.
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_GetAppointmentsByUser
                    @UserId INT,
                    @FromDate DATETIME = NULL,
                    @ToDate DATETIME = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT * FROM Appointments
                    WHERE UserId = @UserId
                    AND (@FromDate IS NULL OR StartedDateTime >= @FromDate)
                    AND (@ToDate IS NULL OR StartedDateTime <= @ToDate)
                    ORDER BY StartedDateTime ASC;
                END");

            // 8. sp_UpdateAppointmentStatus
            // Updates just the status (e.g., "Confirmed", "Cancelled").
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_UpdateAppointmentStatus
                    @AppointmentId INT,
                    @Status NVARCHAR(20)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE Appointments
                    SET Status = @Status
                    WHERE AppointmentId = @AppointmentId;
                END");

            // 9. sp_AddMedication
            // Adds a medication record.
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_AddMedication
                    @PetId INT,
                    @Name NVARCHAR(200),
                    @Dosage NVARCHAR(100),
                    @Frequency NVARCHAR(100),
                    @StartDate DATETIME,
                    @EndDate DATETIME,
                    @Notes NVARCHAR(MAX)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    INSERT INTO Medication (PetId, Name, Dosage, Frequency, StartDate, EndDate, Notes)
                    VALUES (@PetId, @Name, @Dosage, @Frequency, @StartDate, @EndDate, @Notes);
                END");

            // 10. sp_GetMedicationByPet
            // Lists meds for a pet.
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_GetMedicationByPet
                    @PetId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT * FROM Medication WHERE PetId = @PetId;
                END");

            // 11. sp_AddCareLog
            // Logs daily activities. Assumes table 'CareLogs'.
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_AddCareLog
                    @PetId INT,
                    @UserId INT,
                    @Type NVARCHAR(50),
                    @Timestamp DATETIME,
                    @Details NVARCHAR(MAX),
                    @PhotoUrl NVARCHAR(500) = NULL
                AS
                BEGIN
                    SET NOCOUNT ON;
                    INSERT INTO CareLogs (PetId, UserId, Type, Timestamp, Details, PhotoUrl)
                    VALUES (@PetId, @UserId, @Type, @Timestamp, @Details, @PhotoUrl);
                END");

            // 12. sp_GetCareLogsByPet
            // Gets history with a limit (e.g., top 10).
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_GetCareLogsByPet
                    @PetId INT,
                    @Limit INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT TOP (@Limit) * FROM CareLogs 
                    WHERE PetId = @PetId 
                    ORDER BY Timestamp DESC;
                END");

            // 13. sp_AddReminder
            // Adds a reminder.
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_AddReminder
                    @PetId INT,
                    @UserId INT,
                    @Title NVARCHAR(200),
                    @Description NVARCHAR(MAX),
                    @RemindAt DATETIME,
                    @Repeat NVARCHAR(50)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    INSERT INTO Reminders (PetId, UserId, Title, Description, RemindAt, Repeat, IsSent)
                    VALUES (@PetId, @UserId, @Title, @Description, @RemindAt, @Repeat, 0);
                END");

            // 14. sp_GetRemindersByUser
            // Gets reminders, optionally including past ones.
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_GetRemindersByUser
                    @UserId INT,
                    @IncludePast BIT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    IF @IncludePast = 1
                        SELECT * FROM Reminders WHERE UserId = @UserId ORDER BY RemindAt ASC;
                    ELSE
                        SELECT * FROM Reminders WHERE UserId = @UserId AND RemindAt >= GETDATE() ORDER BY RemindAt ASC;
                END");

            // 15. sp_MarkReminderSent
            // Marks a reminder as sent/done.
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_MarkReminderSent
                    @ReminderId INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    UPDATE Reminders SET IsSent = 1 WHERE ReminderId = @ReminderId;
                END");

            // 16. sp_SharePet
            // Grants access. 
            // NOTE: Based on your model, we insert 'SharedWithUserId' into the 'OwnerUserId' column of the Access table.
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_SharePet
                    @OwnerUserId INT,
                    @PetId INT,
                    @SharedWithUserId INT,
                    @Permission NVARCHAR(100)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    -- Security Check: Ensure the person sharing (@OwnerUserId) actually owns the pet
                    IF EXISTS (SELECT 1 FROM Pets WHERE PetId = @PetId AND UserId = @OwnerUserId)
                    BEGIN
                        INSERT INTO PetsSharedAccess (PetId, OwnerUserId, Permission)
                        VALUES (@PetId, @SharedWithUserId, @Permission);
                    END
                END");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverts the database changes by dropping all procedures
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_AddPet");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetPetsByUser");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetPetDetails");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_UpdatePet");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_DeletePet");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_AddAppointment");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetAppointmentsByUser");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_UpdateAppointmentStatus");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_AddMedication");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetMedicationByPet");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_AddCareLog");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetCareLogsByPet");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_AddReminder");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetRemindersByUser");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_MarkReminderSent");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_SharePet");
        }
    }
}
