# ASP.NET Core Web API Help Pages using Swagger

Web application to demonstrate generating Web API documentation by using “Swagger” with the .NET Core implementation Swashbuckle.AspNetCore.

## Setup

1. Download the source code and open the solution in Visual Studio 2017.
2. Build your application and run it.
3. Open a browser and navigate to http://localhost:5000/swagger to view the Swagger UI

If you would like to manually install Swashbuckle packages for your own project, add the Swashbuckle from the Package Manager Console window:

```
PM> Install-Package Swashbuckle.AspNetCore
```

Add the following using statement for the Startup.cs

```cs
using Swashbuckle.AspNetCore.Swagger;
```

Configure the Swagger in the Startup.cs

```cs
public void ConfigureServices(IServiceCollection services)
{
	// Add framework services.
	services.AddMvc();

	// Register the Swagger generator, defining one or more Swagger documents
	services.AddSwaggerGen(c =>
	{
		c.SwaggerDoc("v1", new Info
		{
			Version = "v1",
			Title = "Sample API",
			Description = "A simple example ASP.NET Core Web API",
			TermsOfService = "None",
			Contact = new Contact { Name = "Samples Contributor", Email = "", Url = "https://github.com/nicholashew/AspNet-Samples" },
			License = new License { Name = "Use under License XXX", Url = "https://example.com/license" }
		});
	});
}

public void Configure(IApplicationBuilder app)
{
    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();

    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });

    app.UseMvc();
}
```

## Note

This is an example of Swagger impementation only, you should implement authorize checks for the Swagger UI in real project.