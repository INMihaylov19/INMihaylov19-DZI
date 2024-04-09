using TMS.Data.Data;
using TMS.Services.Contracts;
using TMS.Services.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace TMS.Services.Implementations
{
    public class GroupService : IGroupService
    {
        private readonly TMSContext _context;
        private readonly IMapper _mapper;

        public GroupService(TMSContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<GroupVM> CreateGroupAsync(Data.Models.Group group)
        {
            group.GroupId = Guid.NewGuid().ToString();

            _context.Add(group);

            await _context.SaveChangesAsync();

            return _mapper.Map<GroupVM>(group);
        }
        public async Task DeleteGroupAsync(string groupId)
        {
            var group = await _context
                .Groups
                .FindAsync(groupId);

            if (group == null)
            {
                throw new ArgumentException("Group not found");
            }

            _context.Groups.Remove(group);

            await _context.SaveChangesAsync();
        }
        public async Task<List<GroupVM>> GetAllGroupsAsync()
        {
            var groups = await _context
                .Groups
                .ProjectTo<GroupVM>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return groups;
        }
        public async Task<GroupVM> GetGroupByIdAsync(string groupId)
        {
            var group = await _context
                .Groups
                .Where(g => g.GroupId == groupId)
                .Include(g => g.Users)
                .Include(g => g.Tasks)
                .ProjectTo<GroupVM>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return group;
        }

        public async Task<List<GroupVM>> GetGroupdByNameAsync(string groupName)
        {
            var groups = await _context
                .Groups
                .Where(g => g.GroupName == groupName)
                .ProjectTo<GroupVM>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return groups;
        }

        public async Task UpdateGroupAsync(string groupId, GroupUM group)
        {
            var groupToUpdate = await _context
                .Groups
                .FindAsync(groupId);

            if (groupToUpdate == null)
            {
                throw new ArgumentNullException();
            }

            groupToUpdate.GroupId = groupId;
            groupToUpdate.GroupName = group.GroupName;

            await _context.SaveChangesAsync();
        }

        public async Task AssignUserToGroupAsync(string userId, string groupId)
        {
            var user = await _context
                .Users
                .Where(u => u.Id == userId)
                .Include(u => u.Groups)
                .FirstAsync();

            var groupToAssign = await _context
                .Groups
                .Where(g => g.GroupId == groupId)
                .Include(g => g.Users)
                .FirstAsync();

            groupToAssign.Users.Add(user);
            user.Groups.Add(groupToAssign);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveUserFromGroup(string userId, string groupId)
        {
            var group = await _context
                .Groups
                .Where(g => g.GroupId == groupId)
                .Include(g => g.Users)
                .FirstOrDefaultAsync();

            if (group == null)
            {
                throw new ArgumentException("Group not found");
            }

            var userToRemove = group
                .Users
                .Where(u => u.Id == userId)
                .FirstOrDefault();

            if (userToRemove == null)
            {
                throw new ArgumentException("User not found");
            }

            group.Users.Remove(userToRemove);

            await _context.SaveChangesAsync();
        }

        public async Task<List<UserVM>> GetUsersInGroupAsync(string groupId)
        {
            var users = await _context
                .Users
                .Where(u => u.Groups.Any(g => g.GroupId == groupId))
                .ProjectTo<UserVM>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return users;
        }
    }
}
