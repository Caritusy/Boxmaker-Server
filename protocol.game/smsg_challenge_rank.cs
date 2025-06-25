using System;
using System.Collections.Generic;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "smsg_challenge_rank")]
public class smsg_challenge_rank : IExtensible
{
	private readonly List<challenge_rank> _ranks = new List<challenge_rank>();

	private IExtension extensionObject;

	[ProtoMember(1, Name = "ranks", DataFormat = DataFormat.Default)]
	public List<challenge_rank> ranks => _ranks;

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
