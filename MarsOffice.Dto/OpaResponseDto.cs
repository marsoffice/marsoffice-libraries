namespace MarsOffice.Dto
{
    public class OpaResponseDto<T>
    {
        public string decision_id { get; set; }
        public T Result { get; set; }
    }
}