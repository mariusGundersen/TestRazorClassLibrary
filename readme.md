# Testing razor class library

## Creating from scratch

```
git init .
echo bin/ > .gitignore
echo obj/ >> .gitignore
dotnet new sln
dotnet new razorclasslib -o src/MyRazorClassLib
dotnet new mvc -o test/MyRazorClassLib.Test
dotnet sln add src/MyRazorClassLib/MyRazorClassLib.csproj
dotnet sln add test/MyRazorClassLib.Test/MyRazorClassLib.Test.csproj
dotnet add test/MyRazorClassLib.Test/MyRazorClassLib.Test.csproj reference src/MyRazorClassLib/MyRazorClassLib.csproj
dotnet run --project test/MyRazorClassLib.Test/MyRazorClassLib.Test.csproj
```