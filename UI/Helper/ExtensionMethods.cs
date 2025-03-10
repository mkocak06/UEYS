using Microsoft.AspNetCore.Components;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Web;

namespace UI.Helper;

public static class ExtensionMethods
{
    public static NameValueCollection QueryString(this NavigationManager navigationManager)
    {
        return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
    }

    public static string QueryString(this NavigationManager navigationManager, string key)
    {
        return navigationManager.QueryString()[key];
    }
    public static string Description(this Enum value)
    {
        // get attributes
        var field = value.GetType().GetField(value.ToString());
        var attributes = field.GetCustomAttributes(false);

        // Description is in a hidden Attribute class called DisplayAttribute
        // Not to be confused with DisplayNameAttribute
        dynamic displayAttribute = null;

        if (attributes.Any())
        {
            displayAttribute = attributes.ElementAt(0);
        }

        // return description
        return displayAttribute?.Description ?? value.ToString();
    }

    public static string Avatar(this string value)
    {
        if (value == null)
            return "";
        else if (value.Length < 2)
            return value.Trim().ToUpper();

        var words = value.Trim().Split(" ");
        if (words.Length > 1)
        {
            string FirstLetter, SecondLetter;
            FirstLetter = words[0].First().ToString();
            SecondLetter = words[1].First().ToString();
            return (FirstLetter + SecondLetter).ToUpper();
        }
        else
        {
            return value[..2].ToUpper();
        }
    }
    /// <summary>
    /// If item exists removes otherwise adds
    /// </summary>
    /// <param name="list"></param>
    /// <param name="item"></param>
    /// <returns>updated list</returns>
    public static List<T> AddOrRemove<T>(this List<T> list, T item)
    {
        if (list.Contains(item))
            list.Remove(item);
        else
            list.Add(item);
        return list;
    }
    public static string TitleCase(this string s)
    {
        return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());
    }

    public static bool TcValidator(this string tcNo)
    {
        var returnValue = false;
        if (tcNo is not { Length: 11 }) return false;
        long ATCNO, BTCNO, TcNo;
        long C1, C2, C3, C4, C5, C6, C7, C8, C9, Q1, Q2;

        TcNo = long.Parse(tcNo);

        ATCNO = TcNo / 100;
        BTCNO = TcNo / 100;

        C1 = ATCNO % 10; ATCNO = ATCNO / 10;
        C2 = ATCNO % 10; ATCNO = ATCNO / 10;
        C3 = ATCNO % 10; ATCNO = ATCNO / 10;
        C4 = ATCNO % 10; ATCNO = ATCNO / 10;
        C5 = ATCNO % 10; ATCNO = ATCNO / 10;
        C6 = ATCNO % 10; ATCNO = ATCNO / 10;
        C7 = ATCNO % 10; ATCNO = ATCNO / 10;
        C8 = ATCNO % 10; ATCNO = ATCNO / 10;
        C9 = ATCNO % 10; ATCNO = ATCNO / 10;
        Q1 = ((10 - ((((C1 + C3 + C5 + C7 + C9) * 3) + (C2 + C4 + C6 + C8)) % 10)) % 10);
        Q2 = ((10 - (((((C2 + C4 + C6 + C8) + Q1) * 3) + (C1 + C3 + C5 + C7 + C9)) % 10)) % 10);

        returnValue = ((BTCNO * 100) + (Q1 * 10) + Q2 == TcNo);
        return returnValue;
    }

    public static void PrintJson(this Object o, string info)
    {
        Console.WriteLine(info + " " + System.Text.Json.JsonSerializer.Serialize(o));
    }

    public static async Task<IEnumerable<string>> GetDescriptionOptions(string searchQuery)
    {
        var list = new List<string>()
        {
            "Eğitici Eksikliği",
            "Eğitici Yokluğu",
            "Mekan Eksikliği",
            "Donanım Eksikliği",
            "Portföy Eksikliği",
            "Kurum Talebi",
            "Eğitim Eksikliği",
            "Program Eksikliği"
        };
        return list.Where(x => x.Contains(searchQuery));
    }
    public static string GetProcessColorForTable(this ProcessType processType)
    {
        switch (processType)
        {
            case ProcessType.Start:
                return "table-success";
            case ProcessType.TimeIncreasing:
                return "table-warning";
            case ProcessType.TimeDecreasing:
                return "bg-secondary";
            case ProcessType.Finish:
                return "table-danger";
            case ProcessType.EstimatedFinish:
                return "table-danger";
            case ProcessType.Graduate:
                return "table-info";
            default:
                return "table-light";
        }
    }

    public static string GetProcessColorForTimeline(this ProcessType processType)
    {
        switch (processType)
        {
            case ProcessType.Start:
                return "#bfefed";
            case ProcessType.TimeIncreasing:
                return "#ffe7b8";
            case ProcessType.TimeDecreasing:
                return "#c6c8ca";
            case ProcessType.Finish:
                return "#fccdd2";
            case ProcessType.EstimatedFinish:
                return "#fccdd2";
            case ProcessType.Graduate:
                return "#c7e2ff";
            default:
                return "#fcfcfd";
        }
    }
    public static List<ThesisSubjectType_2> GetThesisSubjects(this ThesisSubjectType_1 thesisSubjectType_1)
    {
        switch (thesisSubjectType_1)
        {
            case ThesisSubjectType_1.Clinical:
                return new List<ThesisSubjectType_2>()
                {
                    ThesisSubjectType_2.Prospektif,
                    ThesisSubjectType_2.Retrospektif,
                    ThesisSubjectType_2.Sectional
                };
            case ThesisSubjectType_1.Laboratory:
                return new List<ThesisSubjectType_2>()
                {
                    ThesisSubjectType_2.Invitro,
                    ThesisSubjectType_2.Animal,
                    ThesisSubjectType_2.Insilico,
                };
            default:
                return new();
        }
    }
    public static List<ReasonType> GetReasonTypes(this ProcessType processType, StudentStatus? studentStatus)
    {
        switch (processType)
        {
            case ProcessType.Start:
                return new List<ReasonType>()
                {
                    ReasonType.BeginningSpecializationEducation,
                    ReasonType.RestartByJudgment,
                    ReasonType.RestartingWithAmnestyLaw,
                    //ReasonType.BranchChange,
                    ReasonType.StartingOverwithExamfortheSameExpertiseBranch,
                };
            case ProcessType.Information:
                return new List<ReasonType>()
                {
                    ReasonType.AnnualLeave,
                    ReasonType.CancellationofNegativeOpinionbyCourtDecision,
                    ReasonType.RayLeave
                };
            case ProcessType.TimeIncreasing:

                if (studentStatus == StudentStatus.Rotation)
                {
                    return new List<ReasonType>()
                {
                    ReasonType.MaternityLeave,
                    ReasonType.HealthReport,
                    ReasonType.UnpaidVacation,
                    ReasonType.ForceMajeure,
                    ReasonType.MarriageLeave,
                    ReasonType.DeathLeave,
                    ReasonType.Accompaniment,
                    ReasonType.PaternityLeave,
                    ReasonType.DueToUnusualCircumstances,
                    ReasonType.Other,
                };
                }
                return new List<ReasonType>()
                {
                    ReasonType.MilitaryLeave,
                    ReasonType.MaternityLeave,
                    ReasonType.HealthReport,
                    ReasonType.UnpaidVacation,
                    //ReasonType.RelocationAndTimeExtensionDueToNegativeOpinion,
                    ReasonType.ExtensionByApplicationForSubjection,
                    ReasonType.ForceMajeure,
                    ReasonType.MarriageLeave,
                    ReasonType.DeathLeave,
                    ReasonType.Accompaniment,
                    ReasonType.PaternityLeave,
                    ReasonType.DueToUnusualCircumstances,
                    //ReasonType.TimeExtensionDueToThesisDefence,
                    ReasonType.Other,
                };
            case ProcessType.TimeDecreasing:
                return new List<ReasonType>()
                {
                    ReasonType.CountingTheEducationPeriodWithTheDecisionOfTheAcademicBoard,
                    ReasonType.CountingEducationPeriodsDueToLegislation,
                    ReasonType.TimeDecreasinginAccordanceWithLawNumber1219,
                    //ReasonType.TimeDecreasingDueToThesisDefence,
                    ReasonType.KKTCHalfTimed,
                    ReasonType.Other,
                };
            case ProcessType.Finish:
                if (studentStatus == StudentStatus.Rotation)
                {
                    return new List<ReasonType>()
                {
                    ReasonType.LeftDueToResignation,
                    ReasonType.LeftWithConsent,
                    ReasonType.LeftDueToDeath,
                    ReasonType.LeavingDuetoRePassingtheExam,
                    ReasonType.TerminationSuspensionofCivilService,
                    //ReasonType.Other,
                };
                }
                return new List<ReasonType>()
                {
                    //ReasonType.LeftDueToFailureInTheFinalExam,
                    ReasonType.LeftDueToResignation,
                    //ReasonType.SuccessfullyCompleted,
                    ReasonType.LeftWithConsent,
                    //ReasonType.LeftDueToFailureInThesis,
                    ReasonType.LeftDueToDeath,
                    ReasonType.KKTCHalfTimed,
                    ReasonType.LeavingDuetoRePassingtheExam,
                    ReasonType.TerminationSuspensionofCivilService,
                    ReasonType.Other,
                    ReasonType.BranchChange,
                    ReasonType.RegistrationByMistake,
                    ReasonType.Conviction,
                    ReasonType.BeingConsideredIndependent,
                    ReasonType.Exportation,
                    ReasonType.BannedFromProfession,
                };
            case ProcessType.Transfer:
                return new List<ReasonType>()
                {
                    ReasonType.UnexcusedTransfer,
                    ReasonType.ExcusedTransfer,
                    ReasonType.BranchChange_End,
                    ReasonType.Other,
                };
            default:
                return new();
        }
    }

    public static List<QuotaType_2> GetQuotaTypes(this QuotaType_1 quotaType_1)
    {
        switch (quotaType_1)
        {
            case QuotaType_1.MinistryOfHealth:
                return new List<QuotaType_2>()
                {
                    QuotaType_2.EAH,
                    QuotaType_2.SBA
                };
            case QuotaType_1.Uni:
                return new List<QuotaType_2>()
                {
                    QuotaType_2.University_State,
                    QuotaType_2.University_Private
                };
            case QuotaType_1.KKTC:
                return new List<QuotaType_2>()
                {
                    QuotaType_2.KKTCFullTime,
                    QuotaType_2.KKTCHalfTime
                };
            case QuotaType_1.MinistryOfInterior:
                return new List<QuotaType_2>()
                {
                    QuotaType_2.JGK,
                };
            case QuotaType_1.MSB:
                return new List<QuotaType_2>()
                {
                    QuotaType_2.HKK,
                    QuotaType_2.KKK,
                    QuotaType_2.DKK
                };
            case QuotaType_1.TDMM:
                return new List<QuotaType_2>()
                {
                    QuotaType_2.Vet,
                    QuotaType_2.Chemist,
                    QuotaType_2.Pharmacist
                };
            default:
                return new();
        }
    }
}