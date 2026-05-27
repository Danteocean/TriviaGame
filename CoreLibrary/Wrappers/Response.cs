namespace CoreLibrary.Wrappers;

public class Response<T>
{
    public Response(int v)
    {
    }

    public Response() { }

    public Response(T data)
    {
        State = String.Empty;
        Message = String.Empty;
        Succeeded = true;
        Data = data;

    }

    public String State { get; set; }

    public String Message { get; set; }

    public Boolean Succeeded { get; set; }

    public T? Data { get; set; }
}