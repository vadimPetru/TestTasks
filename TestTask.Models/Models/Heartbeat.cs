namespace TestTask.Models.Models;

public sealed class Heartbeat {

    private int _chanId;
    private string _message;

    public int ChanId { get => _chanId;}
    public string Message { get => _message; }

    private Heartbeat(object[] objects)
    {
        _chanId = (int)objects[0];
        _message = (string)objects[1];
    }

    public static Heartbeat CreateInstance(object[] objects)
    {
        return new Heartbeat(objects);
    }
}



