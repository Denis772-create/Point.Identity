namespace Point.Services.Identity.Web.ViewModels.Device;

public class DeviceAuthorizationViewModel : ConsentViewModel
{
    public string UserCode { get; set; }
    public bool ConfirmUserCode { get; set; }
}