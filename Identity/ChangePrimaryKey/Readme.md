# ASP.NET Core Identity Change Primary Key

Web application to demonstrate changing primary type in Identity from guid string to int, by default ASP.NET Core Identity use a string value for the primary keys.

First, when creating new ASP.NET Core Web Application, in the ASP.NET Core template selecting dialog, choose “Web Application”, then click on the “Change Authentication” button, and choose “Individual User Accounts”

## The model

After the project has been created. Open the ApplicationUser.cs in the “Model” folder, you will see that it inherit from IdentityUser which then inherit from IdentityUser<string>. So we need to change that

```cs
public class ApplicationUser : IdentityUser<int>
```

## The DbContext

We need to modify a little bit the inheritance of ApplicationDbContext. Instead of inheriting from IdentityDbContext<ApplicationUser>, it should be

```cs
 public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
```

## The Startup.cs

Telling ASP.NET Core Identity that we are using int, not string

```cs
services.AddIdentity<ApplicationUser, IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext, int>()
    .AddDefaultTokenProviders();
```

## The Migrations

We will need to re-create entity framework migration classes. Delete the project template generated file in folder “Data\Migrations”, then go to the Package Manager Console and run the command below 
```
PM> Add-Migration CreateIdentitySchema
```

Initial database with command below

```
PM> Update-database
```