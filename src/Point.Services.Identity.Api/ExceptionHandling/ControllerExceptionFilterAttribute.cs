namespace Point.Services.Identity.Web.ExceptionHandling;

public class ControllerExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
    private readonly IModelMetadataProvider _modelMetadataProvider;

    public ControllerExceptionFilterAttribute(ITempDataDictionaryFactory tempDataDictionaryFactory, 
        IModelMetadataProvider modelMetadataProvider)
    {
        _tempDataDictionaryFactory = tempDataDictionaryFactory;
        _modelMetadataProvider = modelMetadataProvider;
    }

    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is not UserFriendlyErrorPageException &&
            context.Exception is not UserFriendlyViewException) return;

        if(CreateNotification(context, out var tempData)) return;
        HandleUserFriendlyViewException(context);
        ProcessException(context, tempData);

        //Clear toastr notification from temp
        ClearNotification(tempData);
    }

    private bool CreateNotification(ExceptionContext context, out ITempDataDictionary tempData)
    {
        tempData = _tempDataDictionaryFactory.GetTempData(context.HttpContext);
        CreateNotification(NotificationHelpers.AlertType.Error, tempData, context.Exception.Message);

        return !tempData.ContainsKey(NotificationHelpers.NotificationKey);
    }

    protected void CreateNotification(NotificationHelpers.AlertType alertType, ITempDataDictionary tempData,
        string message, string title = "")
    {
        var toast = new NotificationHelpers.Alert
        {
            Type = alertType,
            Message = message,
            Title = title
        };

        var alerts = new List<NotificationHelpers.Alert>();

        if (tempData.ContainsKey(NotificationHelpers.NotificationKey))
        {
            alerts = JsonConvert.DeserializeObject<List<NotificationHelpers.Alert>>(
                tempData[NotificationHelpers.NotificationKey].ToString());
            tempData.Remove(NotificationHelpers.NotificationKey);
        }

        alerts.Add(toast);

        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        var alertJson = JsonConvert.SerializeObject(alerts, settings);
        tempData.Add(NotificationHelpers.NotificationKey, alertJson);
    }

    private void HandleUserFriendlyViewException(ExceptionContext context)
    {
        if (context.Exception is not UserFriendlyViewException userFriendlyViewException) return;

        if (userFriendlyViewException.ErrorMessages != null && userFriendlyViewException.ErrorMessages.Any())
        {
            foreach (var message in userFriendlyViewException.ErrorMessages)
            {
                context.ModelState.AddModelError(message.ErrorKey, message.ErrorMessage);
            }
        }
        else
        {
            context.ModelState.AddModelError(userFriendlyViewException.ErrorKey, context.Exception.Message);
        }
    }

    private void ProcessException(ExceptionContext context, ITempDataDictionary tempData)
    {
        if (context.ActionDescriptor is not ControllerActionDescriptor controllerActionDescriptor) return;

        const string errorViewName = "Error";

        var result = new ViewResult
        {
            ViewName = context.Exception is UserFriendlyViewException
                ? controllerActionDescriptor.ActionName
                : errorViewName,
            TempData = tempData,
            ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState)
            {
                { "Notifications", tempData[NotificationHelpers.NotificationKey] },
            }
        };

        //For UserFriendlyException is necessary return model with latest form state
        if (context.Exception is UserFriendlyViewException exception)
        {
            result.ViewData.Model = exception.Model;
        }

        context.ExceptionHandled = true;
        context.Result = result;
    }

    private static void ClearNotification(ITempDataDictionary tempData)
    {
        tempData.Remove(NotificationHelpers.NotificationKey);
    }
}