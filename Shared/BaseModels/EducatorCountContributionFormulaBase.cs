namespace Shared.BaseModels
{
    public class EducatorCountContributionFormulaBase
    {
        public int? MinEducatorCount { get; set; }
        public int? MaxEducatorCount { get; set; }
        public double? Coefficient { get; set; }
        public double? BaseScore { get; set; }
        public bool? IsExpert { get; set; }
    }
}
