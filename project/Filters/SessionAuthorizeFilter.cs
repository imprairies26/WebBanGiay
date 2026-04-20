using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace project.Filters
{
    public class SessionAuthorizeFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetString("UserId");
            var roleIdStr = context.HttpContext.Session.GetString("UserRoleId");
            
            var controllerName = context.RouteData.Values["controller"]?.ToString();
            var actionName = context.RouteData.Values["action"]?.ToString();
            var areaName = context.RouteData.Values["area"]?.ToString();

            // 1. TRANG CÔNG KHAI
            bool isPublicPage = 
                (controllerName == "Account") || 
                (controllerName == "Home" && actionName == "Index") || 
                (controllerName == "Product" && actionName == "Detail") || 
                (controllerName == "Cart");

            // 2. CHƯA ĐĂNG NHẬP
            if (string.IsNullOrEmpty(userId) && !isPublicPage)
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { area = "" });
                return;
            }

            // 3. KIỂM TRA PHÂN QUYỀN (1: Admin, 2: Staff, 3: Customer)
            if (!string.IsNullOrEmpty(roleIdStr))
            {
                int roleId = int.Parse(roleIdStr);

                if (areaName == "Admin" && roleId != 1)
                {
                    context.Result = new RedirectToActionResult("Index", "Home", new { area = "User" });
                }
                else if (areaName == "POS" && roleId != 2 && roleId != 1)
                {
                    context.Result = new RedirectToActionResult("Index", "Home", new { area = "User" });
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) {}
    }
}
