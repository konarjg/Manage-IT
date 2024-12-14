using EFModeling.EntityProperties.DataAnnotations.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ForgotPasswordConroller
{ 
    public static void SubmitForgotPasswordForm(User data, out string error)
    {
        User user;
        bool success = UserManager.Instance.UserExists(data, out user);

        if (!success)
        {
            error = "";
            return;
        }


    }

}