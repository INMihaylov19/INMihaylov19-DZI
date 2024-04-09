using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Moq;
using TMS.Data.Data;
using TMS.Data.Models;
using TMS.Services.Contracts;
using TMS.Services.Implementations;
using TMS.Services.Models;
using Xunit;

namespace Tests.Services.ServiceTests
{
    public class GroupServiceTests
    {
        [Fact]

        public async void CreateGroupAsync_ShouldCreateGroup()
        {
            //Arrange 
            var options = new DbContextOptionsBuilder<TMSContext>()
                .UseInMemoryDatabase(databaseName: "CreateGroupAsync_ShouldCreateGroup")
                .Options;

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TMS.WebHost.Models.Mapper>();
            });

            IMapper mapper = configuration.CreateMapper();

            using (var context = new TMSContext(options))
            {
                context.Database.EnsureCreated();

                context.Groups.Add(new Group
                {
                    GroupId = "test",
                    GroupName = "Test Group"
                });

                await context.SaveChangesAsync();
            }

            using (var context = new TMSContext(options))
            {
                var groupService = new GroupService(context, mapper);

                //Act

                var result = await groupService.CreateGroupAsync(new Group
                {
                    GroupId = "test",
                    GroupName = "Test Group"
                });

                //Assert
                Assert.NotNull(result);
                Assert.Equal("Test Group", result.GroupName);
            }
        }
        [Fact]

        public async void DeleteGroupAsync_ShouldDeleteGroup()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<TMSContext>()
                .UseInMemoryDatabase(databaseName: "DeleteGroupAsync_ShouldDeleteGroup")
                .Options;

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TMS.WebHost.Models.Mapper>();
            });

            IMapper mapper = configuration.CreateMapper();

            using (var context = new TMSContext(options))
            {
                context.Database.EnsureCreated();

                context.Groups.Add(new Group
                {
                    GroupId = "test",
                    GroupName = "Test Group"
                });

                await context.SaveChangesAsync();
            }

            using (var context = new TMSContext(options))
            {
                var groupService = new GroupService(context, mapper);

                //Act
                await groupService.DeleteGroupAsync("test");

                //Assert
                Assert.Equal(0, context.Groups.Count());
            }
        }

        [Fact]
        public async void GetAllGroupsAsync_ShouldReturnAllGroups()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<TMSContext>()
                .UseInMemoryDatabase(databaseName: "GetAllGroupsAsync_ShouldReturnAllGroups")
                .Options;

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TMS.WebHost.Models.Mapper>();
            });

            IMapper mapper = configuration.CreateMapper();

            using (var context = new TMSContext(options))
            {
                context.Database.EnsureCreated();

                context.Groups.Add(new Group
                {
                    GroupId = "test",
                    GroupName = "Test Group"
                });

                context.Groups.Add(new Group
                {
                    GroupId = "test2",
                    GroupName = "Test Group 2"
                });

                await context.SaveChangesAsync();
            }

            using (var context = new TMSContext(options))
            {
                var groupService = new GroupService(context, mapper);

                //Act
                var result = await groupService.GetAllGroupsAsync();

                //Assert
                Assert.Equal(2, result.Count);
            }
        }

        [Fact]
        public async void GetGroupByIdAsync_ShouldReturnGroup()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<TMSContext>()
                .UseInMemoryDatabase(databaseName: "GetGroupByIdAsync_ShouldReturnGroup")
                .Options;

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TMS.WebHost.Models.Mapper>();
            });

            IMapper mapper = configuration.CreateMapper();

            using (var context = new TMSContext(options))
            {
                context.Database.EnsureCreated();

                context.Groups.Add(new Group
                {
                    GroupId = "test",
                    GroupName = "Test Group"
                });

                await context.SaveChangesAsync();
            }

            using (var context = new TMSContext(options))
            {
                var groupService = new GroupService(context, mapper);

                //Act
                var result = await groupService.GetGroupByIdAsync("test");

                //Assert
                Assert.NotNull(result);
                Assert.Equal("Test Group", result.GroupName);
            }
        }

        [Fact]
        public async void GetGroupByNameAsync_ShouldReturnGroup()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<TMSContext>()
                .UseInMemoryDatabase(databaseName: "GetGroupByNameAsync_ShouldReturnGroup")
                .Options;

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TMS.WebHost.Models.Mapper>();
            });

            IMapper mapper = configuration.CreateMapper();

            using (var context = new TMSContext(options))
            {
                context.Database.EnsureCreated();

                context.Groups.Add(new Group
                {
                    GroupId = "test",
                    GroupName = "Test Group"
                });

                await context.SaveChangesAsync();
            }

            using (var context = new TMSContext(options))
            {
                var groupService = new GroupService(context, mapper);

                //Act
                var result = await groupService.GetGroupdByNameAsync("Test Group");

                //Assert
                Assert.NotNull(result);
                Assert.Equal("Test Group", result[0].GroupName);
            }
        }
    }
}
