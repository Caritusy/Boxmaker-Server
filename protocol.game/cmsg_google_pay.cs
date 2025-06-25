using System;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "cmsg_google_pay")]
public class cmsg_google_pay : IExtensible
{
	private msg_common _common;

	private int _id;

	private string _package_name;

	private string _product_id;

	private string _purchase_token;

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

	[ProtoMember(3, IsRequired = true, Name = "package_name", DataFormat = DataFormat.Default)]
	public string package_name
	{
		get
		{
			return _package_name;
		}
		set
		{
			_package_name = value;
		}
	}

	[ProtoMember(4, IsRequired = true, Name = "product_id", DataFormat = DataFormat.Default)]
	public string product_id
	{
		get
		{
			return _product_id;
		}
		set
		{
			_product_id = value;
		}
	}

	[ProtoMember(5, IsRequired = true, Name = "purchase_token", DataFormat = DataFormat.Default)]
	public string purchase_token
	{
		get
		{
			return _purchase_token;
		}
		set
		{
			_purchase_token = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
