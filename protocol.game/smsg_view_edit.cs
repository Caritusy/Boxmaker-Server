using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "smsg_view_edit")]
public class smsg_view_edit : IExtensible
{
	private List<edit_data> _infos = new List<edit_data>();

	private int _exp;

	private int _level;

	private IExtension extensionObject;

	[ProtoMember(1, Name = "infos", DataFormat = DataFormat.Default)]
	public List<edit_data> infos 
	{
		get { return _infos; }
		set { _infos = value; }
	}

	[ProtoMember(2, IsRequired = false, Name = "exp", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
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

	[ProtoMember(3, IsRequired = false, Name = "level", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int level
	{
		get
		{
			return _level;
		}
		set
		{
			_level = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
