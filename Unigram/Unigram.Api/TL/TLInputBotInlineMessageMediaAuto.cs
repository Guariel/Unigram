// <auto-generated/>
using System;

namespace Telegram.Api.TL
{
	public partial class TLInputBotInlineMessageMediaAuto : TLInputBotInlineMessageBase, ITLMediaCaption 
	{
		[Flags]
		public enum Flag : Int32
		{
			ReplyMarkup = (1 << 2),
		}

		public bool HasReplyMarkup { get { return Flags.HasFlag(Flag.ReplyMarkup); } set { Flags = value ? (Flags | Flag.ReplyMarkup) : (Flags & ~Flag.ReplyMarkup); } }

		public Flag Flags { get; set; }
		public String Caption { get; set; }

		public TLInputBotInlineMessageMediaAuto() { }
		public TLInputBotInlineMessageMediaAuto(TLBinaryReader from, bool cache = false)
		{
			Read(from, cache);
		}

		public override TLType TypeId { get { return TLType.InputBotInlineMessageMediaAuto; } }

		public override void Read(TLBinaryReader from, bool cache = false)
		{
			Flags = (Flag)from.ReadInt32();
			Caption = from.ReadString();
			if (HasReplyMarkup) ReplyMarkup = TLFactory.Read<TLReplyMarkupBase>(from, cache);
			if (cache) ReadFromCache(from);
		}

		public override void Write(TLBinaryWriter to, bool cache = false)
		{
			UpdateFlags();

			to.Write(0x292FED13);
			to.Write((Int32)Flags);
			to.Write(Caption);
			if (HasReplyMarkup) to.WriteObject(ReplyMarkup, cache);
			if (cache) WriteToCache(to);
		}

		private void UpdateFlags()
		{
			HasReplyMarkup = ReplyMarkup != null;
		}
	}
}