using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.RequestModels;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using UI.Pages.Student.Students.StudentDetail.Store;
using UI.Services;

namespace UI.Pages.User.Educator.Tabs
{
    public partial class ProgramInformation
    {
        [Parameter] public bool IsEditing { get; set; }
        [Inject] public IState<EducatorDetailState> EducatorState { get; set; }
        [Inject] public IEducatorService EducatorService { get; set; }
        [Inject] public IUserService UserService { get; set; }
        [Inject] public ICurriculumService CurriculumService { get; set; }
        [Inject] public IProgramService ProgramService { get; set; }
        [Inject] public IExpertiseBranchService ExpertiseBranchService { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public ISweetAlert SweetAlert { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        private EducatorResponseDTO Educator => EducatorState.Value.Educator;
        private EducatorResponseDTO EducatorAdd => EducatorState.Value.EducatorAdd;
        private EditContext _ecUpdate, _ecAdd;
        private bool _loading;
        protected override void OnInitialized()
        {
            _ecUpdate = new EditContext(new EducatorResponseDTO());
            _ecAdd = new EditContext(EducatorAdd);
            if (IsEditing)
            {
                SubscribeToAction<EducatorDetailSetAction>(action =>
                {
                    _ecUpdate = new EditContext(action.Educator);
                });
            }
            base.OnInitialized();
        }
    }
}