using Helpers;

using Maps.Components;

using System;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

using UnityEngine;

namespace Visuals.Systems
{
	public interface MeshCreatorSystemVisualComponent : ISystemStateComponentData
	{
		Entity ValueEntity { get; }
		bool IsValid { get; }
	}

	public abstract class MeshCreatorSystem<TSystemComponent, TComponent> : ComponentSystem
		where TSystemComponent : struct, MeshCreatorSystemVisualComponent where TComponent : struct, IComponentData
	{
		#region Consts
		private static readonly ComponentType[] EMPTY_TYPES_ARRAY = new ComponentType[0];
		#endregion Consts

		protected EntityArchetype _meshArchetype;
		protected Mesh _mesh;

		#region Properties
		protected virtual bool CreateDefaultMesh => true;
		protected abstract float Extends { get; }
		protected virtual ComponentType[] AdditionalArchetypeTypes => EMPTY_TYPES_ARRAY;
		#endregion Properties

		protected override void OnCreate()
		{
			base.OnCreate();

			// Default types
			ComponentType[] componentTypes = new ComponentType[]{
				typeof( RenderMesh ),
				typeof( LocalToWorld ),
				typeof( Translation ),
				typeof( RenderBounds ),
				//typeof( PerInstanceCullingTag )
			};

			// Add additional
			if ( AdditionalArchetypeTypes.Length > 0 )
			{
				var oldSize = componentTypes.Length;
				Array.Resize( ref componentTypes, oldSize + AdditionalArchetypeTypes.Length );
				for ( int i = oldSize; i < componentTypes.Length; i++ )
				{
					componentTypes[i] = AdditionalArchetypeTypes[i - oldSize];
				}
			}

			// Create archetype
			_meshArchetype = EntityManager.CreateArchetype( componentTypes );

			if ( CreateDefaultMesh )
			{
				_mesh = MeshCreator.Quad( Extends, quaternion.Euler( new float3( math.radians( -90 ), 0, 0 ) ) );
			}
		}

		protected override void OnUpdate()
		{
			Entities
				.WithNone<TComponent>()
				.ForEach( ( Entity entity, ref TSystemComponent visual ) =>
				{
					if ( visual.IsValid )
					{
						PostUpdateCommands.DestroyEntity( visual.ValueEntity );
					}
					PostUpdateCommands.RemoveComponent<TSystemComponent>( entity );
				} );
		}

		protected Entity CreateVisualEntity( Material material, ref MapIndex mapIndex, float orderIndex )
		{
			// Need valid entity right now so can not use PostUpdateCommands here
			// TODO: Create all required entities via NativeArray overload
			var visualEntity = EntityManager.CreateEntity( _meshArchetype );
			PostUpdateCommands.SetSharedComponent( visualEntity, new RenderMesh { mesh = _mesh, material = material } );
			PostUpdateCommands.SetComponent( visualEntity, new Translation { Value = new float3( mapIndex.Index2D.x, 0.1f * orderIndex, mapIndex.Index2D.y ) } );
			PostUpdateCommands.SetComponent( visualEntity, new RenderBounds
			{
				Value = new AABB()
				{
					Center = float3.zero,
					Extents = new float3( Extends, 0, Extends )
				}
			} );
			return visualEntity;
		}
	}
}