using System;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "smsg_view_edit_single")]
public class smsg_view_edit_single : IExtensible
{
	private edit_data _info;

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = true, Name = "info", DataFormat = DataFormat.Default)]
	public edit_data info
	{
		get
		{
			return _info;
		}
		set
		{
			_info = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
