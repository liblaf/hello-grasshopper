using System;
using System.Collections.Generic;

namespace HelloGrasshopper
{
    internal class Edge<T> : IComparable<Edge<T>>, IEquatable<Edge<T>>
        where T : IComparable<T>
    {
        public Edge(Vertex<T> from, Vertex<T> to)
        {
            switch (from.CompareTo(to))
            {
                case -1:
                    this.from = from;
                    this.to = to;
                    break;
                case 0:
                    throw new Exception("Cannot create an edge from a vertex to itself.");
                case 1:
                    this.from = to;
                    this.to = from;
                    break;
                default:
                    throw new Exception("Unexpected comparison result.");
            }
        }

        public int CompareTo(Edge<T> other)
        {
            switch (this.from.CompareTo(other.from))
            {
                case -1:
                    return -1;
                case 0:
                    return this.to.CompareTo(other.to);
                case 1:
                    return 1;
                default:
                    throw new Exception("Unexpected comparison result.");
            }
        }

        public bool Equals(Edge<T> other) =>
            this.from.Equals(other.from) && this.to.Equals(other.to);

        public override int GetHashCode() => this.from.GetHashCode() ^ this.to.GetHashCode();

        public Vertex<T> from { get; set; }

        public Vertex<T> to { get; set; }
    }

    internal class Vertex<T> : IComparable<Vertex<T>>, IEquatable<Vertex<T>>
        where T : IComparable<T>
    {
        public Vertex(T data)
        {
            this.data = data;
            this.adjacencies = new HashSet<Vertex<T>>();
        }

        public int CompareTo(Vertex<T> other) => this.data.CompareTo(other.data);

        public bool Equals(Vertex<T> other) => this.data.Equals(other.data);

        public override int GetHashCode() => this.data.GetHashCode();

        public void AddAdjacency(Vertex<T> v)
        {
            this.adjacencies.Add(v);
            v.adjacencies.Add(this);
        }

        public T data { get; set; }

        public HashSet<Vertex<T>> adjacencies { get; set; }
    }

    internal class Graph<T>
        where T : IComparable<T>, IEquatable<T>
    {
        public Graph() => this.vertices = new HashSet<Vertex<T>>();

        public void AddEdge(Vertex<T> u, Vertex<T> v) => u.AddAdjacency(v);

        public void AddEdge(T u, T v)
        {
            this.vertices.TryGetValue(new Vertex<T>(u), out Vertex<T> uu);
            this.vertices.TryGetValue(new Vertex<T>(v), out Vertex<T> vv);
            this.AddEdge(uu, vv);
        }

        public void AddVertex(Vertex<T> v) => this.vertices.Add(v);

        public void AddVertex(T data) => this.vertices.Add(new Vertex<T>(data));

        public void ContractVertex(Vertex<T> root, Vertex<T> vertex)
        {
            foreach (Vertex<T> u in vertex.adjacencies)
                this.AddEdge(root, u);
            this.RemoveVertex(vertex);
        }

        public void ContractVertices(Vertex<T> root, IEnumerable<Vertex<T>> vertices)
        {
            foreach (Vertex<T> u in vertices)
                this.ContractVertex(root, u);
        }

        public void RemoveDegree2Vertex(Vertex<T> vertex)
        {
            if (vertex.adjacencies.Count != 2)
                return;
            List<Vertex<T>> adjacencies = new List<Vertex<T>>(vertex.adjacencies);
            this.AddEdge(adjacencies[0], adjacencies[1]);
            this.RemoveVertex(vertex);
        }

        public void RemoveDegree2Vertices()
        {
            List<Vertex<T>> vertices = new List<Vertex<T>>(this.vertices);
            this.RemoveDegree2Vertices(vertices);
        }

        public void RemoveDegree2Vertices(IEnumerable<Vertex<T>> vertices)
        {
            foreach (Vertex<T> u in vertices)
                this.RemoveDegree2Vertex(u);
        }

        public void RemoveIsolatedVertices()
        {
            List<Vertex<T>> to_remove = new List<Vertex<T>>();
            foreach (Vertex<T> v in this.vertices)
                if (v.adjacencies.Count == 0)
                    to_remove.Add(v);
            this.vertices.ExceptWith(to_remove);
        }

        public void RemoveLoops()
        {
            foreach (Vertex<T> v in this.vertices)
                v.adjacencies.Remove(v);
        }

        public void RemoveVertex(Vertex<T> vertex)
        {
            foreach (Vertex<T> u in vertex.adjacencies)
                u.adjacencies.Remove(vertex);
            this.vertices.Remove(vertex);
        }

        public void RemoveVertices(IEnumerable<Vertex<T>> vertices)
        {
            foreach (Vertex<T> v in vertices)
                this.RemoveVertex(v);
        }

        public HashSet<Edge<T>> edges
        {
            get
            {
                HashSet<Edge<T>> edges = new HashSet<Edge<T>>();
                foreach (Vertex<T> u in this.vertices)
                    foreach (Vertex<T> v in u.adjacencies)
                        edges.Add(new Edge<T>(u, v));
                return edges;
            }
        }

        public HashSet<Vertex<T>> vertices { get; set; }
    }
}
