using AutoMapper;
using System.Runtime.CompilerServices;
using TMS.Data.Models;
using TMS.Services.Models;
namespace TMS.WebHost.Models
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            this.CreateMap<User, UserVM>();
            this.CreateMap<TMS.Data.Models.Task, TaskVM>();
            this.CreateMap<Group, GroupVM>();
            this.CreateMap<TaskIM, TMS.Data.Models.Task>();
            this.CreateMap<UserIM, TMS.Data.Models.User>();
            this.CreateMap<GroupIM, TMS.Data.Models.Group>();
            this.CreateMap<GroupVM, TMS.Services.Models.GroupUM>();
            this.CreateMap<TaskVM, TMS.Services.Models.TaskUM>();
            this.CreateMap<TaskVM, TMS.Data.Models.Task>();
            this.CreateMap<UserVM, TMS.Services.Models.UserUM>();
            this.CreateMap<User, TMS.Services.Models.UserUM>();
            this.CreateMap<UserVM, User>();
            this.CreateMap<Group, List<GroupVM>>();
        }
    }
}
