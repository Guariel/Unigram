// <auto-generated/>
using System;

namespace Telegram.Api.TL
{
	public partial class TLUpdateNewEncryptedMessage : TLUpdateBase 
	{
		public TLEncryptedMessageBase Message { get; set; }
		public Int32 Qts { get; set; }

		public TLUpdateNewEncryptedMessage() { }
		public TLUpdateNewEncryptedMessage(TLBinaryReader from, TLType type = TLType.UpdateNewEncryptedMessage)
		{
			Read(from, type);
		}

		public override TLType TypeId { get { return TLType.UpdateNewEncryptedMessage; } }

		public override void Read(TLBinaryReader from, TLType type = TLType.UpdateNewEncryptedMessage)
		{
			Message = TLFactory.Read<TLEncryptedMessageBase>(from);
			Qts = from.ReadInt32();
		}

		public override void Write(TLBinaryWriter to)
		{
			to.Write(0x12BCBD9A);
			to.WriteObject(Message);
			to.Write(Qts);
		}
	}
}