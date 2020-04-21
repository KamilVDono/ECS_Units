using FSM.Editor.Edges;

using System;

using UnityEditor.Experimental.GraphView;

using UnityEngine.UIElements;

namespace FSM.Editor.Nodes
{
	public class FSMNodePort : Port
	{
		private FSMNodePort( Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type )
		: base( portOrientation, portDirection, portCapacity, type ) { }

		public static Port Create( IEdgeConnectorListener connectorListener, Direction direction, string name )
		{
			var port = new FSMNodePort(Orientation.Horizontal, direction, Capacity.Multi, null)
			{
				m_EdgeConnector = new EdgeConnector<FSMEdgeView>(connectorListener),
			};
			port.AddManipulator( port.m_EdgeConnector );
			port.portName = name;
			return port;
		}
	}
}