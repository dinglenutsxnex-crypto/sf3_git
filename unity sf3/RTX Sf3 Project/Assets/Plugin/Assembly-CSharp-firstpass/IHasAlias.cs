using UnityEngine;

public interface IHasAlias
{
	GameObject gameObject { get; }

	string Alias { get; set; }
}
