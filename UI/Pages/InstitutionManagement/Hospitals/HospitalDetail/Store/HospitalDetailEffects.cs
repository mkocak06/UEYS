using System;
using System.Threading.Tasks;
using AutoMapper;
using Fluxor;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using UI.Helper;
using UI.Pages.Hospitals.HospitalDetail.Store;
using UI.Services;

namespace UI.Pages.HospitalDetail.Store
{
    public class HospitalDetailEffects
    {
        private readonly IMapper _mapper;
        private readonly IState<HospitalDetailState> _hospitalDetailState;
        private readonly IHospitalService _hospitalService;

        public HospitalDetailEffects(IMapper mapper, IState<HospitalDetailState> hospitalDetailState, IHospitalService hospitalService)
        {
            _mapper = mapper;
            _hospitalDetailState = hospitalDetailState;
            _hospitalService = hospitalService;
        }

        [EffectMethod]
        public async Task LoadHospitalDetails(HospitalDetailLoadAction action, IDispatcher dispatcher)
        {
            try
            {
                var response = await _hospitalService.GetById(action.id);
                if(response.Result)
                {
                    dispatcher.Dispatch(new HospitalDetailSetAction(response.Item));
                }
                else
                {
                    dispatcher.Dispatch(new HospitalDetailFailureAction(response));
                }
            }
            catch (Exception e)
            {
                dispatcher.Dispatch(new HospitalDetailFailureAction(new ResponseWrapper<Shared.ResponseModels.HospitalResponseDTO>() { Result=false, Message=e.Message}));
            }
        }

        [EffectMethod]
        public async Task UpdateHospitalDetail(HospitalDetailUpdateAction action, IDispatcher dispatcher)
        {
            try
            {
                var response = await _hospitalService.Update(action.id, action.hospitalDetail);
                if (response.Result)
                
                {
                    dispatcher.Dispatch(new HospitalDetailUpdateSuccessAction(response.Item));
                }
                else{
                    dispatcher.Dispatch(new HospitalDetailUpdateFailureAction(response));
                }
            }
            catch (Exception e)
            {
                dispatcher.Dispatch(new HospitalDetailUpdateFailureAction(new ResponseWrapper<HospitalResponseDTO>() { Result = false, Message = e.Message }));
            }
        }
    }
}