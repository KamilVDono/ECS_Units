using Helpers;

using Maps.Components;

using Rendering.Components;

using System;
using System.Collections.Generic;

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

	// TODO: If want use in more effective way should use CommandBuffer
	// TODO: When SystemBase Entities starts support generic types change ComponentSystem to SystemBase
	public abstract class MeshCreatorSystem<TSystemComponent, TComponent> : ComponentSystem
		where TSystemComponent : struct, MeshCreatorSystemVisualComponent where TComponent : struct, IComponentData
	{
		#region Consts
		private static readonly ComponentType[] EMPTY_TYPES_ARRAY = new ComponentType[0];
		#endregion Consts

		protected EntityArchetype _meshArchetype;
		protected Mesh _mesh;

		private static Dictionary<string, Material> _shader2Material = new Dictionary<string, Material>();

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
				typeof( MainColorMaterialProperty ),
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
				_mesh = MeshCreator.Quad( Extends, quaternion.Euler( new float3( math.radians( 90 ), 0, 0 ) ) );
			}
		}

		protected override void OnUpdate()
		{
			// When render owner removed remove render
			Entities
				.WithNone<TComponent>()
				.ForEach( ( Entity entity, ref TSystemComponent visual ) =>
				{
					if ( visual.IsValid )
					{
						EntityManager.DestroyEntity( visual.ValueEntity );
					}
					EntityManager.RemoveComponent<TSystemComponent>( entity );
				} );
		}

		protected Entity CreateVisualEntity( string shader, float4 mainColor, ref MapIndex mapIndex, float orderIndex )
		{
			// Need valid entity right now so can not use PostUpdateCommands here
			// TODO: Create all required entities via NativeArray overload
			var visualEntity = EntityManager.CreateEntity( _meshArchetype );
			EntityManager.SetSharedComponentData( visualEntity, new RenderMesh { mesh = _mesh, material = MaterialFromShader( shader ) } );
			EntityManager.SetComponentData( visualEntity, new Translation { Value = new float3( mapIndex.Index2D.x, 0.1f * orderIndex, mapIndex.Index2D.y ) } );
			EntityManager.SetComponentData( visualEntity, new RenderBounds
			{
				Value = new AABB()
				{
					Center = float3.zero,
					Extents = new float3( Extends, 0, Extends )
				}
			} );
			EntityManager.SetComponentData( visualEntity, new MainColorMaterialProperty { Color = mainColor } );
			return visualEntity;
		}

		private Material MaterialFromShader( string shader )
		{
			if ( _shader2Material.TryGetValue( shader, out var material ) == false )
			{
				material = new Material( Shader.Find( shader ) )
				{
					enableInstancing = true
				};
				_shader2Material.Add( shader, material );
			}
			return material;
		}
	}
}