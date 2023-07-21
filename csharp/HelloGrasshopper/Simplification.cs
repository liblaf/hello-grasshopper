using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace HelloGrasshopper
{
    public class Simplification : GH_Component
    {
        public Simplification()
            : base(
                name: "Simplification",
                nickname: "SimpleC",
                description: "Simplify curves",
                category: "liblaf",
                subCategory: "Curve"
            ) { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter(
                name: "Curves",
                nickname: "C",
                description: "list of curves to simplify",
                access: GH_ParamAccess.list
            );
            pManager.AddNumberParameter(
                name: "Tolerance1",
                nickname: "t1",
                description: "If any points are less than this distance apart, they will be combined",
                access: GH_ParamAccess.item,
                @default: 0.01
            );
            pManager.AddNumberParameter(
                name: "Tolerance2",
                nickname: "t2",
                description: "If any points are less than this distance apart, they will be combined",
                access: GH_ParamAccess.item,
                @default: 0.1
            );
        }

        public override Guid ComponentGuid => new Guid("E22536A3-C049-406C-A211-6BFA020EB593");

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter(
                name: "UniqueCurves",
                nickname: "C",
                description: "list of unique curves",
                access: GH_ParamAccess.list
            );
            pManager.AddPointParameter(
                name: "UniquePoints",
                nickname: "Q",
                description: "list of unique points",
                access: GH_ParamAccess.list
            );
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> curves = new List<Curve>();
            double tolerance_1 = 0.0;
            double tolerance_2 = 0.0;
            DA.GetDataList(name: "Curves", list: curves);
            DA.GetData(name: "Tolerance1", destination: ref tolerance_1);
            DA.GetData(name: "Tolerance2", destination: ref tolerance_2);
            Graph<Point3d> graph = new Graph<Point3d>();
            foreach (Curve curve in curves)
            {
                foreach (Curve segment in curve.DuplicateSegments())
                {
                    Point3d start = segment.PointAtStart;
                    Point3d end = segment.PointAtEnd;
                    graph.AddVertex(start);
                    graph.AddVertex(end);
                    graph.AddEdge(start, end);
                }
            }
            Simplify(ref graph, tolerance_1);
            Simplify(ref graph, tolerance_2);
            graph.RemoveDegree2Vertices();
            List<LineCurve> unique_curves = new List<LineCurve>();
            foreach (Edge<Point3d> e in graph.edges)
                unique_curves.Add(new LineCurve(from: e.from.data, to: e.to.data));
            DA.SetDataList(paramName: "UniqueCurves", data: unique_curves);
            List<Point3d> unique_points = new List<Point3d>();
            foreach (Vertex<Point3d> v in graph.vertices)
                unique_points.Add(v.data);
            DA.SetDataList(paramName: "UniquePoints", data: unique_points);
        }

        internal void Simplify(ref Graph<Point3d> graph, double tolerance = 0.01)
        {
            List<Vertex<Point3d>> vertices = new List<Vertex<Point3d>>(graph.vertices);
            vertices.Sort(
                (Vertex<Point3d> u, Vertex<Point3d> v) =>
                {
                    if (u.adjacencies.Count == v.adjacencies.Count)
                        return u.data.CompareTo(v.data);
                    else
                        return v.adjacencies.Count.CompareTo(u.adjacencies.Count);
                }
            );
            foreach (Vertex<Point3d> u in vertices)
            {
                if (!graph.vertices.Contains(u))
                    continue;
                List<Vertex<Point3d>> to_remove = new List<Vertex<Point3d>>();
                foreach (Vertex<Point3d> v in graph.vertices)
                {
                    if (u == v)
                        continue;
                    if (u.data.DistanceTo(v.data) < tolerance)
                        to_remove.Add(v);
                }
                graph.ContractVertices(root: u, vertices: to_remove);
            }
            graph.RemoveLoops();
            graph.RemoveIsolatedVertices();
        }
    }
}
