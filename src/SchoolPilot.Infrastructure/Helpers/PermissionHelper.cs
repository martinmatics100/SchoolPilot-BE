

using Microsoft.EntityFrameworkCore;
using SchoolPilot.Common.Enums;
using SchoolPilot.Data.Context;
using SchoolPilot.Data.Entities.Users;

namespace SchoolPilot.Infrastructure.Helpers
{

    public interface IPermissionHelper
    {
        Task<bool> UserHasPermission(Guid userId, Guid accountId, PermissionActions permissionAction, ParentPermission parentPermission);

        Task<bool> HasPermission(Guid userAffiliationId, Guid accountId, PermissionActions permissionAction, ParentPermission parentPermission);
    }

    public class PermissionHelper : IPermissionHelper
    {
        private readonly ReadSchoolPilotContext _readContext;

        public PermissionHelper(ReadSchoolPilotContext readContext)
        {
            _readContext = readContext;
        }

        public async Task<bool> UserHasPermission(Guid userId, Guid accountId, PermissionActions permissionAction, ParentPermission parentPermission)
        {
            var userAffiliationId = await (from userAffiliation in _readContext.UserAffiliations
                                           where userAffiliation.AccountId == accountId
                                                 && userAffiliation.UserId == userId
                                                 && !userAffiliation.IsDeprecated
                                           select userAffiliation.Id).FirstOrDefaultAsync();
            if (userAffiliationId == Guid.Empty)
            {
                return false;
            }

            return await (from userPrinciples in _readContext.UserPrinciples
                          join principle in _readContext.Principles on userPrinciples.PrincipleId equals principle.Id
                          join permission in _readContext.Permissions on principle.Id equals permission.PrincipleId
                          where userPrinciples.UserAffiliationId == userAffiliationId
                                && userPrinciples.AccountId == accountId
                                && permission.Resource == ((int)parentPermission).ToString()
                                && permission.Action == ((int)permissionAction)
                                && permission.Value == PermissionValue.Granted
                          select permission).AnyAsync();
        }

        public async Task<bool> HasPermission(Guid userAffiliationId, Guid accountId, PermissionActions permissionAction, ParentPermission parentPermission)
        {
            return await (from userPrinciples in _readContext.UserPrinciples
                          join principle in _readContext.Principles on userPrinciples.PrincipleId equals principle.Id
                          join permission in _readContext.Permissions on principle.Id equals permission.PrincipleId
                          where userPrinciples.UserAffiliationId == userAffiliationId
                                && userPrinciples.AccountId == accountId
                                && permission.Resource == ((int)parentPermission).ToString()
                                && permission.Action == ((int)permissionAction)
                                && permission.Value == PermissionValue.Granted
                          select permission).AnyAsync();
        }

    }
}
