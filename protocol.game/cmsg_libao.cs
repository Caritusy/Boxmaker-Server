using System;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "cmsg_libao")]
public class cmsg_libao : IExtensible
{
	private msg_common _common;

	private string _code;

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = true, Name = "common", DataFormat = DataFormat.Default)]
	public msg_common common
	{
		get
		{
			return _common;
		}
		set
		{
			_common = value;
		}
	}

	[ProtoMember(2, IsRequired = true, Name = "code", DataFormat = DataFormat.Default)]
	public string code
	{
		get
		{
			return _code;
		}
		set
		{
			_code = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
