using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using TMS.Data.Data;
using TMS.Services.Contracts;
using TMS.Services.Models;

namespace TMS.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly TMSContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public UserService(TMSContext context,
            IMapper mapper,
            ICurrentUser currentUser)
        {
            _context = context;
            _mapper = mapper;
            _currentUser = currentUser;
        }
        public async Task<UserVM> CreateUserAsync(Data.Models.User user)
        {
            user.Id = Guid.NewGuid().ToString();

            _context.Add(user);

            await _context.SaveChangesAsync();

            return _mapper.Map<UserVM>(user);
        }
        public async Task DeleteUserAsync(string userId)
        {
            var user = await _context
                .Users
                .Include(u => u.Tasks)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            foreach (var task in user.Tasks)
            {
                task.UserId = null;
                task.User = null;
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();
        }
        public async Task<List<UserVM>> GetAllUsersAsync()
        {
            var users = await _context
                .Users
                .ProjectTo<UserVM>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return users;
        }

        public async Task<TMS.Data.Models.User> GetUserByEmailAsync(string email)
        {
            var user = await _context
                .Users
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<List<UserVM>> GetUserByFirstNameAsync(string firstName)
        {
            var user = await _context
                .Users
                .Where(u => u.FirstName == firstName)
                .ProjectTo<UserVM>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return user;
        }

        public async Task<UserVM> GetUserByIdAsync(string userId)
        {
            var user = await _context
                .Users
                .Where(u => u.Id == userId)
                .Include(u => u.Tasks)
                .ProjectTo<UserVM>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<List<UserVM>?> GetUserByRoleAsync(string searchValue)
        {
            var role = (TMS.Data.Enums.UserRole)Enum.Parse(typeof(TMS.Data.Enums.UserRole), searchValue);

            var user = await _context
                .Users
                .Where(u => u.Role == role)
                .ProjectTo<UserVM>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return user;
        }

        public async Task<List<UserVM>> GetUserByUsernameAsync(string userName)
        {
            var user = await _context
                .Users
                .Where(u => u.UserName == userName)
                .ProjectTo<UserVM>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return user;
        }

        public async Task UpdateUserAsync(string userId, UserUM user)
        {
            var userToUpdate = await _context
                .Users
                .FindAsync(userId);

            userToUpdate.Id = userId;
            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.UserName = user.Username;
            userToUpdate.Email = user.Email;
            userToUpdate.Role = user.Role;

            await _context.SaveChangesAsync();
        }

        public UserVM GetTasksFofCurrentUserAsync()
        {
            string id = _currentUser?.Id.ToString();

            var user = _context
                .Users
                .Where(u => u.Id == id)
                .Include(u => u.Tasks)
                .FirstOrDefault();

            return _mapper.Map<UserVM>(user);
        }

        public UserVM GetGroupsOfCurrentUser()
        {
            string id = _currentUser?.Id.ToString();

            var user = _context
                .Users
                .Where(u => u.Id == id)
                .Include(u => u.Groups)
                .FirstOrDefault();

            return _mapper.Map<UserVM>(user);
        }

        public bool HasClaim(string claimType, string claimValue)
        {
            return _context
                .RoleClaims
                .Any(c => c.ClaimType == claimType && c.ClaimValue == claimValue);
        }

        public async Task<int> DaysSinceRegistrationAsync(string userId)
        {
            var userVM = _context
                .Users
                .Where(u => u.Id == userId)
                .ProjectTo<UserVM>(_mapper.ConfigurationProvider)
                .FirstOrDefault();

            var daysSinceRegistration = DateTime.Now.Day - userVM.CreatedOn.Day;

            return daysSinceRegistration;
        }
    }
}
