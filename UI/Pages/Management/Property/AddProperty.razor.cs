using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.Types;
using UI.Helper;
using UI.Models;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.Management.Property
{
    public partial class AddProperty
    {
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public IPropertyService PropertyService { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        private PropertyResponseDTO _property;
        private EditContext _editContext;
        private bool _saving;
        private SingleMapView _singleMapView;
        private InputText _inputText;
        private InputSelect<PropertyType> _inputType;
        private List<BreadCrumbLink> _links;

        protected override void OnInitialized()
        {
            _property = new PropertyResponseDTO();
            _editContext = new EditContext(_property);
            base.OnInitialized();
            _links = new List<BreadCrumbLink>();
            {
                new BreadCrumbLink()
                {
                    IsActive = true,
                    To = "/",
                    OrderIndex = 0,
                    Title = L["Homepage"]
                }; new BreadCrumbLink()
                {
                    IsActive = true,
                    To = "/management/properties",
                    OrderIndex = 1,
                    Title = L["_List", L["Property"]]
                };
                new BreadCrumbLink()
                {
                    IsActive = false,
                    OrderIndex = 1,
                    Title = L["add_new", L["Property"]]
                };
            }
        }

        private async Task Save ()
        {
            if (!_editContext.Validate()) return;
            
            _saving = true;
            StateHasChanged();
            _property.Name = _property.Name.ToUpper();
            var dto = Mapper.Map<PropertyDTO>(_property);
            try
            {
                var response = await PropertyService.Add(dto);

                if (response.Result)
                {
                    SweetAlert.ToastAlert(SweetAlertIcon.success, $"{L["Successfully Added"]}");
                    NavigationManager.NavigateTo($"./management/properties");
                }
                else
                {
                    throw new Exception(response.Message);
                }
            }
            catch (Exception ex)
            {
                SweetAlert.ToastAlert(SweetAlertIcon.error, ex.Message);
                Console.WriteLine(ex.Message);
            }
            _saving = false;
            StateHasChanged();
        }
        private void OnChangeName(string name)
        {
            _property.Name = name;
        }
    } 
}
