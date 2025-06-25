using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "smsg_complete_map")]
public class smsg_complete_map : IExtensible
{
	private int _exp;

	private int _rank;

	private int _testify;

	private int _extra_exp;

	private int _mapid;

	private int _support;

	private IExtension extensionObject;

	[DefaultValue(0)]
	[ProtoMember(1, IsRequired = false, Name = "exp", DataFormat = DataFormat.TwosComplement)]
	public int exp
	{
		get
		{
			return _exp;
		}
		set
		{
			_exp = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(2, IsRequired = false, Name = "rank", DataFormat = DataFormat.TwosComplement)]
	public int rank
	{
		get
		{
			return _rank;
		}
		set
		{
			_rank = value;
		}
	}

	[ProtoMember(3, IsRequired = false, Name = "testify", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int testify
	{
		get
		{
			return _testify;
		}
		set
		{
			_testify = value;
		}
	}

	[ProtoMember(4, IsRequired = false, Name = "extra_exp", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int extra_exp
	{
		get
		{
			return _extra_exp;
		}
		set
		{
			_extra_exp = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(5, IsRequired = false, Name = "mapid", DataFormat = DataFormat.TwosComplement)]
	public int mapid
	{
		get
		{
			return _mapid;
		}
		set
		{
			_mapid = value;
		}
	}

	[ProtoMember(6, IsRequired = false, Name = "support", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int support
	{
		get
		{
			return _support;
		}
		set
		{
			_support = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
