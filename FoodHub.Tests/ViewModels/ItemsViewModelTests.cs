using System.Collections.Generic;
using FoodHub.Models;
using FoodHub.ViewModels;
using Xunit;

public class ItemsViewModelTests
{
    [Fact]
    //Tests the ItemsViewModel constructor with a list of items and a view name.
    public void ItemsViewModel_ListItem()
    {
        // Arrange
        var testItems = new List<Item>
        {
            new Item { ItemId = 1, Name = "Test Item 1" },
            new Item { ItemId = 2, Name = "Test Item 2" }
        };
        var testViewName = "TestView";

        // Act
        var viewModel = new ItemsViewModel(testItems, testViewName);

        // Assert
        Assert.Equal(testItems, viewModel.Items);
        Assert.Equal(testViewName, viewModel.CurrentViewName);
    }

    [Fact]
    //Tests the ItemsViewModel constructor with a null view name.
    public void ItemsViewModel_NullViewName()
    {
        // Arrange
        var testItems = new List<Item>
        {
            new Item { ItemId = 1, Name = "Test Item 1" },
            new Item { ItemId = 2, Name = "Test Item 2" }
        };
        string? testViewName = null;

        // Act
        var viewModel = new ItemsViewModel(testItems, testViewName);

        // Assert
        Assert.Equal(testItems, viewModel.Items);
        Assert.Null(viewModel.CurrentViewName);
    }
    [Fact]
    //Tests the ItemsViewModel constructor with an empty list of items.
    public void ItemsViewModel_EmptyItemsList()
    {
        // Arrange
        var emptyItems = new List<Item>();
        var testViewName = "TestView";

        // Act
        var viewModel = new ItemsViewModel(emptyItems, testViewName);

        // Assert
        Assert.Empty(viewModel.Items);
        Assert.Equal(testViewName, viewModel.CurrentViewName);
    }
}
