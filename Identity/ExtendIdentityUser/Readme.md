# ASP.NET Core Identity Extend Identity Property

Web application to demonstrate extend Identity Property.

First, when creating new ASP.NET Core Web Application, in the ASP.NET Core template selecting dialog, choose “Web Application”, then click on the “Change Authentication” button, and choose “Individual User Accounts”

## The model

After the project has been created. Open the ApplicationUser.cs in the “Model” folder, and then add some additional property

```cs
public class ApplicationUser : IdentityUser
{
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public DateTime? BirthDate { set; get; }

	public string FullName()
	{
		return LastName + " " + FirstName;
	}
}
```

## The ViewModel

Modify a little bit of RegisterViewModel and IndexViewModel.

```cs
namespace ExtendIdentityUser.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "LastName")]
        public string LastName { get; set; }
		
		...
		...
    }
}
```

```cs
namespace ExtendIdentityUser.Models.ManageViewModels
{
    public class IndexViewModel
    {
		...
		...
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
```

## The Controller

Implement FirstName, LastName in the AccountController.cs and ManageController.cs to see that changes effect.

## The Migrations

Go to the Package Manager Console and run the command below 

```
PM> Add-Migration ExtendIdentityProperty
```