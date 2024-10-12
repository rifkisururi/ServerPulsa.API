namespace PedagangPulsa.Web.Middleware
{
    public class UserInfoMiddleware
    {
        private readonly RequestDelegate _next;

        public UserInfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var username = context.User.Identity.Name;
                context.Items["Username"] = username;
            }

            await _next(context);
        }
    }
}
