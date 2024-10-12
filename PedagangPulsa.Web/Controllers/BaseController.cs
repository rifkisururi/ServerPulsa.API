using Microsoft.AspNetCore.Mvc;

namespace PedagangPulsa.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected string Username
        {
            get
            {
                return User.Identity.IsAuthenticated ? User.Identity.Name : string.Empty;
            }
        }
    }
}
