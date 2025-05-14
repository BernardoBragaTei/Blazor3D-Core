using HomagGroup.Blazor3D.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomagGroup.Blazor3D.Events
{
    public class RayIntersectArgs
    {
        public string ContainerId { get; set; }
        public RayIntersectData[] IntersectData { get; set; }
    }

    public class RayIntersectData
    {
        public double Distance { get; set; }
        public string ObjectId { get; set; }
        public Vector3 Point { get; set; }
    }


}
