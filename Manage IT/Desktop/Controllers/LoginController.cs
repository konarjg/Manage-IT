using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Desktop
{
    public static class LoginController
    {
        public static void SubmitLoginForm(User data, out string error)
        {
            bool success = UserManager.Instance.LoginUser(data);

            if (!success)
            {
                error = "Provided credentials are invalid!";
                return;
            }

            //Remove when the project page is finished
            error = "Logged in!";
        }
    }
}
