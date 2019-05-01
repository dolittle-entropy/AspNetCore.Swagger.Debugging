# AspNetCore.Swagger.Debugging

To try it out, install the `Dolittle.AspNetCore.Swagger.Debugging` package, then in your _Startup class_, add the following:
```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDolittleSwagger();
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseDolittleSwagger();
    }
```