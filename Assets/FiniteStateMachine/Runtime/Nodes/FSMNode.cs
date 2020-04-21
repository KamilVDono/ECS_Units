using System;

using UnityEngine;

namespace FSM.Runtime.Nodes
{
	[Serializable]
	public abstract class FSMNode : ScriptableObject
	{
		public FSMGraph Parent;
		public Rect Position;
	}
}