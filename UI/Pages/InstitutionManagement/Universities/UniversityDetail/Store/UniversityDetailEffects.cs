using Fluxor;
using Shared.ResponseModels;
using Shared.ResponseModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Services;

namespace UI.Pages.InstitutionManagement.Universities.UniversityDetail.Store
{
    public class UniversityEffects
    {
        private readonly IUniversityService universityService;
        private readonly IAffiliationService affiliationService;
        private readonly IHospitalService hospitalService;
        public UniversityEffects(IUniversityService universityService, IAffiliationService affiliationService, IHospitalService hospitalService)
        {
            this.universityService = universityService;
            this.affiliationService = affiliationService;
            this.hospitalService = hospitalService;
        }

        [EffectMethod]
        public async Task LoadUniversities(UniversityDetailLoadAction action, IDispatcher dispatcher)
        {
            ResponseWrapper<UniversityResponseDTO> currentUniversity;
            try
            {

                currentUniversity = await universityService.GetById(action.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                currentUniversity = new();
            }
            dispatcher.Dispatch(new UniversityDetailSetAction(currentUniversity.Item));
        }
        [EffectMethod]
        public async Task LoadUniversityHospitals(HospitalsLoadAction action, IDispatcher dispatcher)
        {
            ResponseWrapper<List<HospitalResponseDTO>> hospitals;
            try
            {

                hospitals = await hospitalService.GetListByUniversityId(action.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                hospitals = new();
            }
            dispatcher.Dispatch(new HospitalsSetAction(hospitals.Item));
        }
        [EffectMethod]
        public async Task LoadAffiliations(AffiliationsLoadAction action, IDispatcher dispatcher)
        {
            ResponseWrapper<List<AffiliationResponseDTO>> affiliations;
            try
            {
                affiliations = await affiliationService.GetListByUniversityId(action.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                affiliations = new();
            }
            dispatcher.Dispatch(new AffiliationsSetAction(affiliations.Item));
        }
        [EffectMethod]
        public async Task OnDelete(UniversityDetailDeleteAction action, IDispatcher dispatcher)
        {
            try
            {
                await universityService.Delete(action.University.Id);
                dispatcher.Dispatch(new UniversityDetailDeleteSuccessAction());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //dispatcher.Dispatch(new UniversityDetailDeleteFailureAction());
            }
        }


    }
}
