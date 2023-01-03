using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;

namespace Point.Services.Identity.Web.Helpers.Localization;

public class ResourceViewLocalizer : IViewLocalizer, IViewContextAware
{
    private class MockEnvironment : IWebHostEnvironment
    {
        // This fake environment implementation helps us leveraging .NET's default ViewLocalizer implementation 
        // without having to rewrite it completely. The only used property is ApplicationName. The other properties 
        // should be unused.

        public string ApplicationName { get; set; }

        #region Unused
        public IFileProvider WebRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string WebRootPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string EnvironmentName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ContentRootPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IFileProvider ContentRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion
    }

    private readonly ViewLocalizer _internalViewLocalizer;
    private readonly ViewLocalizer _defaultViewLocalizer;
    private readonly string _assemblyName;

    public ResourceViewLocalizer(IHtmlLocalizerFactory localizerFactory, IWebHostEnvironment hostingEnvironment)
    {
        if (localizerFactory == null)
        {
            throw new ArgumentNullException(nameof(localizerFactory));
        }

        if (hostingEnvironment == null)
        {
            throw new ArgumentNullException(nameof(hostingEnvironment));
        }

        _assemblyName = Assembly.GetExecutingAssembly().GetName().Name!;
        _defaultViewLocalizer = new ViewLocalizer(localizerFactory, hostingEnvironment);
        _internalViewLocalizer = new ViewLocalizer(localizerFactory, new MockEnvironment { ApplicationName = _assemblyName });
    }

    public LocalizedString GetString(string name)
    {
        // Resolves the resources into the default localizer first (to allow resource overriding by library 
        // consumers).
        LocalizedString str = _defaultViewLocalizer.GetString(name);
        return str.ResourceNotFound ? _internalViewLocalizer.GetString(name) : str;
    }

    public LocalizedString GetString(string name, params object[] arguments)
    {
        LocalizedString str = _defaultViewLocalizer.GetString(name, arguments);
        return str.ResourceNotFound ? _internalViewLocalizer.GetString(name, arguments) : str;
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return _defaultViewLocalizer.GetAllStrings(includeParentCultures)
            .Union(_internalViewLocalizer.GetAllStrings(includeParentCultures));
    }

    public LocalizedHtmlString this[string name]
    {
        get
        {
            LocalizedHtmlString str = _defaultViewLocalizer[name];
            return str.IsResourceNotFound ? _internalViewLocalizer[name] : str;
        }
    }

    public LocalizedHtmlString this[string name, params object[] arguments]
    {
        get
        {
            LocalizedHtmlString str = _defaultViewLocalizer[name, arguments];
            return str.IsResourceNotFound ? _internalViewLocalizer[name, arguments] : str;
        }
    }

    public void Contextualize(ViewContext viewContext)
    {
        _internalViewLocalizer.Contextualize(viewContext);
        _defaultViewLocalizer.Contextualize(viewContext);
    }
}