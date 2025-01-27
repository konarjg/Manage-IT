using EFModeling.EntityProperties.DataAnnotations.Annotations;

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
