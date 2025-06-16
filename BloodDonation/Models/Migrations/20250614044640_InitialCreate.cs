using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                    table.PrimaryKey("PK__BloodCom__667AC1267A572D98", x => x.componentID);
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
                    table.PrimaryKey("PK__BloodTyp__C879D794DF7754C0", x => x.bloodTypeID);
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
                    table.PrimaryKey("PK__Document__EFAAADE56699808C", x => x.documentID);
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
                    table.PrimaryKey("PK__Location__30646B0E7475C78D", x => x.locationID);
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
                    table.PrimaryKey("PK__users__CB9A1CDF1883DE01", x => x.userID);
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
                    table.PrimaryKey("PK__Blog__FA0AA70D9275A0A2", x => x.blogID);
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
                    table.PrimaryKey("PK__BloodRec__A9B8B5421A98978D", x => x.recipientID);
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
                    table.PrimaryKey("PK__Report__1C9B4ECD1C1211AF", x => x.reportID);
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
                    table.PrimaryKey("PK__BloodReq__E3C5DE518396DD28", x => x.requestID);
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
                    table.PrimaryKey("PK__BloodDon__F7F4F433F50E75C4", x => x.donationID);
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
                    quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 450)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BloodUni__55D792156DF7DE87", x => x.unitID);
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
                    table.PrimaryKey("PK__Certific__A15CBE8E306F0F2A", x => x.certificateID);
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
                    table.PrimaryKey("PK__Notifica__4BA5CE8999822DB9", x => x.notificationID);
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
                    table.PrimaryKey("PK__Donation__19BDBDB32D843D12", x => x.historyID);
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
                    weight = table.Column<double>(type: "float", nullable: true),
                    height = table.Column<double>(type: "float", nullable: true),
                    medicalHistory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isAvailable = table.Column<bool>(type: "bit", nullable: true),
                    lastDonationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    nextEligibleDate = table.Column<DateOnly>(type: "date", nullable: true),
                    locationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    closestFacilityID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Donor__A595D731BCCB1D8C", x => x.donorID);
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
                    table.PrimaryKey("PK__MedicalF__AA548184FAC7F769", x => x.facilityID);
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
                name: "UQ__Certific__410CE512ECEAED6C",
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
