# Testing razor class library

This repository demonstrates how to make [razor class libraries](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class?view=aspnetcore-2.1&tabs=netcore-cli) testable, without having to recompile and restart the project.

## Creating the example solution

This entire repository was made like this:

```
mkdir MyRazorClassLib
cd MyRazorClassLib
git init .
echo bin/ > .gitignore
echo obj/ >> .gitignore
dotnet new sln
dotnet new razorclasslib -o src/MyRazorClassLib
dotnet new mvc -o test/MyRazorClassLib.Test
dotnet sln add src/MyRazorClassLib/MyRazorClassLib.csproj
dotnet sln add test/MyRazorClassLib.Test/MyRazorClassLib.Test.csproj
dotnet add test/MyRazorClassLib.Test/MyRazorClassLib.Test.csproj reference src/MyRazorClassLib/MyRazorClassLib.csproj
```

This creates a solution with two projects, a [razor class library](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/ui-class?view=aspnetcore-2.1&tabs=netcore-cli), that you might pack and publish as a nuget, and a test mvc project for testing the razor class library. The mvc project is not intended to be published, it is just there for testing the razor class library.

## Testing and finding the problem

Now that the solution has been made we can test it by running the following command

```
dotnet run --project test/MyRazorClassLib.Test/MyRazorClassLib.Test.csproj
```


Now go to [https://localhost:5001/MyFeature/page1](https://localhost:5001/MyFeature/Page1) in your favorite browser. You should see a blank page, with nothing on it. That's a bit boring, so find the `src/MyRazorClassLib/Areas/MyFeature/Pages/Page1.cshtml` file and add some html content inside  the `<body>`. When you are done adding some content, go back to the browser and refresh.

What happens? Nothing. To see the change you have to stop your application (ctr+c in the commandline), recompile and start it again. That's a bit annoying, especially as you are likely to make many small changes. Why can't it update right away, like normal razor pages in MVC projects do?

There is a bit of discussion about this [here](https://github.com/aspnet/Razor/issues/2426), and that is where I got most of the idea for the following solution from.

## The solution

We need just two changes to make it possible to edit the razor pages. The first is some way to locate the `MyRazorClassLib.csproj` file location. We can do that with this class, which must be in the root of the `MyRazorClassLib` project:

```csharp
// src/MyRazorClassLib/Locator.cs
using System.IO;
using System.Runtime.CompilerServices;

namespace MyRazorClassLib
{
    public static class Locator
    {
        public static string Root => GetRoot();

        public static string GetRoot([CallerFilePath] string path = null)
            => Path.GetDirectoryName(path);
    }
}
```

This class uses the [`[CallerFilePath]`](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.callerfilepathattribute?view=netcore-2.1) attribute to find the C# source file, and then the [`Path.GetDirectoryName`](https://docs.microsoft.com/en-us/dotnet/api/system.io.path.getdirectoryname?view=netcore-2.1) to find the folder path.

Next we need to add just one line to the `Startup` class:

```csharp
// test/MyRazorClassLib.Test/Startup.cs (ConfigureServices(), line 36)
services.Configure<RazorViewEngineOptions>(o => o.FileProviders.Add(new PhysicalFileProvider(Locator.Root)));
```

This line goes in the `ConfigureServices` method. It adds a new `PhysicalFileProvider` to the `RazorViewEngineOptions` that will look in the other project as well.

If you run the project again and try editing the `Page1.cshtml` file you will see that it is updated with new content each time you refresh the browser. There is no longer any need to stop, recompile and start again. Great!