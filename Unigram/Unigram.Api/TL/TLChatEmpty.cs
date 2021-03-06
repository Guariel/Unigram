// <auto-generated/>
using System;

namespace Telegram.Api.TL
{
	public partial class TLChatEmpty : TLChatBase 
	{

		public TLChatEmpty() { }
		public TLChatEmpty(TLBinaryReader from, bool cache = false)
		{
			Read(from, cache);
		}

		public override TLType TypeId { get { return TLType.ChatEmpty; } }

		public override void Read(TLBinaryReader from, bool cache = false)
		{
			Id = from.ReadInt32();
			if (cache) ReadFromCache(from);
		}

		public override void Write(TLBinaryWriter to, bool cache = false)
		{
			to.Write(0x9BA2D800);
			to.Write(Id);
			if (cache) WriteToCache(to);
		}
	}
}