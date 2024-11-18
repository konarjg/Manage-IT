using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop
{
    public static class RegisterController
    {
        public static void SubmitRegisterForm(User data, out string error)
        {
            UserManager.Instance.RegisterUser(data, out error);
        }
    }
}
