using Aiursoft.Scanner.Abstractions;
using Aiursoft.Tracer.Configuration;
using Aiursoft.Tracer.Controllers;
using Aiursoft.Tracer.Entities;
using Aiursoft.Tracer.Services.Authentication;
using Aiursoft.Tracer.Services.FileStorage;
using Aiursoft.UiStack.Layout;
using Aiursoft.UiStack.Navigation;
using Aiursoft.UiStack.Views.Shared.Components.FooterMenu;
using Aiursoft.UiStack.Views.Shared.Components.LanguagesDropdown;
using Aiursoft.UiStack.Views.Shared.Components.MegaMenu;
using Aiursoft.UiStack.Views.Shared.Components.Navbar;
using Aiursoft.UiStack.Views.Shared.Components.SideAdvertisement;
using Aiursoft.UiStack.Views.Shared.Components.Sidebar;
using Aiursoft.UiStack.Views.Shared.Components.SideLogo;
using Aiursoft.UiStack.Views.Shared.Components.SideMenu;
using Aiursoft.UiStack.Views.Shared.Components.UserDropdown;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Aiursoft.Tracer.Services;

public class ViewModelArgsInjector(
    IStringLocalizer<ViewModelArgsInjector> localizer,
    StorageService storageService,
    NavigationState<Startup> navigationState,
    IAuthorizationService authorizationService,
    IOptions<AppSettings> appSettings,
    SignInManager<User> signInManager) : IScopedDependency
{

    // ReSharper disable once UnusedMember.Local
    private void _useless_for_localizer()
    {
        // Titles, navbar strings.
        _ = localizer["Features"];
        _ = localizer["Index"];
        _ = localizer["Directory"];
        _ = localizer["Users"];
        _ = localizer["Roles"];
        _ = localizer["Administration"];
        _ = localizer["System"];
        _ = localizer["Info"];
        _ = localizer["Manage"];
        _ = localizer["Login"];
        _ = localizer["System Info"];
        _ = localizer["Create User"];
        _ = localizer["User Details"];
        _ = localizer["Edit User"];
        _ = localizer["Delete User"];
        _ = localizer["Create Role"];
        _ = localizer["Role Details"];
        _ = localizer["Edit Role"];
        _ = localizer["Delete Role"];
        _ = localizer["Change Profile"];
        _ = localizer["Change Avatar"];
        _ = localizer["Change Password"];
        _ = localizer["Home"];
        _ = localizer["Settings"];
        _ = localizer["Profile Settings"];
        _ = localizer["Personal"];
        _ = localizer["Unauthorized"];
        _ = localizer["Error"];
        _ = localizer["Network Quality Tester"];
        _ = localizer["Tester"];
        _ = localizer["Self host your own server"];
    
        _ = localizer["Background Jobs"];
        _ = localizer["Global Settings"];
        _ = localizer["Permissions"];
    }

    public void InjectSimple(
        HttpContext context,
        UiStackLayoutViewModel toInject)
    {
        toInject.PageTitle = localizer[toInject.PageTitle ?? "View"];
        toInject.AppName = localizer["Network Quality Tester"];
        toInject.Theme = UiTheme.Light;
        toInject.SidebarTheme = UiSidebarTheme.Default;
        toInject.Layout = UiLayout.Fluid;
        toInject.ContentNoPadding = true;
    }

    public void Inject(
        HttpContext context,
        UiStackLayoutViewModel toInject)
    {
        var preferDarkTheme = context.Request.Cookies[ThemeController.ThemeCookieKey] == true.ToString();
        toInject.PageTitle = localizer[toInject.PageTitle ?? "View"];
        toInject.AppName = localizer["Network Quality Tester"];
        toInject.Theme = preferDarkTheme ? UiTheme.Dark : UiTheme.Light;
        toInject.SidebarTheme = preferDarkTheme ? UiSidebarTheme.Dark : UiSidebarTheme.Default;
        toInject.Layout = UiLayout.Fluid;
        toInject.FooterMenu = new FooterMenuViewModel
        {
            AppBrand = new Link { Text = localizer["Network Quality Tester"], Href = "https://gitlab.aiursoft.com/aiursoft/tracer" },
            Links =
            [
                new Link { Text = localizer["Home"], Href = "/" },
                new Link { Text = "Aiursoft", Href = "https://www.aiursoft.com" },
            ]
        };
        toInject.Navbar = new NavbarViewModel
        {
            ThemeSwitchApiCallEndpoint = "/api/switch-theme"
        };

        var currentViewingController = context.GetRouteValue("controller")?.ToString();
        var navGroupsForView = new List<NavGroup>();

        foreach (var groupDef in navigationState.NavMap)
        {
            var itemsForView = new List<CascadedSideBarItem>();
            foreach (var itemDef in groupDef.Items)
            {
                var linksForView = new List<CascadedLink>();
                foreach (var linkDef in itemDef.Links)
                {
                    bool isVisible;
                    if (string.IsNullOrEmpty(linkDef.RequiredPolicy))
                    {
                        isVisible = true;
                    }
                    else
                    {
                        var authResult = authorizationService.AuthorizeAsync(context.User, linkDef.RequiredPolicy).Result;
                        isVisible = authResult.Succeeded;
                    }

                    if (isVisible)
                    {
                        linksForView.Add(new CascadedLink
                        {
                            Href = linkDef.Href,
                            Text = localizer[linkDef.Text]
                        });
                    }
                }

                if (linksForView.Any())
                {
                    itemsForView.Add(new CascadedSideBarItem
                    {
                        UniqueId = itemDef.UniqueId,
                        Text = localizer[itemDef.Text],
                        LucideIcon = itemDef.Icon,
                        IsActive = linksForView.Any(l =>
                        {
                            // Extract controller name from href (e.g., "/Manage/Index" -> "Manage")
                            var hrefController = l.Href.TrimStart('/').Split('/').FirstOrDefault();
                            // Exact match to avoid false positives like "Manage" matching "ManagePayroll"
                            return string.Equals(hrefController, currentViewingController, StringComparison.OrdinalIgnoreCase);
                        }),                        Links = linksForView
                    });
                }
            }

            if (itemsForView.Any())
            {
                navGroupsForView.Add(new NavGroup
                {
                    Name = localizer[groupDef.Name],
                    Items = itemsForView.Select(t => (SideBarItem)t).ToList()
                });
            }
        }

        toInject.Sidebar = new SidebarViewModel
        {
            SideLogo = new SideLogoViewModel
            {
                AppName = localizer["Network Quality Tester"],
                LogoUrl = "/logo.svg",
                Href = "/"
            },
            SideMenu = new SideMenuViewModel
            {
                Groups = navGroupsForView
            }
        };

        var currentCulture = context.Features
            .Get<IRequestCultureFeature>()?
            .RequestCulture.Culture.Name; // zh-CN

        // ReSharper disable once RedundantNameQualifier
        var suppportedCultures = Aiursoft.WebTools.OfficialPlugins.LocalizationPlugin.SupportedCultures
            .Select(c => new LanguageSelection
            {
                Link = $"/Culture/Set?culture={c.Key}&returnUrl={context.Request.Path}",
                Name = c.Value // 中文 - 中国
            })
            .ToArray();

        // ReSharper disable once RedundantNameQualifier
        toInject.Navbar.LanguagesDropdown = new LanguagesDropdownViewModel
        {
            Languages = suppportedCultures,
            SelectedLanguage = new LanguageSelection
            {
                Name = Aiursoft.WebTools.OfficialPlugins.LocalizationPlugin.SupportedCultures[currentCulture ?? "en-US"],
                Link = "#",
            }
        };

        if (signInManager.IsSignedIn(context.User))
        {
            var avatarPath = context.User.Claims.First(c => c.Type == UserClaimsPrincipalFactory.AvatarClaimType)
                .Value;
            toInject.Navbar.UserDropdown = new UserDropdownViewModel
            {
                UserName = context.User.Claims.First(c => c.Type == UserClaimsPrincipalFactory.DisplayNameClaimType).Value,
                UserAvatarUrl = $"{storageService.RelativePathToInternetUrl(avatarPath)}?w=100&square=true",
                IconLinkGroups =
                [
                    new IconLinkGroup
                    {
                        Links =
                        [
                            new IconLink { Icon = "user", Text = localizer["Profile"], Href = "/Manage" },
                        ]
                    },
                    new IconLinkGroup
                    {
                        Links =
                        [
                            new IconLink { Icon = "log-out", Text = localizer["Sign out"], Href = "/Account/Logoff" }
                        ]
                    }
                ]
            };
        }
        else
        {
            toInject.Sidebar.SideAdvertisement = new SideAdvertisementViewModel
            {
                Title = localizer["Login"],
                Description = localizer["Login to get access to all features."],
                Href = "/Account/Login",
                ButtonText = localizer["Login"]
            };

            var allowRegister = appSettings.Value.Local.AllowRegister;
            var links = new List<IconLink>
            {
                new()
                {
                    Text = localizer["Login"],
                    Href = "/Account/Login",
                    Icon = "user"
                }
            };
            if (allowRegister && appSettings.Value.LocalEnabled)
            {
                links.Add(new IconLink
                {
                    Text = localizer["Register"],
                    Href = "/Account/Register",
                    Icon = "user-plus"
                });
            }
            toInject.Navbar.UserDropdown = new UserDropdownViewModel
            {
                UserName = localizer["Click to login"],
                UserAvatarUrl = string.Empty,
                IconLinkGroups =
                [
                    new IconLinkGroup
                    {
                        Links = links.ToArray()
                    }
                ]
            };
        }
    }
}
