namespace Service.ResponseModels
{
    public class Result<T>
    {
        public int StatusCode { get; set; }
        public T Response { get; set; }
    }
}
