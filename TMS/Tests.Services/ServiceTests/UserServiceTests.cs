using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Data.Data;
using TMS.Services.Contracts;
using TMS.Services.Implementations;

namespace Tests.Services.ServiceTests
{
    public class UserServiceTests
    {
        private ICurrentUser currentUser;

        [Fact]
        public async void CreateUserAsync_ShouldCreateUser()
        {
            //Arrange 
            var options = new DbContextOptionsBuilder<TMSContext>()
                .UseInMemoryDatabase(databaseName: "CreateUserAsync_ShouldCreateUser")
                .Options;

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TMS.WebHost.Models.Mapper>();
            });

            IMapper mapper = configuration.CreateMapper();

            using (var context = new TMSContext(options))
            {
                context.Database.EnsureCreated();

                context.Users.Add(new TMS.Data.Models.User
                {
                    Id = "test",
                    UserName = "Test",
                    FirstName = "Test",
                    LastName = "User",
                    Email = "Test@gmail.com",
                    Role = TMS.Data.Enums.UserRole.Employee,
                });

                await context.SaveChangesAsync();
            }

            using (var context = new TMSContext(options))
            {
                var userService = new UserService(context, mapper, currentUser);

                //Act

                var result = await userService.CreateUserAsync(new TMS.Data.Models.User
                {
                    Id = "test",
                    UserName = "Test",
                    FirstName = "Test",
                    LastName = "User",
                    Email = "Test@gmail.com",
                    Role = TMS.Data.Enums.UserRole.Employee,
                });

                //Assert
                Assert.NotNull(result);
                Assert.Equal("Test", result.Username);
                Assert.Equal("Test", result.FirstName);
                Assert.Equal("User", result.LastName);
                Assert.Equal("Test@gmail.com", result.Email);
                Assert.Equal(TMS.Data.Enums.UserRole.Employee, result.Role);
            }
        }

        [Fact]
        public async void DeleteUserAsync_ShouldDeleteUser()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<TMSContext>()
                .UseInMemoryDatabase(databaseName: "DeleteUserAsync_ShouldDeleteUser")
                .Options;

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TMS.WebHost.Models.Mapper>();
            });

            IMapper mapper = configuration.CreateMapper();

            using (var context = new TMSContext(options))
            {
                context.Database.EnsureCreated();

                context.Users.Add(new TMS.Data.Models.User
                {
                    Id = "test",
                    UserName = "Test",
                    FirstName = "Test",
                    LastName = "User",
                    Email = "Test@gmail.com",
                    Role = TMS.Data.Enums.UserRole.Employee,
                });

                await context.SaveChangesAsync();
            }

            using (var context = new TMSContext(options))
            {
                var userService = new UserService(context, mapper, currentUser);

                //Act

                await userService.DeleteUserAsync("test");


                //Assert
                Assert.Equal(0, context.Users.Count());
            }
        }

        [Fact]
        public async void GetAllUsersAsync_ShouldReturnAllUsers()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<TMSContext>()
                .UseInMemoryDatabase(databaseName: "GetAllUsersAsync_ShouldReturnAllUsers")
                .Options;

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TMS.WebHost.Models.Mapper>();
            });

            IMapper mapper = configuration.CreateMapper();

            using (var context = new TMSContext(options))
            {
                context.Database.EnsureCreated();

                context.Users.Add(new TMS.Data.Models.User
                {
                    Id = "test",
                    UserName = "Test",
                    FirstName = "Test",
                    LastName = "User",
                    Email = "Test@gmail.com",
                    Role = TMS.Data.Enums.UserRole.Employee
                });

                context.Users.Add(new TMS.Data.Models.User
                {
                    Id = "test2",
                    UserName = "Test2",
                    FirstName = "Test2",
                    LastName = "User2",
                    Email = "Test2@gmail.com",
                    Role = TMS.Data.Enums.UserRole.Employer
                });

                await context.SaveChangesAsync();
            }

            using (var context = new TMSContext(options))
            {
                var userService = new UserService(context, mapper, currentUser);

                //Act
                var result = await userService.GetAllUsersAsync();

                //Assert
                Assert.Equal(2, result.Count());
            }
        }

        [Fact]
        public async void GetUserByIdAsync_ShouldReturnUserById()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<TMSContext>()
                .UseInMemoryDatabase(databaseName: "GetUserByIdAsync_ShouldReturnUserById")
                .Options;

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TMS.WebHost.Models.Mapper>();
            });

            IMapper mapper = configuration.CreateMapper();

            using (var context = new TMSContext(options))
            {
                context.Database.EnsureCreated();

                context.Users.Add(new TMS.Data.Models.User
                {
                    Id = "test",
                    UserName = "Test",
                    FirstName = "Test",
                    LastName = "User",
                    Email = "Test@gmail.com",
                    Role = TMS.Data.Enums.UserRole.Employee
                });

                await context.SaveChangesAsync();
            }

            using (var context = new TMSContext(options))
            {
                var userService = new UserService(context, mapper, currentUser);

                //Act
                var result = await userService.GetUserByIdAsync("test");

                //Assert
                Assert.NotNull(result);
                Assert.Equal("Test", result.Username);
                Assert.Equal("Test", result.FirstName);
                Assert.Equal("User", result.LastName);
                Assert.Equal("Test@gmail.com", result.Email);
                Assert.Equal(TMS.Data.Enums.UserRole.Employee, result.Role);
            }
        }

        [Fact]
        public async void UpdateUserAsync_ShouldUpdateUser()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<TMSContext>()
                .UseInMemoryDatabase(databaseName: "UpdateUserAsync_ShouldUpdateUser")
                .Options;

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TMS.WebHost.Models.Mapper>();
            });

            IMapper mapper = configuration.CreateMapper();

            using (var context = new TMSContext(options))
            {
                context.Database.EnsureCreated();

                context.Users.Add(new TMS.Data.Models.User
                {
                    Id = "test123",
                    UserName = "Test",
                    FirstName = "Test",
                    LastName = "User",
                    Email = "Test@gmail.com",
                    Role = TMS.Data.Enums.UserRole.Employee
                });

                await context.SaveChangesAsync();
            }

            using (var context = new TMSContext(options))
            {
                var userService = new UserService(context, mapper, currentUser);

                var userUM = new TMS.Services.Models.UserUM
                {
                    Id = "test123",
                    Username = "Test1",
                    FirstName = "Test1",
                    LastName = "User1",
                    Email = "Test1@gmail.com",
                    Role = TMS.Data.Enums.UserRole.Employer
                };

                //Act
                await userService.UpdateUserAsync("test123", userUM);

                //Assert
                var result = await context.Users.FirstOrDefaultAsync(x => x.Id == "test123");
                Assert.NotNull(result);
                Assert.Equal("Test1", result.UserName);
                Assert.Equal("Test1", result.FirstName);
                Assert.Equal("User1", result.LastName);
                Assert.Equal("Test1@gmail.com", result.Email);
                Assert.Equal(TMS.Data.Enums.UserRole.Employer, result.Role);
            }
        }
    }
}
