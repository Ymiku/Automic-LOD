using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
namespace PolyReduction{
public class AutomaticLOD : MonoBehaviour {
		public Mesh meshToGenerate;
		private Vector3[] vertices;
		private int[] triangles;
		private Vector3[] normals;
		private PRVertex[] prVertices;
		private PRTriangle[] prTriangle;
		private int vertexNum = 0;
		private List<ReductionData> reductionData = new List<ReductionData>();
		// Use this for initialization
		void Start () {
			
			Generate ();
		}
		void Generate()
		{
			if (meshToGenerate == null)
				meshToGenerate = GetComponent<MeshFilter> ().mesh;
			meshToGenerate = Object.Instantiate<Mesh> (meshToGenerate);

			vertices = meshToGenerate.vertices;
			triangles = meshToGenerate.triangles;
			normals = meshToGenerate.normals;
			vertexNum = vertices.Length;
			prVertices = new PRVertex[vertices.Length];
			prTriangle = new PRTriangle[triangles.Length/3];
			int i;
			int j;
			for (i = 0; i <vertices.Length; i++) {
				prVertices [i] = new PRVertex (i,vertices[i]);
			}
			for (i = 0,j=0; i < triangles.Length; i+=3,j+=1) {
				prTriangle [j] = new PRTriangle (i,prVertices[triangles[i]],prVertices[triangles[i+1]],prVertices[triangles[i+2]]);
			}
			for (i = 0; i < prTriangle.Length; i++) {
				prTriangle [i].vertex [0].face.Add (prTriangle [i]);
				prTriangle [i].vertex [1].face.Add (prTriangle [i]);
				prTriangle [i].vertex [2].face.Add (prTriangle [i]);
				for (j = 0; j < 3; j++) {
					for (int k = 0; k < 3; k++) {
						if (j == k)
							continue;
						if(!prTriangle [i].vertex [j].neighbor.Contains(prTriangle [i].vertex [k]))
						{
							prTriangle [i].vertex [j].neighbor.Add(prTriangle [i].vertex [k]);
						}
					}
				}
			}

			for (i = 0; i < prVertices.Length; i++) {
				ComputeEdgeCostAtVertex(prVertices[i]);
			}
			for (int zx = 0; zx < 60; zx++) {
				PRVertex mn = MinimunCostEdge ();
				Collapse (mn,mn.collapse);
				vertexNum--;
			}


			for (int C6H14O2 = 0; C6H14O2 < reductionData.Count; C6H14O2++) {
				ApplyData (reductionData[C6H14O2]);
			}
			meshToGenerate.vertices = vertices;
			meshToGenerate.triangles = triangles;
			GetComponent<MeshFilter> ().mesh = meshToGenerate;
		}
		public PRVertex MinimunCostEdge()
		{
			PRVertex t=prVertices[0];
			for (int i = 0; i < prVertices.Length; i++) {
				if (prVertices [i].cost < t.cost)
					t = prVertices [i];
			}
			return t;
		}
		public float ComputeEdgeCollapseCost(PRVertex u,PRVertex v)
		{
			float edgeLength = MathExtra.GetV3L (u.pos-v.pos);
			float curvature = 0f;
			List<PRTriangle> sides = new List<PRTriangle> ();
			for (int i = 0; i < u.face.Count; i++) {
				if (u.face [i].HasVertex (v))
					sides.Add (u.face[i]);
			}
			for (int i = 0; i < u.face.Count; i++) {
				float mincurv = 1f;
				for (int j = 0; j < sides.Count; j++) {
					float dotprod = Vector3.Dot (u.face [i].normal, sides [j].normal);
					mincurv = Mathf.Min (mincurv,(1f-dotprod)*0.5f);
				}
				curvature = Mathf.Max (curvature,mincurv);
			}
			return edgeLength * curvature;
		}
		public void ComputeEdgeCostAtVertex(PRVertex v)
		{
			if (v.neighbor.Count == 0) {
				v.collapse = null;
				v.cost = 1000000f;
				return;
			}
			v.cost = 1000000f;
			v.collapse = null;
			for (int i = 0; i < v.neighbor.Count; i++) {
				float c;
				c = ComputeEdgeCollapseCost (v,v.neighbor[i]);
				if (c < v.cost) {
					v.collapse = v.neighbor [i];
					v.cost = c;
				}

			}
		}
		public void Collapse(PRVertex u,PRVertex v)
		{
			if (v==null) {
				Debug.Log ("!!!");//prVertices [u.id] = null;
				return;
			}
			//Debug.Log (u.id.ToString()+"  "+v.id.ToString()+"  "+u.cost.ToString());
			int i;
			List<PRVertex> tmp = new List<PRVertex> ();
			for (i = 0; i < u.neighbor.Count; i++) {
				tmp.Add(u.neighbor[i]);
			}
			ReductionData rd = new ReductionData ();
			rd.vertexU = u.id;
			rd.vertexV = v.id;
			v.neighbor.Remove (u);
			for (i = u.face.Count-1; i >= 0; i--) {
				u.face[i].ReplaceVertex(u,v);
			}
			for (int j = 0; j < u.face.Count; j++) {
				rd.triangleID.Add(u.face[j].id);
			}
			reductionData.Add (rd);
			ComputeEdgeCostAtVertex (v);
			//prVertices [u.id] = null;
			for (i = 0; i < tmp.Count; i++) {
				ComputeEdgeCostAtVertex(tmp[i]);
			}
			//prVertices [u.id] = null;
			u.cost = 10000000f;
		}
		public void ApplyData(ReductionData rd)
		{

			for (int i = 0; i < rd.triangleID.Count; i++) {
				if (triangles [rd.triangleID [i]] == rd.vertexV || triangles [rd.triangleID [i] + 1] == rd.vertexV || triangles [rd.triangleID [i] + 2] == rd.vertexV) {
					triangles [rd.triangleID [i]] = triangles [rd.triangleID [i]+1] = triangles [rd.triangleID [i]+2] = 0;
				} else {
					if (triangles [rd.triangleID [i]] == rd.vertexU) {
						triangles [rd.triangleID [i]] = rd.vertexV;
						continue;
					}
					if (triangles [rd.triangleID [i]+1] == rd.vertexU) {
						triangles [rd.triangleID [i]+1] = rd.vertexV;
						continue;
					}
					if (triangles [rd.triangleID [i]+2] == rd.vertexU) {
						triangles [rd.triangleID [i]+2] = rd.vertexV;
						continue;
					}
				}
			}
		}
		public void BackData(ReductionData rd)
		{
			
		}

	}
}
