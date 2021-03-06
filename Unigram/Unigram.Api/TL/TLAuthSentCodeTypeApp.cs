// <auto-generated/>
using System;

namespace Telegram.Api.TL
{
	public partial class TLAuthSentCodeTypeApp : TLAuthSentCodeTypeBase 
	{

		public TLAuthSentCodeTypeApp() { }
		public TLAuthSentCodeTypeApp(TLBinaryReader from, bool cache = false)
		{
			Read(from, cache);
		}

		public override TLType TypeId { get { return TLType.AuthSentCodeTypeApp; } }

		public override void Read(TLBinaryReader from, bool cache = false)
		{
			Length = from.ReadInt32();
			if (cache) ReadFromCache(from);
		}

		public override void Write(TLBinaryWriter to, bool cache = false)
		{
			to.Write(0x3DBB5986);
			to.Write(Length);
			if (cache) WriteToCache(to);
		}
	}
}