using System;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "msg_common")]
public class msg_common : IExtensible
{
	private int _userid;

	private int _pck_id;

	private string _sig;

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = true, Name = "userid", DataFormat = DataFormat.TwosComplement)]
	public int userid
	{
		get
		{
			return _userid;
		}
		set
		{
			_userid = value;
		}
	}

	[ProtoMember(2, IsRequired = true, Name = "pck_id", DataFormat = DataFormat.TwosComplement)]
	public int pck_id
	{
		get
		{
			return _pck_id;
		}
		set
		{
			_pck_id = value;
		}
	}

	[ProtoMember(3, IsRequired = true, Name = "sig", DataFormat = DataFormat.Default)]
	public string sig
	{
		get
		{
			return _sig;
		}
		set
		{
			_sig = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
