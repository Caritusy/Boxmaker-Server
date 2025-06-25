using System;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "smsg_favorite_map")]
public class smsg_favorite_map : IExtensible
{
	private int _num;

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = true, Name = "num", DataFormat = DataFormat.TwosComplement)]
	public int num
	{
		get
		{
			return _num;
		}
		set
		{
			_num = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
