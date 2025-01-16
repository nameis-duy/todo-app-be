using Application.Interface.Service;

namespace API.Controllers
{
    public class AuthController(IAccountService accountService) : BaseController
    {
    }
}
