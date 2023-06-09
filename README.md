# .NET7, Vue, Bootstrap, Toastr Demo

![app](app.png)

Easiest way to run:
 - 1. Install Docker
 - 2. From project's root folder run command:
 ```
 docker compose up
 ```
 It will create container and images for backend, frontend.
 - 3. In browser type ``` http://localhost:7115/healthcheck ``` for healthcheck.
 - 4. In browser type ``` http://localhost:5175 ```. It will open Vue app.

### Example of Dependency Injection:
Program.cs
``` 
builder.Services.AddScoped<IZipService, ZipService>();
```
ZipController.cs 
```
private readonly IZipService _zipService;

public ZipController(IZipService zipService)
{
    _zipService = zipService;
}
```
 
### Example of middlewares
Program.cs
```
app.Use(next => async context =>
{
    var stopWatch = new Stopwatch();
    stopWatch.Start();

    context.Response.OnStarting(() =>
    {
        stopWatch.Stop();
        context.Response.Headers.Add("X-ResponseTime-Ms", stopWatch.ElapsedMilliseconds.ToString());
        return Task.CompletedTask;
    });

    await next(context);
});
```
This block of code adds X-ResponseTime-Ms in headers, which indicates how much time was spent in ms for execution of particular method

### Example of error handling
ZipController.cs
```
  try
  {
      List<Node> tree = _zipService.GetZipContent(zipName);
      return Ok(new { content = tree });
  }
  catch (FileNotFoundException ex)
  {
      return NotFound(ex.Message);
  }
  ```
  
  ### Swagger support
  ```
  if (app.Environment.IsDevelopment())
  {
    app.UseSwagger();
    app.UseSwaggerUI();
  }
  ```
 
 ## Project structure:
 ```
  \---.NET7_Vue-main
    |   app.png
    |   docker-compose.yml
    |   README.md
    |   TestTaskDescription.docx
    |   
    +---API_Tests
    |   |   API_Tests.csproj
    |   |   Usings.cs
    |   |   ZipControllerTest.cs
    |   |                
    |   +---Properties
    |          Resources.Designer.cs
    |          Resources.resx
    |                         
    +---backend
    |   |   .dockerignore
    |   |   .gitignore
    |   |   appsettings.Development.json
    |   |   appsettings.json
    |   |   backend.csproj
    |   |   Dockerfile
    |   |   Program.cs
    |   |   
    |   +---Constants
    |   |       EM.cs
    |   |       
    |   +---Controllers
    |   |       ZipController.cs
    |   |       
    |   +---IServices
    |   |       ICultureService.cs
    |   |       IZipService.cs
    |   |       
    |   +---Models
    |   |       Node.cs
    |   |       ZipEntry.cs
    |   |       
    |   +---Properties
    |   |       launchSettings.json
    |   |       
    |   +---Services
    |   |       CultureService.cs
    |   |       ZipService.cs
    |   |       
    |   \---zips
    |       |   CatGame - Copy.zip
    |       |   CatGame - Copy2.zip
    |       |   CatGame.zip
    |       |   CatGame2.zip
    |       |   
    |       \---CatGame
    |           +---dlls
    |           |       CatGame.dll
    |           |       
    |           +---images
    |           |       0.png
    |           |       1.png
    |           |       2.png
    |           |       3.png
    |           |       
    |           \---languages
    |                   CatGame.xml
    |                   CatGame_en.xml
    |                   
    \---frontend
        |   .dockerignore
        |   .eslintrc.cjs
        |   .gitignore
        |   .prettierrc.json
        |   cypress.config.ts
        |   Dockerfile
        |   env.d.ts
        |   index.html
        |   package-lock.json
        |   package.json
        |   README.md
        |   tsconfig.app.json
        |   tsconfig.json
        |   tsconfig.node.json
        |   tsconfig.vitest.json
        |   vite.config.ts
        |   vitest.config.ts
        |   
        +---.vscode
        |       extensions.json
        |       
        +---cypress
        |   +---e2e
        |   |       example.cy.ts
        |   |       tsconfig.json
        |   |       
        |   +---fixtures
        |   |       example.json
        |   |       
        |   \---support
        |           commands.ts
        |           e2e.ts
        |           
        +---public
        |       favicon.ico
        |       
        \---src
            |   App.vue
            |   main.js
            |   
            +---assets
            |       base.css
            |       logo.svg
            |       main.css
            |       
            +---components
            |       Node.vue
            |       ZipComponent.vue
            |       
            +---router
            |       index.ts
            |       
            \---stores
                    counter.ts
                    
```
