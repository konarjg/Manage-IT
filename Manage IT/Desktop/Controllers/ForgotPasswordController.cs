using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
<<<<<<< HEAD:Manage IT/Desktop/Controllers/ForgotPasswordConroller.cs
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
=======

public static class ForgotPasswordController
{
    public static void SubmitForgotPasswordForm(User data, out string error)
    {
        UserManager.Instance.SendPasswordRestorationEmail(data, out error);
    }
}
>>>>>>> main:Manage IT/Desktop/Controllers/ForgotPasswordController.cs
