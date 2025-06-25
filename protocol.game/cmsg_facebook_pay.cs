using System;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "cmsg_facebook_pay")]
public class cmsg_facebook_pay : IExtensible
{
	private msg_common _common;

	private int _id;

	private string _signed_request;

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

	[ProtoMember(2, IsRequired = true, Name = "id", DataFormat = DataFormat.TwosComplement)]
	public int id
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
		}
	}

	[ProtoMember(3, IsRequired = true, Name = "signed_request", DataFormat = DataFormat.Default)]
	public string signed_request
	{
		get
		{
			return _signed_request;
		}
		set
		{
			_signed_request = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
