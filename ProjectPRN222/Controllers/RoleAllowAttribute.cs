using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProjectPRN222.Controllers
{
    [RoleAllow(5)]
    public class RoleAllowAttribute : Attribute, IAuthorizationFilter
    {
        private readonly int[] _allowedRoles;

        public RoleAllowAttribute(params int[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var roleId = context.HttpContext.Session.GetInt32("RoleId");

            // Nếu roleId không nằm trong danh sách được phép, chuyển về AccessDenied
            if (roleId == null || !_allowedRoles.Contains(roleId.Value))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
            }
        }
    }
}
