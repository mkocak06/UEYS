using System.Collections.Generic;

namespace UI.Models;

public class ValidationResultModel
{
    public string Key { get; set; }
    public List<ValidationResultValueModel> Value { get; set; }
}

public class ValidationResultValueModel
{
    public string ErrorMessage { get; set; }
}