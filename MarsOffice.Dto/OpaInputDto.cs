namespace MarsOffice.Dto
{
    public class OpaInputDto<T> where T:class
    {
        public T Input { get; set; }
    }
}