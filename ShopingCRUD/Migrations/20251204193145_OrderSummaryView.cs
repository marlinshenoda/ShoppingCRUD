using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopingCRUD.Migrations
{
    /// <inheritdoc />
    public partial class OrderSummaryView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create a read-only view that summarizes orders with customer data and computed total.
            // Note: table and column names must match the EF model / database schema.
            migrationBuilder.Sql(@"
CREATE VIEW IF NOT EXISTS OrderSummaryView AS
SELECT 
    o.OrderId,
    o.OrderDate,
    c.CustomerName AS CustomerName,
    c.PhoneNumber AS PhoneNumber,
    c.Email AS CustomerEmail,
    IFNULL(SUM(orw.Quantity * orw.UnitPrice), 0) AS TotalAmount
FROM Orders o
    JOIN Customers c ON c.CustomerId = o.CustomerId
LEFT JOIN OrderRows orw ON orw.OrderId = o.OrderId
GROUP BY o.OrderId, o.OrderDate, c.CustomerName, c.PhoneNumber, c.Email;
");

            //
            // TRIGGER: AFTER INSERT ON OrderRows
            // Recompute the parent order's TotalAmount after a new order row is inserted.
            //
            migrationBuilder.Sql(@"
CREATE TRIGGER IF NOT EXISTS trg_OrderRow_Insert
AFTER INSERT ON OrderRows
BEGIN
    UPDATE Orders
    SET TotalAmount = (
        SELECT IFNULL(SUM(Quantity * UnitPrice), 0)
        FROM OrderRows
        WHERE OrderId = NEW.OrderId
    )
    WHERE OrderId = NEW.OrderId;
END;
");

            //
            // TRIGGER: AFTER UPDATE ON OrderRows
            // Recompute the parent order's TotalAmount after an order row is updated.
            //
            migrationBuilder.Sql(@"
CREATE TRIGGER IF NOT EXISTS trg_OrderRow_Update
AFTER UPDATE ON OrderRows
BEGIN
    UPDATE Orders
    SET TotalAmount = (
        SELECT IFNULL(SUM(Quantity * UnitPrice), 0)
        FROM OrderRows
        WHERE OrderId = NEW.OrderId
    )
    WHERE OrderId = NEW.OrderId;
END;
");

            //
            // TRIGGER: AFTER DELETE ON OrderRows
            // Recompute the parent order's TotalAmount after an order row is deleted.
            //
            migrationBuilder.Sql(@"
CREATE TRIGGER IF NOT EXISTS trg_OrderRow_Delete
AFTER DELETE ON OrderRows
BEGIN
    UPDATE Orders
    SET TotalAmount = (
        SELECT IFNULL(SUM(Quantity * UnitPrice), 0)
        FROM OrderRows
        WHERE OrderId = OLD.OrderId
    )
    WHERE OrderId = OLD.OrderId;
END;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop triggers first (they depend on OrderRows/Orders), then drop the view.
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_OrderRow_Insert;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_OrderRow_Update;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_OrderRow_Delete;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS OrderSummaryView;");
        }
    }
}
