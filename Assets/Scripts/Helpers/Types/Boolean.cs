using System;

namespace Helpers.Types
{
	[System.Serializable]
	public struct Boolean : IEquatable<Boolean>
	{
		public const byte FALSE = 0;
		public const byte TRUE = 1;

		public byte BoolValue;

		public Boolean( bool value ) => BoolValue = ( value ? TRUE : FALSE );

		public Boolean( byte value ) => BoolValue = ( value == FALSE ? FALSE : TRUE );

		public static implicit operator bool( Boolean value ) => value.BoolValue == TRUE;

		public static implicit operator Boolean( bool value ) => new Boolean( value );

		public static implicit operator byte( Boolean value ) => value.BoolValue;

		public static implicit operator Boolean( byte value ) => new Boolean( value );

		public static bool operator ==( Boolean left, Boolean right ) => left.BoolValue == right.BoolValue;

		public static bool operator !=( Boolean left, Boolean right ) => left.BoolValue != right.BoolValue;

		public static bool operator ==( Boolean left, byte right ) => left.BoolValue == right;

		public static bool operator !=( Boolean left, byte right ) => left.BoolValue != right;

		public static bool operator ==( byte left, Boolean right ) => left == right.BoolValue;

		public static bool operator !=( byte left, Boolean right ) => left != right.BoolValue;

		public static bool operator ==( Boolean left, int right ) => left.BoolValue == right;

		public static bool operator !=( Boolean left, int right ) => left.BoolValue != right;

		public static bool operator ==( int left, Boolean right ) => left == right.BoolValue;

		public static bool operator !=( int left, Boolean right ) => left != right.BoolValue;

		public override string ToString() => BoolValue == TRUE ? true.ToString() : false.ToString();

		public override bool Equals( object obj ) => obj is Boolean boolean && Equals( boolean );

		public bool Equals( Boolean other ) => BoolValue == other.BoolValue;

		public override int GetHashCode() => BoolValue.GetHashCode();
	}
}