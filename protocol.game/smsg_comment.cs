using System;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "smsg_comment")]
public class smsg_comment : IExtensible
{
	private comment _comment;

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = true, Name = "comment", DataFormat = DataFormat.Default)]
	public comment comment
	{
		get
		{
			return _comment;
		}
		set
		{
			_comment = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
