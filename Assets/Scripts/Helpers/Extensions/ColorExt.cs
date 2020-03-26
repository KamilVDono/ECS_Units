using Unity.Mathematics;

using UnityEngine;

namespace Helpers.Extensions
{
	public static class ColorExt
	{
		public static float4 AsFloat4( this Color color ) => new float4( color.r, color.g, color.b, color.a );

		public static float4 AsFloat4( this Color32 color ) => new float4( color.r / 255f, color.g / 255f, color.b / 255f, color.a / 255f );

		public static float3 AsFloat3( this Color color ) => new float3( color.r, color.g, color.b );
	}
}