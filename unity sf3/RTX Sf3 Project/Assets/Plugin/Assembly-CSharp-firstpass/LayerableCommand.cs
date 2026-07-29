using System;

public class LayerableCommand
{
	private readonly string _creatorName;

	private int _layerCounter;

	public bool IsAnyLayerActive
	{
		get
		{
			return _layerCounter > 0;
		}
	}

	private LayerableCommand(string creatorName)
	{
		_creatorName = creatorName;
		_layerCounter = 0;
	}

	public void AddLayer(Action onFirstLayerAdded)
	{
		_layerCounter++;
		if (_layerCounter == 1)
		{
			onFirstLayerAdded.InvokeSafe();
		}
	}

	public void RemoveLayer(Action onLastLayerRemoved)
	{
		_layerCounter--;
		if (_layerCounter == 0)
		{
			onLastLayerRemoved.InvokeSafe();
		}
	}

	public static LayerableCommand Create(string creatorName)
	{
		return new LayerableCommand(creatorName);
	}
}
