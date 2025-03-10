using Microsoft.AspNetCore.Components;
using Shared.Types;
using System;
using System.Collections.Generic;

namespace UI.Models
{
    public class MapLayerModel
    {
        public MarkerType MarkerType { get; set; }
        public List<MarkerModel> MarkerList{ get; set; }
    }
    public class MarkerModel
    {
        public Tuple<float,float> LatLong { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public string ColorCode { get; set; }
    }
}
