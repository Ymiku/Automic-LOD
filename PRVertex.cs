using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace PolyReduction{
	public class PRVertex {
		public int id;
		public Vector3 pos;
		public List<PRVertex> neighbor = new List<PRVertex>();
		public List<PRTriangle> face = new List<PRTriangle>();
		public float cost;
		public PRVertex collapse;
		public PRVertex(int id,Vector3 pos)
		{
			this.id = id;
			this.pos = pos;
		}
		public void AddNeighbor(PRVertex v)
		{
			if ((!neighbor.Contains (v))&&v!=this)
				neighbor.Add (v);
		}
		public void AddFace(PRTriangle t)
		{
			if (!face.Contains (t))
				face.Add (t);
		}
	}
}
