using System;
using Constellation;

[System.Serializable]
public class NodeView
{
    public NodeData NodeData;
    private float positionX;
    private float positionY;
    private float previousNodePositionX;
    private float previousNodePositionY;
    private string nodeName;
    private float nodeSizeX;
    private float nodeSizeY;
    private float previousNodeSizeX;
    private float previousNodeSizeY;


    public NodeView(float _positionX, float _positionY, string _nodeName, float _nodeSizeX, float _nodeSizeY)
    {
        positionX = _positionX;
        positionY = _positionY;
        nodeName = _nodeName;
        nodeSizeX = _nodeSizeX;
        nodeSizeY = _nodeSizeY;
        LockNodeSize();
        LockNodePosition();
    }

    public void UpdateNodeSize(float _x, float _y)
    {
        nodeSizeX = Math.Max(_x, 50);
        nodeSizeY = Math.Max(_y, 50);
    }

    public void SetPosition(float _x, float _y)
    {
        positionX = _x;
        positionY = _y;
    }

    public void SetName(string _name)
    {
        nodeName = _name;
    }

    public string GetName()
    {
        return nodeName;
    }

    public float GetPositionX()
    {
        return positionX;
    }

    public float GetPositionY()
    {
        return positionY;
    }

    public float GetSizeX()
    {
        return nodeSizeX;
    }

    public float GetSizeY()
    {
        return nodeSizeY;
    }

    public void LockNodeSize()
    {
        previousNodeSizeX = nodeSizeX;
        previousNodeSizeY = nodeSizeY;
    }

    public void LockNodePosition()
    {
        previousNodePositionX = positionX;
        previousNodePositionY = positionY;
    }

    public float GetPreviousNodePositionX()
    {
        return previousNodePositionX; 
    }

    public float GetPreviousNodePositionY()
    {
        return previousNodePositionY;
    }

    public float GetPreviousNodeSizeX()
    {
        return previousNodeSizeX;
    }

    public float GetPreviousNodeSizeY()
    {
        return previousNodeSizeY;
    }
}
