

using AutoMapper;
using Fluxor;
using Shared.ResponseModels;
using Shared.ResponseModels.Program;
using Shared.ResponseModels.Wrapper;
using System;
using System.Threading.Tasks;
using UI.Services;

namespace UI.Pages.InstitutionManagement.Programs.ProgramDetail.Store
{
    public class ProgramDetailEffects
    {
        private readonly IMapper _mapper;
        private readonly IState<ProgramDetailState> _programDetailState;
        private readonly IProgramService _programService;

        public ProgramDetailEffects(IMapper mapper, IState<ProgramDetailState> programDetailState, IProgramService programService)
        {
            _mapper = mapper;
            _programDetailState = programDetailState;
            _programService = programService;
        }

        [EffectMethod]
        public async Task LoadProgramDetails(ProgramDetailLoadAction action, IDispatcher dispatcher)
        {
            try
            {
                var response = await _programService.GetWithBreadCrumb(action.id);
                if (response.Result)
                {
                    if (response.Item.Program.Faculty == null)
                    {
                        response.Item.Program.Faculty = new FacultyResponseDTO()
                        {
                            University = new UniversityResponseDTO()
                        };
                    }
                    dispatcher.Dispatch(new ProgramDetailSetAction(response.Item));
                }
                else
                {
                    dispatcher.Dispatch(new ProgramDetailFailureAction(response));
                }
            }
            catch (Exception e)
            {
                dispatcher.Dispatch(new ProgramDetailFailureAction(new ResponseWrapper<ProgramBreadcrumbResponseDTO>() { Result = false, Message = e.Message }));
            }
        }

        [EffectMethod]
        public async Task UpdateProgramDetail(ProgramDetailUpdateAction action, IDispatcher dispatcher)
        {
            try
            {
                var response = await _programService.Update(action.id, action.ProgramDetail);
                if (response.Result)

                {
                    dispatcher.Dispatch(new ProgramDetailUpdateSuccessAction(response.Item));
                }
                else
                {
                    dispatcher.Dispatch(new ProgramDetailUpdateFailureAction(response));
                }
            }
            catch (Exception e)
            {
                dispatcher.Dispatch(new ProgramDetailUpdateFailureAction(new ResponseWrapper<ProgramResponseDTO>() { Result = false, Message = e.Message }));
            }
        }

    }
}

