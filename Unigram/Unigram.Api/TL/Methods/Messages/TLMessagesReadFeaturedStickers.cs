// <auto-generated/>
using System;

namespace Telegram.Api.TL.Methods.Messages
{
	/// <summary>
	/// RCP method messages.readFeaturedStickers
	/// </summary>
	public partial class TLMessagesReadFeaturedStickers : TLObject
	{
		public TLMessagesReadFeaturedStickers() { }
		public TLMessagesReadFeaturedStickers(TLBinaryReader from, bool cache = false)
		{
			Read(from, cache);
		}

		public override TLType TypeId { get { return TLType.MessagesReadFeaturedStickers; } }

		public override void Read(TLBinaryReader from, bool cache = false)
		{
			if (cache) ReadFromCache(from);
		}

		public override void Write(TLBinaryWriter to, bool cache = false)
		{
			to.Write(0xE21CBB);
			if (cache) WriteToCache(to);
		}
	}
}