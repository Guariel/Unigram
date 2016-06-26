// <auto-generated/>
using System;

namespace Telegram.Api.TL
{
	public partial class TLInputMediaGeoPoint : TLInputMediaBase 
	{

		public TLInputMediaGeoPoint() { }
		public TLInputMediaGeoPoint(TLBinaryReader from, TLType type = TLType.InputMediaGeoPoint)
		{
			Read(from, type);
		}

		public override TLType TypeId { get { return TLType.InputMediaGeoPoint; } }

		public override void Read(TLBinaryReader from, TLType type = TLType.InputMediaGeoPoint)
		{
			GeoPoint = TLFactory.Read<TLInputGeoPointBase>(from);
		}

		public override void Write(TLBinaryWriter to)
		{
			to.Write(0xF9C44144);
			to.WriteObject(GeoPoint);
		}
	}
}