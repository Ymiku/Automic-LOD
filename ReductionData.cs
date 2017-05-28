using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace PolyReduction{
	public class ReductionData : ScriptableObject {
		public int vertexU;
		public int vertexV;
		public List<int> triangleID = new List<int>();
	}
}
