// <auto-generated/>
using System;

namespace Telegram.Api.TL
{
	public partial class TLUpdateNewAuthorization : TLUpdateBase 
	{
		public Int64 AuthKeyId { get; set; }
		public String Device { get; set; }
		public String Location { get; set; }

		public TLUpdateNewAuthorization() { }
		public TLUpdateNewAuthorization(TLBinaryReader from, bool cache = false)
		{
			Read(from, cache);
		}

		public override TLType TypeId { get { return TLType.UpdateNewAuthorization; } }

		public override void Read(TLBinaryReader from, bool cache = false)
		{
			AuthKeyId = from.ReadInt64();
			Date = from.ReadInt32();
			Device = from.ReadString();
			Location = from.ReadString();
			if (cache) ReadFromCache(from);
		}

		public override void Write(TLBinaryWriter to, bool cache = false)
		{
			to.Write(0x8F06529A);
			to.Write(AuthKeyId);
			to.Write(Date);
			to.Write(Device);
			to.Write(Location);
			if (cache) WriteToCache(to);
		}
	}
}