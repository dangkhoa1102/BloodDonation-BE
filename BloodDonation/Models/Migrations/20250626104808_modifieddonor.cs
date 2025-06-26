using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
<<<<<<<< HEAD:BloodDonation/Models/Migrations/20250626104808_modifieddonor.cs
    public partial class modifieddonor : Migration
========
    public partial class AddHealthCheckModifyDonor : Migration
>>>>>>>> develop:BloodDonation/Models/Migrations/20250625154835_AddHealthCheck-ModifyDonor.cs
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BloodComponent",
                columns: table => new
                {
                    componentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    componentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    compatibilityRules = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    storageRequirements = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
<<<<<<<< HEAD:BloodDonation/Models/Migrations/20250626104808_modifieddonor.cs
                    table.PrimaryKey("PK__BloodCom__667AC126621C9E40", x => x.componentID);
========
                    table.PrimaryKey("PK__BloodCom__667AC126F26CABC1", x => x.componentID);
>>>>>>>> develop:BloodDonation/Models/Migrations/20250625154835_AddHealthCheck-ModifyDonor.cs
                });

            migrationBuilder.CreateTable(
                name: "BloodType",
                columns: table => new
                {
                    bloodTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    aboType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    rhFactor = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
<<<<<<<< HEAD:BloodDonation/Models/Migrations/20250626104808_modifieddonor.cs
                    table.PrimaryKey("PK__BloodTyp__C879D79464B25B49", x => x.bloodTypeID);
========
                    table.PrimaryKey("PK__BloodTyp__C879D794BE3A5040", x => x.bloodTypeID);
>>>>>>>> develop:BloodDonation/Models/Migrations/20250625154835_AddHealthCheck-ModifyDonor.cs
                });

            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    documentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    uploadDate = table.Column<DateOnly>(type: "date", nullable: true),
                    fileType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    downloadCount = table.Column<int>(type: "int", nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
<<<<<<<< HEAD:BloodDonation/Models/Migrations/20250626104808_modifieddonor.cs
                    table.PrimaryKey("PK__Document__EFAAADE5A994DC1C", x => x.documentID);
========
                    table.PrimaryKey("PK__Document__EFAAADE5699BA8E4", x => x.documentID);
>>>>>>>> develop:BloodDonation/Models/Migrations/20250625154835_AddHealthCheck-ModifyDonor.cs
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    locationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    latitude = table.Column<double>(type: "float", nullable: true),
                    longitude = table.Column<double>(type: "float", nullable: true),
                    address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    city = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    district = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
<<<<<<<< HEAD:BloodDonation/Models/Migrations/20250626104808_modifieddonor.cs
                    table.PrimaryKey("PK__Location__30646B0EE9A2DD78", x => x.locationID);
========
                    table.PrimaryKey("PK__Location__30646B0E7DF4F6FC", x => x.locationID);
>>>>>>>> develop:BloodDonation/Models/Migrations/20250625154835_AddHealthCheck-ModifyDonor.cs
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    userID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    userIdCard = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    fullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    dateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
<<<<<<<< HEAD:BloodDonation/Models/Migrations/20250626104808_modifieddonor.cs
                    table.PrimaryKey("PK__users__CB9A1CDF65F7D8DE", x => x.userID);
========
                    table.PrimaryKey("PK__users__CB9A1CDFB74F38F1", x => x.userID);
>>>>>>>> develop:BloodDonation/Models/Migrations/20250625154835_AddHealthCheck-ModifyDonor.cs
                });

            migrationBuilder.CreateTable(
                name: "Blog",
                columns: table => new
                {
                    blogID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    authorID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    publishDate = table.Column<DateOnly>(type: "date", nullable: true),
                    category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    viewCount = table.Column<int>(type: "int", nullable: true, defaultValue: 0)
                },
                constraints: table =>
                {
<<<<<<<< HEAD:BloodDonation/Models/Migrations/20250626104808_modifieddonor.cs
                    table.PrimaryKey("PK__Blog__FA0AA70D669396E2", x => x.blogID);
========
                    table.PrimaryKey("PK__Blog__FA0AA70DF494B3C9", x => x.blogID);
>>>>>>>> develop:BloodDonation/Models/Migrations/20250625154835_AddHealthCheck-ModifyDonor.cs
                    table.ForeignKey(
                        name: "FK_Blog_Author",
                        column: x => x.authorID,
                        principalTable: "users",
                        principalColumn: "userID");
                });

            migrationBuilder.CreateTable(
                name: "BloodRecipient",
                columns: table => new
                {
                    recipientID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    userID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    urgencyLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    medicalCondition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    contactInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
<<<<<<<< HEAD:BloodDonation/Models/Migrations/20250626104808_modifieddonor.cs
                    table.PrimaryKey("PK__BloodRec__A9B8B542CA122FE7", x => x.recipientID);
========
                    table.PrimaryKey("PK__BloodRec__A9B8B54217264492", x => x.recipientID);
>>>>>>>> develop:BloodDonation/Models/Migrations/20250625154835_AddHealthCheck-ModifyDonor.cs
                    table.ForeignKey(
                        name: "FK_BloodRecipient_User",
                        column: x => x.userID,
                        principalTable: "users",
                        principalColumn: "userID");
                });

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    reportID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    reportType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    generatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    generationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    parameters = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
<<<<<<<< HEAD:BloodDonation/Models/Migrations/20250626104808_modifieddonor.cs
                    table.PrimaryKey("PK__Report__1C9B4ECD2BD40B30", x => x.reportID);
========
                    table.PrimaryKey("PK__Report__1C9B4ECDF896B590", x => x.reportID);
>>>>>>>> develop:BloodDonation/Models/Migrations/20250625154835_AddHealthCheck-ModifyDonor.cs
                    table.ForeignKey(
                        name: "FK_Report_User",
                        column: x => x.generatedBy,
                        principalTable: "users",
                        principalColumn: "userID");
                });

            migrationBuilder.CreateTable(
                name: "BloodRequest",
                columns: table => new
                {
                    requestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    recipientID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    bloodTypeRequired = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    quantityNeeded = table.Column<int>(type: "int", nullable: true),
                    urgencyLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    requestDate = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
<<<<<<<< HEAD:BloodDonation/Models/Migrations/20250626104808_modifieddonor.cs
                    table.PrimaryKey("PK__BloodReq__E3C5DE51755946BB", x => x.requestID);
========
                    table.PrimaryKey("PK__BloodReq__E3C5DE512BE91403", x => x.requestID);
>>>>>>>> develop:BloodDonation/Models/Migrations/20250625154835_AddHealthCheck-ModifyDonor.cs
                    table.ForeignKey(
                        name: "FK_BloodRequest_BloodType",
                        column: x => x.bloodTypeRequired,
                        principalTable: "BloodType",
                        principalColumn: "bloodTypeID");
                    table.ForeignKey(
                        name: "FK_BloodRequest_Recipient",
                        column: x => x.recipientID,
                        principalTable: "BloodRecipient",
                        principalColumn: "recipientID");
                });

            migrationBuilder.CreateTable(
                name: "BloodDonation",
                columns: table => new
                {
                    donationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    donorID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    requestID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    donationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    certificateID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
<<<<<<<< HEAD:BloodDonation/Models/Migrations/20250626104808_modifieddonor.cs
                    table.PrimaryKey("PK__BloodDon__F7F4F433645EE0FF", x => x.donationID);
========
                    table.PrimaryKey("PK__BloodDon__F7F4F433B63F6689", x => x.donationID);
>>>>>>>> develop:BloodDonation/Models/Migrations/20250625154835_AddHealthCheck-ModifyDonor.cs
                    table.ForeignKey(
                        name: "FK_BloodDonation_Request",
                        column: x => x.requestID,
                        principalTable: "BloodRequest",
                        principalColumn: "requestID");
                });

            migrationBuilder.CreateTable(
                name: "BloodUnit",
                columns: table => new
                {
                    unitID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    donationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    bloodTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    componentType = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    expiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 450),
                    requestID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
<<<<<<<< HEAD:BloodDonation/Models/Migrations/20250626104808_modifieddonor.cs
                    table.PrimaryKey("PK__BloodUni__55D79215BA01BA5A", x => x.unitID);
========
                    table.PrimaryKey("PK__BloodUni__55D79215DA87E384", x => x.unitID);
>>>>>>>> develop:BloodDonation/Models/Migrations/20250625154835_AddHealthCheck-ModifyDonor.cs
                    table.ForeignKey(
                        name: "FK_BloodUnit_BloodType",
                        column: x => x.bloodTypeID,
                        principalTable: "BloodType",
                        principalColumn: "bloodTypeID");
                    table.ForeignKey(
                        name: "FK_BloodUnit_Component",
                        column: x => x.componentType,
                        principalTable: "BloodComponent",
                        principalColumn: "componentID");
                    table.ForeignKey(
                        name: "FK_BloodUnit_Donation",
                        column: x => x.donationID,
                        principalTable: "BloodDonation",
                        principalColumn: "donationID");
                    table.ForeignKey(
                        name: "FK_BloodUnit_Request",
                        column: x => x.requestID,
                        principalTable: "BloodRequest",
                        principalColumn: "requestID");
                });

            migrationBuilder.CreateTable(
                name: "Certificate",
                columns: table => new
                {
                    certificateID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    donorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    donationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    staffID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    certificateNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    issueDate = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    certificateType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Blood Donation"),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    lastModified = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
<<<<<<<< HEAD:BloodDonation/Models/Migrations/20250626104808_modifieddonor.cs
                    table.PrimaryKey("PK__Certific__A15CBE8E683D5BA0", x => x.certificateID);
========
                    table.PrimaryKey("PK__Certific__A15CBE8E388020DB", x => x.certificateID);
>>>>>>>> develop:BloodDonation/Models/Migrations/20250625154835_AddHealthCheck-ModifyDonor.cs
                    table.ForeignKey(
                        name: "FK_Certificate_Donation",
                        column: x => x.donationID,
                        principalTable: "BloodDonation",
                        principalColumn: "donationID");
                    table.ForeignKey(
                        name: "FK_Certificate_Staff",
                        column: x => x.staffID,
                        principalTable: "users",
                        principalColumn: "userID");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    notificationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    userID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    notificationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    sendDate = table.Column<DateOnly>(type: "date", nullable: true),
                    isRead = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    scheduledDate = table.Column<DateOnly>(type: "date", nullable: true),
                    certificateID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
<<<<<<<< HEAD:BloodDonation/Models/Migrations/20250626104808_modifieddonor.cs
                    table.PrimaryKey("PK__Notifica__4BA5CE898AE7CC61", x => x.notificationID);
========
                    table.PrimaryKey("PK__Notifica__4BA5CE8910416400", x => x.notificationID);
>>>>>>>> develop:BloodDonation/Models/Migrations/20250625154835_AddHealthCheck-ModifyDonor.cs
                    table.ForeignKey(
                        name: "FK_Notification_Certificate",
                        column: x => x.certificateID,
                        principalTable: "Certificate",
                        principalColumn: "certificateID");
                    table.ForeignKey(
                        name: "FK_Notification_User",
                        column: x => x.userID,
                        principalTable: "users",
                        principalColumn: "userID");
                });

            migrationBuilder.CreateTable(
                name: "DonationHistory",
                columns: table => new
                {
                    historyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    donorID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    donationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    healthStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nextEligibleDate = table.Column<DateOnly>(type: "date", nullable: true),
                    certificateID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
<<<<<<<< HEAD:BloodDonation/Models/Migrations/20250626104808_modifieddonor.cs
                    table.PrimaryKey("PK__Donation__19BDBDB3C2244AE2", x => x.historyID);
========
                    table.PrimaryKey("PK__Donation__19BDBDB36E09FAAA", x => x.historyID);
>>>>>>>> develop:BloodDonation/Models/Migrations/20250625154835_AddHealthCheck-ModifyDonor.cs
                    table.ForeignKey(
                        name: "FK_DonationHistory_Certificate",
                        column: x => x.certificateID,
                        principalTable: "Certificate",
                        principalColumn: "certificateID");
                });

            migrationBuilder.CreateTable(
                name: "Donor",
                columns: table => new
                {
                    donorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    userID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    bloodTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    isAvailable = table.Column<bool>(type: "bit", nullable: true),
                    lastDonationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    nextEligibleDate = table.Column<DateOnly>(type: "date", nullable: true),
                    locationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    closestFacilityID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CurrentMedications = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
<<<<<<<< HEAD:BloodDonation/Models/Migrations/20250626104808_modifieddonor.cs
                    table.PrimaryKey("PK__Donor__A595D73150F694D9", x => x.donorID);
========
                    table.PrimaryKey("PK__Donor__A595D73182E52831", x => x.donorID);
>>>>>>>> develop:BloodDonation/Models/Migrations/20250625154835_AddHealthCheck-ModifyDonor.cs
                    table.ForeignKey(
                        name: "FK_Donor_BloodType",
                        column: x => x.bloodTypeID,
                        principalTable: "BloodType",
                        principalColumn: "bloodTypeID");
                    table.ForeignKey(
                        name: "FK_Donor_Location",
                        column: x => x.locationID,
                        principalTable: "Location",
                        principalColumn: "locationID");
                    table.ForeignKey(
                        name: "FK_Donor_User",
                        column: x => x.userID,
                        principalTable: "users",
                        principalColumn: "userID");
                });

            migrationBuilder.CreateTable(
                name: "HealthCheck",
                columns: table => new
                {
                    healthCheckID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    donorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    weight = table.Column<double>(type: "float", nullable: true),
                    height = table.Column<double>(type: "float", nullable: true),
                    heartRate = table.Column<int>(type: "int", nullable: true),
                    temperature = table.Column<double>(type: "float", nullable: true),
                    blood_pressure = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    medicalHistory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    currentMedications = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    allergies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HealthCheck_Date = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
<<<<<<<< HEAD:BloodDonation/Models/Migrations/20250626104808_modifieddonor.cs
                    HealthCheck_Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Completed")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HealthCh__31AFA16E57E14455", x => x.healthCheckID);
========
                    HealthCheck_Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HealthCh__31AFA16E0C212F82", x => x.healthCheckID);
>>>>>>>> develop:BloodDonation/Models/Migrations/20250625154835_AddHealthCheck-ModifyDonor.cs
                    table.ForeignKey(
                        name: "FK_HealthCheck_Donor",
                        column: x => x.donorID,
                        principalTable: "Donor",
                        principalColumn: "donorID");
                });

            migrationBuilder.CreateTable(
                name: "MedicalFacility",
                columns: table => new
                {
                    facilityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    facilityName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    capacity = table.Column<int>(type: "int", nullable: true),
                    specialization = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    coordinates = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    closestDonorID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
<<<<<<<< HEAD:BloodDonation/Models/Migrations/20250626104808_modifieddonor.cs
                    table.PrimaryKey("PK__MedicalF__AA54818459FDEADA", x => x.facilityID);
========
                    table.PrimaryKey("PK__MedicalF__AA5481849741A8E3", x => x.facilityID);
>>>>>>>> develop:BloodDonation/Models/Migrations/20250625154835_AddHealthCheck-ModifyDonor.cs
                    table.ForeignKey(
                        name: "FK_MedicalFacility_Donor",
                        column: x => x.closestDonorID,
                        principalTable: "Donor",
                        principalColumn: "donorID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blog_Author",
                table: "Blog",
                column: "authorID");

            migrationBuilder.CreateIndex(
                name: "IX_BloodDonation_Certificate",
                table: "BloodDonation",
                column: "certificateID");

            migrationBuilder.CreateIndex(
                name: "IX_BloodDonation_donorID",
                table: "BloodDonation",
                column: "donorID");

            migrationBuilder.CreateIndex(
                name: "IX_BloodDonation_requestID",
                table: "BloodDonation",
                column: "requestID");

            migrationBuilder.CreateIndex(
                name: "IX_BloodRecipient_userID",
                table: "BloodRecipient",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_BloodRequest_BloodType",
                table: "BloodRequest",
                column: "bloodTypeRequired");

            migrationBuilder.CreateIndex(
                name: "IX_BloodRequest_recipientID",
                table: "BloodRequest",
                column: "recipientID");

            migrationBuilder.CreateIndex(
                name: "IX_BloodRequest_Status",
                table: "BloodRequest",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_BloodUnit_BloodType",
                table: "BloodUnit",
                column: "bloodTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_BloodUnit_componentType",
                table: "BloodUnit",
                column: "componentType");

            migrationBuilder.CreateIndex(
                name: "IX_BloodUnit_donationID",
                table: "BloodUnit",
                column: "donationID");

            migrationBuilder.CreateIndex(
                name: "IX_BloodUnit_ExpiryDate",
                table: "BloodUnit",
                column: "expiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_BloodUnit_Request",
                table: "BloodUnit",
                column: "requestID");

            migrationBuilder.CreateIndex(
                name: "IX_BloodUnit_Status",
                table: "BloodUnit",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_Donation",
                table: "Certificate",
                column: "donationID");

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_Donor",
                table: "Certificate",
                column: "donorID");

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_IssueDate",
                table: "Certificate",
                column: "issueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_Number",
                table: "Certificate",
                column: "certificateNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_Staff",
                table: "Certificate",
                column: "staffID");

            migrationBuilder.CreateIndex(
<<<<<<<< HEAD:BloodDonation/Models/Migrations/20250626104808_modifieddonor.cs
                name: "UQ__Certific__410CE51207A82C52",
========
                name: "UQ__Certific__410CE51220B3E296",
>>>>>>>> develop:BloodDonation/Models/Migrations/20250625154835_AddHealthCheck-ModifyDonor.cs
                table: "Certificate",
                column: "certificateNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DonationHistory_Certificate",
                table: "DonationHistory",
                column: "certificateID");

            migrationBuilder.CreateIndex(
                name: "IX_DonationHistory_donorID",
                table: "DonationHistory",
                column: "donorID");

            migrationBuilder.CreateIndex(
                name: "IX_Donor_BloodType",
                table: "Donor",
                column: "bloodTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Donor_closestFacilityID",
                table: "Donor",
                column: "closestFacilityID");

            migrationBuilder.CreateIndex(
                name: "IX_Donor_Location",
                table: "Donor",
                column: "locationID");

            migrationBuilder.CreateIndex(
                name: "IX_Donor_User",
                table: "Donor",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_HealthCheck_Date",
                table: "HealthCheck",
                column: "HealthCheck_Date");

            migrationBuilder.CreateIndex(
                name: "IX_HealthCheck_Donor",
                table: "HealthCheck",
                column: "donorID");

            migrationBuilder.CreateIndex(
                name: "IX_HealthCheck_Status",
                table: "HealthCheck",
                column: "HealthCheck_Status");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalFacility_closestDonorID",
                table: "MedicalFacility",
                column: "closestDonorID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_Certificate",
                table: "Notification",
                column: "certificateID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_IsRead",
                table: "Notification",
                column: "isRead");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_User",
                table: "Notification",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_Report_GeneratedBy",
                table: "Report",
                column: "generatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_BloodDonation_Certificate",
                table: "BloodDonation",
                column: "certificateID",
                principalTable: "Certificate",
                principalColumn: "certificateID");

            migrationBuilder.AddForeignKey(
                name: "FK_BloodDonation_Donor",
                table: "BloodDonation",
                column: "donorID",
                principalTable: "Donor",
                principalColumn: "donorID");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificate_Donor",
                table: "Certificate",
                column: "donorID",
                principalTable: "Donor",
                principalColumn: "donorID");

            migrationBuilder.AddForeignKey(
                name: "FK_DonationHistory_Donor",
                table: "DonationHistory",
                column: "donorID",
                principalTable: "Donor",
                principalColumn: "donorID");

            migrationBuilder.AddForeignKey(
                name: "FK_Donor_MedicalFacility",
                table: "Donor",
                column: "closestFacilityID",
                principalTable: "MedicalFacility",
                principalColumn: "facilityID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BloodRecipient_User",
                table: "BloodRecipient");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificate_Staff",
                table: "Certificate");

            migrationBuilder.DropForeignKey(
                name: "FK_Donor_User",
                table: "Donor");

            migrationBuilder.DropForeignKey(
                name: "FK_BloodDonation_Certificate",
                table: "BloodDonation");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalFacility_Donor",
                table: "MedicalFacility");

            migrationBuilder.DropTable(
                name: "Blog");

            migrationBuilder.DropTable(
                name: "BloodUnit");

            migrationBuilder.DropTable(
                name: "Document");

            migrationBuilder.DropTable(
                name: "DonationHistory");

            migrationBuilder.DropTable(
                name: "HealthCheck");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "BloodComponent");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "Certificate");

            migrationBuilder.DropTable(
                name: "BloodDonation");

            migrationBuilder.DropTable(
                name: "BloodRequest");

            migrationBuilder.DropTable(
                name: "BloodRecipient");

            migrationBuilder.DropTable(
                name: "Donor");

            migrationBuilder.DropTable(
                name: "BloodType");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "MedicalFacility");
        }
    }
}
