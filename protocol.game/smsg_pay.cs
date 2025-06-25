using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "smsg_pay")]
public class smsg_pay : IExtensible
{
	private int _jewel;

	private IExtension extensionObject;

	[DefaultValue(0)]
	[ProtoMember(1, IsRequired = false, Name = "jewel", DataFormat = DataFormat.TwosComplement)]
	public int jewel
	{
		get
		{
			return _jewel;
		}
		set
		{
			_jewel = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
