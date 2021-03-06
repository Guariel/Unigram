// <auto-generated/>
using System;

namespace Telegram.Api.TL
{
	public partial class TLUpdateChatParticipants : TLUpdateBase 
	{
		public TLChatParticipantsBase Participants { get; set; }

		public TLUpdateChatParticipants() { }
		public TLUpdateChatParticipants(TLBinaryReader from, bool cache = false)
		{
			Read(from, cache);
		}

		public override TLType TypeId { get { return TLType.UpdateChatParticipants; } }

		public override void Read(TLBinaryReader from, bool cache = false)
		{
			Participants = TLFactory.Read<TLChatParticipantsBase>(from, cache);
			if (cache) ReadFromCache(from);
		}

		public override void Write(TLBinaryWriter to, bool cache = false)
		{
			to.Write(0x7761198);
			to.WriteObject(Participants, cache);
			if (cache) WriteToCache(to);
		}
	}
}