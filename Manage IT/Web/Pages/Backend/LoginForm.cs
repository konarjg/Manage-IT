﻿using EFModeling.EntityProperties.DataAnnotations.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

public class LoginForm : PageModel
{
    public string Error { get; set; }

    public void OnGet()
    {
        
    }

    public IActionResult OnPost(string credential, string password)
    {
        if (credential == null || password == null)
        {
            Error = "You have to fill in every field!";
            return null;
        }

        password = Security.HashText(password, Encoding.UTF8);

        var data = new User();
        data.Email = credential;
        data.Login = credential;
        data.Password = password;

        bool success = UserManager.Instance.LoginUser(data);

        if (!success)
        {
            Error = "Provided credentials are invalid!";
            return null;
        }

        return Redirect("~/ProjectManagement");
    }
}