using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barks.Migrations
{
    /// <inheritdoc />
    public partial class AddProductCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Flavor",
                table: "Products",
                type: "TEXT",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemCode",
                table: "Products",
                type: "TEXT",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "Products",
                type: "TEXT",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Products",
                type: "TEXT",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ItemCode",
                table: "Products",
                column: "ItemCode",
                unique: true);

            migrationBuilder.Sql(
                """
                INSERT OR IGNORE INTO "Products"
                    ("ItemCode", "ProductName", "Flavor", "Size", "Category", "Unit", "Quantity", "MinimumStock", "Price", "Supplier")
                VALUES
                    ('BC-100', 'Banana Carrot 100g (Trial Pack)', 'Banana Carrot', '100g (Trial Pack)', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('BC-250', 'Banana Carrot 250g', 'Banana Carrot', '250g', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('BC-950', 'Banana Carrot 950g', 'Banana Carrot', '950g', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('PBO-100', 'Peanut Butter Oats 100g (Trial Pack)', 'Peanut Butter Oats', '100g (Trial Pack)', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('PBO-250', 'Peanut Butter Oats 250g', 'Peanut Butter Oats', '250g', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('PBO-950', 'Peanut Butter Oats 950g', 'Peanut Butter Oats', '950g', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('BM-100', 'Banana Milk 100g (Trial Pack)', 'Banana Milk', '100g (Trial Pack)', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('BM-250', 'Banana Milk 250g', 'Banana Milk', '250g', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('BM-950', 'Banana Milk 950g', 'Banana Milk', '950g', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('PL-100', 'Pumpkin Liver 100g (Trial Pack)', 'Pumpkin Liver', '100g (Trial Pack)', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('PL-250', 'Pumpkin Liver 250g', 'Pumpkin Liver', '250g', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('PL-950', 'Pumpkin Liver 950g', 'Pumpkin Liver', '950g', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('BMG-100', 'Beef Malunggay 100g (Trial Pack)', 'Beef Malunggay', '100g (Trial Pack)', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('BMG-250', 'Beef Malunggay 250g', 'Beef Malunggay', '250g', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('BMG-950', 'Beef Malunggay 950g', 'Beef Malunggay', '950g', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('PP-100', 'Pumpkin Parsley 100g (Trial Pack)', 'Pumpkin Parsley', '100g (Trial Pack)', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('PP-250', 'Pumpkin Parsley 250g', 'Pumpkin Parsley', '250g', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('PP-950', 'Pumpkin Parsley 950g', 'Pumpkin Parsley', '950g', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('CB-100', 'Camote Bacon 100g (Trial Pack)', 'Camote Bacon', '100g (Trial Pack)', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('CB-250', 'Camote Bacon 250g', 'Camote Bacon', '250g', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('CB-950', 'Camote Bacon 950g', 'Camote Bacon', '950g', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('CMG-100', 'Chicken Malunggay 100g (Trial Pack)', 'Chicken Malunggay', '100g (Trial Pack)', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('CMG-250', 'Chicken Malunggay 250g', 'Chicken Malunggay', '250g', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('CMG-950', 'Chicken Malunggay 950g', 'Chicken Malunggay', '950g', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('MC-100', 'Milk Carrot 100g (Trial Pack)', 'Milk Carrot', '100g (Trial Pack)', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('MC-250', 'Milk Carrot 250g', 'Milk Carrot', '250g', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('MC-950', 'Milk Carrot 950g', 'Milk Carrot', '950g', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('AST-100', 'Assorted 100g (Trial Pack)', 'Assorted', '100g (Trial Pack)', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('AST-250', 'Assorted 250g', 'Assorted', '250g', 'Pet Biscuits', 'pack', 0, 0, 0, ''),
                    ('AST-950', 'Assorted 950g', 'Assorted', '950g', 'Pet Biscuits', 'pack', 0, 0, 0, '');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_ItemCode",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Flavor",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ItemCode",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Products");
        }
    }
}
