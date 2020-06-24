// Copyright (c) Rotorz Limited. All rights reserved. Licensed under the MIT license. See LICENSE
// file in the project root.

using System;

using UnityEngine;
using UnityEngine.Serialization;

namespace Rotorz.Games.Reflection
{
	/// <summary>
	/// Reference to a class <see cref="System.Type"/> with support for Unity serialization.
	/// </summary>
	[Serializable]
	public sealed class ClassTypeReference : ISerializationCallbackReceiver
	{
		[SerializeField, FormerlySerializedAs("_classRef")]
		private string classRef;

		private Type type;

		/// <summary>
		/// Gets or sets type of class reference.
		/// </summary>
		/// <exception cref="System.ArgumentException">
		/// If <paramref name="value"/> is not a class type.
		/// </exception>
		public Type Type
		{
			get => this.type;
			set
			{
				if ( value != null && !( value.IsClass || value.IsValueType ) )
				{
					throw new ArgumentException( string.Format( "'{0}' is not a class type.", value.FullName ), "value" );
				}

				this.type = value;
				this.classRef = GetClassRef( value );
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ClassTypeReference"/> class.
		/// </summary>
		public ClassTypeReference()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ClassTypeReference"/> class.
		/// </summary>
		/// <param name="assemblyQualifiedClassName">Assembly qualified class name.</param>
		public ClassTypeReference( string assemblyQualifiedClassName )
		{
			this.Type = !string.IsNullOrEmpty( assemblyQualifiedClassName )
				? Type.GetType( assemblyQualifiedClassName )
				: null;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ClassTypeReference"/> class.
		/// </summary>
		/// <param name="type">Class type.</param>
		/// <exception cref="System.ArgumentException">
		/// If <paramref name="type"/> is not a class type.
		/// </exception>
		public ClassTypeReference( Type type ) => this.Type = type;

		public static string GetClassRef( Type type )
		{
			return type != null
				? type.FullName + ", " + type.Assembly.GetName().Name
				: "";
		}

		public static implicit operator string( ClassTypeReference typeReference ) => typeReference.classRef;

		public static implicit operator Type( ClassTypeReference typeReference ) => typeReference.Type;

		public static implicit operator ClassTypeReference( Type type ) => new ClassTypeReference( type );

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if ( !string.IsNullOrEmpty( this.classRef ) )
			{
				this.type = System.Type.GetType( this.classRef );

				if ( this.type == null )
				{
					Debug.LogWarning( string.Format( "'{0}' was referenced but class type was not found.", this.classRef ) );
				}
			}
			else
			{
				this.type = null;
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		public override string ToString() => this.Type != null ? this.Type.FullName : "(None)";
	}
}