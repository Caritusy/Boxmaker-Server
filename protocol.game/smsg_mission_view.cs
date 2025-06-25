using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "smsg_mission_view")]
public class smsg_mission_view : IExtensible
{
	private int _life;

	private int _index;

	private int _hard;

	private int _start;

	private int _br_max;

	private IExtension extensionObject;

	[DefaultValue(0)]
	[ProtoMember(1, IsRequired = false, Name = "life", DataFormat = DataFormat.TwosComplement)]
	public int life
	{
		get
		{
			return _life;
		}
		set
		{
			_life = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(2, IsRequired = false, Name = "index", DataFormat = DataFormat.TwosComplement)]
	public int index
	{
		get
		{
			return _index;
		}
		set
		{
			_index = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(3, IsRequired = false, Name = "hard", DataFormat = DataFormat.TwosComplement)]
	public int hard
	{
		get
		{
			return _hard;
		}
		set
		{
			_hard = value;
		}
	}

	[ProtoMember(4, IsRequired = false, Name = "start", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int start
	{
		get
		{
			return _start;
		}
		set
		{
			_start = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(5, IsRequired = false, Name = "br_max", DataFormat = DataFormat.TwosComplement)]
	public int br_max
	{
		get
		{
			return _br_max;
		}
		set
		{
			_br_max = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
