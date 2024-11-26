using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FoodHub.Controllers;
using FoodHub.DAL;
using FoodHub.Models;
using FoodHub.ViewModels;
using Microsoft.AspNetCore.Http;

namespace FoodHub.Tests.Controllers
{
    /*
     This class contains tests for the ItemController class. 
    The item don't need to be real, just for testing puposes.

    */

    public class ItemControllerTests
    {
        private readonly Mock<IItemRepository> _mockItemRepository; //using this to MOCK the IItemRepository
        private readonly Mock<ILogger<ItemController>> _mockLogger; //using this to MOCK the ILogger
        private readonly ItemController _controller; //using this to test the ItemController

        public ItemControllerTests()
        {
            _mockItemRepository = new Mock<IItemRepository>(); //creating a new Mock of IItemRepository
            _mockLogger = new Mock<ILogger<ItemController>>(); //creating a new Mock of ILogger
            _controller = new ItemController(_mockItemRepository.Object, _mockLogger.Object); //creating a new ItemController with the Mocks
        }

        [Fact]
        //Test two objects
        public async Task TestTable() //testing the Table method
        {
            // Arrange
            var itemList = new List<Item>()
            {
                //Creating two new Items, to test the Table method
                new Item
                { //The item don't nessecery have to be real, just for testing
                    ItemId = 1,
                    Name = "Fried Chicken Wing",
                    Energy = 300,
                    Carbohydrate = 10,
                    TotalFat = 20,
                    SaturatedFat = 5,
                    UnsaturedFat = 15,
                    Sugar = 1,
                    DietaryFiber = 0,
                    Protein = 25,
                    Salt = 1.5m,
                    ItemCategoryId = 1
                },
                new Item
                {
                    ItemId = 2,
                    Name = "Brown Cheese",
                    Energy = 350,
                    Carbohydrate = 5,
                    TotalFat = 30,
                    SaturatedFat = 15,
                    UnsaturedFat = 15,
                    Sugar = 2,
                    DietaryFiber = 0,
                    Protein = 20,
                    Salt = 1.0m,
                    ItemCategoryId = 2
                }
            };

            var mockItemRepository = new Mock<IItemRepository>(); //creating a new Mock of IItemRepository
            _ = mockItemRepository.Setup(repo => repo.GetAll()).ReturnsAsync(itemList);
            var mockLogger = new Mock<ILogger<ItemController>>();
            var itemController = new ItemController(mockItemRepository.Object, mockLogger.Object);

            // Act
            var result = await itemController.Table(); //calling the Table method

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result); //checking if the result is a ViewResult
            var itemsViewModel = Assert.IsAssignableFrom<ItemsViewModel>(viewResult.ViewData.Model);
            Assert.Equal(2, itemsViewModel.Items.Count());
            Assert.Equal(itemList, itemsViewModel.Items);
        }
        [Fact]
        //to test see if the total amount of fat is eaual to the sum of saturated and unsaturated fat
        public async Task TestCreateItem_InvalidTotalFat_ReturnsValidationError()
        {
            // Arrange
            var newItem = new Item
            {
                ItemId = 3,
                Name = "Test Item",
                Energy = 100,
                Carbohydrate = 10,
                TotalFat = 5, // Total fat is less than the sum of saturated and unsaturated fat
                SaturatedFat = 3,
                UnsaturedFat = 3,
                Sugar = 5,
                DietaryFiber = 2,
                Protein = 10,
                Salt = 0.5m,
                ItemCategoryId = 1
            };

            var mockItemRepository = new Mock<IItemRepository>();
            var mockLogger = new Mock<ILogger<ItemController>>();
            var itemController = new ItemController(mockItemRepository.Object, mockLogger.Object);

            // Act
            var imageUrl = new Mock<IFormFile>().Object; // Mock the IFormFile parameter
            var result = await itemController.Create(newItem, imageUrl);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result); // Check if the result is a ViewResult
            var modelState = viewResult.ViewData.ModelState;
            Assert.False(modelState.IsValid); // Model state should be invalid
            Assert.Contains("TotalFat", modelState.Keys); // Check if the error is related to TotalFat
            var error = modelState["TotalFat"]?.Errors?.FirstOrDefault();
            Assert.NotNull(error); // Ensure error is not null before proceeding
            Assert.Equal("Total fat must be equal to the sum of saturated fat and unsaturated fat", error.ErrorMessage); // Check the error message
        }

        [Fact]
        public async Task TestDetailsNotFound() //Testing the Details method to see if it returns a NotFoundObjectResult
        {
            // Arrange
            int testItemId = 999;
            var mockItemRepository = new Mock<IItemRepository>();
            _ = mockItemRepository.Setup(repo => repo.GetItemById(testItemId)).ReturnsAsync((Item?)null);
            var mockLogger = new Mock<ILogger<ItemController>>();
            var itemController = new ItemController(mockItemRepository.Object, mockLogger.Object);

            // Act
            var result = await itemController.Details(testItemId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Item not found for the ItemId", notFoundResult.Value); //checking if the result is a NotFoundObjectResult
        }

        [Fact]
        public async Task TestUpdateGetItemNotFound() //Testing the Update method to see if it returns a BadRequestObjectResult
        {
            // Arrange
            int testItemId = 999;
            var mockItemRepository = new Mock<IItemRepository>();
            _ = mockItemRepository.Setup(repo => repo.GetItemById(testItemId)).ReturnsAsync((Item)null);
            var mockLogger = new Mock<ILogger<ItemController>>();
            var itemController = new ItemController(mockItemRepository.Object, mockLogger.Object);

            // Act
            var result = await itemController.Update(testItemId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Item not found for the ItemId", badRequestResult.Value);
        }

        [Fact]
        public async Task Delete_ItemNotFound_ReturnsBadRequest() //Testing the Delete method to see if it returns a BadRequestObjectResult
        {
            // Arrange
            int itemId = 1;
            _ = _mockItemRepository.Setup(repo => repo.GetItemById(itemId)).ReturnsAsync((Item)null);

            // Act
            var result = await _controller.Delete(itemId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Item not found for the ItemId", badRequestResult.Value);
        }

        [Fact]
        public async Task Delete_ItemFound_ReturnsViewWithItem()
        {
            // Arrange
            int itemId = 1;
            var item = new Item
            {
                ItemId = 1,
                Name = "Test Item",
                Energy = 200,
                Carbohydrate = 5,
                TotalFat = 10,
                SaturatedFat = 2,
                UnsaturedFat = 8,
                Sugar = 1,
                DietaryFiber = 0,
                Protein = 15,
                Salt = 0.5m,
                ItemCategoryId = 1
            };

            _ = _mockItemRepository.Setup(repo => repo.GetItemById(itemId)).ReturnsAsync(item);

            // Act
            var result = await _controller.Delete(itemId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(item, viewResult.Model);
        }
        //failed to delete item
        [Fact]
        public async Task DeleteConfirmed_ItemDeletionFailed_ReturnsBadRequest()
        {
            // Arrange
            int itemId = 1;
            _ = _mockItemRepository.Setup(repo => repo.Delete(itemId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteConfirmed(itemId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Item deletion failed", badRequestResult.Value);
        }
        //successful deletion of item
        [Fact]
        public async Task DeleteConfirmed_ItemDeleted_ReturnsRedirectToAction()
        {
            // Arrange
            int itemId = 1;
            _ = _mockItemRepository.Setup(repo => repo.Delete(itemId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteConfirmed(itemId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(ItemController.Table), redirectToActionResult.ActionName);
        }
    }
}