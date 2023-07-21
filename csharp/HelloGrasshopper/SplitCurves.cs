using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using Rhino.Geometry;
using Rhino.Geometry.Intersect;

namespace HelloGrasshopper
{
    public class SplitCurves : GH_Component
    {
        public SplitCurves()
            : base(
                name: "SplitCurves",
                nickname: "SplitC",
                description: "Split curves at intersections",
                category: "liblaf",
                subCategory: "Curve"
            ) { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter(
                name: "Curves",
                nickname: "C",
                description: "Curves to split",
                access: GH_ParamAccess.tree
            );
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter(
                name: "Curves",
                nickname: "C",
                description: "Split curves",
                access: GH_ParamAccess.tree
            );
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.GetDataTree(name: "Curves", tree: out GH_Structure<GH_Curve> curves);
            GH_Structure<GH_Curve> result = new GH_Structure<GH_Curve>();
            foreach (GH_Path path in curves.Paths)
            {
                for (int i = 0; i < curves[path].Count; ++i)
                {
                    Curve curve = curves[path][i].Value;
                    List<double> parameters = new List<double>();
                    foreach (GH_Curve gh_cutter in curves.AllData(skipNulls: true))
                    {
                        Curve cutter = gh_cutter.Value;
                        if (cutter == curve)
                            continue;
                        CurveIntersections intersections = Intersection.CurveCurve(
                            curve,
                            cutter,
                            tolerance: 0.0,
                            overlapTolerance: 0.0
                        );
                        parameters.AddRange(
                            intersections.Select(intersection => intersection.ParameterA)
                        );
                    }
                    parameters.Sort();
                    parameters = parameters
                        .Prepend(curve.Domain.Min)
                        .Append(curve.Domain.Max)
                        .ToList();
                    List<GH_Curve> segments = new List<GH_Curve>();
                    for (int j = 0; j + 1 < parameters.Count; ++j)
                    {
                        double t0 = parameters[j];
                        double t1 = parameters[j + 1];
                        segments.Add(new GH_Curve(curve.Trim(t0, t1)));
                    }
                    result.AppendRange(data: segments, path: path.AppendElement(i));
                }
            }
            DA.SetDataTree(paramIndex: 0, tree: result);
        }

        public override Guid ComponentGuid => new Guid("42F95841-B049-4F0E-B9C6-3B61EADF2DBF");
    }
}
