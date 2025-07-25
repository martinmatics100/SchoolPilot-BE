using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolPilot.Data.Postgres.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    OwnerTitle = table.Column<int>(type: "integer", nullable: false),
                    SchooOwnerFirstName = table.Column<string>(type: "text", nullable: true),
                    SchoolOwnerLastName = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    Flags = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLogDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActivityLogId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyDisplayName = table.Column<string>(type: "text", nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: false),
                    NewValue = table.Column<string>(type: "text", nullable: false),
                    EntryType = table.Column<int>(type: "integer", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubEntityType = table.Column<int>(type: "integer", nullable: true),
                    SubEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActionType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DomainId = table.Column<Guid>(type: "uuid", nullable: false),
                    DomainType = table.Column<int>(type: "integer", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    ActionId = table.Column<int>(type: "integer", nullable: false),
                    Summary = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    EntityName = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    ExtraDetails = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressLine1 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AddressLine2 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ZipCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    County = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric(8,6)", nullable: true),
                    Longitude = table.Column<decimal>(type: "numeric(9,6)", nullable: true),
                    IsValidated = table.Column<bool>(type: "boolean", nullable: false),
                    IsBypassed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhoneNumbers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    Extension = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    Country = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneNumbers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Principles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Principles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProviderFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Feature = table.Column<int>(type: "integer", nullable: false),
                    ValueType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<int>(type: "integer", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolPhoneNumber",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    Extension = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    Country = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolPhoneNumber", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolSettingsPhoneNumbers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    Extension = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    Country = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolSettingsPhoneNumbers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchoolSubjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SubjectCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    StaffId = table.Column<Guid>(type: "uuid", nullable: true),
                    Level = table.Column<int[]>(type: "integer[]", maxLength: 20, nullable: false),
                    Category = table.Column<int[]>(type: "integer[]", maxLength: 50, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    IsDeprecated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolSubjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Staffs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Department = table.Column<int>(type: "integer", nullable: false),
                    Designation = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    IsDeprecated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staffs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoredFilesMetadatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    StoragePath = table.Column<string>(type: "text", nullable: false),
                    PublicUrl = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    IsDeprecated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredFilesMetadatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAffiliationPhoneNumber",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    Extension = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    Country = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAffiliationPhoneNumber", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAffiliationsHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserAffiliationId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Comments = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    Status = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAffiliationsHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDefaultSchools",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDefaultSchools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    AgencyLocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPrimaryLocation = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    IsDeprecated = table.Column<bool>(type: "boolean", nullable: false),
                    LastValidated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LoginId = table.Column<Guid>(type: "uuid", nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    InviteId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolLocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    HashedPassword = table.Column<string>(type: "text", nullable: false),
                    LoginId = table.Column<Guid>(type: "uuid", nullable: true),
                    StudentAddress_Id = table.Column<Guid>(type: "uuid", nullable: true),
                    ClassRoom = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    IsDeprecated = table.Column<bool>(type: "boolean", nullable: false),
                    LastLoginTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastLogoutTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Addresses_StudentAddress_Id",
                        column: x => x.StudentAddress_Id,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PrincipleId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Resource = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    ResourceType = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_Principles_PrincipleId",
                        column: x => x.PrincipleId,
                        principalTable: "Principles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schools",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CurrentTerm = table.Column<int>(type: "integer", nullable: false),
                    SchoolCategory = table.Column<int>(type: "integer", nullable: false),
                    SchoolType = table.Column<int>(type: "integer", nullable: false),
                    SchoolStatus = table.Column<int>(type: "integer", nullable: false),
                    ContactPersonEmail = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ContactPersonPhone_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolPrimaryAdress_Id = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    SuspendDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DisabledOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeprecated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schools_Addresses_SchoolPrimaryAdress_Id",
                        column: x => x.SchoolPrimaryAdress_Id,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Schools_SchoolPhoneNumber_ContactPersonPhone_Id",
                        column: x => x.ContactPersonPhone_Id,
                        principalTable: "SchoolPhoneNumber",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchoolSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolId = table.Column<Guid>(type: "uuid", nullable: false),
                    TasksToBypass = table.Column<int>(type: "integer", nullable: false),
                    AllowNewStudentRegistrations = table.Column<bool>(type: "boolean", nullable: false),
                    SubmitterId = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    SubmitterName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SubmitterPhoneNumber_Id = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolSettings_SchoolSettingsPhoneNumbers_SubmitterPhoneNum~",
                        column: x => x.SubmitterPhoneNumber_Id,
                        principalTable: "SchoolSettingsPhoneNumbers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAffiliations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MiddleInitial = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<int>(type: "integer", nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateOfHire = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HomePhone_Id = table.Column<Guid>(type: "uuid", nullable: true),
                    MobilePhone_Id = table.Column<Guid>(type: "uuid", nullable: true),
                    Address_Id = table.Column<Guid>(type: "uuid", nullable: true),
                    EarliestLoginTimeInMinutes = table.Column<int>(type: "integer", nullable: true),
                    AutomaticLogoutTimeInMinutes = table.Column<int>(type: "integer", nullable: true),
                    AllowWeekendAccess = table.Column<bool>(type: "boolean", nullable: false),
                    TimeZoneOffset = table.Column<short>(type: "smallint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    IsDeprecated = table.Column<bool>(type: "boolean", nullable: false),
                    AllowEmergencyAccessRequest = table.Column<bool>(type: "boolean", nullable: false),
                    AllowReadOnlyAccess = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAffiliations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAffiliations_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAffiliations_Addresses_Address_Id",
                        column: x => x.Address_Id,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserAffiliations_UserAffiliationPhoneNumber_HomePhone_Id",
                        column: x => x.HomePhone_Id,
                        principalTable: "UserAffiliationPhoneNumber",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserAffiliations_UserAffiliationPhoneNumber_MobilePhone_Id",
                        column: x => x.MobilePhone_Id,
                        principalTable: "UserAffiliationPhoneNumber",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserAffiliations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchoolLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchoolId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TimeZone = table.Column<int>(type: "integer", nullable: false),
                    Address_Id = table.Column<Guid>(type: "uuid", nullable: true),
                    PrimaryPhone_Id = table.Column<Guid>(type: "uuid", nullable: true),
                    IsMainLocation = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeprecated = table.Column<bool>(type: "boolean", nullable: false),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolLocations_Addresses_Address_Id",
                        column: x => x.Address_Id,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SchoolLocations_PhoneNumbers_PrimaryPhone_Id",
                        column: x => x.PrimaryPhone_Id,
                        principalTable: "PhoneNumbers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SchoolLocations_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPrinciples",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserAffiliationId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrincipleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPrinciples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPrinciples_Principles_PrincipleId",
                        column: x => x.PrincipleId,
                        principalTable: "Principles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPrinciples_UserAffiliations_UserAffiliationId",
                        column: x => x.UserAffiliationId,
                        principalTable: "UserAffiliations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogDetails_AccountId",
                table: "ActivityLogDetails",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogDetails_ActivityLogId",
                table: "ActivityLogDetails",
                column: "ActivityLogId");

            migrationBuilder.CreateIndex(
                name: "IDX_AccountId_UserId",
                table: "ActivityLogs",
                columns: new[] { "AccountId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IDX_DomainType_EntityType",
                table: "ActivityLogs",
                columns: new[] { "DomainType", "EntityType" });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_CreatedOn",
                table: "ActivityLogs",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_EntityId",
                table: "ActivityLogs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_AccountId",
                table: "Addresses",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IDX_PermissionPrinciple",
                table: "Permissions",
                columns: new[] { "PrincipleId", "Resource", "ResourceType", "Action" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_AccountId",
                table: "Permissions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Principles_AccountId",
                table: "Principles",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IDX_ProviderFeatureSetting_AccountId",
                table: "ProviderFeatures",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IDX_ProviderFeatureSetting_PId_Category",
                table: "ProviderFeatures",
                columns: new[] { "ProviderId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderFeatures_LocationId",
                table: "ProviderFeatures",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolLocations_AccountId",
                table: "SchoolLocations",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolLocations_Address_Id",
                table: "SchoolLocations",
                column: "Address_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolLocations_PrimaryPhone_Id",
                table: "SchoolLocations",
                column: "PrimaryPhone_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolLocations_SchoolId_Name",
                table: "SchoolLocations",
                columns: new[] { "SchoolId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "Idx_SchoolPhoneNo_AccId",
                table: "SchoolPhoneNumber",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolPhoneNumber_Type",
                table: "SchoolPhoneNumber",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Schools_AccountId",
                table: "Schools",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Schools_ContactPersonPhone_Id",
                table: "Schools",
                column: "ContactPersonPhone_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Schools_SchoolPrimaryAdress_Id",
                table: "Schools",
                column: "SchoolPrimaryAdress_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolSettings_SchoolId",
                table: "SchoolSettings",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolSettings_SubmitterPhoneNumber_Id",
                table: "SchoolSettings",
                column: "SubmitterPhoneNumber_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolSubjects_AccountId",
                table: "SchoolSubjects",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolSubjects_SchoolId",
                table: "SchoolSubjects",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_Email",
                table: "Staffs",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_SchoolAccountId",
                table: "Staffs",
                column: "SchoolAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoredFilesMetadatas_AccountId",
                table: "StoredFilesMetadatas",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoredFilesMetadatas_FileName",
                table: "StoredFilesMetadatas",
                column: "FileName");

            migrationBuilder.CreateIndex(
                name: "IX_Students_AccountId",
                table: "Students",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_SchoolId",
                table: "Students",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentAddress_Id",
                table: "Students",
                column: "StudentAddress_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAffiliations_AccountId",
                table: "UserAffiliations",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAffiliations_Address_Id",
                table: "UserAffiliations",
                column: "Address_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAffiliations_Email",
                table: "UserAffiliations",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_UserAffiliations_HomePhone_Id",
                table: "UserAffiliations",
                column: "HomePhone_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAffiliations_MobilePhone_Id",
                table: "UserAffiliations",
                column: "MobilePhone_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAffiliations_Status",
                table: "UserAffiliations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_UserAffiliations_Title",
                table: "UserAffiliations",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "UIX_UserAffiliations_UserId_AccountId",
                table: "UserAffiliations",
                columns: new[] { "UserId", "AccountId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_UserAffiliationHistory_UserAffiliationId",
                table: "UserAffiliationsHistories",
                column: "UserAffiliationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAffiliationsHistories_AccountId",
                table: "UserAffiliationsHistories",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IDX_UserDefaultProviders_UserId_AccountId",
                table: "UserDefaultSchools",
                columns: new[] { "UserId", "AccountId" });

            migrationBuilder.CreateIndex(
                name: "Idx_UserLocation_AccountId",
                table: "UserLocations",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLocations_UserId",
                table: "UserLocations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPrinciples_AccountId",
                table: "UserPrinciples",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPrinciples_PrincipleId",
                table: "UserPrinciples",
                column: "PrincipleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPrinciples_UserAffiliationId",
                table: "UserPrinciples",
                column: "UserAffiliationId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Users_LoginId",
                table: "Users",
                column: "LoginId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLogDetails");

            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "AppUserRoles");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "ProviderFeatures");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "SchoolLocations");

            migrationBuilder.DropTable(
                name: "SchoolSettings");

            migrationBuilder.DropTable(
                name: "SchoolSubjects");

            migrationBuilder.DropTable(
                name: "Staffs");

            migrationBuilder.DropTable(
                name: "StoredFilesMetadatas");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "UserAffiliationsHistories");

            migrationBuilder.DropTable(
                name: "UserDefaultSchools");

            migrationBuilder.DropTable(
                name: "UserLocations");

            migrationBuilder.DropTable(
                name: "UserPrinciples");

            migrationBuilder.DropTable(
                name: "PhoneNumbers");

            migrationBuilder.DropTable(
                name: "Schools");

            migrationBuilder.DropTable(
                name: "SchoolSettingsPhoneNumbers");

            migrationBuilder.DropTable(
                name: "Principles");

            migrationBuilder.DropTable(
                name: "UserAffiliations");

            migrationBuilder.DropTable(
                name: "SchoolPhoneNumber");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "UserAffiliationPhoneNumber");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
