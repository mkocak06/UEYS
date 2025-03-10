using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using Shared.Types;
using UI.Helper;
using UI.Models;
using UI.Pages.InstitutionManagement.Curriculum;
using UI.Pages.Student.Students.StudentDetail.Store;
using UI.Services;
using UI.SharedComponents.Components;

namespace UI.Pages.Student.Students.StudentDetail
{
    partial class StudentTag
    {
        [Parameter] public long? Id { get; set; }
        [Inject] public IStudentService StudentService { get; set; }
        [Inject] public IDocumentService DocumentService { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }

        private StudentResponseDTO Student = new StudentResponseDTO();
        private List<DocumentResponseDTO> Documents = new();
        private MyModal FileModal;
        private MyModal CancelRegistrationModal;
        private List<BreadCrumbLink> _links;
        private string registrationStatementValidatorMessage;
        private string[] perfectionValues = { "", "Bilir", "Nasıl Yapıldığını Bilir", "Gösterir", "Yapar" };
        private bool saving = true;
        protected override void OnInitialized()
        {

            base.OnInitialized();
        }

        protected override async void OnParametersSet()
        {
            if (Id != null)
            {
                var response = await StudentService.GetRegistrationStudentById((long)Id);
                if (response.Result)
                    Student = response.Item;
                else
                    SweetAlert.IconAlert(SweetAlertIcon.error, L["Warning"], L["An Error Occured."]);
                saving = false;
                StateHasChanged();
            }
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
                    To = "/student/registrationStudents",
                    OrderIndex = 1,
                    Title = L["Students Sent for Registration"]
                },
                new BreadCrumbLink()
                {
                    IsActive = false,
                    OrderIndex = 2,
                    Title = Student.User.Name,
                }
            };
            StateHasChanged();
            base.OnParametersSet();
        }
        private async Task OnDownloadHandler(EducationTrackingResponseDTO educationTracking)
        {
            Documents = educationTracking.Documents.ToList();
            FileModal.OpenModal();
            StateHasChanged();
        }

        private async void FinishEducation()
        {
            saving = true;
            Student.Status = StudentStatus.EducationEnded;
            Student.IsDeleted = true;
            Student.DeleteDate = DateTime.UtcNow;
            try
            {
                var response = await StudentService.Update(Student.Id, Mapper.Map<StudentDTO>(Student));
                saving = false;
                if (response.Result)
                    NavigationManager.NavigateTo($"./student/registrationStudents");
                else
                    SweetAlert.ErrorAlert();
            }
            catch (Exception)
            {
                SweetAlert.ErrorAlert();
            }
        }

        private async Task DocumentView(long id, DocumentTypes docType)
        {
            Documents.Clear();
            try
            {
                var response = await DocumentService.GetListByTypeByEntity(id, docType);
                if (response.Result)
                {
                    Documents = response.Item;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            StateHasChanged();
        }


        private async void CancelRegistration()
        {
            registrationStatementValidatorMessage = null;
            if (string.IsNullOrEmpty(Student.RegistrationStatements))
            {
                registrationStatementValidatorMessage = "Tescili neden iptal ettiğinizi açılayınız";
                StateHasChanged();
                return;
            }

            saving = true;
            Student.Status = StudentStatus.Gratuated;
            try
            {
                var response = await StudentService.Update(Student.Id, Mapper.Map<StudentDTO>(Student));
                saving = false;
                CancelRegistrationModal.CloseModal();

                if (response.Result)
                    NavigationManager.NavigateTo($"./student/registrationStudents");
                else
                    SweetAlert.ErrorAlert();
            }
            catch (Exception)
            {
                SweetAlert.ErrorAlert();
            }
        }
    }
}
