using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.FilterModels.Base;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using UI.Helper;
using UI.Models;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.QuotaRequest
{

    public partial class AddQuotaRequest
    {
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }

        [Inject] public IQuotaRequestService QuotaRequestService { get; set; }
        [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        private QuotaRequestResponseDTO _quotaRequest;
        private EditContext _ec;
        private bool _saving;
        private MyModal _quotaRequestListModal;
        private List<QuotaRequestResponseDTO> _quotaRequests;
        private FilterDTO _filter;
        private List<BreadCrumbLink> _links;

        protected override void OnInitialized()
        {
            _quotaRequest = new QuotaRequestResponseDTO() { Year = DateTime.UtcNow.Year };
            _ec = new EditContext(_quotaRequest);
            base.OnInitialized();
            _links = new List<BreadCrumbLink>()
                {
                    new BreadCrumbLink()
                {
                    IsActive = true,
                    To = "/",
                    OrderIndex = 0,
                    Title = L["Homepage"]
                },new BreadCrumbLink()
                {
                    IsActive = true,
                    To = "/quota-request/active-requests",
                    OrderIndex = 1,
                    Title = L["_List",L["Quota Request"]]
                },
                new BreadCrumbLink()
                {
                    IsActive = false,
                    OrderIndex = 1,
                    Title = L["add_new",L["Quota Request"]]
                },
                };
        }
        private async Task Save()
        {
            if (!_ec.Validate()) return;

            _saving = true;
            StateHasChanged();

            _filter = new FilterDTO()
            {
                Filter = new()
                {
                    Filters = new()
                {
                            new Filter()
                            {
                                Field="Profession.Code",
                                Operator="eq",
                                Value= _quotaRequest.Type == PlacementExamType.EUS ? 3 : _quotaRequest.Type == PlacementExamType.DUS ? 2 : 1
                            },
                            new Filter()
                            {
                                Field="IsPrincipal",
                                Operator="eq",
                                Value=_quotaRequest.Type == PlacementExamType.YDUS ? false : true
                            },
                },
                    Logic = "and"
                },
                Sort = new[]{new Sort()
                    {
                        Field = "Name",
                        Dir = SortType.ASC
                    }},
                pageSize = 150
            };

            var expBrResponse = await ExpertiseBranchService.GetPaginateList(_filter);
            try
            {
                _quotaRequest.GlobalQuota ??= new List<GlobalQuotaExpertiseBranchModel>();
                foreach (var item in expBrResponse.Items)
                    _quotaRequest.GlobalQuota.Add(new() { ExpertiseBranchId = item.Id, ExpertiseBranchName = item.Name });
                var dto = Mapper.Map<QuotaRequestDTO>(_quotaRequest);

                var response = await QuotaRequestService.Add(dto);
                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Added"]}");
                    NavigationManager.NavigateTo($"/quota-request/active-requests");
                }
                else
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception e)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, e.Message);
                Console.WriteLine(e.Message);
                e.Message.PrintJson("message");
            }
            _saving = false;
            StateHasChanged();
        }
    }
}