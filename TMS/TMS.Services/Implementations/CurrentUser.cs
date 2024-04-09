using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Services.Contracts;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace TMS.Services.Implementations
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private Guid? _userId;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? Id
        {
            get
            {
                if (!_userId.HasValue)
                {
                    var rawUserId = _httpContextAccessor?
                        .HttpContext?
                        .User?
                        .Claims
                        .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?
                        .Value;

                    if (!string.IsNullOrWhiteSpace(rawUserId) && Guid.TryParse(rawUserId, out var parsedUserId))
                    {
                        _userId = parsedUserId;
                    }
                }

                return _userId;
            }
        }
    }
}
