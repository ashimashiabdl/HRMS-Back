using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Identity.Infrastructure.Migrations.CustomIdentity
{
    /// <inheritdoc />
    public partial class AddIPAddressAndUserAgentToUserLoginHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // حذف index که به ستون IPAddress وابسته است
            migrationBuilder.DropIndex(
                name: "IX_User_Login_History_AspNetUserId_IsSuccess_CreateDate",
                schema: "Identity",
                table: "User_Login_History");

            // تغییر نوع ستون IPAddress
            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "Identity",
                table: "User_Login_History",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            // افزودن ستون UserAgent
            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                schema: "Identity",
                table: "User_Login_History",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            // ایجاد مجدد index با INCLUDE IPAddress (مطابق با اسکریپت موجود)
            // استفاده از SQL مستقیم چون EF Core از INCLUDE در CreateIndex پشتیبانی نمی‌کند
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_User_Login_History_AspNetUserId_IsSuccess_CreateDate' 
                               AND object_id = OBJECT_ID('Identity.User_Login_History'))
                BEGIN
                    CREATE NONCLUSTERED INDEX [IX_User_Login_History_AspNetUserId_IsSuccess_CreateDate]
                    ON [Identity].[User_Login_History] ([AspNetUserId], [IsSuccess], [CreateDate] DESC)
                    INCLUDE ([IPAddress], [IsDeleted])
                    WITH (ONLINE = ON, FILLFACTOR = 90);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // حذف index
            migrationBuilder.DropIndex(
                name: "IX_User_Login_History_AspNetUserId_IsSuccess_CreateDate",
                schema: "Identity",
                table: "User_Login_History");

            // حذف ستون UserAgent
            migrationBuilder.DropColumn(
                name: "UserAgent",
                schema: "Identity",
                table: "User_Login_History");

            // بازگرداندن نوع ستون IPAddress به حالت قبلی
            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                schema: "Identity",
                table: "User_Login_History",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(45)",
                oldMaxLength: 45,
                oldNullable: true);

            // ایجاد مجدد index با INCLUDE IPAddress
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_User_Login_History_AspNetUserId_IsSuccess_CreateDate' 
                               AND object_id = OBJECT_ID('Identity.User_Login_History'))
                BEGIN
                    CREATE NONCLUSTERED INDEX [IX_User_Login_History_AspNetUserId_IsSuccess_CreateDate]
                    ON [Identity].[User_Login_History] ([AspNetUserId], [IsSuccess], [CreateDate] DESC)
                    INCLUDE ([IPAddress], [IsDeleted])
                    WITH (ONLINE = ON, FILLFACTOR = 90);
                END
            ");
        }
    }
}
