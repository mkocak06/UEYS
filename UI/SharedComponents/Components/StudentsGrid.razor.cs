using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.ResponseModels;
using UI.Services;
using System.Linq;
using Shared.FilterModels.Base;
using Shared.Types;
using Microsoft.JSInterop;
using UI.Models;
using UI.Pages.Student.Students.StudentDetail.Store;
using UI.SharedComponents.Components;
using AutoMapper;
using Shared.RequestModels;
using Shared.ResponseModels.Wrapper;
using UI.Helper;
using System.Globalization;

namespace UI.SharedComponents.Components
{
    public partial class StudentsGrid
    {
        [Parameter] public long ProgramId { get; set; }
        [Parameter] public long HospitalId { get; set; }
        [Parameter] public long UniversityId { get; set; }
        [Parameter] public bool? IsDependentProgram { get; set; }
        [Parameter] public bool? IsMainProgram { get; set; }
        [Parameter] public ProgramType? ProtocolOrComplement { get; set; }

    }
}