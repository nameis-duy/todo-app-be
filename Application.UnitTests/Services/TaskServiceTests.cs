using Application.DTOs.Base;
using Application.DTOs.Task;
using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Service;
using Application.Others;
using Domain.Entity;
using Domain.Enum.Task;
using Mapster;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;

namespace Application.UnitTests.Services
{
    public class TaskServiceTests
    {
        // Init mock data
        private readonly Tasks MockTask = new() { Id = 1, Priority = Priority.Low, Status = Status.Pending };
        private List<Tasks> MockTaskList = [];
        // Inject service
        private readonly Mock<ITaskRepo> mockTaskRepo;
        private ITaskService taskService;
        private readonly Mock<IUnitOfWork> uowMock;
        private readonly Mock<IClaimService> claimServiceMock;
        private readonly Mock<DbSet<Tasks>> mockTaskListQueryable;

        public TaskServiceTests()
        {
            mockTaskRepo = new Mock<ITaskRepo>();
            uowMock = new Mock<IUnitOfWork>();
            claimServiceMock = new Mock<IClaimService>();
            taskService = new TaskService(mockTaskRepo.Object, uowMock.Object, claimServiceMock.Object);
            MockTaskList = [MockTask];
            mockTaskListQueryable = MockTaskList.AsQueryable().BuildMockDbSet();
        }

        #region GetTaskById
        [Fact]
        public async Task GetTaskById_ShouldReturnTask_WhenTaskExist()
        {
            //Arrange
            var mockTaskVm = MockTaskList.FirstOrDefault(t => t.Id == MockTask.Id);
            mockTaskRepo.Setup(repo => repo.GetAll(false)).Returns(mockTaskListQueryable.Object);

            //Act
            var task = await taskService.GetTaskById(MockTask.Id);

            //Assert
            Assert.IsType<ResponseResult<TaskVM>>(task);
            Assert.NotNull(task.Data);
            Assert.Equivalent(mockTaskVm.Adapt<TaskVM>(), task.Data);
            Assert.True(task.IsSucceed);
        }

        [Fact]
        public async Task GetTaskById_ShouldReturnNull_WhenTaskNotFound()
        {
            //Arrange
            mockTaskRepo.Setup(repo => repo.GetAll(false)).Returns(mockTaskListQueryable.Object);

            //Act
            var task = await taskService.GetTaskById(2);

            //Assert
            Assert.IsType<ResponseResult<TaskVM>>(task);
            Assert.Null(task.Data);
            Assert.True(task.IsSucceed);
        }
        #endregion

        #region GetAllTask
        [Fact]
        public async Task GetAllTask_ShouldReturnTasks_WhenListNotEmpty()
        {
            //Arrange
            mockTaskRepo.Setup(repo => repo.GetAll(false)).Returns(mockTaskListQueryable.Object);

            //Act
            var result = await taskService.GetAllTasksAsync();

            //Assert
            Assert.IsType<ResponseResult<IEnumerable<TaskVM>>>(result);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.Contains(result.Data, t => t.Id == MockTask.Id);
            Assert.Single(result.Data);
            Assert.True(result.IsSucceed);
        }

        [Fact]
        public async Task GetAllTask_ShouldReturnEmptyList_WhenListHaveNoTask()
        {
            //Arrange
            var mockEmptyTaskList = new List<Tasks>().AsQueryable().BuildMockDbSet();
            mockTaskRepo.Setup(repo => repo.GetAll(false)).Returns(mockEmptyTaskList.Object);

            //Act
            var result = await taskService.GetAllTasksAsync();

            //Assert
            Assert.IsType<ResponseResult<IEnumerable<TaskVM>>>(result);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
            Assert.True(result.IsSucceed);
        }

        [Fact]
        public async Task GetAllTask_ShouldReturnCorrectOrder_WhenListHaveManyTasks()
        {
            //Arrange
            List<Tasks> mockTasks =
            [
                new() { Id = 1, Status = Status.Pending, Priority = Priority.Medium },
                new() { Id = 2, Status = Status.Pending, Priority = Priority.High, },
                new() { Id = 3, Status = Status.Pending, Priority = Priority.Low },
                new() { Id = 4, Status = Status.Completed, Priority = Priority.Medium },
            ];
            var mockManyTaskList = mockTasks.AsQueryable().BuildMockDbSet();
            mockTaskRepo.Setup(repo => repo.GetAll(false)).Returns(mockManyTaskList.Object);

            //Act
            var result = await taskService.GetAllTasksAsync();

            //Assert
            var taskResult = result.Data!.ToList();
            Assert.Equal(Priority.High.ToString(), taskResult[0].Priority);
            Assert.Equal(Priority.Medium.ToString(), taskResult[1].Priority);
            Assert.Equal(Priority.Low.ToString(), taskResult[2].Priority);
            Assert.Equal(Status.Completed.ToString(), taskResult[3].Status);

            Assert.IsType<ResponseResult<IEnumerable<TaskVM>>>(result);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.Equal(4, result.Data.Count());
            Assert.True(result.IsSucceed);
        }
        #endregion

        #region GetPage
        [Fact]
        public async Task GetPageAsync_ShouldReturnPaginationTask_WhenValidPageSizeAndPageIndex()
        {
            //Arrange
            var pageIndex = 0;
            var pageSize = 10;
            var taskPage = new Pagination<Tasks>
            {
                Items = MockTaskList,
                PageSize = pageSize,
                PageNumber = pageIndex,
                TotalItems = MockTaskList.Count
            };
            mockTaskRepo.Setup(repo => repo.GetPageAsync(pageIndex, pageSize, It.IsAny<Expression<Func<Tasks, bool>>>()))
                .ReturnsAsync(taskPage);

            //Act
            var result = await taskService.GetPageAsync(pageIndex, pageSize);

            //Assert
            Assert.IsType<Pagination<TaskVM>>(result);
            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
            Assert.Contains(result.Items, t => t.Id == MockTask.Id);
            Assert.Equal(pageIndex, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(MockTaskList.Count, result.TotalItems);
        }

        [Fact]
        public async Task GetPageAsync_ShouldReturnEmptyPaginationTask_WhenNoTaskFound()
        {
            //Arrange
            var pageIndex = 0;
            var pageSize = 10;
            var taskPage = new Pagination<Tasks>
            {
                Items = [],
                PageSize = pageSize,
                PageNumber = pageIndex,
                TotalItems = MockTaskList.Count
            };
            mockTaskRepo.Setup(repo => repo.GetPageAsync(pageIndex, pageSize, It.IsAny<Expression<Func<Tasks, bool>>>()))
                .ReturnsAsync(taskPage);

            //Act
            var result = await taskService.GetPageAsync(pageIndex, pageSize);

            //Assert
            Assert.IsType<Pagination<TaskVM>>(result);
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(pageIndex, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(MockTaskList.Count, result.TotalItems);
        }

        [Fact]
        public async Task GetPageAsync_ShouldReturnEmptyPaginationTask_WhenPageIndexExceedTotalPage()
        {
            //Arrange
            var pageIndex = 10;
            var pageSize = 10;
            var taskPage = new Pagination<Tasks>
            {
                Items = [],
                PageSize = pageSize,
                PageNumber = pageIndex,
                TotalItems = MockTaskList.Count
            };
            mockTaskRepo.Setup(repo => repo.GetPageAsync(pageIndex, pageSize, It.IsAny<Expression<Func<Tasks, bool>>>()))
                .ReturnsAsync(taskPage);

            //Act
            var result = await taskService.GetPageAsync(pageIndex, pageSize);

            //Assert
            Assert.IsType<Pagination<TaskVM>>(result);
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(pageIndex, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
        }
        #endregion

        #region CreateTaskAsync
        [Fact]
        public async Task CreateTaskAsync_ShouldCreateTask_WhenValidRequest()
        {
            //Arrange
            var taskCreateDto = new TaskCreateRequest();
            mockTaskRepo.Setup(repo => repo.AddAsync(It.IsAny<Tasks>())).Returns(Task.CompletedTask);
            uowMock.Setup(uow => uow.SaveChangeAsync()).ReturnsAsync(true);

            //Act
            var result = await taskService.CreateTaskAsync(taskCreateDto);

            //Assert
            mockTaskRepo.Verify(repo => repo.AddAsync(It.IsAny<Tasks>()), Times.Once);
            uowMock.Verify(uow => uow.SaveChangeAsync(), Times.Once);

            Assert.IsType<ResponseResult<TaskVM>>(result);
            Assert.NotNull(result.Data);
            Assert.True(result.IsSucceed);
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldThrowUnauthorizedAccessException_WhenUserInvalid()
        {
            //Arrange
            var taskCreateDto = new TaskCreateRequest();
            claimServiceMock.Setup(claimService => claimService.GetCurrentUserId()).Returns(-1);
            taskService = new TaskService(mockTaskRepo.Object, uowMock.Object, claimServiceMock.Object);
            mockTaskRepo.Setup(repo => repo.AddAsync(It.IsAny<Tasks>())).Returns(Task.CompletedTask);
            uowMock.Setup(uow => uow.SaveChangeAsync()).ReturnsAsync(true);

            //Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await taskService.CreateTaskAsync(taskCreateDto));
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldThrowDbUpdateException_WhenSaveToDatabaseFailed()
        {
            //Arrange
            var taskCreateDto = new TaskCreateRequest();
            mockTaskRepo.Setup(repo => repo.AddAsync(It.IsAny<Tasks>())).Returns(Task.CompletedTask);
            uowMock.Setup(uow => uow.SaveChangeAsync()).ReturnsAsync(false);

            //Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(async () => await taskService.CreateTaskAsync(taskCreateDto));
        }
        #endregion

        #region UpdateTaskAsync
        [Fact]
        public async Task UpdateTaskAsync_ShouldUpdateTask_WhenValidInput()
        {
            //Arrange
            var taskUpdateDto = new TaskUpdateRequest() { Id = MockTask.Id, Title = "New Title" };
            mockTaskRepo.Setup(repo => repo.FirstOrDefaultAsync(taskUpdateDto.Id)).ReturnsAsync(MockTask);
            mockTaskRepo.Setup(repo => repo.Update(MockTask)).Verifiable();
            uowMock.Setup(uow => uow.SaveChangeAsync()).ReturnsAsync(true);

            //Act
            var result = await taskService.UpdateTaskAsync(taskUpdateDto);

            //Assert
            mockTaskRepo.Verify(repo => repo.FirstOrDefaultAsync(taskUpdateDto.Id), Times.Once);
            mockTaskRepo.Verify(repo => repo.Update(MockTask), Times.Once);
            uowMock.Verify(repo => repo.SaveChangeAsync(), Times.Once);

            Assert.IsType<ResponseResult<TaskVM>>(result);
            Assert.NotNull(result.Data);
            Assert.Equal(MockTask.Id, result.Data.Id);
            Assert.Equal("New Title", result.Data.Title);
            Assert.True(result.IsSucceed);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldThrowDbUpdateException_WhenSaveToDatabaseFailed()
        {
            //Arrange
            var taskUpdateDto = new TaskUpdateRequest() { Id = MockTask.Id };
            mockTaskRepo.Setup(repo => repo.FirstOrDefaultAsync(taskUpdateDto.Id)).ReturnsAsync(MockTask);
            mockTaskRepo.Setup(repo => repo.Update(MockTask)).Verifiable();
            uowMock.Setup(uow => uow.SaveChangeAsync()).ReturnsAsync(false);

            //Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(async () => await taskService.UpdateTaskAsync(taskUpdateDto));
        }
        #endregion

        #region UpdateTaskPriorityAsync
        [Fact]
        public async Task UpdateTaskPriorityAsync_ShouldChangeTaskPriority_WhenPriorityInputValid()
        {
            //Arrange
            var taskChangePriorityRequestDto = new TaskChangePriorityRequest() { Id = MockTask.Id, Priority = Priority.High };
            mockTaskRepo.Setup(repo => repo.FirstOrDefaultAsync(taskChangePriorityRequestDto.Id)).ReturnsAsync(MockTask);
            mockTaskRepo.Setup(repo => repo.Update(MockTask)).Verifiable();
            uowMock.Setup(uow => uow.SaveChangeAsync()).ReturnsAsync(true);

            //Act
            var result = await taskService.UpdateTaskPriorityAsync(taskChangePriorityRequestDto);

            //Assert
            mockTaskRepo.Verify(repo => repo.FirstOrDefaultAsync(taskChangePriorityRequestDto.Id), Times.Once);
            mockTaskRepo.Verify(repo => repo.Update(MockTask), Times.Once);
            uowMock.Verify(repo => repo.SaveChangeAsync(), Times.Once);

            Assert.IsType<ResponseResult<TaskVM>>(result);
            Assert.NotNull(result.Data);
            Assert.Equal(MockTask.Id, result.Data.Id);
            Assert.Equal(Priority.High.ToString(), result.Data.Priority);
            Assert.True(result.IsSucceed);
        }

        [Fact]
        public async Task UpdateTaskPriorityAsync_ShouldThrowDbUpdateException_WhenSaveToDatabaseFailed()
        {
            //Arrange
            var taskChangePriorityRequestDto = new TaskChangePriorityRequest() { Id = MockTask.Id, Priority = Priority.High };
            mockTaskRepo.Setup(repo => repo.FirstOrDefaultAsync(taskChangePriorityRequestDto.Id)).ReturnsAsync(MockTask);
            mockTaskRepo.Setup(repo => repo.Update(MockTask)).Verifiable();
            uowMock.Setup(uow => uow.SaveChangeAsync()).ReturnsAsync(false);

            //Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(async () => await taskService.UpdateTaskPriorityAsync(taskChangePriorityRequestDto));
        }
        #endregion

        #region UpdateTaskStatusAsync
        [Fact]
        public async Task UpdateTaskStatusAsync_ShouldChangeTaskStatus_WhenStatusInputValid()
        {
            //Arrange
            var taskChangeStatusRequestDto = new TaskChangeStatusRequest() { Id = MockTask.Id, Status = Status.Completed };
            mockTaskRepo.Setup(repo => repo.FirstOrDefaultAsync(taskChangeStatusRequestDto.Id)).ReturnsAsync(MockTask);
            mockTaskRepo.Setup(repo => repo.Update(MockTask)).Verifiable();
            uowMock.Setup(uow => uow.SaveChangeAsync()).ReturnsAsync(true);

            //Act
            var result = await taskService.UpdateTaskStatusAsync(taskChangeStatusRequestDto);

            //Assert
            mockTaskRepo.Verify(repo => repo.FirstOrDefaultAsync(taskChangeStatusRequestDto.Id), Times.Once);
            mockTaskRepo.Verify(repo => repo.Update(MockTask), Times.Once);
            uowMock.Verify(repo => repo.SaveChangeAsync(), Times.Once);

            Assert.IsType<ResponseResult<TaskVM>>(result);
            Assert.NotNull(result.Data);
            Assert.Equal(MockTask.Id, result.Data.Id);
            Assert.Equal(Status.Completed.ToString(), result.Data.Status);
            Assert.True(result.IsSucceed);
        }

        [Fact]
        public async Task UpdateTaskStatusAsync_ShouldThrowDbUpdateException_WhenSaveToDatabaseFailed()
        {
            //Arrange
            var taskChangeStatusRequestDto = new TaskChangeStatusRequest() { Id = MockTask.Id, Status = Status.Completed };
            mockTaskRepo.Setup(repo => repo.FirstOrDefaultAsync(taskChangeStatusRequestDto.Id)).ReturnsAsync(MockTask);
            mockTaskRepo.Setup(repo => repo.Update(MockTask)).Verifiable();
            uowMock.Setup(uow => uow.SaveChangeAsync()).ReturnsAsync(false);

            //Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(async () => await taskService.UpdateTaskStatusAsync(taskChangeStatusRequestDto));
        }
        #endregion

        #region RemoveTaskAsync
        [Fact]
        public async Task RemoveTaskAsync_ShouldChangeTaskRemoveStatus_WhenTaskExist()
        {
            //Arrange
            var taskRemoveRequestDto = new TaskRemoveRequest() { Id = MockTask.Id };
            mockTaskRepo.Setup(repo => repo.FirstOrDefaultAsync(taskRemoveRequestDto.Id)).ReturnsAsync(MockTask);
            mockTaskRepo.Setup(repo => repo.Update(MockTask)).Verifiable();
            uowMock.Setup(uow => uow.SaveChangeAsync()).ReturnsAsync(true);

            //Act
            var result = await taskService.RemoveTaskAsync(taskRemoveRequestDto);

            //Assert
            mockTaskRepo.Verify(repo => repo.FirstOrDefaultAsync(taskRemoveRequestDto.Id), Times.Once);
            mockTaskRepo.Verify(repo => repo.Update(MockTask), Times.Once);
            uowMock.Verify(repo => repo.SaveChangeAsync(), Times.Once);

            Assert.IsType<ResponseResult<int>>(result);
            Assert.Equal(MockTask.Id, result.Data);
            Assert.True(result.IsSucceed);
        }

        [Fact]
        public async Task RemoveTaskAsync_ShouldThrowDbUpdateException_WhenSaveToDatabaseFailed()
        {
            //Arrange
            var taskRemoveRequestDto = new TaskRemoveRequest() { Id = MockTask.Id };
            mockTaskRepo.Setup(repo => repo.FirstOrDefaultAsync(taskRemoveRequestDto.Id)).ReturnsAsync(MockTask);
            mockTaskRepo.Setup(repo => repo.Update(MockTask)).Verifiable();
            uowMock.Setup(uow => uow.SaveChangeAsync()).ReturnsAsync(false);

            //Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(async () => await taskService.RemoveTaskAsync(taskRemoveRequestDto));
        }
        #endregion
    }
}
