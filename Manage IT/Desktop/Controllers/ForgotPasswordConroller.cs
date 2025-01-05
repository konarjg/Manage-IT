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
            error = string.Empty;
            User user;
            try
            {
                bool success = UserManager.Instance.UserExists(data, out user);
                if (!success)
                {
                    error = "User does not exist.";
                    return;
                }

                // Add additional logic for handling password reset, e.g., sending a reset email.
                error = "Password reset instructions sent.";
            }
            catch (Exception ex)
            {
                // Handle potential exceptions
                error = $"An error occurred: {ex.Message}";
            }
        }

            error = "";
            return;
        }

        error = "";
    }
}
