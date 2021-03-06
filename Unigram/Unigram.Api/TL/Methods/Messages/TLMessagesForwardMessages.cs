// <auto-generated/>
using System;

namespace Telegram.Api.TL.Methods.Messages
{
	/// <summary>
	/// RCP method messages.forwardMessages
	/// </summary>
	public partial class TLMessagesForwardMessages : TLObject
	{
		[Flags]
		public enum Flag : Int32
		{
			Silent = (1 << 5),
			Background = (1 << 6),
		}

		public bool IsSilent { get { return Flags.HasFlag(Flag.Silent); } set { Flags = value ? (Flags | Flag.Silent) : (Flags & ~Flag.Silent); } }
		public bool IsBackground { get { return Flags.HasFlag(Flag.Background); } set { Flags = value ? (Flags | Flag.Background) : (Flags & ~Flag.Background); } }

		public Flag Flags { get; set; }
		public TLInputPeerBase FromPeer { get; set; }
		public TLVector<Int32> Id { get; set; }
		public TLVector<Int64> RandomId { get; set; }
		public TLInputPeerBase ToPeer { get; set; }

		public TLMessagesForwardMessages() { }
		public TLMessagesForwardMessages(TLBinaryReader from, bool cache = false)
		{
			Read(from, cache);
		}

		public override TLType TypeId { get { return TLType.MessagesForwardMessages; } }

		public override void Read(TLBinaryReader from, bool cache = false)
		{
			Flags = (Flag)from.ReadInt32();
			FromPeer = TLFactory.Read<TLInputPeerBase>(from, cache);
			Id = TLFactory.Read<TLVector<Int32>>(from, cache);
			RandomId = TLFactory.Read<TLVector<Int64>>(from, cache);
			ToPeer = TLFactory.Read<TLInputPeerBase>(from, cache);
			if (cache) ReadFromCache(from);
		}

		public override void Write(TLBinaryWriter to, bool cache = false)
		{
			UpdateFlags();

			to.Write(0x708E0195);
			to.Write((Int32)Flags);
			to.WriteObject(FromPeer, cache);
			to.WriteObject(Id, cache);
			to.WriteObject(RandomId, cache);
			to.WriteObject(ToPeer, cache);
			if (cache) WriteToCache(to);
		}

		private void UpdateFlags()
		{
		}
	}
}