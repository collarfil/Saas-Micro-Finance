using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saas_Micro_Finance.DataAccess.Migrations.TenantMigrations
{
    /// <inheritdoc />
    public partial class RepairTenantMigrationState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ============================================================
            // 1. CUSTOMERS -> APPLICATION USER
            // ============================================================

            // Ensure Customers.ApplicationUserId exists.
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

            // Ensure Customers.ApplicationUserId is nullable.
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

            // Ensure Customer FK exists.
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
            // 2. EMPLOYEES -> APPLICATION USER
            // ============================================================

            // Ensure Employees.ApplicationUserId exists.
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

            // Ensure Employees.ApplicationUserId is nullable.
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

            // Ensure Employee FK exists.
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
            // 3. EMPLOYEES -> BRANCH
            // ============================================================

            // Ensure Employees.BranchId exists as nullable.
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

            // Ensure it remains nullable for historical tenants.
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

            // Remove invalid historical BranchId values.
            migrationBuilder.Sql(@"
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
            ");

            // Ensure Employee Branch index exists.
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

            // Ensure Employee -> Branch FK exists.
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
                        CONSTRAINT [DF_Repair_Employees_Street]
                        DEFAULT ('');
                END
            ");


            // ============================================================
            // 5. DEPARTMENTS -> BRANCH
            // ============================================================

            // Ensure Departments.BranchId exists as nullable.
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

            // Ensure it remains nullable for historical tenants.
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

            // Remove invalid historical BranchId values.
            migrationBuilder.Sql(@"
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
            ");

            // Ensure Department Branch index exists.
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

            // Ensure Department -> Branch FK exists.
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
            // One-time repair migration.
            // No rollback is intentionally provided.
        }
    }
}