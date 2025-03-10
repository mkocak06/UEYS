using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UI.Services;
using UI.Models;

namespace UI.Pages.Archive.Documents
{
    public partial class ArchiveDocuments
    {
        [Inject] private IDocumentService DocumentService { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        private List<DocumentResponseDTO> _documents;
        private bool _loading = false;
        [Inject] public IDispatcher Dispatcher { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await GetDocuments();

            await base.OnInitializedAsync();
        }

        private async Task GetDocuments()
        {
            var response = await DocumentService.GetDeletedList();
            if (response.Item != null) 
            {
                _documents = response.Item;
                StateHasChanged();
            }
            else
            {
                _loading = true;
                SweetAlert.ErrorAlert();
            }
        }

        //private async Task OnUndeleteHandler(DocumentResponseDTO document)
        //{
        //    var confirm = await SweetAlert.ConfirmAlert($"{L["Are you sure?"]}",
        //        $"{L["Are you sure you want to take back this item?."]}",
        //        SweetAlertIcon.question, true, $"{L["Take Back"]}", $"{L["Cancel"]}");

        //    if (confirm)
        //    {
        //        try
        //        {
        //            document.IsDeleted= false;
        //            var dto = Mapper.Map<DocumentDTO>(document);

        //            var response = await DocumentService.Update((long)document.Id, dto);

        //            if (response.Result)
        //            {
        //                NavigationManager.NavigateTo($"./archive/documents");
        //                _documents.Remove(document);
        //            }
        //            else
        //            {
        //                throw new Exception(response.Message);
        //            }
        //            SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Item Took Back!"]}");
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e);
        //            SweetAlert.ErrorAlert();
        //            throw;
        //        }
        //    }
        //}
    }
}
