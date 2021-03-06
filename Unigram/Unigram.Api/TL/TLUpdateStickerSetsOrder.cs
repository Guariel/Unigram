// <auto-generated/>
using System;

namespace Telegram.Api.TL
{
	public partial class TLUpdateStickerSetsOrder : TLUpdateBase 
	{
		public TLVector<Int64> Order { get; set; }

		public TLUpdateStickerSetsOrder() { }
		public TLUpdateStickerSetsOrder(TLBinaryReader from, bool cache = false)
		{
			Read(from, cache);
		}

		public override TLType TypeId { get { return TLType.UpdateStickerSetsOrder; } }

		public override void Read(TLBinaryReader from, bool cache = false)
		{
			Order = TLFactory.Read<TLVector<Int64>>(from, cache);
			if (cache) ReadFromCache(from);
		}

		public override void Write(TLBinaryWriter to, bool cache = false)
		{
			to.Write(0xF0DFB451);
			to.WriteObject(Order, cache);
			if (cache) WriteToCache(to);
		}
	}
}