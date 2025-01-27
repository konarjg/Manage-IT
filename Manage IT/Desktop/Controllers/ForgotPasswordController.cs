using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace Desktop
{
    public static class ForgotPasswordController
    {
        public static void SubmitForgotPasswordForm(User data, out string error)
        {
            UserManager.Instance.SendPasswordRestorationEmail(data, out error);
        }
    }
}
