using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Data.Data;
using TMS.Services.Implementations;

namespace Tests.Services.ServiceTests
{
    public class TaskServiceTests
    {
        [Fact]

        public async void CreateTaskAsync_ShouldCreateTask()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<TMSContext>()
                .UseInMemoryDatabase(databaseName: "CreateTaskAsync_ShouldCreateTask")
                .Options;

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TMS.WebHost.Models.Mapper>();
            });

            IMapper mapper = configuration.CreateMapper();

            using (var context = new TMSContext(options))
            {
                context.Database.EnsureCreated();

                context.Tasks.Add(new TMS.Data.Models.Task
                {
                    TaskId = "test123",
                    Name = "Test",
                    Description = "Test",
                    Category = TMS.Data.Enums.TaskCategory.Executive,
                    Priority = TMS.Data.Enums.TaskPriority.Medium,
                    Status = TMS.Data.Enums.TaskStatus.InProgress,
                    CreatedOn = new DateTime(2019, 05, 09, 9, 15, 0, 0, 0, 0),
                    StartDate = new DateTime(2024, 05, 09, 9, 15, 0, 0, 0, 0),
                    DueDate = new DateTime(2024, 05, 09, 9, 15, 0, 0, 0, 0),
                    GroupId = "test111",
                    UserId = "test222"
                });

                await context.SaveChangesAsync();
            }

            using (var context = new TMSContext(options))
            {
                var taskService = new TaskService(context, mapper);

                //Act

                var result = await taskService.CreateTaskAsync(new TMS.Data.Models.Task
                {
                    TaskId = "test123",
                    Name = "Test",
                    Description = "Test",
                    Category = TMS.Data.Enums.TaskCategory.Executive,
                    Priority = TMS.Data.Enums.TaskPriority.Medium,
                    Status = TMS.Data.Enums.TaskStatus.InProgress,
                    CreatedOn = new DateTime(2019, 05, 09, 9, 15, 0, 0, 0, 0),
                    StartDate = new DateTime(2024, 05, 09, 9, 15, 0, 0, 0, 0),
                    DueDate = new DateTime(2024, 05, 09, 9, 15, 0, 0, 0, 0),
                    GroupId = "test111",
                    UserId = "test222"
                });

                //Assert
                Assert.NotNull(result);
                Assert.Equal("Test", result.Name);
                Assert.Equal("Test", result.Description);
                Assert.Equal(TMS.Data.Enums.TaskCategory.Executive, result.Category);
                Assert.Equal(TMS.Data.Enums.TaskPriority.Medium, result.Priority);
                Assert.Equal(TMS.Data.Enums.TaskStatus.InProgress, result.Status);
                Assert.Equal(new DateTime(2024, 05, 09, 9, 15, 0, 0, 0, 0), result.StartDate);
                Assert.Equal(new DateTime(2024, 05, 09, 9, 15, 0, 0, 0, 0), result.DueDate);
                Assert.Equal("test111", result.GroupId);
                Assert.Equal("test222", result.UserId);
            }
        }
        [Fact]
        public async void DeleteTaskAsync_ShouldDeleteTask()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<TMSContext>()
                .UseInMemoryDatabase(databaseName: "DeleteTaskAsync_ShouldDeleteTask")
                .Options;

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TMS.WebHost.Models.Mapper>();
            });

            IMapper mapper = configuration.CreateMapper();

            using (var context = new TMSContext(options))
            {
                context.Database.EnsureCreated();

                context.Tasks.Add(new TMS.Data.Models.Task
                {
                    TaskId = "test123",
                    Name = "Test",
                    Description = "Test",
                    Category = TMS.Data.Enums.TaskCategory.Executive,
                    Priority = TMS.Data.Enums.TaskPriority.Medium,
                    Status = TMS.Data.Enums.TaskStatus.InProgress,
                    CreatedOn = new DateTime(2019, 05, 09, 9, 15, 0),
                    StartDate = new DateTime(2024, 05, 09, 9, 15, 0),
                    DueDate = new DateTime(2024, 05, 09, 9, 15, 0),
                    GroupId = "test111",
                    UserId = "test222"
                });

                await context.SaveChangesAsync();
            }

            using (var context = new TMSContext(options))
            {
                var taskService = new TaskService(context, mapper);

                //Act
                await taskService.DeleteTaskAsync("test123");

                //Assert
                Assert.Equal(0, context.Tasks.Count());
            }
        }

        [Fact]
        public async void GetAllTasksAsync_ShouldReturnAllTasks()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<TMSContext>()
                .UseInMemoryDatabase(databaseName: "GetAllTasksAsync_ShouldReturnAllTasks")
                .Options;

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TMS.WebHost.Models.Mapper>();
            });

            IMapper mapper = configuration.CreateMapper();

            using (var context = new TMSContext(options))
            {
                context.Database.EnsureCreated();

                context.Tasks.Add(new TMS.Data.Models.Task
                {
                    TaskId = "test123",
                    Name = "Test",
                    Description = "Test",
                    Category = TMS.Data.Enums.TaskCategory.Executive,
                    Priority = TMS.Data.Enums.TaskPriority.Medium,
                    Status = TMS.Data.Enums.TaskStatus.InProgress,
                    CreatedOn = new DateTime(2019, 05, 09, 9, 15, 0),
                    StartDate = new DateTime(2024, 05, 09, 9, 15, 0),
                    DueDate = new DateTime(2024, 05, 09, 9, 15, 0),
                    GroupId = "test111",
                    UserId = "test222"
                });

                context.Tasks.Add(new TMS.Data.Models.Task
                {
                    TaskId = "test1234",
                    Name = "Test1",
                    Description = "Test1",
                    Category = TMS.Data.Enums.TaskCategory.PersonalWork,
                    Priority = TMS.Data.Enums.TaskPriority.Low,
                    Status = TMS.Data.Enums.TaskStatus.InProgress,
                    CreatedOn = new DateTime(2019, 05, 09, 9, 15, 0),
                    StartDate = new DateTime(2024, 05, 09, 9, 15, 0),
                    DueDate = new DateTime(2024, 05, 09, 9, 15, 0),
                    GroupId = "test121",
                    UserId = "test212"
                });

                await context.SaveChangesAsync();
            }

            using (var context = new TMSContext(options))
            {
                var taskService = new TaskService(context, mapper);

                //Act
                var result = await taskService.GetAllTasksAsync();

                //Assert
                Assert.Equal(2, result.Count);
            }
        }

        [Fact]
        public async void GetTaskByIdAsync_ShouldReturnTask()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<TMSContext>()
                .UseInMemoryDatabase(databaseName: "GetTaskByIdAsync_ShouldReturnTask")
                .Options;

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TMS.WebHost.Models.Mapper>();
            });

            IMapper mapper = configuration.CreateMapper();

            using (var context = new TMSContext(options))
            {
                context.Database.EnsureCreated();

                context.Tasks.Add(new TMS.Data.Models.Task
                {
                    TaskId = "test123",
                    Name = "Test",
                    Description = "Test",
                    Category = TMS.Data.Enums.TaskCategory.Executive,
                    Priority = TMS.Data.Enums.TaskPriority.Medium,
                    Status = TMS.Data.Enums.TaskStatus.InProgress,
                    CreatedOn = new DateTime(2019, 05, 09, 9, 15, 0),
                    StartDate = new DateTime(2024, 05, 09, 9, 15, 0),
                    DueDate = new DateTime(2024, 05, 09, 9, 15, 0),
                    GroupId = "test111",
                    UserId = "test222"
                });

                await context.SaveChangesAsync();
            }

            using (var context = new TMSContext(options))
            {
                var taskService = new TaskService(context, mapper);

                //Act
                var result = await taskService.GetTaskByIdAsync("test123");

                //Assert
                Assert.NotNull(result);
                Assert.Equal("Test", result.Name);
                Assert.Equal("Test", result.Description);
                Assert.Equal(TMS.Data.Enums.TaskCategory.Executive, result.Category);
                Assert.Equal(TMS.Data.Enums.TaskPriority.Medium, result.Priority);
                Assert.Equal(TMS.Data.Enums.TaskStatus.InProgress, result.Status);
                Assert.Equal(new DateTime(2019, 05, 09, 9, 15, 0), result.CreatedOn);
                Assert.Equal(new DateTime(2024, 05, 09, 9, 15, 0), result.StartDate);
                Assert.Equal(new DateTime(2024, 05, 09, 9, 15, 0), result.DueDate);
                Assert.Equal("test111", result.GroupId);
                Assert.Equal("test222", result.UserId);
            }
        }

        [Fact]
        public async void UpdateTaskAsync_ShouldUpdateTask()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<TMSContext>()
                .UseInMemoryDatabase(databaseName: "UpdateTaskAsync_ShouldUpdateTask")
                .Options;

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TMS.WebHost.Models.Mapper>();
            });

            IMapper mapper = configuration.CreateMapper();

            using (var context = new TMSContext(options))
            {
                context.Database.EnsureCreated();

                context.Tasks.Add(new TMS.Data.Models.Task
                {
                    TaskId = "test123",
                    Name = "Test",
                    Description = "Test",
                    Category = TMS.Data.Enums.TaskCategory.Executive,
                    Priority = TMS.Data.Enums.TaskPriority.Medium,
                    Status = TMS.Data.Enums.TaskStatus.InProgress,
                    CreatedOn = new DateTime(2019, 05, 09, 9, 15, 0),
                    StartDate = new DateTime(2024, 05, 09, 9, 15, 0),
                    DueDate = new DateTime(2024, 05, 09, 9, 15, 0),
                    GroupId = "test111",
                    UserId = "test222"
                });

                await context.SaveChangesAsync();
            }

            using (var context = new TMSContext(options))
            {
                var taskService = new TaskService(context, mapper);

                //Act
                await taskService.UpdateTaskAsync("test123", new TMS.Services.Models.TaskUM
                {
                    TaskId = "test123",
                    Name = "Test1",
                    Description = "Test1",
                    Category = TMS.Data.Enums.TaskCategory.PersonalWork,
                    Priority = TMS.Data.Enums.TaskPriority.Low,
                    Status = TMS.Data.Enums.TaskStatus.Completed,
                    StartDate = new DateTime(2024, 05, 09, 9, 15, 0),
                    DueDate = new DateTime(2024, 05, 09, 9, 15, 0),
                    GroupId = "test111",
                    UserId = "test222"
                });

                //Assert
                Assert.Equal("Test1", context.Tasks.First().Name);
                Assert.Equal("Test1", context.Tasks.First().Description);
                Assert.Equal(TMS.Data.Enums.TaskCategory.PersonalWork, context.Tasks.First().Category);
                Assert.Equal(TMS.Data.Enums.TaskPriority.Low, context.Tasks.First().Priority);
                Assert.Equal(TMS.Data.Enums.TaskStatus.Completed, context.Tasks.First().Status);
                Assert.Equal(new DateTime(2024, 05, 09, 9, 15, 0), context.Tasks.First().StartDate);
                Assert.Equal(new DateTime(2024, 05, 09, 9, 15, 0), context.Tasks.First().DueDate);
                Assert.Equal("test111", context.Tasks.First().GroupId);
                Assert.Equal("test222", context.Tasks.First().UserId);
            }
        }
    }
}
