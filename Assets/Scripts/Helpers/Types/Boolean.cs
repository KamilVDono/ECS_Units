namespace Helpers.Types
{
	[System.Serializable]
	public struct Boolean
	{
		public byte boolValue;

		public Boolean( bool value ) => boolValue = (byte)( value ? 1 : 0 );

		public static implicit operator bool( Boolean value ) => value.boolValue == 1;

		public static implicit operator Boolean( bool value ) => new Boolean( value );

		public override string ToString() => boolValue == 1 ? true.ToString() : false.ToString();
	}
}