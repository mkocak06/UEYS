using UI.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.Types;
using UI.Helper;

namespace UI.SharedComponents.Components;

public partial class ImageUpload
{
    //[Inject] IFileReaderService fileReader { get; set; }
    [Inject] private IFileUploadService FileuploadService { get; set; }
    [Inject] private IAuthenticationService AuthenticationService { get; set; }
    [Inject] private IJSRuntime JSRuntime { get; set; }
    [Inject] private ISweetAlert SweetAlert { get; set; }
    [Parameter] public string imagePath { get; set; } = null;
    [Parameter] public bool EditMode { get; set; }
    [Parameter] public EventCallback<string> OnImageChanged { get; set; }

    string _fileName = string.Empty;
    Stream _fileStream;

    private Guid _uploaderId;
    IBrowserFile _selectedFile;

    protected override void OnInitialized()
    {
        _uploaderId = Guid.NewGuid();
        base.OnInitialized();
    }

    private async void OnResetFile()
    {
        BlockUi();
        await OnImageChanged.InvokeAsync(null);
        UnblockUi();
    }
    private async void OnInputFileChange(InputFileChangeEventArgs e)
    {
        BlockUi();
        _selectedFile = e.GetMultipleFiles()[0];
        _fileName = _selectedFile.Name;
        string extension = Path.GetExtension(_fileName);

        if (_selectedFile.Size > 1024 * 1024 * 1)
        {
            SweetAlert.BasicAlert(L["Dosya boyutu 1 MB'dan fazla olmamalıdır."]);
            UnblockUi();
            return;
        }
        string[] allowedExtensions = { ".jpg", ".png", ".jpeg" };

        if (!allowedExtensions.Contains(extension))
        {
            SweetAlert.BasicAlert(L["Not allowed extension"] + ". " + L["Allowed file types: png, jpg, jpeg."]);
            UnblockUi();
        }
        else
        {
            await using (var s = _selectedFile.OpenReadStream(maxAllowedSize: 1024 * 1024 * 10))
            {
                var ms = new MemoryStream();
                await s.CopyToAsync(ms);
                _fileStream = new MemoryStream(ms.ToArray());
                var base64 = Convert.ToBase64String(ms.ToArray());
                imagePath = "data:image/png;base64," + base64;
            }
            if (imagePath != null)
            {
                await OnImageChanged.InvokeAsync(imagePath);
                UnblockUi();
            }

        }


    }
    public async void SavePhoto(long userId)
    {
        BlockUi();
        try
        {
            imagePath = await FileuploadService.PostImageByte(_fileStream, _fileName, userId);
            if (imagePath != null)
            {
                await OnImageChanged.InvokeAsync(imagePath);
                UnblockUi();
            }
            else { SweetAlert.ErrorAlert(L["An error occurred."]); }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            SweetAlert.ErrorAlert(L["An error occurred."]);
            UnblockUi();
        }
        finally
        { StateHasChanged(); }
    }

    private async void BlockUi()
    {
        await JSRuntime.InvokeAsync<dynamic>("blockElement", "#" + _uploaderId);
    }
    private async void UnblockUi()
    {
        await JSRuntime.InvokeAsync<dynamic>("unblockElement", "#" + _uploaderId);
    }
}