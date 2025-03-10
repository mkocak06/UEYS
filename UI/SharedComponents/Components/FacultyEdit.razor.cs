using Microsoft.AspNetCore.Components;
using Shared.RequestModels;
using Shared.ResponseModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UI.Services;
using UI.SharedComponents.Store;

namespace UI.SharedComponents.Components
{
    public partial class FacultyEdit
    {
        [Parameter]
        public FacultyResponseDTO Faculty
        {
            get => _faculty;
            set
            {
                if (_faculty == value) return;
                _faculty = value;
                FacultyChanged.InvokeAsync(Faculty);
            }
        }

        [Parameter] public EventCallback<FacultyResponseDTO> FacultyChanged { get; set; }
        [Parameter] public EventCallback<FacultyResponseDTO> OnRemove { get; set; }
        [Inject] public IProfessionService ProfessionService { get; set; }

        private FacultyResponseDTO _faculty;
        private List<ProfessionResponseDTO> _professions;
        private ProfessionResponseDTO _profession;

        protected override async Task OnInitializedAsync()
        {
            _profession = Faculty.Profession;

            await base.OnInitializedAsync();
        }

        private async  Task<IEnumerable<ProfessionResponseDTO>> SearchProfessions(string searchQuery)
        {
            var response = await ProfessionService.GetAll();
            _professions = response.Item;

            return _professions.Where(x => x.Name.ToLower().Contains(searchQuery?.ToLower()));
        }
        private void OnChangeProfession(ProfessionResponseDTO profession)
        {
            _profession = profession;
            Faculty.Profession = profession;
            Faculty.ProfessionId = profession.Id;
        }
    }
}
