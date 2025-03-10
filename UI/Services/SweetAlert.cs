using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace UI.Services;

public interface ISweetAlert
{
    void BasicAlert(string message);
    void IconAlert(SweetAlertIcon icon, string title, string message);
    Task<bool> ConfirmAlert(string title, string message, SweetAlertIcon icon, bool showCancelButton, string confirmButtonText, string cancelButtonText);
    void ToastAlert(SweetAlertIcon icon, string message);
    Task<string> InputAlert(string title, bool showCancelButton, string confirmButtonText, string cancelButtonText);
    void ErrorAlert();
    void ErrorAlert(string message);
}
public class SweetAlert : ISweetAlert
{
    private IJSRuntime JS { get; set; }

    public SweetAlert(IJSRuntime JS)
    {
        this.JS = JS;
    }

    [JSInvokable]
    public async void BasicAlert(string message)
    {
        await JS.InvokeAsync<dynamic>("SweetAlert.basicAlert", message);
    }

    [JSInvokable]
    public async void IconAlert(SweetAlertIcon icon, string title, string message)
    {
        await JS.InvokeAsync<dynamic>("SweetAlert.iconAlert", icon.ToString(), title, message);
    }

    [JSInvokable]
    public async Task<bool> ConfirmAlert(string title, string message, SweetAlertIcon icon, bool showCancelButton, string confirmButtonText, string cancelButtonText)
    {
        return await JS.InvokeAsync<bool>("SweetAlert.confirmAlert", title, message, icon.ToString(), showCancelButton, confirmButtonText, cancelButtonText);
    }

    [JSInvokable]
    public async void ToastAlert(SweetAlertIcon icon, string message)
    {
        await JS.InvokeAsync<dynamic>("SweetAlert.toastAlert", icon.ToString(), message);
    }

    public async Task<string> InputAlert(string title, bool showCancelButton, string confirmButtonText, string cancelButtonText)
    {
        return await JS.InvokeAsync<string>("SweetAlert.inputAlert", title, showCancelButton, confirmButtonText, cancelButtonText);
    }

    [JSInvokable]
    public async void ErrorAlert()
    {
        await JS.InvokeAsync<dynamic>("SweetAlert.iconAlert", SweetAlertIcon.error.ToString(), "Bir Hata Oluştu!", "Lütfen sistem yöneticisi ile iletişime geçiniz");
    }

    [JSInvokable]
    public async void ErrorAlert(string message)
    {
        await JS.InvokeAsync<dynamic>("SweetAlert.iconAlert", SweetAlertIcon.error.ToString(), message);
    }
}

public enum SweetAlertIcon
{
    success,
    error,
    warning,
    info,
    question,
}