
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using trackingAPI.Controllers;
using trackingAPI.Data.Contexts;
using trackingAPI.Models;

namespace trackingAPI.UnitTests.Tests
{

    public class IssueControllerTests
    {
        private IssueDbContext _inMemContext;
        private IssueController _controller;

        public IssueControllerTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<IssueDbContext>()
                .UseInMemoryDatabase("SqlServer");
            _inMemContext = new IssueDbContext(optionsBuilder.Options);
            _controller = new IssueController(_inMemContext);
        }

        [Fact]
        public async Task GetById_WithNonexistentItem_ReturnsNotFound() //nameing convention: UnitOfWork_StateUnderTest_ExpectedBehavior() 
        {
            //Arrange
            var optionsBuilder = new DbContextOptionsBuilder<IssueDbContext>();
            var mockSet = new Mock<DbSet<Issue>>();
            var mockContext = new Mock<IssueDbContext>(optionsBuilder.Options);
            mockContext.Setup(m => m.Issues).Returns(mockSet.Object);
            var controller = new IssueController(mockContext.Object);

            //Act
            var result = await controller.GetById(int.MaxValue);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetById_WithExistentItem_ReturnsOkResult()
        {
            //Arrange
            var data = new Issue(1, "Test1", "Yes", 0, 0, DateTime.Now, null);

            //Act
            await _controller.Create(data);

            var result = await _controller.GetById(1);

            //Assert
            Assert.IsType<OkObjectResult>(result);

            //CleanUp
            await _controller.Delete(data.Id);
        }

        [Fact]
        public async Task Get_ReturnsExistentItems()
        {
            //Arrange
            var data = new List<Issue>
            {
                new Issue(1, "Test1", "Yes", 0, 0, DateTime.Now, null ),
                new Issue(2, "Test1", "Yes", 0, 0, DateTime.Now, null ),
                new Issue(3, "Test1", "Yes", 0, 0, DateTime.Now, null ),
            };

            //Act
            foreach (var item in data) await _controller.Create(item);

            var result = (await _controller.Get()).ToList();

            //Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(1, result[0].Id);
            Assert.Equal(2, result[1].Id);
            Assert.Equal(3, result[2].Id);

            //CleanUp
            foreach (var item in data) await _controller.Delete(item.Id);
        }

        [Fact]
        public async Task Get_CheckDbIsEmpty_ReturnsBadRequestResult() //nameing convention: UnitOfWork_StateUnderTest_ExpectedBehavior()
        {
            //Arrange


            //Act
            var result = (await _controller.Get()).ToList();

            //Assert
            Assert.Equal(0, result.Count);
        }

        [Fact]
        public async Task Create_CreateNewItem_ReturnsCreateResult() //nameing convention: UnitOfWork_StateUnderTest_ExpectedBehavior()
        {
            //Arrange
            var data = new Issue(1, "Test1", "Yes", 0, 0, DateTime.Now, null);

            //Act
            var result = await _controller.Create(data);

            //Assert
            Assert.IsType<CreatedAtActionResult>(result);

            //CleanUp
            await _controller.Delete(data.Id);
        }

        [Fact]
        public async Task Create_CreateDuplicateItems_ReturnsBadRequest()
        {
            //Arrange
            var data = new List<Issue>
            {
                new Issue(1, "Test1", "Yes", 0, 0, DateTime.Now, null ),
                new Issue(1, "Test1", "Yes", 0, 0, DateTime.Now, null ),
            };
            IActionResult result = null;

            //Act
            foreach (var item in data) result = await _controller.Create(item);


            //Assert
            Assert.IsType<BadRequestResult>(result);

            //CleanUp
            foreach (var item in data) await _controller.Delete(item.Id);

        }

        [Fact]
        public async Task Delete_DeleteExistentItem_ReturnsNoContextResult() //nameing convention: UnitOfWork_StateUnderTest_ExpectedBehavior()
        {
            //Arrange
            var data = new Issue(1, "Test1", "Yes", 0, 0, DateTime.Now, null);
            await _controller.Create(data);

            //Act
            var result = await _controller.Delete(data.Id);

            //Assert
            Assert.IsType<NoContentResult>(result);

            //CleanUp
            await _controller.Delete(data.Id);
        }

        [Fact]
        public async Task Delete_DeleteNonExistentItem_ReturnsNotFoundResult() //nameing convention: UnitOfWork_StateUnderTest_ExpectedBehavior()
        {
            //Arrange


            //Act
            var result = await _controller.Delete(int.MaxValue);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_UpdateExistentItem_ReturnsNoContextResult() //nameing convention: UnitOfWork_StateUnderTest_ExpectedBehavior()
        {
            //Arrange
            var data = new Issue(1, "Test1", "Yes", 0, 0, DateTime.Now, null);
            await _controller.Create(data);

            //Act
            var result = await _controller.Update(data.Id, data);

            //Assert
            Assert.IsType<NoContentResult>(result);

            //CleanUp
            await _controller.Delete(data.Id);
        }


        [Fact]
        public async Task Update_UpdateNonExistentItem_ReturnsBadRequestResult() //nameing convention: UnitOfWork_StateUnderTest_ExpectedBehavior()
        {
            //Arrange
            var data = new Issue(3, "Test1", "Yes", 0, 0, DateTime.Now, null);

            //Act
            var result = await _controller.Update(data.Id, data);

            //Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Update_UpdateMismatchedItem_ReturnsBadRequestResult() //nameing convention: UnitOfWork_StateUnderTest_ExpectedBehavior()
        {
            //Arrange
            var data = new Issue(1, "Test1", "Yes", 0, 0, DateTime.Now, null);

            //Act
            var result = await _controller.Update(2, data);

            //Assert
            Assert.IsType<BadRequestResult>(result);
        }

    }
}