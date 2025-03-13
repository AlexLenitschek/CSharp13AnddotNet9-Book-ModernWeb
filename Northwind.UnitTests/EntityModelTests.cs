using Northwind.EntityModels; // To use NorthwindContext.

namespace Northwind.UnitTests
{
    public class EntityModelTests
    {
        [Fact]
        public void DatabaseConnectTest()
        {
            using NorthwindContext db = new();
            Assert.True(db.Database.CanConnect());
        }

        [Fact]
        public void CategoryCountTest()
        {
            using NorthwindContext db = new();

            // Arrange
            int expected = 8;
            // Act
            int actual = db.Categories.Count();
            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ProductId1IsChaiTest()
        {
            using NorthwindContext db = new();

            // Arrange
            string expected = "Chai";

            // Act
            Product? product = db.Products.Find(keyValues: 1);
            string? actual = product?.ProductName ?? string.Empty;

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
