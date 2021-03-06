// <auto-generated/>
using System;

namespace Telegram.Api.TL
{
	public partial class TLMessageEntityMention : TLMessageEntityBase 
	{

		public TLMessageEntityMention() { }
		public TLMessageEntityMention(TLBinaryReader from, bool cache = false)
		{
			Read(from, cache);
		}

		public override TLType TypeId { get { return TLType.MessageEntityMention; } }

		public override void Read(TLBinaryReader from, bool cache = false)
		{
			Offset = from.ReadInt32();
			Length = from.ReadInt32();
			if (cache) ReadFromCache(from);
		}

		public override void Write(TLBinaryWriter to, bool cache = false)
		{
			to.Write(0xFA04579D);
			to.Write(Offset);
			to.Write(Length);
			if (cache) WriteToCache(to);
		}
	}
}