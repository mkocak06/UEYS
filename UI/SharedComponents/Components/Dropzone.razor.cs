using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using System;
using System.IO;
using System.Threading.Tasks;
using UI.Helper;
using UI.Services;

namespace UI.SharedComponents.Components
{
    public partial class Dropzone
    {
        [Inject] IHttpService HttpService { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] ISweetAlert SweetAlert { get; set; }
        [Parameter] public EventCallback<bool> OnUpdateHandler { get; set; }
        [Parameter] public DocumentTypes DocumentType { get; set; }
        [Parameter] public long EntityId { get; set; }
        [Parameter] public string Class { get; set; } = "drop-zone";
        public UploadResult UploadResult { get; set; }
        public bool overTheLimit;

        public DocumentResponseDTO DocumentResponseDTO { get; set; }
        ElementReference dropZoneElement;
        InputFile inputFile;
        public IBrowserFile uploadFile;
        IJSObjectReference _module;
        IJSObjectReference _dropZoneInstance;
        public string _selectedFileName;
        public MemoryStream ms;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // Load the JS file
                //_module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./src/js/dropzone.js");

                // Initialize the drop zone
                _dropZoneInstance = await JSRuntime.InvokeAsync<IJSObjectReference>("initializeFileDropZone", dropZoneElement, inputFile.Element);
            }
        }

        public async void ResetStatus()
        {
            uploadFile = null;
            _selectedFileName = null;
            inputFile = null;
            ms = new MemoryStream();
            StateHasChanged();
            await OnUpdateHandler.InvokeAsync(false);
        }

        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            try
            {
                overTheLimit = false;
                uploadFile = e.File;
                StateHasChanged();
                // Check if the file is null then return from the method
                if (uploadFile == null)
                    return;
                if (uploadFile.Size > 3145728)
                {
                    overTheLimit = true;
                    StateHasChanged();
                }
                _selectedFileName = uploadFile.Name;
                await using var s = uploadFile.OpenReadStream(10485760);
                ms = new MemoryStream();
                await s.CopyToAsync(ms);
                StateHasChanged();

                await OnUpdateHandler.InvokeAsync(true);
            }
            catch (Exception exe)
            {
                throw;
            }

        }

        public async Task<ResponseWrapper<DocumentResponseDTO>> SubmitFileAsync(long? entityId = null)
        {
            UploadResult = UploadResult.Pending;
            StateHasChanged();
            if (!string.IsNullOrWhiteSpace(_selectedFileName))//TODO null check
            {
                try
                {
                    if (overTheLimit)
                    {
                        UploadResult = UploadResult.Error;
                        ResetStatus();
                        StateHasChanged();
                        SweetAlert.IconAlert(SweetAlertIcon.error, L["Error"], L["File size cannot exceed 3 MB!"]);
                        return new() { Result = false };
                    }
                    var response = await HttpService.PostFile(new MemoryStream(ms.ToArray()), _selectedFileName, DocumentType, entityId == null ? EntityId : entityId ?? 0);
                    if (response.Result)
                    {
                        UploadResult = UploadResult.Success;
                        SweetAlert.ToastAlert(SweetAlertIcon.success, L["Document Successfully Added"]);
                        StateHasChanged();
                    }
                    else
                    {
                        UploadResult = UploadResult.Error;
                        StateHasChanged();
                        ResetStatus();
                        SweetAlert.IconAlert(SweetAlertIcon.error, L["Error"], response.Message);
                    }
                    return response;
                }
                catch (Exception e)
                {
                    SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], e.Message);
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
            else
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "Hata", "Lütfen bir dosya seçiniz");
                return null;
            }
        }

        public async Task<ResponseWrapper<DocumentResponseDTO>> AddFile()
        {
            UploadResult = UploadResult.Pending;
            StateHasChanged();
            if (!string.IsNullOrWhiteSpace(_selectedFileName))//TODO null check
            {
                try
                {
                    if (overTheLimit)
                    {
                        UploadResult = UploadResult.Error;
                        ResetStatus();
                        StateHasChanged();
                        return new() { Result = false ,Message = "File size cannot exceed 3 MB!" };
                    }
                    var response = await HttpService.PostFile(new MemoryStream(ms.ToArray()), _selectedFileName, DocumentType, EntityId);
                    if (response.Result)
                    {
                        UploadResult = UploadResult.Success;
                        StateHasChanged();
                        return new() { Result = true };
                    }
                    else
                    {
                        UploadResult = UploadResult.Error;
                        StateHasChanged();
                        ResetStatus();
                        return new() { Result = false, Message = response.Message };
                    }
                }
                catch (Exception e)
                {
                    SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], e.Message);
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
            else
            {
                SweetAlert.IconAlert(SweetAlertIcon.error, "Hata", "Lütfen bir dosya seçiniz");
                return null;
            }
        }

        // Unregister the drop zone events
        public async ValueTask DisposeAsync()
        {
            if (_dropZoneInstance != null)
            {
                await _dropZoneInstance.InvokeVoidAsync("dispose");
                await _dropZoneInstance.DisposeAsync();
            }

            if (_module != null)
            {
                await _module.DisposeAsync();
            }
        }
    }

    public enum UploadResult
    {
        Success,
        Error,
        Pending,
    }
}
