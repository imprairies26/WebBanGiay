# CodeFlow Analysis Report

**Repository:** Local Folder
**Analyzed:** 4/21/2026, 4:54:04 PM

## Summary

| Metric | Value |
|--------|-------|
| Health Score | 80/100 (B) |
| Files | 137 |
| Functions | 26 |
| Lines of Code | 19,563 |
| Dependencies | 11 |
| Unused Functions | 0 |
| Security Issues | 19 |

## Security Issues

### HIGH: XSS Vulnerability
- **File:** `Areas/Admin/Views/Product/Create.cshtml`
- **Description:** Direct HTML injection can lead to XSS attacks. Sanitize user input.

### HIGH: XSS Vulnerability
- **File:** `Areas/Admin/Views/Product/Edit.cshtml`
- **Description:** Direct HTML injection can lead to XSS attacks. Sanitize user input.

### HIGH: XSS Vulnerability
- **File:** `Areas/POS/Views/Sale/Index.cshtml`
- **Description:** Direct HTML injection can lead to XSS attacks. Sanitize user input.

### HIGH: SQL Injection Risk
- **File:** `Areas/User/Views/Product/Detail.cshtml`
- **Description:** String concatenation in SQL queries. Use parameterized queries instead.
- **Code:** `<input type="radio" name="size" value="${v.size}" id="size-${v.id}" class="btn-c`

### HIGH: XSS Vulnerability
- **File:** `Areas/User/Views/Product/Detail.cshtml`
- **Description:** Direct HTML injection can lead to XSS attacks. Sanitize user input.

### HIGH: Hardcoded Secret
- **File:** `bin/Debug/net10.0/BuildHost-net472/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.exe.config` (line 9)
- **Description:** Credentials should never be hardcoded. Use environment variables or a secrets manager.
- **Code:** `<assemblyIdentity name="Microsoft.Build" publicKeyToken="b03f5f7f11d50a3a" cultu`

### HIGH: Hardcoded Secret
- **File:** `bin/Debug/net10.0/BuildHost-net472/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.exe.config` (line 15)
- **Description:** Credentials should never be hardcoded. Use environment variables or a secrets manager.
- **Code:** `<assemblyIdentity name="Microsoft.Build.Framework" publicKeyToken="b03f5f7f11d50`

### HIGH: Hardcoded Secret
- **File:** `bin/Debug/net10.0/BuildHost-net472/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.exe.config` (line 21)
- **Description:** Credentials should never be hardcoded. Use environment variables or a secrets manager.
- **Code:** `<assemblyIdentity name="Microsoft.Build.Utilities.Core" publicKeyToken="b03f5f7f`

### HIGH: Hardcoded Secret
- **File:** `bin/Debug/net10.0/BuildHost-net472/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.exe.config` (line 27)
- **Description:** Credentials should never be hardcoded. Use environment variables or a secrets manager.
- **Code:** `<assemblyIdentity name="Microsoft.Build.Tasks.Core" publicKeyToken="b03f5f7f11d5`

### HIGH: Hardcoded Secret
- **File:** `bin/Debug/net10.0/BuildHost-net472/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.exe.config` (line 33)
- **Description:** Credentials should never be hardcoded. Use environment variables or a secrets manager.
- **Code:** `<assemblyIdentity name="Microsoft.IO.Redist" publicKeyToken="cc7b13ffcd2ddd51" c`

### HIGH: Hardcoded Secret
- **File:** `bin/Debug/net10.0/BuildHost-net472/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.exe.config` (line 39)
- **Description:** Credentials should never be hardcoded. Use environment variables or a secrets manager.
- **Code:** `<assemblyIdentity name="System.Buffers" publicKeyToken="cc7b13ffcd2ddd51" cultur`

### HIGH: Hardcoded Secret
- **File:** `bin/Debug/net10.0/BuildHost-net472/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.exe.config` (line 45)
- **Description:** Credentials should never be hardcoded. Use environment variables or a secrets manager.
- **Code:** `<assemblyIdentity name="System.Collections.Immutable" publicKeyToken="b03f5f7f11`

### HIGH: Hardcoded Secret
- **File:** `bin/Debug/net10.0/BuildHost-net472/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.exe.config` (line 51)
- **Description:** Credentials should never be hardcoded. Use environment variables or a secrets manager.
- **Code:** `<assemblyIdentity name="System.Memory" publicKeyToken="cc7b13ffcd2ddd51" culture`

### HIGH: Hardcoded Secret
- **File:** `bin/Debug/net10.0/BuildHost-net472/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.exe.config` (line 57)
- **Description:** Credentials should never be hardcoded. Use environment variables or a secrets manager.
- **Code:** `<assemblyIdentity name="System.Runtime.CompilerServices.Unsafe" publicKeyToken="`

### HIGH: Hardcoded Secret
- **File:** `bin/Debug/net10.0/BuildHost-net472/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.exe.config` (line 63)
- **Description:** Credentials should never be hardcoded. Use environment variables or a secrets manager.
- **Code:** `<assemblyIdentity name="System.Threading.Tasks.Extensions" publicKeyToken="cc7b1`

### HIGH: Hardcoded Secret
- **File:** `codeflow-report.md` (line 44)
- **Description:** Credentials should never be hardcoded. Use environment variables or a secrets manager.
- **Code:** `- **Code:** `<assemblyIdentity name="Microsoft.Build" publicKeyToken="b03f5f7f11`

### HIGH: Hardcoded Secret
- **File:** `codeflow-report.md` (line 64)
- **Description:** Credentials should never be hardcoded. Use environment variables or a secrets manager.
- **Code:** `- **Code:** `<assemblyIdentity name="Microsoft.IO.Redist" publicKeyToken="cc7b13`

### HIGH: Hardcoded Secret
- **File:** `codeflow-report.md` (line 69)
- **Description:** Credentials should never be hardcoded. Use environment variables or a secrets manager.
- **Code:** `- **Code:** `<assemblyIdentity name="System.Buffers" publicKeyToken="cc7b13ffcd2`

### HIGH: Hardcoded Secret
- **File:** `codeflow-report.md` (line 79)
- **Description:** Credentials should never be hardcoded. Use environment variables or a secrets manager.
- **Code:** `- **Code:** `<assemblyIdentity name="System.Memory" publicKeyToken="cc7b13ffcd2d`

## Design Patterns

### 🏭 Factory
Creates objects without specifying exact class. Enables loose coupling and extensibility.

**Files:** `Create.cshtml`, `Edit.cshtml`, `Index.cshtml`, `Detail.cshtml`

### 👁️ Observer/Event
Defines a subscription mechanism for event-driven architecture. Great for decoupling.

**Files:** `Index.cshtml`, `Index.cshtml`, `Index.cshtml`, `Index.cshtml`, `_Layout.cshtml`

### 🌐 Context Provider
React Context for global state. Alternative to prop drilling.

**Files:** `project.pdb`, `project.pdb`, `project.assets.json`, `project.csproj.nuget.dgspec.json`, `codeflow-report.md`

## Architecture Issues

### 1 Architecture Violations
Lower layers importing from higher layers

**Affected:** `utils → services`

### 7 High Complexity Files
Files with complexity score >30

**Affected:** `project.pdb (246)`, `project.pdb (246)`, `staticwebassets.build.json (130)`, `rbcswa.dswa.cache.json (65)`, `rpswa.dswa.cache.json (65)`

## File Details

| File | Folder | Layer | Lines | Functions |
|------|--------|-------|-------|----------|
| `ProductController.cs` | Areas/Admin/Controllers | services | 221 | 2 |
| `DashboardController.cs` | Areas/Admin/Controllers | services | 47 | 1 |
| `UserController.cs` | Areas/Admin/Controllers | services | 65 | 1 |
| `Index.cshtml` | Areas/Admin/Views/Dashboard | ui | 129 | 0 |
| `Create.cshtml` | Areas/Admin/Views/Product | ui | 150 | 0 |
| `Edit.cshtml` | Areas/Admin/Views/Product | ui | 173 | 0 |
| `Index.cshtml` | Areas/Admin/Views/Product | ui | 149 | 0 |
| `Index.cshtml` | Areas/Admin/Views/User | ui | 134 | 0 |
| `_ViewStart.cshtml` | Areas/Admin/Views | ui | 4 | 0 |
| `_ViewImports.cshtml` | Areas/Admin/Views | ui | 6 | 0 |
| `SaleController.cs` | Areas/POS/Controllers | services | 136 | 2 |
| `Index.cshtml` | Areas/POS/Views/Sale | ui | 270 | 0 |
| `_ViewStart.cshtml` | Areas/POS/Views | ui | 4 | 0 |
| `_ViewImports.cshtml` | Areas/POS/Views | ui | 6 | 0 |
| `CartController.cs` | Areas/User/Controllers | services | 143 | 2 |
| `HomeController.cs` | Areas/User/Controllers | services | 104 | 1 |
| `CheckoutController.cs` | Areas/User/Controllers | services | 206 | 1 |
| `ProfileController.cs` | Areas/User/Controllers | services | 123 | 1 |
| `ProductController.cs` | Areas/User/Controllers | services | 38 | 1 |
| `Index.cshtml` | Areas/User/Views/Cart | ui | 152 | 0 |
| `Index.cshtml` | Areas/User/Views/Checkout | ui | 207 | 0 |
| `Success.cshtml` | Areas/User/Views/Checkout | ui | 46 | 0 |
| `Index.cshtml` | Areas/User/Views/Home | ui | 263 | 0 |
| `Detail.cshtml` | Areas/User/Views/Product | ui | 263 | 0 |
| `Index.cshtml` | Areas/User/Views/Profile | ui | 191 | 0 |
| `_ViewImports.cshtml` | Areas/User/Views | ui | 6 | 0 |
| `_ViewStart.cshtml` | Areas/User/Views | ui | 4 | 0 |
| `Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.exe.config` | bin/Debug/net10.0/BuildHost-net472 | utils | 68 | 0 |
| `Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.runtimeconfig.json` | bin/Debug/net10.0/BuildHost-netcore | utils | 14 | 0 |
| `Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.deps.json` | bin/Debug/net10.0/BuildHost-netcore | utils | 171 | 0 |
| `appsettings.json` | bin/Debug/net10.0 | utils | 13 | 0 |
| `project.deps.json` | bin/Debug/net10.0 | utils | 2711 | 0 |
| `project.pdb` | bin/Debug/net10.0 | utils | 823 | 0 |
| `project.runtimeconfig.json` | bin/Debug/net10.0 | utils | 20 | 0 |
| `project.staticwebassets.endpoints.json` | bin/Debug/net10.0 | utils | 1 | 0 |
| `project.staticwebassets.runtime.json` | bin/Debug/net10.0 | utils | 1 | 0 |
| `AccountController.cs` | Controllers | utils | 194 | 5 |
| `HomeController.cs` | Controllers | utils | 21 | 2 |
| `ShoesShop.sql` | Data | utils | 265 | 0 |
| `SessionAuthorizeFilter.cs` | Filters | utils | 50 | 2 |
| `AdminDashboardViewModel.cs` | Models | utils | 17 | 0 |
| `CartItem.cs` | Models | utils | 20 | 0 |
| `Category.cs` | Models | utils | 16 | 0 |
| `Banner.cs` | Models | utils | 18 | 0 |
| `Cart.cs` | Models | utils | 18 | 0 |
| `Order.cs` | Models | utils | 46 | 0 |
| `LoginViewModel.cs` | Models | utils | 20 | 0 |
| `OrderDetail.cs` | Models | utils | 30 | 0 |
| `Product.cs` | Models | utils | 38 | 0 |
| `PasswordResetToken.cs` | Models | utils | 22 | 0 |
| `ProductHomeViewModel.cs` | Models | utils | 19 | 0 |
| `ProductImage.cs` | Models | utils | 20 | 0 |
| `RegisterViewModel.cs` | Models | utils | 30 | 0 |
| `Promotion.cs` | Models | utils | 30 | 0 |
| `ProductVariant.cs` | Models | utils | 28 | 0 |
| `ResetPasswordViewModel.cs` | Models | utils | 26 | 0 |
| `Role.cs` | Models | utils | 13 | 0 |
| `User.cs` | Models | utils | 34 | 0 |
| `ShoesShopContext.cs` | Models | utils | 58 | 4 |
| `ContainerId.cache` | obj/Container | utils | 0 | 0 |
| `ContainerDevelopmentMode.cache` | obj/Container | utils | 0 | 0 |
| `ContainerName.cache` | obj/Container | utils | 0 | 0 |
| `ContainerRunContext.cache` | obj/Container | utils | 0 | 0 |
| `project.styles.css` | obj/Debug/net10.0/scopedcss/bundle | utils | 50 | 0 |
| `project.bundle.scp.css` | obj/Debug/net10.0/scopedcss/projectbundle | utils | 50 | 0 |
| `_Layout.cshtml.rz.scp.css` | obj/Debug/net10.0/scopedcss/Views/Shared | ui | 49 | 0 |
| `.NETCoreApp,Version=v10.0.AssemblyAttributes.cs` | obj/Debug/net10.0 | utils | 5 | 0 |
| `ApiEndpoints.json` | obj/Debug/net10.0 | utils | 1 | 0 |
| `project.AssemblyInfo.cs` | obj/Debug/net10.0 | utils | 24 | 0 |
| `project.AssemblyInfoInputs.cache` | obj/Debug/net10.0 | utils | 2 | 0 |
| `project.csproj.AssemblyReference.cache` | obj/Debug/net10.0 | utils | 122 | 0 |
| `project.assets.cache` | obj/Debug/net10.0 | utils | 332 | 0 |
| `project.csproj.BuildWithSkipAnalyzers` | obj/Debug/net10.0 | utils | 0 | 0 |
| `project.csproj.CoreCompileInputs.cache` | obj/Debug/net10.0 | utils | 2 | 0 |
| `project.csproj.Up2Date` | obj/Debug/net10.0 | utils | 0 | 0 |
| `project.GeneratedMSBuildEditorConfig.editorconfig` | obj/Debug/net10.0 | utils | 160 | 0 |
| `project.csproj.FileListAbsolute.txt` | obj/Debug/net10.0 | utils | 343 | 0 |
| `project.genruntimeconfig.cache` | obj/Debug/net10.0 | utils | 2 | 0 |
| `project.GlobalUsings.g.cs` | obj/Debug/net10.0 | utils | 18 | 0 |
| `project.MvcApplicationPartsAssemblyInfo.cache` | obj/Debug/net10.0 | utils | 0 | 0 |
| `project.pdb` | obj/Debug/net10.0 | utils | 823 | 0 |
| `project.RazorAssemblyInfo.cache` | obj/Debug/net10.0 | utils | 2 | 0 |
| `project.RazorAssemblyInfo.cs` | obj/Debug/net10.0 | utils | 18 | 0 |
| `project.sourcelink.json` | obj/Debug/net10.0 | utils | 1 | 0 |
| `rbcswa.dswa.cache.json` | obj/Debug/net10.0 | utils | 1 | 0 |
| `rjimswa.dswa.cache.json` | obj/Debug/net10.0 | utils | 1 | 0 |
| `rjsmcshtml.dswa.cache.json` | obj/Debug/net10.0 | utils | 1 | 0 |
| `rjsmrazor.dswa.cache.json` | obj/Debug/net10.0 | utils | 1 | 0 |
| `staticwebassets.build.endpoints.json` | obj/Debug/net10.0 | utils | 1 | 0 |
| `rpswa.dswa.cache.json` | obj/Debug/net10.0 | utils | 1 | 0 |
| `staticwebassets.build.json` | obj/Debug/net10.0 | utils | 1 | 0 |
| `staticwebassets.build.json.cache` | obj/Debug/net10.0 | utils | 1 | 0 |
| `staticwebassets.references.upToDateCheck.txt` | obj/Debug/net10.0 | utils | 0 | 0 |
| `staticwebassets.removed.txt` | obj/Debug/net10.0 | utils | 0 | 0 |
| `staticwebassets.development.json` | obj/Debug/net10.0 | utils | 1 | 0 |
| `staticwebassets.upToDateCheck.txt` | obj/Debug/net10.0 | utils | 128 | 0 |
| `swae.build.ex.cache` | obj/Debug/net10.0 | utils | 0 | 0 |
| `project.assets.json` | obj | utils | 6791 | 0 |
| `project.csproj.nuget.dgspec.json` | obj | utils | 533 | 0 |
| `project.csproj.nuget.g.props` | obj | utils | 27 | 0 |

*...and 37 more files*
