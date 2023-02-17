using System.Collections.Generic;
[System.Serializable]
public class GameSave
{
    //key = GUID
    public Dictionary<string, GameObjectSave> gameObjectData;
    public string fase;
    public GameSave()
    {
        gameObjectData = new Dictionary<string, GameObjectSave>();
        fase = null;
    }
}
