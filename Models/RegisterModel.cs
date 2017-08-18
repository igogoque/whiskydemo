using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace whiskyshop.Models
{
    public class RegisterModel
    {

        [Required]
        [Display(Name="Имя")]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не сопадают")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    //для онгуляра свой регистер модел
    public class OutRegisterModel 
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Password2 { get; set; }
        public string OutUrl { get; set; }
    }

    //и свой модел для подтверждения
    public class OutRegConfirm
    {
        public string userid { get; set; }
        public string usertoken { get; set; }
    }

    //и свои модели для сброса пароля
    public class ForgotPassReq
    {
        public string forgotemail { get; set; }
        public string outurl { get; set; }
    }

    public class ForgotPassConfirm
    {
        public string userid { get; set; }
        public string usertoken { get; set; }
        public string Password { get; set; }
        public string Password2 { get; set; }
    }


//модель для логина
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }



    public class PageInfo
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / PageSize); }
        }
    }

    public class ProductListByCat
    {
        public IEnumerable<Product> Products { get; set; }
        public PageInfo PageInfo { get; set; }
    }

    public class PurchasePageList
    {
        public IEnumerable<Purchase> Purchases { get; set; }
        public PageInfo PageInfo { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        public string Email { get; set; }
    }

    public class ResetPassModel
    {
        public string UserId { get; set; }
        public string Token { get; set; }

        [Required]
        [Display(Name = "Новый пароль")]
        [DataType(DataType.Password)]
        public string NewPass { get; set; }

        [Required]
        [Display(Name = "Подтверждение нового пароля")]
        [Compare("NewPass", ErrorMessage = "Пароли не сопадают")]
        [DataType(DataType.Password)]
        public string ConfirmNewPass { get; set; }

    }

    public class PurchaseTransfer
    {
        public int Id { get; set;}
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string UserId { get; set; }
        public int qnty { get; set; }
        public decimal? summa { get; set; }

    }

    public class UserInfo 
    {
        public string UserId { get; set;}
        public string Email { get; set;}
        public string userName { get; set; }
    }

    public class CheckLogin
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string userName { get; set; }
        public bool IsConfirmed { get; set; }
    }

}