using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderFood.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationBetweenRestAndOrderMels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RestaurantId",
                table: "MealOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MealOrders_RestaurantId",
                table: "MealOrders",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_MealOrders_Restaurants_RestaurantId",
                table: "MealOrders",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealOrders_Restaurants_RestaurantId",
                table: "MealOrders");

            migrationBuilder.DropIndex(
                name: "IX_MealOrders_RestaurantId",
                table: "MealOrders");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "MealOrders");
        }
    }
}
