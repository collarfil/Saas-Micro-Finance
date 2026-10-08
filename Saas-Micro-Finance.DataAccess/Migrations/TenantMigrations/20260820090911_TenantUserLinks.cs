using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saas_Micro_Finance.DataAccess.Migrations.TenantMigrations
{
    /// <inheritdoc />
    public partial class TenantUserLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ============================================================
            // 1. CUSTOMERS -> ASPNET USERS
            // ============================================================

            // Drop existing Customer -> ApplicationUser FK if present.
            migrationBuilder.Sql(@"
                IF EXISTS
                (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_Customers_AspNetUsers_ApplicationUserId'
                      AND parent_object_id = OBJECT_ID('dbo.Customers')
                )
                BEGIN
                    ALTER TABLE [dbo].[Customers]
                    DROP CONSTRAINT [FK_Customers_AspNetUsers_ApplicationUserId];
                END
            ");

            // Add Customers.ApplicationUserId if missing.
            migrationBuilder.Sql(@"
                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.columns
                    WHERE object_id = OBJECT_ID('dbo.Customers')
                      AND name = 'ApplicationUserId'
                )
                BEGIN
                    ALTER TABLE [dbo].[Customers]
                    ADD [ApplicationUserId] nvarchar(450) NULL;
                END
            ");

            // Make Customers.ApplicationUserId nullable if it already exists.
            migrationBuilder.Sql(@"
                IF EXISTS
                (
                    SELECT 1
                    FROM sys.columns
                    WHERE object_id = OBJECT_ID('dbo.Customers')
                      AND name = 'ApplicationUserId'
                      AND is_nullable = 0
                )
                BEGIN
                    ALTER TABLE [dbo].[Customers]
                    ALTER COLUMN [ApplicationUserId] nvarchar(450) NULL;
                END
            ");


            // ============================================================
            // 2. EMPLOYEES -> ASPNET USERS
            // ============================================================

            // Drop existing Employee -> ApplicationUser FK if present.
            migrationBuilder.Sql(@"
                IF EXISTS
                (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_Employees_AspNetUsers_ApplicationUserId'
                      AND parent_object_id = OBJECT_ID('dbo.Employees')
                )
                BEGIN
                    ALTER TABLE [dbo].[Employees]
                    DROP CONSTRAINT [FK_Employees_AspNetUsers_ApplicationUserId];
                END
            ");

            // Add Employees.ApplicationUserId if missing.
            migrationBuilder.Sql(@"
                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.columns
                    WHERE object_id = OBJECT_ID('dbo.Employees')
                      AND name = 'ApplicationUserId'
                )
                BEGIN
                    ALTER TABLE [dbo].[Employees]
                    ADD [ApplicationUserId] nvarchar(450) NULL;
                END
            ");

            // Make Employees.ApplicationUserId nullable if it already exists.
            migrationBuilder.Sql(@"
                IF EXISTS
                (
                    SELECT 1
                    FROM sys.columns
                    WHERE object_id = OBJECT_ID('dbo.Employees')
                      AND name = 'ApplicationUserId'
                      AND is_nullable = 0
                )
                BEGIN
                    ALTER TABLE [dbo].[Employees]
                    ALTER COLUMN [ApplicationUserId] nvarchar(450) NULL;
                END
            ");


            // ============================================================
            // 3. EMPLOYEES -> BRANCHES
            // ============================================================

            // Drop existing Employee -> Branch FK if present.
            migrationBuilder.Sql(@"
                IF EXISTS
                (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_Employees_Branches_BranchId'
                      AND parent_object_id = OBJECT_ID('dbo.Employees')
                )
                BEGIN
                    ALTER TABLE [dbo].[Employees]
                    DROP CONSTRAINT [FK_Employees_Branches_BranchId];
                END
            ");

            // Add Employees.BranchId if missing.
            // Nullable is intentional because old tenants may have
            // employees that existed before Branch was introduced.
            migrationBuilder.Sql(@"
                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.columns
                    WHERE object_id = OBJECT_ID('dbo.Employees')
                      AND name = 'BranchId'
                )
                BEGIN
                    ALTER TABLE [dbo].[Employees]
                    ADD [BranchId] int NULL;
                END
            ");

            // Make BranchId nullable if it already exists.
            migrationBuilder.Sql(@"
                IF EXISTS
                (
                    SELECT 1
                    FROM sys.columns
                    WHERE object_id = OBJECT_ID('dbo.Employees')
                      AND name = 'BranchId'
                      AND is_nullable = 0
                )
                BEGIN
                    ALTER TABLE [dbo].[Employees]
                    ALTER COLUMN [BranchId] int NULL;
                END
            ");

            // Any old default value of 0 that does not represent
            // a real Branch is converted to NULL.
            migrationBuilder.Sql(@"
                IF EXISTS
                (
                    SELECT 1
                    FROM sys.columns
                    WHERE object_id = OBJECT_ID('dbo.Employees')
                      AND name = 'BranchId'
                )
                BEGIN
                    UPDATE E
                    SET E.[BranchId] = NULL
                    FROM [dbo].[Employees] E
                    WHERE E.[BranchId] IS NOT NULL
                      AND NOT EXISTS
                      (
                          SELECT 1
                          FROM [dbo].[Branches] B
                          WHERE B.[Id] = E.[BranchId]
                      );
                END
            ");


            // ============================================================
            // 4. EMPLOYEES.Street
            // ============================================================

            migrationBuilder.Sql(@"
                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.columns
                    WHERE object_id = OBJECT_ID('dbo.Employees')
                      AND name = 'Street'
                )
                BEGIN
                    ALTER TABLE [dbo].[Employees]
                    ADD [Street] nvarchar(max) NOT NULL
                        CONSTRAINT [DF_TenantUserLinks_Employees_Street]
                        DEFAULT ('');
                END
            ");


            // ============================================================
            // 5. DEPARTMENTS -> BRANCHES
            // ============================================================

            // Drop existing Department -> Branch FK if present.
            migrationBuilder.Sql(@"
                IF EXISTS
                (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_Departments_Branches_BranchId'
                      AND parent_object_id = OBJECT_ID('dbo.Departments')
                )
                BEGIN
                    ALTER TABLE [dbo].[Departments]
                    DROP CONSTRAINT [FK_Departments_Branches_BranchId];
                END
            ");

            // Add Departments.BranchId if missing.
            migrationBuilder.Sql(@"
                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.columns
                    WHERE object_id = OBJECT_ID('dbo.Departments')
                      AND name = 'BranchId'
                )
                BEGIN
                    ALTER TABLE [dbo].[Departments]
                    ADD [BranchId] int NULL;
                END
            ");

            // Make BranchId nullable if it already exists.
            migrationBuilder.Sql(@"
                IF EXISTS
                (
                    SELECT 1
                    FROM sys.columns
                    WHERE object_id = OBJECT_ID('dbo.Departments')
                      AND name = 'BranchId'
                      AND is_nullable = 0
                )
                BEGIN
                    ALTER TABLE [dbo].[Departments]
                    ALTER COLUMN [BranchId] int NULL;
                END
            ");

            // Convert invalid BranchId values such as 0 to NULL.
            migrationBuilder.Sql(@"
                IF EXISTS
                (
                    SELECT 1
                    FROM sys.columns
                    WHERE object_id = OBJECT_ID('dbo.Departments')
                      AND name = 'BranchId'
                )
                BEGIN
                    UPDATE D
                    SET D.[BranchId] = NULL
                    FROM [dbo].[Departments] D
                    WHERE D.[BranchId] IS NOT NULL
                      AND NOT EXISTS
                      (
                          SELECT 1
                          FROM [dbo].[Branches] B
                          WHERE B.[Id] = D.[BranchId]
                      );
                END
            ");


            // ============================================================
            // 6. EMPLOYEES.BranchId INDEX
            // ============================================================

            migrationBuilder.Sql(@"
                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_Employees_BranchId'
                      AND object_id = OBJECT_ID('dbo.Employees')
                )
                BEGIN
                    CREATE INDEX [IX_Employees_BranchId]
                    ON [dbo].[Employees] ([BranchId]);
                END
            ");


            // ============================================================
            // 7. DEPARTMENTS.BranchId INDEX
            // ============================================================

            migrationBuilder.Sql(@"
                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_Departments_BranchId'
                      AND object_id = OBJECT_ID('dbo.Departments')
                )
                BEGIN
                    CREATE INDEX [IX_Departments_BranchId]
                    ON [dbo].[Departments] ([BranchId]);
                END
            ");


            // ============================================================
            // 8. CUSTOMERS -> ASPNET USERS
            // ============================================================

            migrationBuilder.Sql(@"
                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_Customers_AspNetUsers_ApplicationUserId'
                      AND parent_object_id = OBJECT_ID('dbo.Customers')
                )
                BEGIN
                    ALTER TABLE [dbo].[Customers]
                    ADD CONSTRAINT [FK_Customers_AspNetUsers_ApplicationUserId]
                    FOREIGN KEY ([ApplicationUserId])
                    REFERENCES [dbo].[AspNetUsers] ([Id])
                    ON DELETE NO ACTION;
                END
            ");


            // ============================================================
            // 9. EMPLOYEES -> ASPNET USERS
            // ============================================================

            migrationBuilder.Sql(@"
                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_Employees_AspNetUsers_ApplicationUserId'
                      AND parent_object_id = OBJECT_ID('dbo.Employees')
                )
                BEGIN
                    ALTER TABLE [dbo].[Employees]
                    ADD CONSTRAINT [FK_Employees_AspNetUsers_ApplicationUserId]
                    FOREIGN KEY ([ApplicationUserId])
                    REFERENCES [dbo].[AspNetUsers] ([Id])
                    ON DELETE NO ACTION;
                END
            ");


            // ============================================================
            // 10. EMPLOYEES -> BRANCHES
            // ============================================================

            migrationBuilder.Sql(@"
                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_Employees_Branches_BranchId'
                      AND parent_object_id = OBJECT_ID('dbo.Employees')
                )
                BEGIN
                    ALTER TABLE [dbo].[Employees]
                    ADD CONSTRAINT [FK_Employees_Branches_BranchId]
                    FOREIGN KEY ([BranchId])
                    REFERENCES [dbo].[Branches] ([Id])
                    ON DELETE NO ACTION;
                END
            ");


            // ============================================================
            // 11. DEPARTMENTS -> BRANCHES
            // ============================================================

            migrationBuilder.Sql(@"
                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_Departments_Branches_BranchId'
                      AND parent_object_id = OBJECT_ID('dbo.Departments')
                )
                BEGIN
                    ALTER TABLE [dbo].[Departments]
                    ADD CONSTRAINT [FK_Departments_Branches_BranchId]
                    FOREIGN KEY ([BranchId])
                    REFERENCES [dbo].[Branches] ([Id])
                    ON DELETE NO ACTION;
                END
            ");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This is a repair / compatibility migration.
            // We deliberately do not attempt to restore the old
            // inconsistent tenant schema.
        }
    }
}