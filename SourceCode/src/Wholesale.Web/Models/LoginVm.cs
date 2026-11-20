using System.ComponentModel.DataAnnotations;
namespace Wholesale.Web.Models;
public class LoginVm
{
    [Required] public string UserName { get; set; } = "";
    [Required, DataType(DataType.Password)] public string Password { get; set; } = "";
    public string? ReturnUrl { get; set; }
}
