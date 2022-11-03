using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Point.Services.Identity.Api.Configuration;

public class GenericControllerRouteConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        if (!controller.ControllerType.IsGenericType) return;

        var name = controller.ControllerType.Name;
        var nameWithoutArity = name[..name.IndexOf('`')];
        controller.ControllerName = nameWithoutArity[..nameWithoutArity.LastIndexOf("Controller")];
    }
}