using EFModeling.EntityProperties.DataAnnotations.Annotations;
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
